namespace Applique.MyHotel.Contract.Reservations;

public record MakeReservation(
    Guid RoomId,
    string GuestName,
    string GuestEmail,
    DateOnly CheckIn,
    DateOnly CheckOut);