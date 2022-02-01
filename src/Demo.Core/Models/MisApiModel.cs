namespace Demo.Core.Models
{
    public class MisApiModel
    {
        public string Url { get; set; }
        public MisEndpoint Endpoint { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }

    public class MisEndpoint
    {
        public string Department { get; set; }
        public string Entity { get; set; }
        public string RoleType { get; set; }
        public string Role { get; set; }
        public string Position { get; set; }
        public string PersonPosition { get; set; }
    }

    public class MisResponse<T>
    {
        public bool Succeeded { get; set; }
        public List<T> Data { get; set; }
        public string Message { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string ErrorId { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
