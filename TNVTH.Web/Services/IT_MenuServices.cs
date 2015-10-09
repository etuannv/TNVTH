using System.Collections.Generic;
using TNVTH.Web.Utilities;
using TNVTH.Web.Models;


namespace TNVTH.Web.Services
{
    public interface IT_MenuServices
    {
        IEnumerable<T_Menu> GetAll();

        List<T_Menu> GetAllForDisplay(IEnumerable<T_Menu> excepMenuList = null, string searchKey = null);
        T_Menu GetByID(int id);
        IEnumerable<T_Menu> GetChildren(int? id);
        bool HasChild(int id);
        ReturnValue<bool> AddNewMenu(T_Menu iMenu);
        T_Menu AddNewMenuAndReturn(T_Menu iMenu);
        ReturnValue<bool> UpdateMenu(T_Menu iMenu);
        ReturnValue<bool> DeleteMenu(T_Menu iMenu);
        ReturnValue<bool> DeleteMenu(int id);
        string GetPath(int? parentID);
    }
}
