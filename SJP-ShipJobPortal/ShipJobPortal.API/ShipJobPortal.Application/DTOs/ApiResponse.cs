using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShipJobPortal.Application.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]

        public T? Data { get; set; }

        public ApiResponse(bool success, T? data = default, string? message = null, string? errorCode = null)
        {
            Success = success;
            Message = message;
            ErrorCode = errorCode;
            Data = data;
        }
    }
}