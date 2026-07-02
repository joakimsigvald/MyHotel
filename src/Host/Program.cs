using Applique.MyHotel.Contract.Reservations;
using Applique.MyHotel.Contract.Rooms;
using Applique.MyHotel.Core.Reservations;
using Applique.MyHotel.Core.Rooms;
using Applique.MyHotel.Entry;
using Applique.MyHotel.Host;
using Applique.MyHotel.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddInfra(builder.Configuration.GetConnectionString("Postgres")!);

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