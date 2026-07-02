using Applique.MyHotel.Contract;
using Applique.MyHotel.Contract.Reservations;
using Applique.MyHotel.Contract.Results;
using Applique.MyHotel.Core.Rooms;
using ContractStatus = Applique.MyHotel.Contract.Reservations.ReservationStatus;

namespace Applique.MyHotel.Core.Reservations;

public class ReservationService(
    IRoomRepository rooms,
    IReservationRepository reservations) : IReservationService
{
    public async Task<IReadOnlyList<ReservationDto>> GetReservationsAsync(CancellationToken ct)
        => [.. (await reservations.GetAllAsync(ct)).Select(ToDto)];

    public async Task<Result<Guid>> MakeReservationAsync(MakeReservation command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.GuestName))
            return Failure.Validation("guestName", "Guest name is required.");
        if (command.CheckIn >= command.CheckOut)
            return Failure.Validation("checkOut", "Check-out must be after check-in.");

        var room = await rooms.FindAsync(command.RoomId, ct);
        if (room is null)
            return Failure.NotFound($"Room {command.RoomId} does not exist.");

        if (await reservations.HasOverlappingAsync(command.RoomId, command.CheckIn, command.CheckOut, ct))
            return Failure.Conflict($"Room {room.Number} is already booked in that period.");

        var reservationId = Guid.NewGuid();
        await reservations.AddAsync(reservationId,
            new ReservationMade(reservationId, command.RoomId,
                command.GuestName, command.GuestEmail, command.CheckIn, command.CheckOut), ct);

        return Result<Guid>.Success(reservationId);
    }

    public async Task<Result> CancelReservationAsync(Guid id, CancellationToken ct)
    {
        var reservation = await reservations.FindAsync(id, ct);
        if (reservation is null)
            return Failure.NotFound();
        if (reservation.Status == ReservationStatus.Cancelled)
            return Failure.Conflict("Reservation is already cancelled.");

        await reservations.AppendAsync(id, new ReservationCancelled(id), ct);

        return Result.Success;
    }

    public async Task<IReadOnlyList<StoredEvent>> GetHistoryAsync(Guid id, CancellationToken ct)
        => [.. (await reservations.GetHistoryAsync(id, ct))
            .Select(e => new StoredEvent(e.Version, e.Type, e.Timestamp, e.Data))];

    private static ReservationDto ToDto(Reservation r) => new(
        r.Id, r.RoomId, r.GuestName, r.GuestEmail, r.CheckIn, r.CheckOut,
        r.Status == ReservationStatus.Cancelled ? ContractStatus.Cancelled : ContractStatus.Confirmed);
}