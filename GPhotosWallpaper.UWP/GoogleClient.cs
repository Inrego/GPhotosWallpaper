using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GPhotosWallpaper.UWP.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GPhotosWallpaper.UWP
{
    public class GoogleClient
    {
        private readonly string _refreshToken;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly HttpClient _httpClient = new HttpClient();

        private const string TokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        private const string AlbumsEndpoint = "https://photoslibrary.googleapis.com/v1/albums";
        private const string PhotosEndpoint = "https://photoslibrary.googleapis.com/v1/mediaItems:search";

        public GoogleClient(string refreshToken, string clientId, string clientSecret)
        {
            _refreshToken = refreshToken;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        private async Task<string> GetAccessToken()
        {
            // Builds the Token request
            var tokenRequestBody =
                $"refresh_token={_refreshToken}&client_id={_clientId}&client_secret={_clientSecret}&grant_type=refresh_token";
            var content = new StringContent(tokenRequestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            //output(Environment.NewLine + "Exchanging code for tokens...");
            var response = await _httpClient.PostAsync(TokenEndpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();
            //output(responseString);

            if (!response.IsSuccessStatusCode)
            {
                //output("Authorization code exchange failed.");
                return null;
            }

            // Sets the Authentication header of our HTTP client using the acquired access token.
            var tokens = JObject.Parse(responseString);
            var accessToken = tokens["access_token"].ToObject<string>();
            return accessToken;
        }

        public async Task<Album[]> GetAlbums()
        {
            var accessToken = await GetAccessToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync(AlbumsEndpoint);
            var respStr = await response.Content.ReadAsStringAsync();
            return JObject.Parse(respStr)["albums"].ToObject<Album[]>();
        }

        public async Task<Album> GetAlbum(string albumId)
        {
            var accessToken = await GetAccessToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync(AlbumsEndpoint + "/" + albumId);
            var respStr = await response.Content.ReadAsStringAsync();
            return JObject.Parse(respStr).ToObject<Album>();
        }

        public async Task<List<MediaItem>> GetPhotos(string albumId)
        {
            var mediaItems = new List<MediaItem>();
            string paginationToken = null;
            while (true)
            {
                var photos = await GetAlbumContents(albumId, paginationToken);
                mediaItems.AddRange(photos["mediaItems"].ToObject<MediaItem[]>());
                if (photos.ContainsKey("nextPageToken"))
                    paginationToken = photos["nextPageToken"].ToObject<string>();
                else
                    break;
            }

            return mediaItems;
        }

        private async Task<JObject> GetAlbumContents(string albumId, string paginationToken)
        {
            var accessToken = await GetAccessToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var requestBodyParams = new Dictionary<string, object>();
            requestBodyParams.Add("albumId", albumId);
            if (!string.IsNullOrWhiteSpace(paginationToken))
            {
                requestBodyParams.Add("pageToken", paginationToken);
            }
            requestBodyParams.Add("pageSize", "100");
            /*requestBodyParams.Add("filters", new
            {
                mediaTypeFilter = new
                {
                    mediaTypes = "PHOTO"
                }
            });*/
            var response = await _httpClient.PostAsync(PhotosEndpoint, new StringContent(JsonConvert.SerializeObject(requestBodyParams), Encoding.UTF8));
            var respStr = await response.Content.ReadAsStringAsync();
            return JObject.Parse(respStr);
        }
    }
}
