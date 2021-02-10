using System.Collections.Generic;
using Demo.Util.Middleware;

namespace Demo.Util.Models
{
    public class Response<T>
    {
        public Response()
        {
        }

        public Response(T data)
        {
            Succeeded = true;
            Message = string.Empty;
            Errors = null;
            Data = data;
        }
        public Response(T data, string message)
        {
            Succeeded = true;
            Message = message;
            Errors = null;
            Data = data;
        }

        public Response(bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
            Errors = null;
        }

        public Response(T data, bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
            Errors = null;
            Data = data;
        }

        public Response(T data, bool succeeded, string message, List<ApiError> errors)
        {
            Succeeded = succeeded;
            Message = message;
            Errors = errors;
            Data = data;
        }

        public bool Succeeded { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }

        public List<ApiError> Errors { get; set; }
    }
}
