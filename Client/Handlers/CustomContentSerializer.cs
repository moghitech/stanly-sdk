using System.Net.Http.Json;
using System.Reflection;
using Newtonsoft.Json;
using Refit;

namespace Fusion.Client.Handlers;

internal class CustomContentSerializer : IHttpContentSerializer
{
    public HttpContent ToHttpContent<T>(T item)
    {
        return JsonContent.Create(item);
    }

    public async Task<T?> FromHttpContentAsync<T>(HttpContent content, CancellationToken ct = new())
    {
        var response = await content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<T>(response);
    }

    public string GetFieldNameForProperty(PropertyInfo propertyInfo)
    {
        return propertyInfo.Name;
    }
}