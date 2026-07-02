using Marten;
using Applique.MyHotel.Core.Rooms;

namespace Applique.MyHotel.Infra;

public class RoomRepository(IDocumentSession session) : IRoomRepository
{
    public async Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken ct)
        => await session.Query<Room>().OrderBy(r => r.Number).ToListAsync(ct);

    public Task<Room?> FindAsync(Guid id, CancellationToken ct)
        => session.LoadAsync<Room>(id, ct);

    public async Task<bool> NumberExistsAsync(string number, CancellationToken ct)
        => await session.Query<Room>().AnyAsync(r => r.Number == number, ct);

    public async Task AddAsync(Guid roomId, RoomRegistered registered, CancellationToken ct)
    {
        session.Events.StartStream<Room>(roomId, registered);
        await session.SaveChangesAsync(ct);
    }
}