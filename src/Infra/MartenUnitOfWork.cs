using JasperFx;
using JasperFx.Events;
using Marten;
using Marten.Exceptions;
using Npgsql;
using Applique.MyHotel.Core;
using Applique.MyHotel.Core.Reservations;
using Applique.MyHotel.Core.Rooms;

namespace Applique.MyHotel.Infra;

public class MartenUnitOfWork(IDocumentSession session) : IUnitOfWork
{
    private const string UniqueViolation = "23505";

    private readonly Dictionary<Guid, IEventStream<RoomCalendar>> _calendars = [];
    private readonly Dictionary<Guid, IEventStream<Reservation>> _reservations = [];

    public async Task<RoomCalendar?> GetCalendarAsync(Guid roomId, CancellationToken ct)
    {
        var stream = await session.Events.FetchForWriting<RoomCalendar>(roomId, ct);
        if (stream.Aggregate is null)
            return null;
        _calendars[roomId] = stream;
        return stream.Aggregate;
    }

    public async Task<Reservation?> GetReservationAsync(Guid id, CancellationToken ct)
    {
        var stream = await session.Events.FetchForWriting<Reservation>(id, ct);
        if (stream.Aggregate is null)
            return null;
        _reservations[id] = stream;
        return stream.Aggregate;
    }

    public void StartRoom(Guid roomId, RoomRegistered registered)
        => session.Events.StartStream<Room>(roomId, registered);

    public void StartReservation(Guid reservationId, ReservationMade made)
        => session.Events.StartStream<Reservation>(reservationId, made);

    public void AppendToRoom(Guid roomId, object e)
    {
        if (_calendars.TryGetValue(roomId, out var stream))
            stream.AppendOne(e);
        else
            session.Events.Append(roomId, e);
    }

    public void AppendToReservation(Guid reservationId, object e)
    {
        if (_reservations.TryGetValue(reservationId, out var stream))
            stream.AppendOne(e);
        else
            session.Events.Append(reservationId, e);
    }

    public async Task<CommitResult> CommitAsync(CancellationToken ct)
    {
        try
        {
            await session.SaveChangesAsync(ct);
            return CommitResult.Committed;
        }
        catch (ConcurrencyException)
        {
            return CommitResult.Conflict;
        }
        catch (ExistingStreamIdCollisionException)
        {
            return CommitResult.Conflict;
        }
        catch (MartenCommandException e) when (
            e.InnerException is PostgresException { SqlState: UniqueViolation })
        {
            return CommitResult.Conflict;
        }
    }

    public ValueTask DisposeAsync() => session.DisposeAsync();
}