using Applique.MyHotel.Contract.Results;
using Applique.MyHotel.Contract.Rooms;

namespace Applique.MyHotel.Core.Rooms;

public class RoomService(IRoomReader rooms, IUnitOfWorkFactory uowFactory) : IRoomService
{
    public async Task<IReadOnlyList<RoomDto>> GetRoomsAsync(CancellationToken ct)
        => [.. (await rooms.GetAllAsync(ct)).Select(ToDto)];

    public async Task<RoomDto?> GetRoomAsync(Guid id, CancellationToken ct)
        => await rooms.FindAsync(id, ct) is { } room ? ToDto(room) : null;

    public async Task<Result<Guid>> RegisterRoomAsync(RegisterRoom command, CancellationToken ct)
    {
        var decision = Room.Register(command);
        if (decision.Failure is { } failure)
            return failure;
        var registered = decision.Value!;

        if (await rooms.NumberExistsAsync(registered.Number, ct))
            return Failure.Conflict($"A room with number '{registered.Number}' already exists.");

        await using var uow = uowFactory.Create();
        uow.StartRoom(registered.RoomId, registered);
        if (await uow.CommitAsync(ct) is not CommitResult.Committed)
            return Failure.Conflict($"A room with number '{registered.Number}' already exists.");

        return Result<Guid>.Success(registered.RoomId);
    }

    private static RoomDto ToDto(Room r) => new(r.Id, r.Number, r.Name, r.Capacity, r.PricePerNight);
}