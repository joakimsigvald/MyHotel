using Applique.MyHotel.Contract.Results;

namespace Applique.MyHotel.Contract.Reservations;

public interface IReservationService
{
    Task<IReadOnlyList<ReservationDto>> GetReservationsAsync(CancellationToken ct);
    Task<Result<Guid>> MakeReservationAsync(MakeReservation command, CancellationToken ct);
    Task<Result> CancelReservationAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<StoredEvent>> GetHistoryAsync(Guid id, CancellationToken ct);
}