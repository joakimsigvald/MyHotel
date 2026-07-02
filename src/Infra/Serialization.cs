using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Applique.MyHotel.Infra;

public static class Serialization
{
    // System.Text.Json ignores non-public setters by default, which would silently
    // leave encapsulated aggregate properties at their defaults when Marten loads
    // documents. This modifier binds private setters so read models rehydrate fully.
    public static void Configure(JsonSerializerOptions options)
        => options.TypeInfoResolver = (options.TypeInfoResolver ?? new DefaultJsonTypeInfoResolver())
            .WithAddedModifier(UseNonPublicSetters);

    private static void UseNonPublicSetters(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
            return;
        foreach (var property in typeInfo.Properties)
            if (property.Set is null
                && property.AttributeProvider is PropertyInfo info
                && info.GetSetMethod(true) is { } setter)
                property.Set = (target, value) => info.SetValue(target, value);
    }
}