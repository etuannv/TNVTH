using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TNVTH.Web.Models;
using TNVTH.Web.Utilities;

namespace TNVTH.Web.Services
{
    public class T_AdverServices : IT_AdverServices
    {
        private TNVTHEntities _dataContext;

        public T_AdverServices()
        {
            _dataContext = new TNVTHEntities();
        }

        public IEnumerable<T_Adver> GetAll()
        {
            return from m in _dataContext.T_Adver
                    select m;
        }

        public T_Adver GetByID(int id)
        {
            return _dataContext.T_Adver.Where(m => m.ID == id).SingleOrDefault();
        }

        public bool IsExist(T_Adver iAdver)
        {
            return false;
        }

        public T_Adver AddNewAdverAndReturn(T_Adver iAdver)
        {
            _dataContext.T_Adver.Add(iAdver);
                _dataContext.SaveChanges();
                return iAdver;
        }


        public ReturnValue<bool> AddNewAdver(T_Adver iAdver)
        {
            if (IsExist(iAdver)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.T_Adver.Add(iAdver);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> UpdateAdver(T_Adver iAdver)
        {
            //if (IsExist(iAdver)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.Entry(iAdver).State = EntityState.Modified;
                return new ReturnValue<bool>(_dataContext.SaveChanges() > 0, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteAdver(T_Adver iAdver)
        {
            try
            {
                _dataContext.T_Adver.Remove(iAdver);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> DeleteAdver(int id)
        {
            try
            {
                T_Adver DelAdver = GetByID(id);
                return DeleteAdver(DelAdver);
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public IEnumerable<T_Adver> AdverSearch(string term)
        {
            IEnumerable<T_Adver> ResultList;
            if (!string.IsNullOrEmpty(term))
            {
                string SlugTerm = term.Replace(' ', '-');
                ResultList = _dataContext.T_Adver.Where(m => m.Title.Contains(term));
            }
            else
            {
                ResultList = GetAll();
            }
            return ResultList.OrderByDescending(m => m.ID);
        }

    }
}
