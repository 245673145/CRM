using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.Models
{
  public  class Pagination<T> where T :new()
    {
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public List<T> Data { get; set; }
        public int DataCount { get; set; }
    }
}
