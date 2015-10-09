using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TNVTH.Web.Models;
using TNVTH.Web.Utilities;

namespace TNVTH.Web.Services
{
    public class UserManagerServices : IUserManagerServices
    {
        private TNVTHEntities _dataContext;

        public UserManagerServices()
        {
            _dataContext = new TNVTHEntities();
        }

        public IEnumerable<T_UserProfile> GetAll()
        {
            return from m in _dataContext.T_UserProfile
                   select m;
        }

        public T_UserProfile GetByID(int id)
        {
            return _dataContext.T_UserProfile.Where(m => m.UserId == id).SingleOrDefault();
        }

        public bool IsExist(T_UserProfile iT_UserProfiles)
        {
            T_UserProfile T_UserProfilesFound = _dataContext.T_UserProfile.Where(m => m.UserId != iT_UserProfiles.UserId && m.UserName == iT_UserProfiles.UserName).SingleOrDefault();
            return (T_UserProfilesFound != null) ? true : false;
        }

        public T_UserProfile AddNewT_UserProfileAndReturn(T_UserProfile iT_UserProfiles)
        {
            //Check exist
            T_UserProfile T_UserProfilesFound = _dataContext.T_UserProfile.Where(m => m.UserName == iT_UserProfiles.UserName).SingleOrDefault();
            //Return exist T_UserProfiles
            if (T_UserProfilesFound != null) return T_UserProfilesFound;
            else
            {
                _dataContext.T_UserProfile.Add(iT_UserProfiles);
                _dataContext.SaveChanges();
                return iT_UserProfiles;
            }
        }

        public ReturnValue<bool> AddNewT_UserProfile(T_UserProfile iT_UserProfiles)
        {
            if (IsExist(iT_UserProfiles)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            if (string.IsNullOrEmpty(iT_UserProfiles.UserName)) return new ReturnValue<bool>(false, "Dữ liệu không đúng");
            try
            {
                _dataContext.T_UserProfile.Add(iT_UserProfiles);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> UpdateT_UserProfile(T_UserProfile iT_UserProfiles)
        {
            if (IsExist(iT_UserProfiles)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            if (string.IsNullOrEmpty(iT_UserProfiles.UserName)) return new ReturnValue<bool>(false, "Dữ liệu không đúng");
            try
            {
                _dataContext.Entry(iT_UserProfiles).State = EntityState.Modified;
                return new ReturnValue<bool>(_dataContext.SaveChanges() > 0, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteT_UserProfile(T_UserProfile iT_UserProfiles)
        {
            try
            {
                //Delete role
                //Delete all previous role
                RemoveUserRole(iT_UserProfiles.UserId);
                //Delete webpage membership
                webpages_Membership membership = _dataContext.webpages_Membership.Where(m=>m.UserId == iT_UserProfiles.UserId).SingleOrDefault();
                if(membership != null)
                {
                    _dataContext.webpages_Membership.Remove(membership);
                }
                
                _dataContext.SaveChanges();

                //Delete this profile
                _dataContext.T_UserProfile.Remove(iT_UserProfiles);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> DeleteT_UserProfile(int id)
        {
            try
            {
                T_UserProfile DelT_UserProfiles = GetByID(id);
                return DeleteT_UserProfile(DelT_UserProfiles);
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }


        public IEnumerable<T_UserProfile> Search(string search)
        {
            IEnumerable<T_UserProfile> ResultList = GetAll();
            if(!string.IsNullOrEmpty(search))
            {
                ResultList = ResultList.Where(m=> m.UserName.Contains(search) || m.Fullname.Contains(search) || m.Mobile.Contains(search) || m.Email.Contains(search));
            }
            return ResultList.Where(m=> m.UserName != "admin").OrderBy(a=>a.UserId);
        }

        public T_UserProfile GetByUsername(string username)
        {
            return _dataContext.T_UserProfile.Where(m => m.UserName == username).SingleOrDefault();
        }

        internal System.Collections.IEnumerable GetRoleList()
        {
            return _dataContext.webpages_Roles.OrderBy(m=>m.RoleId);
        }

        public ReturnValue<bool> SetUserInrole(int userId, int roleID)
        {
            
            try
            {
                //Delete all previous role
                RemoveUserRole(userId);
                var MyRole = new webpages_UsersInRoles();
                MyRole.RoleId = roleID;
                MyRole.UserId = userId;
                _dataContext.webpages_UsersInRoles.Add(MyRole);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        private void RemoveUserRole(int userId)
        {
            var q = from m in _dataContext.webpages_UsersInRoles
                    where m.UserId == userId
                    select m;
            _dataContext.webpages_UsersInRoles.RemoveRange(q);
            _dataContext.SaveChanges();
        }

        public webpages_Roles GetRoleByUserId(int userId)
        {
            var q = (from m in _dataContext.webpages_UsersInRoles
                     join n in _dataContext.webpages_Roles on m.RoleId equals n.RoleId
                     where m.UserId == userId
                     select n).FirstOrDefault();
            return q;
        }
    }
}
