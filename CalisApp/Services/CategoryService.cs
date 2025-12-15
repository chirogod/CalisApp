using CalisApp.Models;
using CalisApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CalisApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IAuthService _authService;

        public CategoryService(IAuthService authService)
        {
            _authService = authService;
        }

        public const string UrlApi = "https://calisapi-b6gxc6cuf6a7apes.brazilsouth-01.azurewebsites.net/api/category";

        public async Task<IEnumerable<Category>> GetAllCategories()
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
                var categories = JsonSerializer.Deserialize<IEnumerable<Category>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return categories ?? Enumerable.Empty<Category>();
            }
            catch (JsonException ex)
            {
                throw new Exception("Error al deserializar la respuesta de categorias.", ex);

            }
        }
    }
}
