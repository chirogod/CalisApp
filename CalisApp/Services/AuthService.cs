using CalisApp.Models.DTOs;
using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Debug.WriteLine($"🌐 RESPUESTA API: {responseContent}");
                if (response.IsSuccessStatusCode)
                {
                    string token = string.Empty;

                    // OPCIÓN A: Si la respuesta empieza con '{', es un JSON
                    if (responseContent.Trim().StartsWith("{"))
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, options);
                        token = loginResponse?.Token;
                    }
                    // OPCIÓN B: Si no es JSON, asumimos que ES el token directamente
                    else
                    {
                        // Quitamos comillas dobles si vienen (ej: "eyJ...")
                        token = responseContent.Trim('"');
                    }

                    // Si conseguimos un token válido, lo guardamos
                    if (!string.IsNullOrEmpty(token))
                    {
                        await SecureStorage.SetAsync(AuthConstants.TokenKey, token);
                        return true;
                    }
                }
                Debug.WriteLine($"❌ Error Login: {response.StatusCode} - {responseContent}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ocurrió un error inesperado al iniciar sesión: {ex.Message}");
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

    }
}
