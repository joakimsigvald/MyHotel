namespace Applique.MyHotel.Core.Rooms;

public class Room
{
    public Guid Id { get; set; }
    public string Number { get; set; } = "";
    public string Name { get; set; } = "";
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }

    public static Room Create(RoomRegistered e) => new()
    {
        Id = e.RoomId,
        Number = e.Number,
        Name = e.Name,
        Capacity = e.Capacity,
        PricePerNight = e.PricePerNight,
    };
}