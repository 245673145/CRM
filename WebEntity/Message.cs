using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.Models
{
  public  class Message
    {
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
        public string Messages { get; set; }
        public string MessageCode { get; set; }
    }
    public class Message<T> where T : new()
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Messages { get; set; }
        public string MessageCode { get; set; }
    }

}
