using Marten;
using Applique.MyHotel.Core;
using Applique.MyHotel.Core.Reservations;

namespace Applique.MyHotel.Infra;

public class ReservationRepository(IDocumentSession session) : IReservationRepository
{
    public async Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken ct)
        => await session.Query<Reservation>().OrderBy(r => r.CheckIn).ToListAsync(ct);

    public Task<Reservation?> FindAsync(Guid id, CancellationToken ct)
        => session.LoadAsync<Reservation>(id, ct);

    public async Task<bool> HasOverlappingAsync(
        Guid roomId, DateOnly checkIn, DateOnly checkOut, CancellationToken ct)
        => await session.Query<Reservation>()
            .AnyAsync(r =>
                r.RoomId == roomId
                && r.Status == ReservationStatus.Confirmed
                && r.CheckIn < checkOut
                && r.CheckOut > checkIn, ct);

    public async Task AddAsync(Guid reservationId, ReservationMade made, CancellationToken ct)
    {
        session.Events.StartStream<Reservation>(reservationId, made);
        await session.SaveChangesAsync(ct);
    }

    public async Task AppendAsync(Guid reservationId, ReservationCancelled cancelled, CancellationToken ct)
    {
        session.Events.Append(reservationId, cancelled);
        await session.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<EventRecord>> GetHistoryAsync(Guid id, CancellationToken ct)
    {
        var events = await session.Events.FetchStreamAsync(id, token: ct);
        return [.. events.Select(e => new EventRecord(e.Version, e.EventTypeName, e.Timestamp, e.Data))];
    }
}