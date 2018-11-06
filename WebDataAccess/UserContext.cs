using CRM.Web.Models;
using CRM.Web.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CRM.Web.DataAccess
{
    public class UserContext : DbContext
    {
        public UserContext() : base()
        {

        }
        public User FindUserByID(int userid)
        {
            return Find<User>("select * from [User] where id=@id", userid).FirstOrDefault();
        }
        public List<Role> FindRole(string rolename = "")
        {
            string sql = string.IsNullOrEmpty(rolename) ? "select * from Role" : "select * from role where rolename like '%" + rolename + "%'";
            return this.Find<Role>(sql);
        }
        public int DeleteUser(int userid)
        {
            return Save("delete from [user] where id=id", userid);
        }
        public object AddUser(User user)
        {
            string sql = "insert into [user] (Name,Address,Mobile,Email) values(@name,@address,@mobile,@email) select IDENT_CURRENT('User')";
            return this.FindOnly(sql, user.Name, user.Address, user.Mobile, user.Mobile);
        }
        public List<User> FindUserByRoleID(string roleid)
        {
            string sql = @"
select u.* from [User] u join Role_User r 
on u.ID=r.UserID join Account a on a.UserID=u.ID
where r.RoleID=@roleid";
            return Find<User>(sql, roleid);
        }
        public int AddRoleMenu(string roleid, string[] menuid)
        {
            StringBuilder sql = new StringBuilder("insert into Role_Menu values ");
            //SqlCommand cmd = new SqlCommand(Connection);
            // string sql = "insert into rolemenu values ";
            foreach (string item in menuid)
                sql.Append($"({roleid},{item}),");
            return Save(sql.ToString().TrimEnd(','));
        }
        public int DeleteRoleMenu(string roleid)
        {
            string sql = "delete from Role_Menu where roleid=@roleid";
            return Save(sql, roleid);
        }
        public int DeleteRole(string roleid)
        {
            string sql = "delete from role where id=@roleid";
            return Save(sql, roleid);
        }
        public int AddRoleUser(int userid, int roleid)
        {
            string sql = @"delete from role_user where userid=@userid
 insert into role_user values (@roleid,@user)";
            return Save(sql, userid, roleid, userid);
        }
        public int EnableUser(int userid, bool IsEnable)
        {
            string sql = "";
            if (IsEnable)
                sql = "update [User] set isenable=1 where id=@id  update Account set isenable=1 where userid=@i";
            else
                sql = "update [User] set isenable=0 where id=@id  update Account set isenable=0 where userid=@i";
            return Save(sql, userid, userid);
        }
        public int AdminUser(int userid, bool IsAdmin)
        {
            string sql = "";
            if (IsAdmin)
                sql = "update [User] set isadmin=1 where id=@id ";
            else
                sql = "update [User] set isadmin=0 where id=@id ";
            return Save(sql, userid);
        }
        public int UpdateLoginTime(string userid)
        {
            string sql = "update [User] set LastLoginTime=getdate() where id=@id";
            return Save(sql, userid);
        }
        public Pagination<User> FindUserPage(int pageindex, int pagesize)
        {
            string sql = @"select u.*,a.UserName,a.Password,l.RoleName from [User] u 
join Account a  on u.ID=a.UserID
join Role_User r on r.UserID=a.UserID
join Role l on l.ID=r.RoleID";
            return Find<User>(sql, pageindex, pagesize, "ID", false);
        }
        public User FindUser(int userid)
        {
            string sql = @"select u.*,a.UserName,a.Password,l.RoleName from [User] u 
join Account a  on u.ID=a.UserID
join Role_User r on r.UserID=a.UserID
join Role l on l.ID=r.RoleID where u.id=@userid";
            return Find<User>(sql, userid).FirstOrDefault();
        }
        public int UpdateUserMessage(User user)
        {
            string sql = @"update [User] set name=@name,address=@address,mobile=@mobile,email=@email where id=@userid
 update Account set password=@password where userid=@user";
            return Save(sql, user.Name, user.Address, user.Mobile, user.Email, user.ID, user.Password, user.UserID);
        }
        public int DeleteUser(string userid)
        {
            string sql = "delete from [user] where id=@u delete from account where userid=@id delete from role_user where userid=@d ";
            return Save(sql, userid, userid, userid);
        }
        public Role FindRole(int roleid)
        {
            string sql = "select * from role where id=@id";
            return Find<Role>(sql, roleid).FirstOrDefault();
        }

        public Role AddRole(Role role)

        {
            string sql = "insert role( rolename,OptionTime,Remark) values(@rolename,GETDATE(),'') select IDENT_CURRENT('Role')";
            object roleid = FindOnly(sql, role.RoleName);
            role.ID = Convert.ToInt32(roleid);
            return role;
        }

    }
}
