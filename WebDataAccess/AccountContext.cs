using CRM.Web.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.DataAccess
{
   public class AccountContext:DbContext
    {
        public AccountContext():base()
        {

        }
        public int Add(Account account)
        {
            //SqlConnection conn = new SqlConnection(Connection);
            //conn.Open();
            //SqlCommand cmd = conn.CreateCommand();
            //cmd.Transaction = conn.BeginTransaction();
            string sql = "insert into Account(userid,username,password,remark) values (@userid,@username,@password,@remark)";           
            int result= Save(sql, account.UserID, account.UserName,account.Password, account.Remark);
            return result;
        }
        public Account FindAccount(string username, string password)
        {
            string sql = "select * from account where username=@username and password=@password";
            return Find<Account>(sql, username, password).FirstOrDefault();
        }
        public object FindAccount(string username)
        {
            string sql = "select count(1)  from account where username=@username";
            return FindOnly(sql, username);
        }
    }
}
