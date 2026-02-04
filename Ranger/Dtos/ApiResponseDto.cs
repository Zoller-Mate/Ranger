using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranger.Dtos
{
    internal class ApiResponseDto<T>
    {
        public string Status {  get; set; }
        public string Timestamp { get; set; }
        public T Data { get; set; }
    }
}
