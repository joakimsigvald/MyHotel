namespace Applique.MyHotel.Contract.Results;

public record Failure(FailureKind Kind, string? Message = null, string? Field = null)
{
    public static Failure Validation(string field, string message) => new(FailureKind.Validation, message, field);
    public static Failure Conflict(string message) => new(FailureKind.Conflict, message);
    public static Failure NotFound(string? message = null) => new(FailureKind.NotFound, message);
}