using Marten;
using Applique.MyHotel.Core;
using Applique.MyHotel.Core.Reservations;

namespace Applique.MyHotel.Infra;

public class ReservationReader(IQuerySession session) : IReservationReader
{
    public async Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken ct)
        => await session.Query<Reservation>().OrderBy(r => r.CheckIn).ToListAsync(ct);

    public Task<Reservation?> FindAsync(Guid id, CancellationToken ct)
        => session.LoadAsync<Reservation>(id, ct);

    public async Task<IReadOnlyList<EventRecord>> GetHistoryAsync(Guid id, CancellationToken ct)
    {
        var events = await session.Events.FetchStreamAsync(id, token: ct);
        return [.. events.Select(e => new EventRecord(e.Version, e.EventTypeName, e.Timestamp, e.Data))];
    }
}