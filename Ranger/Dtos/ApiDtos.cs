
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ranger.Dtos
{
    internal class ApiResponseDto<T>
    {
        public string Status { get; set; }
        public string Timestamp { get; set; }
        public T Data { get; set; }
    }

    internal class LogDatesDto
    {
        public List<string> Dates { get; set; }
    }

    internal class LogsDto
    {
        public List<Log> Logs { get; set; }
    }

    internal class Log { 
        public string Timestamp { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
        public string ResponseTime { get; set; }
    }

}
