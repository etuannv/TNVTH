using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TNVTH.Web.Models;
using TNVTH.Web.Utilities;

namespace TNVTH.Web.Services
{
    public class T_MenuServices : IT_MenuServices
    {
        private TNVTHEntities _dataContext;

        public T_MenuServices()
        {
            _dataContext = new TNVTHEntities();
        }

        public IEnumerable<T_Menu> GetAll()
        {
            return from m in _dataContext.T_Menu
                   select m;
        }

        public List<T_Menu> GetAllForDisplay(IEnumerable<T_Menu> excepMenuList = null, string searchKey = null)
        {
            IEnumerable<T_Menu> MenuList = _dataContext.T_Menu;
            if (!string.IsNullOrEmpty(searchKey))
            {
                string searchSlug = searchKey.Replace(' ', '-');
                MenuList = MenuList.Where(
                    m => m.Title.Contains(searchKey) || m.Slug.Contains(searchSlug));
            }

            // Remove exept list
            if (excepMenuList != null)
            {
                MenuList = MenuList.Except(excepMenuList);
            }

            // Modify title
            foreach (var item in MenuList)
            {
                string AddString = "";
                int count = item.ParentPath.Count(f => f == ';');
                for (int i = 0; i < count; i++)
                {
                    AddString += "— ";
                }
                item.Title = AddString + item.Title;
                //Add my id to parrent path
                item.ParentPath = item.ParentPath + ";" + item.ID;
            }
            return MenuList.OrderBy(m => m.ParentPath).ThenBy(i => i.Order).ToList();
        }

        public T_Menu GetByID(int id)
        {
            return _dataContext.T_Menu.Where(m => m.ID == id).SingleOrDefault();
        }
        public IEnumerable<T_Menu> GetChildren(int? id)
        {
            return (from m in _dataContext.T_Menu
                    where m.ParentID == id && m.Enable == true
                    select m).OrderBy(o => o.Order);
        }

        public bool IsExist(T_Menu iMenu)
        {
            T_Menu MenuFound = _dataContext.T_Menu.Where(m => m.ID != iMenu.ID && (m.Title == iMenu.Title || m.Slug == iMenu.Slug)).SingleOrDefault();
            return (MenuFound != null) ? true : false;
        }

        public T_Menu AddNewMenuAndReturn(T_Menu iMenu)
        {
            //Check exist
            T_Menu MenuFound = _dataContext.T_Menu.Where(m => m.Title == iMenu.Title || m.Slug == iMenu.Slug).SingleOrDefault();
            //Return exist Menu
            if (MenuFound != null) return MenuFound;
            else
            {
                _dataContext.T_Menu.Add(iMenu);
                _dataContext.SaveChanges();
                return iMenu;
            }
        }

        public ReturnValue<bool> AddNewMenu(T_Menu iMenu)
        {
            if (IsExist(iMenu)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            if (string.IsNullOrEmpty(iMenu.Title) || string.IsNullOrEmpty(iMenu.Slug)) return new ReturnValue<bool>(false, "Dữ liệu không đúng");
            try
            {
                _dataContext.T_Menu.Add(iMenu);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> UpdateMenu(T_Menu iMenu)
        {
            if (IsExist(iMenu)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            if (string.IsNullOrEmpty(iMenu.Title) || string.IsNullOrEmpty(iMenu.Slug)) return new ReturnValue<bool>(false, "Dữ liệu không đúng");
            try
            {
                _dataContext.Entry(iMenu).State = EntityState.Modified;
                return new ReturnValue<bool>(_dataContext.SaveChanges() > 0, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteMenu(T_Menu iMenu)
        {
            try
            {
                _dataContext.T_Menu.Remove(iMenu);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> DeleteMenu(int id)
        {
            try
            {
                if (HasChild(id)) return new ReturnValue<bool>(false, "Không thể xóa, vì có mục con");
                T_Menu DelMenu = GetByID(id);
                return DeleteMenu(DelMenu);
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }


        public bool HasChild(int id)
        {
            var q = (from m in _dataContext.T_Menu
                     where m.ParentID == id
                     select m).FirstOrDefault();
            if (q != null) return true;
            else return false;
        }

        public string GetPath(int? parentID)
        {
            string Result = "";
            if (parentID != null)
            {
                T_Menu Parent = GetByID((int)parentID);
                Result += Parent.ParentPath;
                Result += ";";
                Result += parentID;
            }
            else
            {
                Result = "0";
            }

            return Result;
        }
    }
}
