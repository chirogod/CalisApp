using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using CalisApp.Models;
using CalisApp.Services.Interfaces;

namespace CalisApp.Services
{
    public class SessionService : ISessionService
    {
        private string UrlApi = "https://localhost:7222/api/session";
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
    }
}
