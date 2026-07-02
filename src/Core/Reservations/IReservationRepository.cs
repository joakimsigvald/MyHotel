namespace Applique.MyHotel.Core.Reservations;

public interface IReservationRepository
{
    Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken ct);
    Task<Reservation?> FindAsync(Guid id, CancellationToken ct);
    Task<bool> HasOverlappingAsync(Guid roomId, DateOnly checkIn, DateOnly checkOut, CancellationToken ct);
    Task AddAsync(Guid reservationId, ReservationMade made, CancellationToken ct);
    Task AppendAsync(Guid reservationId, ReservationCancelled cancelled, CancellationToken ct);
    Task<IReadOnlyList<EventRecord>> GetHistoryAsync(Guid id, CancellationToken ct);
}