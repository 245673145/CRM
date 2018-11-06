using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.Models.Entity
{
   public class Menu
    {
//        create table Menu
//(
// ID int primary key identity(10000,1) not null,
// MenuName varchar(100) not null,
// MenuCode varchar(200) not null,
// MenuAction varchar(1000) default '',
// ParentID int not null default 0,
// MenuIcon varchar(50) ,
// OptionTime datetime default getdate(),
// SortIndex int default 1, 
// IsEnable bit default 1,
// IsDelete bit default 0
//)
  public int ID { get; set; }
  public string MenuName { get; set; }
  public string MenuCode { get; set; }
        public string MenuAction { get; set; }
        public int ParentID { get; set; }
        public string MenuIcon { get; set; }
        public DateTime OptionTime { get; set; }
        public int SortIndex { get; set; }
        public bool IsEnable { get; set; } = true;
        public bool IsDelete { get; set; } = false;
    }
}
