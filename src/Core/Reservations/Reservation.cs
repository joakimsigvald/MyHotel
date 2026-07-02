namespace Applique.MyHotel.Core.Reservations;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string GuestName { get; set; } = "";
    public string GuestEmail { get; set; } = "";
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public ReservationStatus Status { get; set; }

    public static Reservation Create(ReservationMade e) => new()
    {
        Id = e.ReservationId,
        RoomId = e.RoomId,
        GuestName = e.GuestName,
        GuestEmail = e.GuestEmail,
        CheckIn = e.CheckIn,
        CheckOut = e.CheckOut,
        Status = ReservationStatus.Confirmed,
    };

    public void Apply(ReservationCancelled e) => Status = ReservationStatus.Cancelled;
}