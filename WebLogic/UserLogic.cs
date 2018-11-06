using CRM.Web.DataAccess;
using CRM.Web.Models;
using CRM.Web.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.Logic
{
   public class UserLogic
    {
        private UserContext ucontext { get; set; }
        private AccountContext acontext { get; set; }
        public UserLogic()
        {
            ucontext = new UserContext();
            acontext = new AccountContext();
        }
        public List<Role> GetRole(string rolename = "")
        {
            return ucontext.FindRole(rolename);
        }
        public bool AddRoleMenu(string roleid, string menustr)
        {
            if (string.IsNullOrEmpty(roleid) || string.IsNullOrEmpty(menustr))
                return false;
            //if (menustr.IndexOf(',') <= -1)
            //    return false;
            //删除原有权限
            ucontext.DeleteRoleMenu(roleid);
            string[] menuid = menustr.TrimEnd(',').Split(',');
            return ucontext.AddRoleMenu(roleid, menuid)==menuid.Length;            
        }
        public bool DeleteRole(string roleid)
        {
            return ucontext.DeleteRole(roleid)>0;
        }
        public List<User> GetUserByRoleID(string roleid)
        {
            return ucontext.FindUserByRoleID(roleid);
        }
        public Pagination<User> FindPage(int pageindex, int pagesize)
        {
            pageindex = pageindex == 0 ? 1 : pageindex;
            pagesize = pagesize == 0 ? 20 : pagesize;
            return ucontext.FindUserPage(pageindex, pagesize);
        }
        public bool DeleteUser(int userid)
        {
            if (userid == 0)
                return false;
            return ucontext.DeleteUser(userid + "")>0;
        }
        public bool EnableUser(int userid,string flag)
        {
            return ucontext.EnableUser(userid, flag == "isstart") >0;
        }
        public bool IsAdminUser(int userid, string flag)
        {
            return ucontext.AdminUser(userid, flag == "yes") > 0;
        }
        public Message<Role> AddRole(string rolename)
        {
            Message<Role> result = new Message<Role>();
            if (string.IsNullOrEmpty(rolename))
                return new Message<Role> { IsSuccess = false, Messages = "角色名称为空" };
            Role tr = ucontext.FindRole(rolename).FirstOrDefault();
            if(tr!=null)
                return new Message<Role> { IsSuccess = false, Messages = "角色名称已经存在" };
            Role adddata = new Role { RoleName = rolename };
            Role temp = ucontext.AddRole(adddata);
            result.Data = temp;
            result.IsSuccess = true;
            result.Messages = "添加成功";
            return result;
        }
        public Message<User> ModifyUser(User user, string methodtype,string confimpassword)
        {
            Message<User> result = new Message<User>();
            if (string.IsNullOrEmpty(user.Name))
                return new Message<User> { Messages = "姓名为空",IsSuccess=false };
            if (string.IsNullOrEmpty(user.UserName))
                return new Message<User> { Messages = "用户名不能为空", IsSuccess = false };
            if (string.IsNullOrEmpty(user.Mobile))
                return new Message<User> { Messages = "电话号码不能为空", IsSuccess = false };
            if (string.IsNullOrEmpty(user.Password))
                return new Message<User> { Messages = "密码为空", IsSuccess = false };
            if(string.IsNullOrEmpty(user.RoleName))
                return new Message<User> { Messages = "角色未选择", IsSuccess = false };
            switch (methodtype)
            {
                case "add":
                    if (string.IsNullOrEmpty(confimpassword) || confimpassword != user.Password)
                        return new Message<User> { Messages = "确认密码为空或者两次密码的值不相同", IsSuccess = false };
                    //添加用户
                    //验证用户名是否重复
                    object count = acontext.FindAccount(user.UserName);
                    if(Convert.ToInt32(count)>0)
                        return new Message<User> { Messages = "已存在该用户，请使用其他用户名称",IsSuccess=false };
                    object userid = ucontext.AddUser(user);
                    if(string.IsNullOrEmpty(userid+""))
                        return new Message<User> { Messages = "添加用户失败", IsSuccess = false };
                    //添加账户
                    user.ID = Convert.ToInt32(userid);
                    Account account = new Account();
                    account.UserID = user.ID;
                    account.UserName = user.UserName;
                    account.Password = user.Password;
                    account.Remark = "";
                    int t= acontext.Add(account);
                    if (t <= 0)
                    {
                        ucontext.DeleteUser(account.UserID);
                        return new Message<User> { Messages = "添加账号失败", IsSuccess = false };
                    }
                    //添加角色
                    ucontext.AddRoleUser(account.UserID, Convert.ToInt32(user.RoleName));
                    User data = ucontext.FindUser(user.ID);
                    result = new Message<User> { IsSuccess = true, Messages = "添加用户成功",Data=data };
                    break;
                case "update":
                    if(user.ID==0)
                        return new Message<User> { Messages = "参数错误", IsSuccess = false };
                    User tempuser = ucontext.FindUserByID(user.ID);
                    if(user.UserName!=tempuser.UserName)
                        return new Message<User> { Messages = "用户名称禁止修改", IsSuccess = false };
                    if(!tempuser.IsAdmin)
                        return new Message<User> { Messages = "无权限进行此操作", IsSuccess = false };
                  int usernumber=   ucontext.UpdateUserMessage(user);
                    if (usernumber >= 2)
                    {
                        User uda = ucontext.FindUser(tempuser.ID);
                        result = new Message<User> { IsSuccess = true, Messages = "修改用户信息成功",Data=uda};
                    }
                    break;
            }
            return result;
        }
    }
}
