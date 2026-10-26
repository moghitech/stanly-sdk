using Newtonsoft.Json;

namespace Fusion.Caching.Abstractions.Messages;

internal record CacheInvalidateMessage(
    string Key,
    DateTime Timestamp
)
{
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static CacheInvalidateMessage? FromJson(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<CacheInvalidateMessage>(json);
        }
        catch
        {
            return null;
        }
    }
}