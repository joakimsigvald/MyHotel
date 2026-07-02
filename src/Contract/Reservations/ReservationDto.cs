namespace Applique.MyHotel.Contract.Reservations;

public record ReservationDto(
    Guid Id,
    Guid RoomId,
    string GuestName,
    string GuestEmail,
    DateOnly CheckIn,
    DateOnly CheckOut,
    ReservationStatus Status);