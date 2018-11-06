using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.Models.Entity
{
    public class User:Account
    {
        public  int ID { get; set; }
        public new string  OpenID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string LastIP { get; set; }
        public DateTime LastLoginTime { get; set; }
        public bool IsEnable { get; set; }
        public bool IsAdmin { get; set; }
        public string RoleName { get; set; }
    }
}
