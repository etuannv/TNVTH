using System;
using System.Collections.Generic;
using System.Linq;
using TNVTH.Web.Models;
using TNVTH.Web.Utilities;
using System.Data.Entity;
using TNVTH.Web.ViewModels;

namespace TNVTH.Web.Services
{
    public class T_NewsServices : IT_NewsServices
    {
        private TNVTHEntities _dataContext;

        public T_NewsServices()
        {
            _dataContext = new TNVTHEntities();
        }

        public IEnumerable<T_News> GetAll()
        {
            return from m in _dataContext.T_News
                   select m;
        }

        public IEnumerable<T_News> GetNews(int? cateId, string search)
        {
            IEnumerable<T_News> ResultList;
            // Get all category child
            if (cateId.HasValue)
            {
                List<int> CateIDList = _dataContext.T_Tag.Where(y => y.ParentID == cateId).Select(m => m.ID).ToList();
                CateIDList.Add((int)cateId);


                ResultList = from m in _dataContext.T_News
                             join n in _dataContext.T_News_Tag on m.ID equals n.NewsID
                             where CateIDList.Contains(n.TagID)
                             select m;
            }
            else
            {
                ResultList = GetAll();
            }


            if (!string.IsNullOrEmpty(search))
            {
                string SearchSlug = search.Replace(' ', '-');
                ResultList = ResultList.Where(m => m.Title.Contains(search) || m.Slug.Contains(SearchSlug));
            }
            return ResultList.OrderByDescending(k => k.ID);
        }
        public IEnumerable<T_News> GetByTaxonomy(int iCateID)
        {
            List<int> CateIDList = _dataContext.T_Tag.Where(y => y.ParentID == iCateID).Select(m => m.ID).ToList();
            CateIDList.Add(iCateID);

            var data = from n in _dataContext.T_News
                       join m in _dataContext.T_News_Tag on n.ID equals m.NewsID
                       join q in _dataContext.T_Tag on m.TagID equals q.ID
                       where CateIDList.Contains(m.TagID)
                       select n;

            return data.OrderByDescending(a => a.ID);
        }

        public IEnumerable<T_News> GetByTaxonomy(int iCateID, int number)
        {
            return GetByTaxonomy(iCateID).OrderByDescending(m => m.ID).Select(m => m).Take(number);
        }
        //public IEnumerable<T_News> GetByTaxonomyList(List<int> CateIdList, int Number)
        //{

        //    return (from n in _dataContext.T_News
        //               join m in _dataContext.T_News_Tag on n.ID equals m.NewsID
        //               join q in _dataContext.T_Tag on m.TagID equals q.ID
        //               where CateIdList.Contains(m.TagID)
        //               select n).Take(Number);
        //}
        public IEnumerable<T_News> GetRandomByTaxonomy(int iCateID, int number)
        {
            return GetByTaxonomy(iCateID).OrderByDescending(m => Guid.NewGuid()).Select(m => m).Take(number);
        }


        public T_News GetByID(int id)
        {
            return _dataContext.T_News.Where(m => m.ID == id).SingleOrDefault();
        }
        public T_News GetBySlug(string slug)
        {
            return _dataContext.T_News.Where(m => m.Slug == slug).SingleOrDefault();
        }

        public bool IsExist(T_News iNews)
        {
            T_News NewsFound = _dataContext.T_News
                .Where(m => m.ID != iNews.ID && (m.Title == iNews.Title || m.Slug == iNews.Slug))
                .SingleOrDefault();
            return (NewsFound != null) ? true : false;
        }

        public T_News AddNewNewsAndReturn(T_News iNews)
        {
            iNews.CreatedDate = DateTime.Now;
            _dataContext.T_News.Add(iNews);
            _dataContext.SaveChanges();
            return iNews;
        }
        public ReturnValue<bool> AddNewNews(T_News iNews)
        {
            if (IsExist(iNews)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                iNews.CreatedDate = DateTime.Now;
                _dataContext.T_News.Add(iNews);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> UpdateNews(T_News iNews)
        {
            if (IsExist(iNews)) return new ReturnValue<bool>(false, "Mục đã tồn tại");
            try
            {
                iNews.ModifiedDate = DateTime.Now;
                _dataContext.Entry(iNews).State = EntityState.Modified;
                return new ReturnValue<bool>(_dataContext.SaveChanges() > 0, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }

        public ReturnValue<bool> DeleteNews(T_News iNews)
        {
            try
            {
                _dataContext.T_News.Remove(iNews);
                _dataContext.SaveChanges();
                return new ReturnValue<bool>(true, "");
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public ReturnValue<bool> DeleteNews(int id)
        {
            try
            {
                T_News DelNews = GetByID(id);
                return DeleteNews(DelNews);
            }
            catch (Exception)
            {
                return new ReturnValue<bool>(false, "");
            }
        }
        public IEnumerable<T_News> GetLastNews(int limit)
        {
            return _dataContext.T_News.OrderByDescending(m => m.ID).Select(m => m).Take(limit);
        }

        public IEnumerable<T_News> GetRelatedNews(int newsId, int limit)
        {
            // Get related news by 3 part:
            // 1. By news Title and News Description
            // 2. By news Tag
            // 3. By news category
            // The rate for 3 part is: 1 - 3 - 1
            double Weight = limit / 5;
            int LimitByTag = Convert.ToInt32(Math.Round(Weight * 2));
            int LimitByNews = Convert.ToInt32(Math.Round(Weight * 1));
            int LimitByCate = Convert.ToInt32(Math.Round(Weight * 1));
            int LimitByRandom = limit - LimitByNews - LimitByTag - LimitByCate;
            List<int> AlreadyGetList = new List<int>();


            AlreadyGetList.Add(newsId);


            //1. Get related news by infomation of title and description
            List<T_News> ResultByNews = GetRelatedNewsByNews(newsId, LimitByNews, AlreadyGetList).ToList();
            int CountByNewsResult = ResultByNews.Count;
            AlreadyGetList.AddRange(ResultByNews.Select(m => m.ID));

            //2. Get related news by information of tag
            //Search news for same tag
            LimitByTag += (LimitByNews - CountByNewsResult);
            IT_TagServices tagService = new T_TagServices();
            List<T_Tag> TagList = tagService.GetTagByNewsID(TNVTH.Web.Utilities.Constants.TAXONOMY_TAG, newsId).ToList(); ;
            List<T_News> ResultByTag = GetNewsByTagList(TagList, LimitByTag, AlreadyGetList).ToList();
            int CountByTagResult = ResultByTag.Count;
            AlreadyGetList.AddRange(ResultByTag.Select(m => m.ID));


            //3. Get related news by information of category
            LimitByCate += (LimitByTag - CountByTagResult);
            List<T_Tag> CateList = tagService.GetTagByNewsID(TNVTH.Web.Utilities.Constants.TAXONOMY_CATEGORY, newsId).ToList(); ;
            List<T_News> ResultByCate = GetNewsByTagList(CateList, LimitByTag, AlreadyGetList).ToList();
            int CountByCateResult = ResultByCate.Count;
            AlreadyGetList.AddRange(ResultByCate.Select(m => m.ID));

            // 4. Get random video 
            LimitByRandom += LimitByCate - CountByCateResult;
            List<T_News> ResultByRandom = GetRandom(LimitByRandom, AlreadyGetList).ToList();

            // Combine these result
            List<T_News> FinalResult = new List<T_News>();
            FinalResult.AddRange(ResultByNews);
            FinalResult.AddRange(ResultByTag);
            FinalResult.AddRange(ResultByCate);
            FinalResult.AddRange(ResultByRandom);
            return FinalResult;

        }
        public IEnumerable<T_News> GetRandom(int number, List<int> exceptList)
        {
            return (from m in _dataContext.T_News
                    where !exceptList.Contains(m.ID) && m.Status == Constants.NEWS_STATUS_PUBLIC
                    orderby Guid.NewGuid()
                    select m).Take(number);
        }

        private IEnumerable<T_News> GetRelatedNewsByNews(int pictureId, int limit, List<int> AlreadyGetList)
        {
            string CurrentNewsSlug = GetByID(pictureId).Slug;
            string SearchSlug = CurrentNewsSlug.Substring(0, (CurrentNewsSlug.Length / 3) * 2);

            return (from m in _dataContext.T_News
                    where !AlreadyGetList.Contains(m.ID) && m.Status == Constants.NEWS_STATUS_PUBLIC && m.Slug.Contains(SearchSlug)
                    select m).Take(limit).Distinct();
        }


        private IEnumerable<T_News> GetNewsByTagList(List<T_Tag> TagList, int limit)
        {
            List<int> ListTagID = TagList.Select(s => s.ID).ToList();
            return (from m in _dataContext.T_News
                    join n in _dataContext.T_News_Tag on m.ID equals n.NewsID
                    where ListTagID.Contains(n.TagID)
                    select m).Take(limit).Distinct();
        }
        private IEnumerable<T_News> GetNewsByTagList(List<T_Tag> TagList, int limit, int exceptId)
        {
            List<int> ListTagID = TagList.Select(s => s.ID).ToList();
            return (from m in _dataContext.T_News
                    join n in _dataContext.T_News_Tag on m.ID equals n.NewsID
                    where ListTagID.Contains(n.TagID) && m.ID != exceptId
                    select m).Take(limit).Distinct();
        }

        private IEnumerable<T_News> GetNewsByTagList(List<T_Tag> TagList, int limit, List<int> exceptId)
        {
            List<int> ListTagID = TagList.Select(s => s.ID).ToList();
            return (from m in _dataContext.T_News
                    join n in _dataContext.T_News_Tag on m.ID equals n.NewsID
                    where ListTagID.Contains(n.TagID) && !exceptId.Contains(m.ID)
                    select m).Take(limit).Distinct();
        }

        public IEnumerable<T_News> GetNewsByTag(int tagId, int limit)
        {
            var q = (from m in _dataContext.T_News
                     join n in _dataContext.T_News_Tag on m.ID equals n.NewsID
                     where n.TagID == tagId
                     select m).Distinct().OrderByDescending(s => s.ID);
            if (limit > 0)
                return q.Take(limit);
            else
                return q;
        }

        public T_Tag GetCateByNewsID(int newsID)
        {
            return (from m in _dataContext.T_Tag
                    join n in _dataContext.T_News_Tag on m.ID equals n.TagID
                    where n.NewsID == newsID && m.Taxonomy == TNVTH.Web.Utilities.Constants.TAXONOMY_CATEGORY
                    select m).FirstOrDefault();
        }

        public IEnumerable<T_News> Search(string term)
        {
            return (from m in _dataContext.T_News
                    where m.Title.Contains(term) || m.ContentNews.Contains(term)
                    select m).Distinct().OrderByDescending(x => x.ID);
        }

        public List<MauThietKeViewModel> GetMauThietKe(int iCateID)
        {
            List<int> CateIDList = _dataContext.T_Tag.Where(y => y.ParentID == iCateID).Select(m => m.ID).ToList();
            CateIDList.Add(iCateID);

            return (from n in _dataContext.T_News
                       join m in _dataContext.T_News_Tag on n.ID equals m.NewsID
                       join q in _dataContext.T_Tag on m.TagID equals q.ID
                       where CateIDList.Contains(m.TagID)
                       select new {n, q}).ToList().Select(item => new  MauThietKeViewModel(item.n, item.q)).ToList();

        }

        public bool IsInCategory(int id, int iCateID)
        {
            List<int> CateIDList = _dataContext.T_Tag.Where(y => y.ParentID == iCateID).Select(m => m.ID).ToList();
            CateIDList.Add(iCateID);

            var count = (from n in _dataContext.T_News
                    join m in _dataContext.T_News_Tag on n.ID equals m.NewsID
                    join q in _dataContext.T_Tag on m.TagID equals q.ID
                    where CateIDList.Contains(m.TagID) && n.ID == id
                    select n).Count();
            if (count > 0) return true; else return false;
        }
    }
}
