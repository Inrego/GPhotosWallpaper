using Newtonsoft.Json;

namespace GPhotosWallpaper.UWP.Models
{
    public class MediaItem
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("productUrl")] public string ProductUrl { get; set; }
        [JsonProperty("baseUrl")] public string BaseUrl { get; set; }
        [JsonProperty("mimeType")] public string MimeType { get; set; }
        [JsonProperty("fileName")] public string FileName { get; set; }
    }
}
