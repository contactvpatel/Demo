using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Util.Models
{
    public class HttpResponseModel
    {
        public bool Status { get; set; } = true;
        public object Data { get; set; }
        public string Message { get; set; }
        public int TotalRecords { get; set; } = 0;
    }
    public class ResponseToModel<T>
    {
        public T Data { get; set; }
        public int TotalRecords { get; set; }
    }
    public class ListResponseToModel<T>
    {
        public List<T> Data { get; set; } = new();
        public string Responsefields { get; set; }
        public int TotalRecords { get; set; }
    }
    public class ResponseToDynamicModel
    {
        public dynamic Data { get; set; }
        public int TotalRecords { get; set; }
    }
    public class ResponseToDynamicModelDapper<T>
    {
        public List<T> Data { get; set; }
        public int TotalRecords { get; set; }
    }
}