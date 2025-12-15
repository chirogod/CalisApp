using CalisApp.Models;
using CalisApp.Models.DTOs;
using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

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

        public async Task<SessionFullDetailDto> GetSessionFullDetails(int sessionId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(UrlApi + "/" + sessionId + "/details");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"La API fallo con estado {response.StatusCode}. Respuesta: {responseBody}");
            }
            try
            {
                var sesion = JsonSerializer.Deserialize<SessionFullDetailDto>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return sesion ?? new SessionFullDetailDto();
            }
            catch (JsonException)
            {
                throw new Exception($"Error parseando JSON: {responseBody}");
            }
        }

        

        public async Task<List<SessionUserDataDto>> GetUsers(int sessionId)
        {
            try
            {
                var token = await _authService.GetTokenAsync();

                var client = new HttpClient();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await client.GetAsync(UrlApi + "/" + sessionId + "/Users");
                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error al obtener usuarios: {response.StatusCode}");
                    return new List<SessionUserDataDto>();
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseBody))
                    return new List<SessionUserDataDto>();

                var usuarios = JsonSerializer.Deserialize<List<SessionUserDataDto>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return usuarios ?? new List<SessionUserDataDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error crítico: {ex.Message}");
                return new List<SessionUserDataDto>();
            }
            
        }
    }
}
