namespace Applique.MyHotel.Contract.Results;

public record Result(Failure? Failure = null)
{
    public static readonly Result Success = new();
    public static implicit operator Result(Failure failure) => new(failure);
}

public record Result<T>(T? Value, Failure? Failure = null)
{
    public static Result<T> Success(T value) => new(value);
    public static implicit operator Result<T>(Failure failure) => new(default, failure);
}