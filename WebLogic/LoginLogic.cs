using CRM.Web.DataAccess;
using CRM.Web.Models;
using CRM.Web.Models.Entity;
using System;

namespace CRM.Web.Logic
{
    public  class LoginLogic
    {
        private AccountContext acontext { get; set; }
        private UserContext ucontext { get; set; }
        public LoginLogic()
        {
            acontext = new AccountContext();
            ucontext = new UserContext();

        }
        public Message<User> Login(String password, string username)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
                return new Message<User> { Messages = "用户或者密码不能为空", Data = new User(), IsSuccess = false };
            if(password.Length<6)
                return new Message<User> { Messages = "密码长度最小为6", Data = new User(), IsSuccess = false };
            if (password.Length > 16)
                return new Message<User> { Messages = "密码长度最大为16", Data = new User(), IsSuccess = false };
            Account account = acontext.FindAccount(username, password);
            if (account == null)
                return new Message<User> { Messages = "未能查询到相关登录信息", Data = new User(), IsSuccess = false };
            if (!account.IsEnable)
                return new Message<User> { Messages = "该账号已被禁用", Data = new User(), IsSuccess = false };
            User user = ucontext.FindUserByID(account.UserID);
            if (user == null)
                return new Message<User> { Messages = "用户信息不存在", Data = new User(), IsSuccess = false };
            if (!user.IsEnable)
                return new Message<User> { Messages = "用户已被禁用", Data = new User(), IsSuccess = false };
            ucontext.UpdateLoginTime(user.ID+"");
            return new Message<User> { Messages = "登录成功", Data = user, IsSuccess = true };

        }
    }
}
