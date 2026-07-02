namespace Applique.MyHotel.Core.Reservations;

public interface IReservationReader
{
    Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken ct);
    Task<Reservation?> FindAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<EventRecord>> GetHistoryAsync(Guid id, CancellationToken ct);
}