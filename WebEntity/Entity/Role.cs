using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.Models.Entity
{
    public class Role
    {
        //      ID int primary key identity(10000,1) not null,
        //RoleName varchar(50) not null,
        //OptionTime datetime default getdate(),
        //Remark varchar(200)
        public int ID { get; set; }
        public string RoleName { get; set; }
        public DateTime OptionTime { get; set; }
        public string Remark { get; set; }
    }
}
