using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TNVTH.Web.Models;
using TNVTH.Web.Utilities;

namespace TNVTH.Web.Services
{
    public class T_SlideGroupServices : IT_SlideGroupServices
    {
        private TNVTHEntities _dataContext;

        public T_SlideGroupServices()
        {
            _dataContext = new TNVTHEntities();
        }

        public IEnumerable<T_SlideGroup> GetAll()
        {
            return from m in _dataContext.T_SlideGroup
                    select m;
        }

        public T_SlideGroup GetByID(int id)
        {
            return _dataContext.T_SlideGroup.Where(m => m.ID == id).SingleOrDefault();
        }

        public bool IsExist(T_SlideGroup iSlideGroup)
        {
            return false;
        }

        public T_SlideGroup AddNewSlideGroupAndReturn(T_SlideGroup iSlideGroup)
        {
            _dataContext.T_SlideGroup.Add(iSlideGroup);
                _dataContext.SaveChanges();
                return iSlideGroup;
        }


        public ReturnValue<bool> AddNewSlideGroup(T_SlideGroup iSlideGroup)
        {
            if (IsExist(iSlideGroup)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.T_SlideGroup.Add(iSlideGroup);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> UpdateSlideGroup(T_SlideGroup iSlideGroup)
        {
            //if (IsExist(iSlideGroup)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.Entry(iSlideGroup).State = EntityState.Modified;
                return new ReturnValue<bool>(_dataContext.SaveChanges() > 0, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteSlideGroup(T_SlideGroup iSlideGroup)
        {
            try
            {
                _dataContext.T_SlideGroup.Remove(iSlideGroup);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> DeleteSlideGroup(int id)
        {
            try
            {
                T_SlideGroup DelSlideGroup = GetByID(id);
                return DeleteSlideGroup(DelSlideGroup);
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public IEnumerable<T_SlideGroup> SlideGroupSearch(string term)
        {
            IEnumerable<T_SlideGroup> ResultList;
            if (!string.IsNullOrEmpty(term))
            {
                ResultList = _dataContext.T_SlideGroup.Where(m => m.Title.Contains(term));
            }
            else
            {
                ResultList = GetAll();
            }
            return ResultList.OrderByDescending(m => m.ID);
        }
    }
}
