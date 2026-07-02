using Applique.MyHotel.Contract.Results;
using Applique.MyHotel.Contract.Rooms;

namespace Applique.MyHotel.Core.Rooms;

public class RoomService(IRoomRepository rooms) : IRoomService
{
    public async Task<IReadOnlyList<RoomDto>> GetRoomsAsync(CancellationToken ct)
        => [.. (await rooms.GetAllAsync(ct)).Select(ToDto)];

    public async Task<RoomDto?> GetRoomAsync(Guid id, CancellationToken ct)
        => await rooms.FindAsync(id, ct) is { } room ? ToDto(room) : null;

    public async Task<Result<Guid>> RegisterRoomAsync(RegisterRoom command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Number))
            return Failure.Validation("number", "Room number is required.");
        if (command.Capacity < 1)
            return Failure.Validation("capacity", "Capacity must be at least 1.");
        if (await rooms.NumberExistsAsync(command.Number, ct))
            return Failure.Conflict($"A room with number '{command.Number}' already exists.");

        var roomId = Guid.NewGuid();
        await rooms.AddAsync(roomId,
            new RoomRegistered(roomId, command.Number, command.Name, command.Capacity, command.PricePerNight), ct);

        return Result<Guid>.Success(roomId);
    }

    private static RoomDto ToDto(Room r) => new(r.Id, r.Number, r.Name, r.Capacity, r.PricePerNight);
}