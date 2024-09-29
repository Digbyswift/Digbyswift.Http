using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Digbyswift.Http.Extensions;

public static class HttpContentExtensions
{
    /// <summary>
    /// Deserializes and returns JSON from the HttpContext See https://www.newtonsoft.com/json/help/html/Performance.htm#MemoryUsage
    /// </summary>
    public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content, JsonSerializerSettings? options = null)
    {
        using (var stream = await content.ReadAsStreamAsync())
        using (var streamReader = new StreamReader(stream))
        using (var jsonReader = new JsonTextReader(streamReader))
        {
            var serializer = JsonSerializer.Create(options);
            return serializer.Deserialize<T>(jsonReader);
        }
    }
}
