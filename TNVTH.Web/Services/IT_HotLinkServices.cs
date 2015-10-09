using System.Collections.Generic;
using TNVTH.Web.Utilities;
using TNVTH.Web.Models;

namespace TNVTH.Web.Services
{
    public interface IT_HotLinkServices
    {
        IEnumerable<T_HotLink> GetAll();
        T_HotLink GetByID(int id);
        ReturnValue<bool> AddNewHotLink(T_HotLink iHotLink);
        T_HotLink AddNewHotLinkAndReturn(T_HotLink iHotLink);
        ReturnValue<bool> UpdateHotLink(T_HotLink iHotLink);
        ReturnValue<bool> DeleteHotLink(T_HotLink iHotLink);
        ReturnValue<bool> DeleteHotLink(int id);
        IEnumerable<T_HotLink> HotLinkSearch(string type, string term);
        IEnumerable<T_HotLink> GetByType(string type, int limit);
    }
}
