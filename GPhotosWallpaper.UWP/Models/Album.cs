using Newtonsoft.Json;

namespace GPhotosWallpaper.UWP.Models
{
    public class Album
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("productUrl")] public string ProductUrl { get; set; }
        [JsonProperty("mediaItemsCount")] public int MediaItemsCount { get; set; }
        [JsonProperty("coverPhotoBaseUrl")] public string CoverPhotoBaseUrl { get; set; }
        [JsonProperty("coverPhotoMediaItemId")] public string CoverPhotoMediaItemId { get; set; }
    }
}
