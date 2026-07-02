using Applique.MyHotel.Contract.Reservations;
using Applique.MyHotel.Contract.Results;

namespace Applique.MyHotel.Core.Reservations;

public class Reservation
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string GuestName { get; private set; } = "";
    public string GuestEmail { get; private set; } = "";
    public DateOnly CheckIn { get; private set; }
    public DateOnly CheckOut { get; private set; }
    public ReservationStatus Status { get; private set; }

    public static Result<ReservationMade> Make(MakeReservation command)
    {
        if (string.IsNullOrWhiteSpace(command.GuestName))
            return Failure.Validation("guestName", "Guest name is required.");
        if (command.CheckIn >= command.CheckOut)
            return Failure.Validation("checkOut", "Check-out must be after check-in.");

        return Result<ReservationMade>.Success(new ReservationMade(
            Guid.NewGuid(), command.RoomId,
            command.GuestName, command.GuestEmail, command.CheckIn, command.CheckOut));
    }

    public Result<ReservationCancelled> Cancel()
        => Status == ReservationStatus.Cancelled
            ? Failure.Conflict("Reservation is already cancelled.")
            : Result<ReservationCancelled>.Success(new ReservationCancelled(Id));

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