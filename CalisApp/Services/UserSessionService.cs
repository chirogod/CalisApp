using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CalisApp.Models;

namespace CalisApp.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IAuthService _authService;
        public const string UrlApi = "https://calisapi-b6gxc6cuf6a7apes.brazilsouth-01.azurewebsites.net/api/usersession";
        public UserSessionService(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Session> Enroll(string userId, int sessionId)
        {

            var token = await _authService.GetTokenAsync();


            var client = new HttpClient();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            using StringContent jsonContent = new(JsonSerializer.Serialize(new
            {
                UserId = userId,
                SessionId = sessionId
            }),
            Encoding.UTF8,
            "application/json"
            );

            var response = await client.PostAsync(UrlApi + $"/{sessionId}", jsonContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"La API fallo con estado {response.StatusCode}. Respuesta: {responseBody}");
            }


            try
            {
                var jsonResponse = JsonSerializer.Deserialize<Session>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return jsonResponse ?? new Session();
            }
            catch (JsonException)
            {
                throw new Exception($"Error parseando JSON: {responseBody}");
            }
        }

        public async Task UnEnroll(int sessionId)
        {
            var token = await _authService.GetTokenAsync();
            HttpClient client = new HttpClient();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.DeleteAsync(UrlApi + $"/{sessionId}");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"La API fallo con estado {response.StatusCode}. Respuesta: {responseBody}");
            }
        }
    }
}
