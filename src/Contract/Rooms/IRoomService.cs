using Applique.MyHotel.Contract.Results;

namespace Applique.MyHotel.Contract.Rooms;

public interface IRoomService
{
    Task<IReadOnlyList<RoomDto>> GetRoomsAsync(CancellationToken ct);
    Task<RoomDto?> GetRoomAsync(Guid id, CancellationToken ct);
    Task<Result<Guid>> RegisterRoomAsync(RegisterRoom command, CancellationToken ct);
}