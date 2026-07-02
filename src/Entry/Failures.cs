using Microsoft.AspNetCore.Http;
using Applique.MyHotel.Contract.Results;

namespace Applique.MyHotel.Entry;

internal static class Failures
{
    public static IResult ToProblem(this Failure failure) => failure.Kind switch
    {
        FailureKind.Validation => Results.ValidationProblem(new Dictionary<string, string[]>
            { [failure.Field ?? ""] = [failure.Message ?? ""] }),
        FailureKind.Conflict => Results.Conflict(failure.Message),
        _ => failure.Message is null ? Results.NotFound() : Results.NotFound(failure.Message),
    };
}