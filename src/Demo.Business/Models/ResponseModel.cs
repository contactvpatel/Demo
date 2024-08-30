using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Business.Models
{
    public class ResponseModel
    {
        public bool status { get; set; }
        public object data { get; set; }
        public string message { get; set; }
    }
}