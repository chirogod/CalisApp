using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using CalisApp.Models;
using CalisApp.Services.Interfaces;

namespace CalisApp.Services
{
    public class SessionService : ISessionService
    {
        private readonly IAuthService _authService;

        public SessionService(IAuthService authService)
        {
            _authService = authService;
        }

        public const string UrlApi = "https://calisapi-b6gxc6cuf6a7apes.brazilsouth-01.azurewebsites.net/api/session";
        public async Task<IEnumerable<Session>> GetAll()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(UrlApi);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            var sesiones = JsonSerializer.Deserialize<IEnumerable<Session>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return sesiones ?? Enumerable.Empty<Session>();
        }

        public async Task<Session> GetSession(int sessionId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(UrlApi+"/"+sessionId);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            var sesion = JsonSerializer.Deserialize<Session>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return sesion ?? new Session();
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

            var response = await client.PostAsync(UrlApi+"/enroll", jsonContent);

            var responseBody = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonSerializer.Deserialize<Session>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return jsonResponse ?? new Session();
        }
    }
}
