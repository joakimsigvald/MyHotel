using JasperFx;
using JasperFx.Events.Projections;
using Marten;
using Applique.MyHotel.Contract.Rooms;
using Applique.MyHotel.Contract.Reservations;
using Applique.MyHotel.Core.Reservations;
using Applique.MyHotel.Core.Rooms;
using Applique.MyHotel.Entry;
using Applique.MyHotel.Host;
using Applique.MyHotel.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Postgres")!);
    opts.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

    // Inline projections: read models are updated in the same transaction
    // as the event append, so queries immediately reflect committed commands.
    opts.Projections.Add<RoomProjection>(ProjectionLifecycle.Inline);
    opts.Projections.Add<ReservationProjection>(ProjectionLifecycle.Inline);
}).UseLightweightSessions();

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

var app = builder.Build();

app.MapRooms();
app.MapReservations();

if (app.Environment.IsDevelopment())
{
    await app.SeedDevDataAsync();
}

app.Run();