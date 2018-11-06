using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.Models.Entity
{
  public  class Account
    {
  //      ID int primary key identity(10000,1) not null,
  //OpenID varchar(64) not null default replace(newid(),'-',''),
  //UserID int not null ,
  //UserName varchar(32) not null,
  //[Password] varchar(32) not null,
  //CreateTime datetime default getdate(),
  //IsEnable bit default 1,
  //Remark varchar(1000),
  public int ID { get; set; }
        public string OpenID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsEnable { get; set; }
        public string Remark { get; set; }
        public string ReturnUrl { get; set; }
    }
}
