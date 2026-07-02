namespace Applique.MyHotel.Core.Rooms;

public interface IRoomRepository
{
    Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken ct);
    Task<Room?> FindAsync(Guid id, CancellationToken ct);
    Task<bool> NumberExistsAsync(string number, CancellationToken ct);
    Task AddAsync(Guid roomId, RoomRegistered registered, CancellationToken ct);
}