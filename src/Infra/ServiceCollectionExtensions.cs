using JasperFx;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Applique.MyHotel.Core;
using Applique.MyHotel.Core.Reservations;
using Applique.MyHotel.Core.Rooms;

namespace Applique.MyHotel.Infra;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfra(this IServiceCollection services, string connectionString)
    {
        services.AddMarten(opts =>
        {
            opts.Connection(connectionString);
            opts.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
            opts.UseSystemTextJsonForSerialization(configure: Serialization.Configure);

            // Inline projections: read models are updated in the same transaction
            // as the event append, so queries immediately reflect committed commands.
            opts.Projections.Add<RoomProjection>(ProjectionLifecycle.Inline);
            opts.Projections.Add<ReservationProjection>(ProjectionLifecycle.Inline);

            // Decision model: replayed from the room stream on demand, never stored.
            opts.Projections.Add<RoomCalendarProjection>(ProjectionLifecycle.Live);

            // Guards the duplicate-number race that the friendly pre-check cannot.
            opts.Schema.For<Room>().UniqueIndex(r => r.Number);
        }).UseLightweightSessions();

        services.AddScoped<IRoomReader, RoomReader>();
        services.AddScoped<IReservationReader, ReservationReader>();
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

        return services;
    }
}