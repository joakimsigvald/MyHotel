namespace Applique.MyHotel.Core.Reservations;

public record ReservationMade(
    Guid ReservationId,
    Guid RoomId,
    string GuestName,
    string GuestEmail,
    DateOnly CheckIn,
    DateOnly CheckOut);