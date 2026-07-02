namespace Applique.MyHotel.Contract.Rooms;

public record RoomDto(
    Guid Id,
    string Number,
    string Name,
    int Capacity,
    decimal PricePerNight);