namespace Applique.MyHotel.Core.Rooms;

public interface IRoomReader
{
    Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken ct);
    Task<Room?> FindAsync(Guid id, CancellationToken ct);
    Task<bool> NumberExistsAsync(string number, CancellationToken ct);
}