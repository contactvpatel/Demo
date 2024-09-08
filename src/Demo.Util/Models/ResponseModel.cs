namespace Demo.Util.Models
{
    public class ResponseModel
    {
        public bool Status { get; set; } = true;
        public object Data { get; set; }
        public string Message { get; set; }
        public int TotalRecords { get; set; } = 0;
    }
    public class ResponseModelList<T>
    {
        public List<T> Data { get; set; } = new();
        public string Responsefields { get; set; }
        public int TotalRecords { get; set; }
    }
    public class DynamicResponseModel
    {
        public dynamic Data { get; set; }
        public int TotalRecords { get; set; }
    }
}