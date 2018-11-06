using CRM.Web.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Web.DataAccess
{
    public class MenuContext : DbContext
    {
        public MenuContext() : base()
        {

        }
        public int AddMenu(Menu menu)
        {
            string sql = @"insert into Menu(MenuName,MenuCode,MenuAction,ParentID,MenuIcon,SortIndex,IsEnable) values
(@menuname,@menucode,@menuaction,@parentID,'',0,@iseable)";
            return Save(sql, menu.MenuName, menu.MenuCode, menu.MenuAction, menu.ParentID, menu.IsEnable);
        }
        public int UpdateMenu(Menu menu)
        {
            string sql = "update menu set Menuname=@menuname,MenuAction=@action where id=@id";
            return Save(sql, menu.MenuName, menu.MenuAction, menu.ID);
        }
        public int DeleteMenu(Menu menu)
        {
            string sql = "update menu set isdelete=1 where id=@id";
            return Save(sql, menu.ID);
        }
        public object FindIdenity()
        {
            return FindOnly("select IDENT_CURRENT('Menu')+1");
        }
        public List<Menu> FindMenuList(int userid)
        {
            string sql = @"select distinct e.* from Role r 
join Role_User u on u.RoleID=r.ID 
join Role_Menu m on m.RoleID=u.RoleID
join Menu e on e.ID=m.MenuID
where u.UserID=@userid and e.IsDelete=0 and e.IsEnable=1";
            return this.Find<Menu>(sql, userid);
        }
        public List<Menu> FindMenuList()
        {
            string sql = @"select * from menu e where e.IsDelete=0 ";
            return this.Find<Menu>(sql);
        }
        public List<Menu> FindMenu(int roleid)
        {
            string sql = @"select m.* from Role_Menu r join Menu m on r.MenuID=m.ID where r.RoleID=@roleid";
            return this.Find<Menu>(sql,roleid);
        }
    }
}
