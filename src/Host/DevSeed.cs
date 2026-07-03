using Applique.MyHotel.Contract.Rooms;

namespace Applique.MyHotel.Host;

public static class DevSeed
{
    public static async Task SeedDevDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var rooms = scope.ServiceProvider.GetRequiredService<IRoomService>();

        if ((await rooms.GetRoomsAsync(CancellationToken.None)).Count > 0)
            return;

        RegisterRoom[] commands =
        [
            new("101", "Garden View Single", 1, 89m),
            new("102", "Garden View Double", 2, 129m),
            new("201", "Seaside Double", 2, 159m),
            new("202", "Family Suite", 4, 219m),
        ];

        foreach (var command in commands)
            await rooms.RegisterRoomAsync(command, CancellationToken.None);
    }
}