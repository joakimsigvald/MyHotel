namespace Applique.MyHotel.Contract;

public record StoredEvent(long Version, string Type, DateTimeOffset Timestamp, object Data);