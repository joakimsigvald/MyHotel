using Applique.MyHotel.Contract.Results;
using Applique.MyHotel.Contract.Rooms;

namespace Applique.MyHotel.Core.Rooms;

public class Room
{
    public Guid Id { get; private set; }
    public string Number { get; private set; } = "";
    public string Name { get; private set; } = "";
    public int Capacity { get; private set; }
    public decimal PricePerNight { get; private set; }

    public static Result<RoomRegistered> Register(RegisterRoom command)
    {
        if (string.IsNullOrWhiteSpace(command.Number))
            return Failure.Validation("number", "Room number is required.");
        if (command.Capacity < 1)
            return Failure.Validation("capacity", "Capacity must be at least 1.");

        return Result<RoomRegistered>.Success(new RoomRegistered(
            Guid.NewGuid(), command.Number, command.Name, command.Capacity, command.PricePerNight));
    }

    public static Room Create(RoomRegistered e) => new()
    {
        Id = e.RoomId,
        Number = e.Number,
        Name = e.Name,
        Capacity = e.Capacity,
        PricePerNight = e.PricePerNight,
    };
}