using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TNVTH.Web.Models;
using TNVTH.Web.Utilities;

namespace TNVTH.Web.Services
{
    public class T_ConfigServices : IT_ConfigServices
    {
        private TNVTHEntities _dataContext;

        public T_ConfigServices()
        {
            _dataContext = new TNVTHEntities();
        }

        public IEnumerable<T_Config> GetAll()
        {
            return from m in _dataContext.T_Config
                    select m;
        }

        public T_Config GetByID(int id)
        {
            return _dataContext.T_Config.Where(m => m.ID == id).SingleOrDefault();
        }

        public bool IsExist(T_Config iConfig)
        {
            return (_dataContext.T_Config.Where(m => m.Key == iConfig.Key).SingleOrDefault() != null);
        }

        public T_Config AddNewConfigAndReturn(T_Config iConfig)
        {
            _dataContext.T_Config.Add(iConfig);
                _dataContext.SaveChanges();
                return iConfig;
        }


        public ReturnValue<bool> AddNewConfig(T_Config iConfig)
        {
            if (IsExist(iConfig)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.T_Config.Add(iConfig);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> UpdateConfig(T_Config iConfig)
        {
            //if (IsExist(iConfig)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.Entry(iConfig).State = EntityState.Modified;
                return new ReturnValue<bool>(_dataContext.SaveChanges() > 0, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteConfig(T_Config iConfig)
        {
            try
            {
                _dataContext.T_Config.Remove(iConfig);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> DeleteConfig(int id)
        {
            try
            {
                T_Config DelConfig = GetByID(id);
                return DeleteConfig(DelConfig);
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public IEnumerable<T_Config> ConfigSearch(string term)
        {
            IEnumerable<T_Config> ResultList;
            if (!string.IsNullOrEmpty(term))
            {
                ResultList = _dataContext.T_Config.Where(m => m.Title.Contains(term));
            }
            else
            {
                ResultList = GetAll();
            }
            return ResultList.OrderByDescending(m => m.ID);
        }

        public T_Config GetByKey(string key)
        {
            return _dataContext.T_Config.Where(m => m.Key == key).FirstOrDefault();
        }
    }
}
