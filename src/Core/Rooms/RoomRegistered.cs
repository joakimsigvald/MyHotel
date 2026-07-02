namespace Applique.MyHotel.Core.Rooms;

public record RoomRegistered(
    Guid RoomId,
    string Number,
    string Name,
    int Capacity,
    decimal PricePerNight);