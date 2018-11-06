using CRM.Web.DataAccess.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.DataAccess
{
   public class ImportContext:ExcelContext
    {
        public MemoryStream WriteError(MemoryStream ms, List<string[]> erro)
        {
            return WriteSheet(ms,"错误数据", 1, 0, erro);
        }
    }
}
