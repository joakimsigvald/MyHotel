using Applique.MyHotel.Contract.Results;

namespace Applique.MyHotel.Core.Rooms;

// Decision model over the room stream: the ledger of claimed periods.
// Never persisted as a document — always rehydrated by replaying the stream —
// so the no-overlap invariant is guarded by optimistic append on that stream.
public class RoomCalendar
{
    private readonly List<PeriodClaimed> _claims = [];

    public Guid Id { get; private set; }
    public string Number { get; private set; } = "";

    public Result<PeriodClaimed> Claim(Guid reservationId, DateOnly checkIn, DateOnly checkOut)
        => _claims.Any(c => c.CheckIn < checkOut && c.CheckOut > checkIn)
            ? Failure.Conflict($"Room {Number} is already booked in that period.")
            : Result<PeriodClaimed>.Success(new PeriodClaimed(reservationId, checkIn, checkOut));

    public static RoomCalendar Create(RoomRegistered e) => new()
    {
        Id = e.RoomId,
        Number = e.Number,
    };

    public void Apply(PeriodClaimed e) => _claims.Add(e);

    public void Apply(PeriodReleased e) => _claims.RemoveAll(c => c.ReservationId == e.ReservationId);
}