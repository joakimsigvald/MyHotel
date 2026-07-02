using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Applique.MyHotel.Contract.Rooms;

namespace Applique.MyHotel.Entry;

public static class Rooms
{
    public static void MapRooms(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rooms");

        group.MapGet("/", async (IRoomService service, CancellationToken ct) =>
            await service.GetRoomsAsync(ct));

        group.MapPost("/", async (RegisterRoom command, IRoomService service, CancellationToken ct) =>
        {
            var result = await service.RegisterRoomAsync(command, ct);
            return result.Failure is { } failure
                ? failure.ToProblem()
                : Results.Created($"/api/rooms/{result.Value}", new { roomId = result.Value });
        });

        group.MapGet("/{id:guid}", async (Guid id, IRoomService service, CancellationToken ct) =>
            await service.GetRoomAsync(id, ct) is { } room
                ? Results.Ok(room)
                : Results.NotFound());
    }
}