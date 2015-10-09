using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TNVTH.Web.Models;
using TNVTH.Web.Utilities;

namespace TNVTH.Web.Services
{
    public class T_TagServices : IT_TagServices
    {
        private TNVTHEntities _dataContext;

        public T_TagServices()
        {
            _dataContext = new TNVTHEntities();
        }

        public IEnumerable<T_Tag> GetAll()
        {
            return from m in _dataContext.T_Tag
                    select m;
        }
        public IEnumerable<T_Tag> GetByTaxonomy(string taxonomy)
        {
            return _dataContext.T_Tag.Where(m => m.Taxonomy == taxonomy).OrderBy(m => m.Title);
        }


        public IEnumerable<T_Tag> GetByTaxonomyForDisplay(string taxonomy, IEnumerable<T_Tag> excepTagList = null, string searchKey = null)
        {
            IEnumerable<T_Tag> TagList;
            if (!string.IsNullOrEmpty(searchKey))
            {
                string searchSlug = searchKey.Replace(' ', '-');
                TagList =
                    _dataContext.T_Tag.Where(
                    m => m.Taxonomy == taxonomy
                   && (m.Title.Contains(searchKey) || m.Slug.Contains(searchSlug)))
                   .OrderBy(m => m.ParentPath);
            }
            else
            {
                TagList = _dataContext.T_Tag.Where(m => m.Taxonomy == taxonomy).OrderBy(m => m.ParentPath);
            }
            // Remove exept list
            if (excepTagList != null)
            {
                TagList = TagList.Except(excepTagList);
            }
            if (taxonomy == Constants.TAXONOMY_CATEGORY)
            {
                // Modify title
                foreach (var item in TagList)
                {
                    string AddString = "";
                    int count = item.ParentPath.Count(f => f == ';');
                    for (int i = 0; i < count; i++)
                    {
                        AddString += "— ";
                    }
                    item.Title = AddString + item.Title;
                }
            }

            return TagList;
        }


        public IEnumerable<T_Tag> GetByTaxonomyInCate(string taxonomy, int? cateId, IEnumerable<T_Tag> excepTagList = null, string searchKey = null)
        {
            IEnumerable<T_Tag> TagList;
            if (!string.IsNullOrEmpty(searchKey))
            {
                string searchSlug = searchKey.Replace(' ', '-');
                TagList =
                    _dataContext.T_Tag.Where(
                    m => m.Taxonomy == taxonomy
                   && (m.Title.Contains(searchKey) || m.Slug.Contains(searchSlug)))
                   .OrderBy(m => m.ParentPath);
            }
            else
            {
                TagList = _dataContext.T_Tag.Where(m => m.Taxonomy == taxonomy && m.ParentID == cateId).OrderBy(m => m.ParentPath);
            }
            // Remove exept list
            if (excepTagList != null)
            {
                TagList = TagList.Except(excepTagList);
            }

            //if (taxonomy == Constants.TAXONOMY_CATEGORY)
            //{
            //    // Modify title
            //    foreach (var item in TagList)
            //    {
            //        string AddString = "";
            //        int count = item.ParentPath.Count(f => f == ';');
            //        for (int i = 0; i < count; i++)
            //        {
            //            AddString += "— ";
            //        }
            //        item.Title = AddString + item.Title;
            //    }
            //}

            return TagList;
        }
        public IEnumerable<T_Tag> GetByTaxonomy(string taxonomy, string searchKey)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                string searchSlug = searchKey.Replace(' ', '-');
                return _dataContext.T_Tag.Where(
                    m => m.Taxonomy == taxonomy
                    && (m.Title.Contains(searchKey) || m.Slug.Contains(searchKey)))
                    .OrderBy(m => m.Title);
            }
            else
            {
                return _dataContext.T_Tag.Where(m => m.Taxonomy == taxonomy).OrderBy(m => m.Title);
            }
        }

        public IEnumerable<T_Tag> GetByTaxonomy(string taxonomy, int limit)
        {
            return _dataContext.T_Tag.Where(m => m.Taxonomy == taxonomy).OrderBy(m => Guid.NewGuid()).Take(limit);
            
        }
        public T_Tag GetByID(int id)
        {
            return _dataContext.T_Tag.Where(m => m.ID == id).SingleOrDefault();
        }
        public T_Tag GetBySlug(string slug)
        {
            return _dataContext.T_Tag.Where(m => m.Slug == slug).SingleOrDefault();
        }

        public bool IsExist(T_Tag iTag)
        {
            T_Tag TagFound = _dataContext.T_Tag.Where( m => m.ID != iTag.ID && m.Taxonomy == iTag.Taxonomy &&(m.Title == iTag.Title || m.Slug == iTag.Slug)).SingleOrDefault();
            return (TagFound != null) ? true : false;
        }

        public T_Tag AddNewTagAndReturn(T_Tag iTag)
        {
            //Check exist
            T_Tag TagFound = _dataContext.T_Tag.Where( m =>m.Taxonomy == iTag.Taxonomy && ( m.Title == iTag.Title || m.Slug == iTag.Slug)).SingleOrDefault();
            //Return exist Tag
            if (TagFound != null) return TagFound;
            else
            {
                iTag.CreatedDate = DateTime.Now;
                _dataContext.T_Tag.Add(iTag);
                _dataContext.SaveChanges();
                return iTag;
            }
        }
        
        public T_Tag AddNewTagAndReturn(string iTag)
        {
            iTag = iTag.Trim();
            T_Tag NewTag = new T_Tag();
            NewTag.Title = iTag;
            NewTag.Slug = Common.ToUrlSlug(iTag);
            NewTag.Taxonomy = Constants.TAXONOMY_TAG;
            return AddNewTagAndReturn(NewTag);
        }

        public ReturnValue<bool> AddNewTag(T_Tag iTag)
        {
            if (IsExist(iTag)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            if (string.IsNullOrEmpty(iTag.Title) || string.IsNullOrEmpty(iTag.Slug)) return new ReturnValue<bool>(false, "Dữ liệu không đúng");
            try
            {
                iTag.CreatedDate = DateTime.Now;
                _dataContext.T_Tag.Add(iTag);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> UpdateTag(T_Tag iTag)
        {
            if (IsExist(iTag)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            if (string.IsNullOrEmpty(iTag.Title) || string.IsNullOrEmpty(iTag.Slug)) return new ReturnValue<bool>(false, "Dữ liệu không đúng");
            try
            {
                iTag.ModifiedDate = DateTime.Now;
                _dataContext.Entry(iTag).State = EntityState.Modified;
                return new ReturnValue<bool>(_dataContext.SaveChanges() > 0, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteTag(T_Tag iTag)
        {
            try
            {
                _dataContext.T_Tag.Remove(iTag);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> DeleteTag(int id)
        {
            try
            {
                if (TagHasChild(id)) return new ReturnValue<bool>(false, "Không thể xóa, vì có mục con");
                if(CategoryHasNews(id)) return new ReturnValue<bool>(false, "Không thể xóa, vì có có bài viết trong mục này");
                T_Tag DelTag = GetByID(id);
                return DeleteTag(DelTag);
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        private bool CategoryHasNews(int tagId)
        {
            var q = (from m in _dataContext.T_Tag
                     join n in _dataContext.T_News_Tag on m.ID equals n.NewsID
                     where n.TagID == tagId && m.Taxonomy == TNVTH.Web.Utilities.Constants.TAXONOMY_CATEGORY
                     select m).SingleOrDefault();
            if (q != null) return true;
            else return false;
        }

        private bool TagHasChild(int tagId)
        {
            var q = (from m in _dataContext.T_Tag
                    where m.ParentID == tagId
                    select m).SingleOrDefault();
            if (q != null) return true;
            else return false;
        }

        public string GetPath(int? parentID)
        {
            string Result = "";
            if (parentID != null)
            {
                T_Tag Parent = GetByID((int)parentID);
                Result += Parent.ParentPath;
                Result += ";";
            }
            Result += Common.GetUniqueString();
            return Result;
        }

        public IEnumerable<KeyValuePair<int, string>> TagSearch(string term)
        {
            string searchkey = Common.RemoveUnicode(term);
            List<T_Tag> data = new List<T_Tag>();
            if (!string.IsNullOrEmpty(term))
            {
                string SlugTerm = term.Replace(' ', '-');
                data = _dataContext.T_Tag.Where(
                    m => m.Taxonomy == Utilities.Constants.TAXONOMY_TAG
                    && (m.Title.Contains(searchkey) || m.Slug.Contains(SlugTerm))
                    ).ToList();
            }
            else
            {
                data = (from type in _dataContext.T_Tag
                        where type.Taxonomy == Utilities.Constants.TAXONOMY_TAG
                        orderby type.Title
                        select type).ToList();
            }


            return data.Select(m => new KeyValuePair<int, string>(m.ID, m.Title)).Take(10);
        }
        public IEnumerable<T_Tag> GetTagByNewsID(string taxonomy, int newsId)
        {
            return from m in _dataContext.T_News_Tag
                    join n in _dataContext.T_Tag on m.TagID equals n.ID
                    where m.NewsID == newsId && n.Taxonomy == taxonomy
                    select n;
        }
    }
}
