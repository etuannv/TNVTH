using System.Collections.Generic;
using TNVTH.Web.Utilities;
using TNVTH.Web.Models;


namespace TNVTH.Web.Services
{
    public interface IUserManagerServices
    {
        IEnumerable<T_UserProfile> GetAll();
        T_UserProfile GetByID(int id);
        ReturnValue<bool> AddNewT_UserProfile(T_UserProfile iUserProfile);
        T_UserProfile AddNewT_UserProfileAndReturn(T_UserProfile iUserProfile);
        ReturnValue<bool> UpdateT_UserProfile(T_UserProfile iUserProfile);
        ReturnValue<bool> DeleteT_UserProfile(T_UserProfile iUserProfile);
        ReturnValue<bool> DeleteT_UserProfile(int id);
        T_UserProfile GetByUsername(string username);
    }
}
