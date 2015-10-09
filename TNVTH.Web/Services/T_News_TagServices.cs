using System;
using System.Collections.Generic;
using System.Linq;
using TNVTH.Web.Utilities;
using TNVTH.Web.Models;
using System.Data.Entity;

namespace TNVTH.Web.Services
{
    public class T_News_TagServices : IT_News_TagServices
    {
        private TNVTHEntities _dataContext;

        public T_News_TagServices()
        {
            _dataContext = new TNVTHEntities();
        }

        public IEnumerable<T_News_Tag> GetAll()
        {
            var q = from m in _dataContext.T_News_Tag
                    select m;
            return q; 
        }

        public T_News_Tag GetByID(int id)
        {
            return (from m in _dataContext.T_News_Tag
                    where m.ID == id
                     select m).SingleOrDefault();
        }
        public IEnumerable<T_Tag> GetTagByNewsID(int iNewsID, string taxonomy)
        {
            return from m in _dataContext.T_News_Tag
                   join t in _dataContext.T_Tag on m.TagID equals t.ID
                   where (m.NewsID == iNewsID && m.T_Tag.Taxonomy == taxonomy)
                   select t;

        }
        public IEnumerable<T_News_Tag> GetByTagID(int iTagsID, string taxonomy)
        {
            return _dataContext.T_News_Tag.Where(m => m.TagID == iTagsID&& m.T_Tag.Taxonomy == taxonomy);
        }
        public bool IsExist(T_News_Tag iNews_Tag)
        {
            T_News_Tag TagFound = _dataContext.T_News_Tag.Where(m => m.NewsID == iNews_Tag.NewsID && m.TagID == iNews_Tag.TagID).SingleOrDefault();
            return (TagFound != null) ? true : false;
        }

        //public T_News_Tag AddNewTagAndReturn(T_News_Tag iNews_Tag)
        //{
        //    return _newTagRepository.AddAndReturn(iNews_Tag);
        //}

        public ReturnValue<bool> AddNewNews_Tag(int iNewsID, int iTagID)
        {
            T_News_Tag NewItem = new T_News_Tag();
            NewItem.NewsID = iNewsID;
            NewItem.TagID = iTagID;
            return AddNewNews_Tag(NewItem);
        }
        public ReturnValue<bool> AddNewNews_Tag(T_News_Tag iNews_Tag)
        {
            if (IsExist(iNews_Tag)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.T_News_Tag.Add(iNews_Tag);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> UpdateNews_Tag(T_News_Tag iNews_Tag)
        {
            //if (IsExist(iNews_Tag)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                _dataContext.Entry(iNews_Tag).State = EntityState.Modified;
                return new ReturnValue<bool>(_dataContext.SaveChanges() > 0, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteNews_Tag(T_News_Tag iNews_Tag)
        {
            try
            {
                _dataContext.T_News_Tag.Remove(iNews_Tag);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> DeleteNews_Tag(int id)
        {
            try
            {
                T_News_Tag DelTag = GetByID(id);
                return DeleteNews_Tag(DelTag);

            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteAllTagByNewsID(int iNewsID)
        {
            try
            {
                _dataContext.T_News_Tag.RemoveRange(_dataContext.T_News_Tag.Where(m => m.NewsID == iNewsID));
                if(_dataContext.SaveChanges() > 0) return new ReturnValue<bool>(true, "");
                else return new ReturnValue<bool>(false, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

    }
}
