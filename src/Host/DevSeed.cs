using Marten;
using Applique.MyHotel.Core.Rooms;

namespace Applique.MyHotel.Host;

public static class DevSeed
{
    public static async Task SeedDevDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        if (await session.Query<Room>().AnyAsync())
            return;

        RoomRegistered[] rooms =
        [
            new(Guid.NewGuid(), "101", "Garden View Single", 1, 89m),
            new(Guid.NewGuid(), "102", "Garden View Double", 2, 129m),
            new(Guid.NewGuid(), "201", "Seaside Double", 2, 159m),
            new(Guid.NewGuid(), "202", "Family Suite", 4, 219m),
        ];

        foreach (var room in rooms)
            session.Events.StartStream<Room>(room.RoomId, room);

        await session.SaveChangesAsync();
        app.Logger.LogInformation("Seeded {Count} rooms", rooms.Length);
    }
}