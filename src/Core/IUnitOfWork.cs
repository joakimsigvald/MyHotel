using Applique.MyHotel.Core.Reservations;
using Applique.MyHotel.Core.Rooms;

namespace Applique.MyHotel.Core;

// One unit of work = one transaction. Loads return version-tracked aggregates;
// staged events are committed atomically, and a concurrent change to a tracked
// stream (or a uniqueness race) surfaces as CommitResult.Conflict instead of
// being persisted.
public interface IUnitOfWork : IAsyncDisposable
{
    Task<RoomCalendar?> GetCalendarAsync(Guid roomId, CancellationToken ct);
    Task<Reservation?> GetReservationAsync(Guid id, CancellationToken ct);
    void StartRoom(Guid roomId, RoomRegistered registered);
    void StartReservation(Guid reservationId, ReservationMade made);
    void AppendToRoom(Guid roomId, object e);
    void AppendToReservation(Guid reservationId, object e);
    Task<CommitResult> CommitAsync(CancellationToken ct);
}