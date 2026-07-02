using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Applique.MyHotel.Contract.Reservations;

namespace Applique.MyHotel.Entry;

public static class Reservations
{
    public static void MapReservations(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reservations");

        group.MapGet("/", async (IReservationService service, CancellationToken ct) =>
            await service.GetReservationsAsync(ct));

        group.MapPost("/", async (MakeReservation command, IReservationService service, CancellationToken ct) =>
        {
            var result = await service.MakeReservationAsync(command, ct);
            return result.Failure is { } failure
                ? failure.ToProblem()
                : Results.Created($"/api/reservations/{result.Value}", new { reservationId = result.Value });
        });

        group.MapPost("/{id:guid}/cancel", async (Guid id, IReservationService service, CancellationToken ct) =>
        {
            var result = await service.CancelReservationAsync(id, ct);
            return result.Failure is { } failure
                ? failure.ToProblem()
                : Results.NoContent();
        });

        // The full event history of one reservation — handy for demos and debugging.
        group.MapGet("/{id:guid}/history", async (Guid id, IReservationService service, CancellationToken ct) =>
        {
            var events = await service.GetHistoryAsync(id, ct);
            return events.Count == 0
                ? Results.NotFound()
                : Results.Ok(events);
        });
    }
}