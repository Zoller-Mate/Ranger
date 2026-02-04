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

        public async Task<ApiResponseDto<List<LogDateDto>>> GetAviableLogDatesAsync()
        {
            var response = await _client.GetAsync("dev/logs");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<ApiResponseDto<List<LogDateDto>>>(json) ?? throw new Exception("Invalid API response");
        }
        
    }
}
