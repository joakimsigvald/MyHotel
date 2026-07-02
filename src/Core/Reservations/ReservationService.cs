using Applique.MyHotel.Contract;
using Applique.MyHotel.Contract.Reservations;
using Applique.MyHotel.Contract.Results;
using Applique.MyHotel.Core.Rooms;
using ContractStatus = Applique.MyHotel.Contract.Reservations.ReservationStatus;

namespace Applique.MyHotel.Core.Reservations;

public class ReservationService(
    IReservationReader reservations,
    IUnitOfWorkFactory uowFactory) : IReservationService
{
    private const int MaxAttempts = 5;

    public async Task<IReadOnlyList<ReservationDto>> GetReservationsAsync(CancellationToken ct)
        => [.. (await reservations.GetAllAsync(ct)).Select(ToDto)];

    public async Task<Result<Guid>> MakeReservationAsync(MakeReservation command, CancellationToken ct)
    {
        var decision = Reservation.Make(command);
        if (decision.Failure is { } failure)
            return failure;
        var made = decision.Value!;

        // Optimistic append on the room stream guards the no-overlap invariant;
        // a version conflict means someone else booked concurrently, so re-decide
        // against the fresh calendar instead of failing non-overlapping requests.
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            await using var uow = uowFactory.Create();

            var calendar = await uow.GetCalendarAsync(made.RoomId, ct);
            if (calendar is null)
                return Failure.NotFound($"Room {made.RoomId} does not exist.");

            var claim = calendar.Claim(made.ReservationId, made.CheckIn, made.CheckOut);
            if (claim.Failure is { } claimFailure)
                return claimFailure;

            uow.AppendToRoom(made.RoomId, claim.Value!);
            uow.StartReservation(made.ReservationId, made);

            if (await uow.CommitAsync(ct) is CommitResult.Committed)
                return Result<Guid>.Success(made.ReservationId);

            // Jittered backoff so concurrent losers don't retry in lockstep.
            await Task.Delay(Random.Shared.Next(10, 50 * attempt), ct);
        }

        return Failure.Conflict("The room is being booked concurrently, please retry.");
    }

    public async Task<Result> CancelReservationAsync(Guid id, CancellationToken ct)
    {
        await using var uow = uowFactory.Create();

        var reservation = await uow.GetReservationAsync(id, ct);
        if (reservation is null)
            return Failure.NotFound();

        var decision = reservation.Cancel();
        if (decision.Failure is { } failure)
            return failure;

        uow.AppendToReservation(id, decision.Value!);
        uow.AppendToRoom(reservation.RoomId, new PeriodReleased(id));

        if (await uow.CommitAsync(ct) is not CommitResult.Committed)
            return Failure.Conflict("The reservation was changed concurrently, please retry.");

        return Result.Success;
    }

    public async Task<IReadOnlyList<StoredEvent>> GetHistoryAsync(Guid id, CancellationToken ct)
        => [.. (await reservations.GetHistoryAsync(id, ct))
            .Select(e => new StoredEvent(e.Version, e.Type, e.Timestamp, e.Data))];

    private static ReservationDto ToDto(Reservation r) => new(
        r.Id, r.RoomId, r.GuestName, r.GuestEmail, r.CheckIn, r.CheckOut,
        r.Status == ReservationStatus.Cancelled ? ContractStatus.Cancelled : ContractStatus.Confirmed);
}