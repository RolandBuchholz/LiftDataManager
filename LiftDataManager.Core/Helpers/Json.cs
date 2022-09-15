using Newtonsoft.Json;

namespace LiftDataManager.Core.Helpers;

public static class Json
{
    public static async Task<T?> ToObjectAsync<T>(string? value)
    {
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
        return await Task.Run(() => JsonConvert.DeserializeObject<T?>(value, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include }));
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
    }

    public static async Task<string> StringifyAsync(object? value)
    {
        return await Task.Run(() => JsonConvert.SerializeObject(value));
    }
}
