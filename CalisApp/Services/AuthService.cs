using CalisApp.Models.DTOs;
using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace CalisApp.Services
{
    public static class AuthConstants
    {
        public const string TokenKey = "auth_token";
        public const string ApiBaseUrl = "https://calisapi-b6gxc6cuf6a7apes.brazilsouth-01.azurewebsites.net";
    }
    public class LoginResponse
    {
        public string Token { get; set; }
    }
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(AuthConstants.ApiBaseUrl) };
        }

        public string UrlApi = "https://calisapi-b6gxc6cuf6a7apes.brazilsouth-01.azurewebsites.net/";
        public async Task<bool> Login(string email, string password)
        {
            
            try
            {
                var loginRequest = new LoginDto {
                    Email = email,
                    Password = password
                };
                var jsonContent = JsonContent.Create(loginRequest);
                var response = await _httpClient.PostAsync("api/user/login", jsonContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"RESPUESTA API: {responseContent}");
                if (response.IsSuccessStatusCode)
                {
                    string token = string.Empty;

                    if (responseContent.Trim().StartsWith("{"))
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, options);
                        token = loginResponse?.Token;
                    }
                    else
                    {
                        token = responseContent.Trim('"');
                    }

                    if (!string.IsNullOrEmpty(token))
                    {
                        await SecureStorage.SetAsync(AuthConstants.TokenKey, token);
                        await GuardarDatosDelToken(token);
                        return true;
                    }
                }
                Debug.WriteLine($"Error Login: {response.StatusCode} - {responseContent}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ocurrio un error inesperado al iniciar sesion: {ex.Message}");
                return false;
            }
        }

        public Task<string> GetTokenAsync()
        {
            return SecureStorage.GetAsync(AuthConstants.TokenKey);
        }

        public void Logout()
        {
            SecureStorage.Remove(AuthConstants.TokenKey);
        }

        private async Task GuardarDatosDelToken(string tokenString)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(tokenString);

            var sesion = new UserDataDto
            {
                Id = jwtToken.Claims.FirstOrDefault(a => a.Type == "nameid")?.Value,
                FullName = jwtToken.Claims.FirstOrDefault(a => a.Type == "unique_name")?.Value,
                Email = jwtToken.Claims.FirstOrDefault(a => a.Type == "email")?.Value,
                Role = jwtToken.Claims.FirstOrDefault(a => a.Type == "role")?.Value,
                ExpirationDate = jwtToken.ValidTo
            };

            string jsonSesion = JsonSerializer.Serialize(sesion);
            await SecureStorage.SetAsync("user_session", jsonSesion);

            Debug.WriteLine($"LEYENDO TOKEN");
            Debug.WriteLine($"ID: {sesion.Id}");
            Debug.WriteLine($"Nombre: {sesion.FullName}");
            Debug.WriteLine($"Email: {sesion.Email}");
            Debug.WriteLine($"Rol: {sesion.Role}");
            Debug.WriteLine($"Expira: {sesion.ExpirationDate}");
        }
        public async Task<UserDataDto> ObtenerSesion()
        {
            var jsonSesion = await SecureStorage.GetAsync("user_session");
            if (string.IsNullOrEmpty(jsonSesion)) return null;

            return JsonSerializer.Deserialize<UserDataDto>(jsonSesion);
        }
    }
}
