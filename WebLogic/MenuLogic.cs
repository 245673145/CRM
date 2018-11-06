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
    public class MenuLogic

    {
        private MenuContext mcontext { get; set; }
        public MenuLogic()
        {
            mcontext = new MenuContext();
        }
        public List<Dictionary<Menu, List<Menu>>> FindMenu(int useid)
        {
            List<Menu> selectmenu = mcontext.FindMenuList(useid);
            List<Dictionary<Menu, List<Menu>>> result = new List<Dictionary<Menu, List<Menu>>>();
            List<Menu> parentmenu = selectmenu.Where(s => s.ParentID == 0).ToList();
            foreach (var item in parentmenu)
            {
                Dictionary<Menu, List<Menu>> dic_menu = new Dictionary<Menu, List<Menu>>();
                List<Menu> childmenu = new List<Menu>();
                foreach (var child in selectmenu)
                {

                    if (child.ParentID == item.ID)
                        childmenu.Add(child);
                }
                dic_menu.Add(item, childmenu);
                result.Add(dic_menu);
            }
            return result;
        }
        public List<Dictionary<Menu, List<Menu>>> FindMenu()
        {
            List<Menu> selectmenu = mcontext.FindMenuList();
            List<Dictionary<Menu, List<Menu>>> result = new List<Dictionary<Menu, List<Menu>>>();
            List<Menu> parentmenu = selectmenu.Where(s => s.ParentID == 0).ToList();
            foreach (var item in parentmenu)
            {
                Dictionary<Menu, List<Menu>> dic_menu = new Dictionary<Menu, List<Menu>>();
                List<Menu> childmenu = new List<Menu>();
                foreach (var child in selectmenu)
                {

                    if (child.ParentID == item.ID)
                        childmenu.Add(child);
                }
                dic_menu.Add(item, childmenu);
                result.Add(dic_menu);
            }
            return result;
        }
        public List<Menu> FindMenuByRoleID(int roleid)
        {
            return mcontext.FindMenu(roleid);
        }
        public Message<Menu> ModifyMenu(Menu menu, string methordtype)
        {
            Message<Menu> result = new Message<Menu>();
            if (string.IsNullOrEmpty(menu.MenuAction))
                return new Message<Menu> { Messages = "未填写菜单地址", IsSuccess = false, Data = menu };
            if (string.IsNullOrEmpty(menu.MenuName))
               return new Message<Menu> { Messages = "菜单名称为空", IsSuccess = false, Data = menu };
            switch (methordtype)
            {
                case "add":
                    //表示一级菜单
                    if (menu.ParentID == 0)
                        menu.MenuCode = mcontext.FindIdenity() +".";
                    else
                        menu.MenuCode = menu.ParentID + "." + mcontext.FindIdenity() + ".";
                    int r = mcontext.AddMenu(menu);
                    result = new Message<Menu> { Messages = r > 0 ? "菜单添加成功" : "菜单添加失败", IsSuccess = r > 0, Data = menu };
                    break;
                case "update":
                    if (menu.ID == 0)
                        result = new Message<Menu> { Messages = "未指定相关ID", IsSuccess = false, Data = menu };
                    int t = mcontext.UpdateMenu(menu);
                    result = new Message<Menu> { Messages = t > 0 ? "修改菜单成功" : "菜单添加失败", IsSuccess = t > 0, Data = menu };
                    break;
                case "delete":
                    if (menu.ID == 0)
                        result = new Message<Menu> { Messages = "未指定相关ID", IsSuccess = false, Data = menu };
                    int j = mcontext.DeleteMenu(menu);
                    result = new Message<Menu> { Messages = j > 0 ? "删除菜单成功" : "删除菜单失败", IsSuccess = j > 0, Data = menu };
                    break;

            }
            return result;
        }
    }
}
