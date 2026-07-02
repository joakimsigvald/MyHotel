namespace Applique.MyHotel.Core.Rooms;

public record PeriodClaimed(Guid ReservationId, DateOnly CheckIn, DateOnly CheckOut);