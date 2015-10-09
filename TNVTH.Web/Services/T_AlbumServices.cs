using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TNVTH.Web.Models;
using TNVTH.Web.Utilities;

namespace TNVTH.Web.Services
{
    public class T_AlbumServices : IT_AlbumServices
    {
        private TNVTHEntities _dataContext;

        public T_AlbumServices()
        {
            _dataContext = new TNVTHEntities();
        }

        public IEnumerable<T_Album> GetAll()
        {
            return from m in _dataContext.T_Album
                    select m;
        }

        public T_Album GetByID(int id)
        {
            return _dataContext.T_Album.Where(m => m.ID == id).SingleOrDefault();
        }

        public bool IsExist(T_Album iAlbum)
        {
            return false;
        }

        public T_Album AddNewAlbumAndReturn(T_Album iAlbum)
        {
            _dataContext.T_Album.Add(iAlbum);
                _dataContext.SaveChanges();
                return iAlbum;
        }


        public ReturnValue<bool> AddNewAlbum(T_Album iAlbum)
        {
            if (IsExist(iAlbum)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                iAlbum.Slug = TNVTH.Web.Utilities.Common.ToUrlSlug(iAlbum.Title);
                _dataContext.T_Album.Add(iAlbum);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> UpdateAlbum(T_Album iAlbum)
        {
            //if (IsExist(iAlbum)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.Entry(iAlbum).State = EntityState.Modified;
                return new ReturnValue<bool>(_dataContext.SaveChanges() > 0, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteAlbum(T_Album iAlbum)
        {
            try
            {
                _dataContext.T_Album.Remove(iAlbum);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> DeleteAlbum(int id)
        {
            try
            {
                T_Album DelAlbum = GetByID(id);
                return DeleteAlbum(DelAlbum);
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public IEnumerable<T_Album> AlbumSearch(string term)
        {
            IEnumerable<T_Album> ResultList;
            if (!string.IsNullOrEmpty(term))
            {
                ResultList = _dataContext.T_Album.Where(m => m.Title.Contains(term));
            }
            else
            {
                ResultList = GetAll();
            }
            return ResultList.OrderByDescending(m => m.ID);
        }
    }
}
