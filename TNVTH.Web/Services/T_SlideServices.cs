using System;
using System.Collections.Generic;
using System.Linq;
using TNVTH.Web.Models;
using System.Data.Entity;
using TNVTH.Web.Utilities;
using System.IO;

namespace TNVTH.Web.Services
{
    public class T_SlideServices : IT_SlideServices
    {
        private TNVTHEntities _dataContext;

        public T_SlideServices()
        {
            _dataContext = new TNVTHEntities();
        }

        public IEnumerable<T_Slide> GetAll()
        {
            return from m in _dataContext.T_Slide.Include(m => m.T_SlideGroup)
                   select m;
        }

        public T_Slide GetByID(int id)
        {
            return _dataContext.T_Slide.Where(m => m.ID == id).SingleOrDefault();
        }

        public bool IsExist(T_Slide iSlide)
        {
            return false;
        }

        public T_Slide AddNewSlideAndReturn(T_Slide iSlide)
        {
            _dataContext.T_Slide.Add(iSlide);
            _dataContext.SaveChanges();
            return iSlide;
        }


        public ReturnValue<bool> AddNewSlide(T_Slide iSlide)
        {
            if (IsExist(iSlide)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.T_Slide.Add(iSlide);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> UpdateSlide(T_Slide iSlide)
        {
            //if (IsExist(iSlide)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.Entry(iSlide).State = EntityState.Modified;
                return new ReturnValue<bool>(_dataContext.SaveChanges() > 0, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteSlide(T_Slide iSlide)
        {
            try
            {
                //Delete this slide
                _dataContext.T_Slide.Remove(iSlide);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> DeleteSlide(int id)
        {
            try
            {
                T_Slide DelSlide = GetByID(id);
                return DeleteSlide(DelSlide);
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public IEnumerable<T_Slide> SlideSearch(string term, int? slideGroupID)
        {
            IEnumerable<T_Slide> ResultList = GetAll();
            if (slideGroupID != null)
            {
                ResultList = from m in ResultList
                             where (m.GroupID == slideGroupID)
                             select m;
            }
            if (!string.IsNullOrEmpty(term))
            {
                ResultList = from m in ResultList
                             where (m.Title.Contains(term))
                             select m;
            }
            return ResultList.OrderByDescending(m => m.ID);
        }
        public IEnumerable<T_Slide> GetEnableSlide()
        {
            return _dataContext.T_Slide.Where(m => m.Enable == true);
        }
        public IEnumerable<T_Slide> GetSlideByGroupID(int GroupID)
        {
            return _dataContext.T_Slide.Where(m => m.GroupID == GroupID);
        }

        public void DeleteSlideBySlideGroup(int groupID)
        {
            IEnumerable<T_Slide> ListSlide = GetSlideByGroupID(groupID);
            foreach (T_Slide slide in ListSlide)
            {
                DeleteSlide(slide);
            }
        }

        public void AdvertismentClick(int adId)
        {
            T_Slide Adverd = GetByID(adId);
            Adverd.Click += 1;
            UpdateSlide(Adverd);
        }
    }
}
