namespace Applique.MyHotel.Core;

public record EventRecord(long Version, string Type, DateTimeOffset Timestamp, object Data);