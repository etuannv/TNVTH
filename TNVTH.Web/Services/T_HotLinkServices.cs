using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TNVTH.Web.Models;
using TNVTH.Web.Utilities;

namespace TNVTH.Web.Services
{
    public class T_HotLinkServices : IT_HotLinkServices
    {
        private TNVTHEntities _dataContext;

        public T_HotLinkServices()
        {
            _dataContext = new TNVTHEntities();
        }

        public IEnumerable<T_HotLink> GetAll()
        {
            return from m in _dataContext.T_HotLink
                    select m;
        }

        public T_HotLink GetByID(int id)
        {
            return _dataContext.T_HotLink.Where(m => m.ID == id).SingleOrDefault();
        }

        public bool IsExist(T_HotLink iHotLink)
        {
            return (_dataContext.T_HotLink.Where(m => m.Title == iHotLink.Title && m.Type == iHotLink.Type).SingleOrDefault() != null);
        }

        public T_HotLink AddNewHotLinkAndReturn(T_HotLink iHotLink)
        {
            _dataContext.T_HotLink.Add(iHotLink);
                _dataContext.SaveChanges();
                return iHotLink;
        }


        public ReturnValue<bool> AddNewHotLink(T_HotLink iHotLink)
        {
            if (IsExist(iHotLink)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.T_HotLink.Add(iHotLink);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> UpdateHotLink(T_HotLink iHotLink)
        {
            //if (IsExist(iHotLink)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.Entry(iHotLink).State = EntityState.Modified;
                return new ReturnValue<bool>(_dataContext.SaveChanges() > 0, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteHotLink(T_HotLink iHotLink)
        {
            try
            {
                _dataContext.T_HotLink.Remove(iHotLink);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> DeleteHotLink(int id)
        {
            try
            {
                T_HotLink DelHotLink = GetByID(id);
                return DeleteHotLink(DelHotLink);
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public IEnumerable<T_HotLink> HotLinkSearch(string type, string term)
        {
            IEnumerable<T_HotLink> ResultList = GetAll();
            if (!string.IsNullOrEmpty(term))
            {
                ResultList = _dataContext.T_HotLink.Where(m => m.Title.Contains(term));
            }

            if(!string.IsNullOrEmpty(type))
            {
                ResultList = _dataContext.T_HotLink.Where(m => m.Type == type);
            }
            return ResultList.OrderByDescending(m => m.ID);
        }


        public IEnumerable<T_HotLink> GetByType(string type, int limit)
        {
            return _dataContext.T_HotLink.Where(m => m.Type == type && m.Enabled == true).Take(limit);
        }
    }
}
