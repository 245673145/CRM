using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.Models.Entity
{
    public class ImportRecord
    {

        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// ImportTime
        /// </summary>
        public DateTime? ImportTime
        {
            get;
            set;
        }
        /// <summary>
        /// FileName
        /// </summary>
        public string FileName
        {
            get;
            set;
        }
        /// <summary>
        /// SuccessCount
        /// </summary>
        public int? SuccessCount
        {
            get;
            set;
        }
        /// <summary>
        /// TotalCount
        /// </summary>
        public int? TotalCount
        {
            get;
            set;
        }

    }
}
