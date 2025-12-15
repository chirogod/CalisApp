using CalisApp.Models;
using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalisApp.Services
{
    public class VideoService : IVideoService
    {
        private readonly IAuthService _authService;

        public VideoService(IAuthService authService)
        {
            _authService = authService;
        }

        public const string UrlApi = "https://calisapi-b6gxc6cuf6a7apes.brazilsouth-01.azurewebsites.net/api/video";

        public async Task<IEnumerable<Video>> GetAllVideos()
        {
            var token = await _authService.GetTokenAsync();
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await client.GetAsync(UrlApi);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var videos = JsonSerializer.Deserialize<IEnumerable<Video>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return videos ?? Enumerable.Empty<Video>();
            }
            catch (JsonException ex)
            {
                throw new Exception("Error al deserializar la respuesta de videos.", ex);

            }
        }

        public async Task<IEnumerable<Video>> GetAllVideosByCategory(int categoryId)
        {
            var token = await _authService.GetTokenAsync();
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await client.GetAsync($"{UrlApi}?categoryId={categoryId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var videos = JsonSerializer.Deserialize<IEnumerable<Video>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return videos ?? Enumerable.Empty<Video>();
            }
            catch (JsonException ex)
            {
                throw new Exception("Error al deserializar la respuesta de videos.", ex);

            }
        }

        public async Task<IEnumerable<Video>> SearchVideos(string searchTerm)
        {
            var token = await _authService.GetTokenAsync();
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await client.GetAsync($"{UrlApi}?search={searchTerm}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var videos = JsonSerializer.Deserialize<IEnumerable<Video>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return videos ?? Enumerable.Empty<Video>();
            }
            catch (JsonException ex)
            {
                throw new Exception("Error al deserializar la respuesta de videos.", ex);

            }

        }

        public async Task<Video> GetVideoDetails(int id)
        {
            var token = await _authService.GetTokenAsync();
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await client.GetAsync($"{UrlApi}/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var video = JsonSerializer.Deserialize<Video>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return video!;
            }
            catch (JsonException ex)
            {
                throw new Exception("Error al deserializar la respuesta del video.", ex);

            }
        }
    }
}
