using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ranger.Dtos;

namespace Ranger.Services
{
    internal class ApiService
    {
       
        private readonly HttpClient _client;

        public ApiService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:3000/api/v0/")
            };
        }

        public async Task<ApiResponseDto<LogDatesDto>> GetAviableLogDatesAsync() => await GetRequestAsync<LogDatesDto>("dev/logs");
        public async Task<ApiResponseDto<LogsDto>> GetLogsByDateAsync(string date) => await GetRequestAsync<LogsDto>($"dev/logs/{date}");

        private async Task<ApiResponseDto<T>> GetRequestAsync<T>(string route)
        {
            var response = await _client.GetAsync(route);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ApiResponseDto<T>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new Exception("Invalid API response");
        }
    }
}
