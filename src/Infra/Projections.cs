using Marten.Events.Aggregation;
using Applique.MyHotel.Core.Reservations;
using Applique.MyHotel.Core.Rooms;

namespace Applique.MyHotel.Infra;

// Explicit projections so Marten's source generator runs in this assembly,
// keeping Core free of the Marten package. They delegate to the aggregates.

public partial class RoomProjection : SingleStreamProjection<Room, Guid>
{
    public static Room Create(RoomRegistered e) => Room.Create(e);
}

public partial class ReservationProjection : SingleStreamProjection<Reservation, Guid>
{
    public static Reservation Create(ReservationMade e) => Reservation.Create(e);

    public void Apply(ReservationCancelled e, Reservation reservation) => reservation.Apply(e);
}