using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
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
                BaseAddress = new Uri("http://ranger.zoller.dev/api/v0/"),
            };
            _client.DefaultRequestHeaders.Add("x-dev-password", "gyere_gyere_kismadar");
        }

        public class ApiResult<T>
        {
            public bool IsSuccess { get; set; }
            public T? Data { get; set; }
            public string? ErrorMessage { get; set; }
        }


        public async Task<ApiResponseDto<LogDatesDto>> GetAviableLogDatesAsync() => await GetRequestAsync<LogDatesDto>("dev/logs");
        public async Task<ApiResponseDto<LogsDto>> GetLogsByDateAsync(string date) => await GetRequestAsync<LogsDto>($"dev/logs/{date}");
        public async Task<ApiResponseDto<DatabaseDto>> GetDatabaseDumpAsync() => await GetRequestAsync<DatabaseDto>($"dev/databasedump");

        private async Task<ApiResult<T>> GetRequestAsync<T>(string route)
        {
            try
            {
                var response = await _client.GetAsync(route);

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResult<T>
                    {
                        IsSuccess = false,
                        ErrorMessage = $"HTTP {(int)response.StatusCode}"
                    };
                }

                var json = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<ApiResponseDto<T>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data == null)
                {
                    return new ApiResult<T>
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid API response"
                    };
                }

                return new ApiResult<T>
                {
                    IsSuccess = true,
                    Data = data.Data // vagy ami nálad a payload
                };
            }
            catch (HttpRequestException)
            {
                return new ApiResult<T>
                {
                    IsSuccess = false,
                    ErrorMessage = "Nem sikerült csatlakozni a szerverhez"
                };
            }
            catch (TaskCanceledException)
            {
                return new ApiResult<T>
                {
                    IsSuccess = false,
                    ErrorMessage = "Timeout"
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<T>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public void SetStaticApiKey(string password) // ezt még implementálni kell... amikor elindítod az api-t, akkor kelljen megadni a key-t. szól ha nem jó.
        {
                _client.DefaultRequestHeaders.Remove("x-dev-password");
                _client.DefaultRequestHeaders.Add("x-dev-password", password);
        }
    }
}
