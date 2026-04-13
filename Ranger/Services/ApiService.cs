using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Ranger.Dtos;
using Ranger.Properties;

namespace Ranger.Services
{
    internal class ApiService
    {
        private readonly HttpClient _client;

        public ApiService()
        {
            
            _client = new HttpClient
            {
                //BaseAddress = new Uri(Settings.Default.ServerAddress)
                BaseAddress = new Uri("http://ranger.zoller.dev/api/v1/")
            };

            //_client.DefaultRequestHeaders.Add("x-dev-password", "gyere_gyere_kismadar");
            _client.DefaultRequestHeaders.Add("x-dev-password", Settings.Default.DevApiKey);
        }

        // ===== RESULT WRAPPER =====
        public class ApiResult<T>
        {
            public bool IsSuccess { get; set; }
            public T? Data { get; set; }
            public string ErrorMessage { get; set; } = "";
        }

        // ===== PUBLIC METHODS =====
        public Task<ApiResult<LogDatesDto>> GetAviableLogDatesAsync()
            => GetRequestAsync<LogDatesDto>("dev/logs");

        public Task<ApiResult<LogsDto>> GetLogsByDateAsync(string date)
            => GetRequestAsync<LogsDto>($"dev/logs/{date}");

        public Task<ApiResult<DatabaseDto>> GetDatabaseDumpAsync()
            => GetRequestAsync<DatabaseDto>("dev/databasedump");


        // ===== CORE REQUEST HANDLER =====
        private async Task<ApiResult<T>> GetRequestAsync<T>(string route)
        {
            try
            {
                var response = await _client.GetAsync(route);

                var json = await response.Content.ReadAsStringAsync();

                // ===== SUCCESS =====
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponseDto<T>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (apiResponse == null)
                    {
                        return Fail<T>("Invalid API response");
                    }

                    return new ApiResult<T>
                    {
                        IsSuccess = true,
                        Data = apiResponse.Data
                    };
                }

                // ===== ERROR (HTTP != 2xx) =====
                var errorMessage = ExtractErrorMessage(json);

                return Fail<T>($"HTTP {(int)response.StatusCode}: {errorMessage}");
            }
            catch (HttpRequestException)
            {
                return Fail<T>("Nem sikerült csatlakozni a szerverhez.");
            }
            catch (TaskCanceledException)
            {
                return Fail<T>("A kérés timeoutolt.");
            }
            catch (Exception ex)
            {
                return Fail<T>($"Unexpected error: {ex.Message}");
            }
        }


        // ===== ERROR PARSER =====
        private string ExtractErrorMessage(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "Unknown error";

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // common patterns
                if (root.TryGetProperty("message", out var message))
                    return message.GetString() ?? "Unknown error";

                if (root.TryGetProperty("error", out var error))
                    return error.GetString() ?? "Unknown error";

                if (root.TryGetProperty("title", out var title))
                    return title.GetString() ?? "Unknown error";

                // fallback
                return json;
            }
            catch
            {
                return json; // nem JSON → raw text
            }
        }


        // ===== HELPER =====
        private ApiResult<T> Fail<T>(string message)
        {
            return new ApiResult<T>
            {
                IsSuccess = false,
                ErrorMessage = message
            };
        }


        // ===== API KEY =====
        public void SetStaticApiKey(string password)
        {
            _client.DefaultRequestHeaders.Remove("x-dev-password");
            _client.DefaultRequestHeaders.Add("x-dev-password", password);
        }
    }
}