using Demo.Api.Middleware;

namespace Demo.Api.Models
{
    public class Response<T>
    {
        public Response()
        {
        }

        public Response(T data)
        {
            Message = string.Empty;
            Errors = null;
            Data = data;
        }

        public Response(T data, string message)
        {
            Message = message;
            Errors = null;
            Data = data;
        }

        public T Data { get; set; }

        public string Message { get; set; }

        public List<ApiError> Errors { get; set; }
    }
}
