using Applique.MyHotel.Contract.Results;
using Applique.MyHotel.Contract.Rooms;
using Applique.MyHotel.Core.Rooms;
using Xspec;
using Xspec.Assert;
using Xunit;
using static Applique.MyHotel.Core.Rooms.Room;

namespace Applique.MyHotel.Core.Spec.Rooms.Room;

public abstract class WhenRegister : Spec<Result<RoomRegistered>>
{
    private static readonly Tag<string> _roomNumber = new(nameof(_roomNumber));
    private static readonly Tag<int> _capacity = new(nameof(_capacity));

    protected WhenRegister()
        => When(_ => Register(new RegisterRoom(The(_roomNumber)!, A<string>(), The(_capacity), A<decimal>())));

    public class GivenValidCommand : WhenRegister
    {
        public GivenValidCommand() => Given(_capacity).Is(2);

        [Fact] public void ThenRoomIsRegistered() => Result.Value!.Number.Is(The(_roomNumber)!);

        [Fact] public void ThenHasNoFailure() => Result.Has(_ => _!.Failure is null);
    }

    public class GivenEmptyNumber : WhenRegister
    {
        public GivenEmptyNumber() => Given(_roomNumber).Is("").And(_capacity).Is(2);

        [Fact] public void ThenFailsValidation() => Result.Failure!.Kind.Is(FailureKind.Validation);
    }

    public class GivenZeroCapacity : WhenRegister
    {
        public GivenZeroCapacity() => Given(_capacity).Is(0);

        [Fact] public void ThenFailsValidation() => Result.Failure!.Kind.Is(FailureKind.Validation);
    }
}