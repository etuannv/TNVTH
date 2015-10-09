using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using MvcSiteMapProvider.Web.Mvc.Filters;
using TNVTH.Web.Models;
using TNVTH.Web.Services;
using TNVTH.Web.ViewModels;

namespace TNVTH.Web.Controllers
{
    public class ArticleController : Controller
    {
        //
        // GET: /Article/
        public ActionResult Index()
        {
            return View();
        }

        [SiteMapTitle("Title")]
        public ActionResult Detail(int id, string slug)
        {
            T_NewsServices _newServices = new T_NewsServices();
            T_News ANews = _newServices.GetByID(id);
            int GoiTKId = 1037;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_GoiThietKe_ID.ToString()), out GoiTKId);
            int SieuThiWebId = 1039;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_MauThietKe_ID.ToString()), out SieuThiWebId);

            int DichVuId = 1031;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_DichVu_ID.ToString()), out DichVuId);

            if (_newServices.IsInCategory(id, GoiTKId))
            {
                return View("XemGoiThietKeWeb", ANews);
            }

            if (_newServices.IsInCategory(id, SieuThiWebId))
            {
                return View("XemMauThietKeWeb", ANews);
            }

            if (_newServices.IsInCategory(id, DichVuId))
            {
                return View("XemChiTietDichVu", ANews);
            }

            return View(ANews);
        }

        //public ActionResult XemGoiThietKeWeb(int id, string slug)
        //{
        //    T_NewsServices _newServices = new T_NewsServices();
        //    T_News ANews = _newServices.GetByID(id);
        //    return View(ANews);
        //}
        public ActionResult ContentByTag(int id, int? page, int limit = 0)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int PageSizeClient;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeClient.ToString()), out PageSizeClient);
            PageSizeClient = (PageSizeClient < 1) ? 20 : PageSizeClient;

            IT_TagServices tagServices = new T_TagServices();
            T_Tag ThisTag = tagServices.GetByID(id);
            ViewBag.TagTitle = ThisTag.Title;
            //Get limit itme
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<T_News> NewsList = _newServices.GetNewsByTag(id, limit);
            IPagedList<T_News> Model = MvcPaging.PagingExtensions.ToPagedList(NewsList, currentPageIndex, PageSizeClient, NewsList.Count());
            return View("ContentByTag", Model);

        }


        [SiteMapTitle("Title")]
        public ActionResult ListInCate(int id, int? page)
        {
            T_TagServices tagServices = new T_TagServices();
            ViewBag.CateTitle = tagServices.GetByID(id).Title;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            // Get all with paging
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<T_News> NewsList = _newServices.GetByTaxonomy(id);
            int PageSizeClient;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeClient.ToString()), out PageSizeClient);
            PageSizeClient = (PageSizeClient < 1) ? 20 : PageSizeClient;
            IPagedList<T_News> Model = MvcPaging.PagingExtensions.ToPagedList(NewsList, currentPageIndex, PageSizeClient, NewsList.Count());
            return View("ListInCate", Model);

        }

        [SiteMapTitle("Title")]
        public ActionResult Blog(int? page, int? id)
        {

            T_TagServices tagServices = new T_TagServices();
            int BlogId = 1;
            if (id.HasValue)
            {
                BlogId = (int)id;
            }
            else
            {
                Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_Blog_ID.ToString()), out BlogId);
            }
            ViewBag.CateId = BlogId;
            ViewBag.CateTitle = tagServices.GetByID(BlogId).Title;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            // Get all with paging
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<T_News> NewsList = _newServices.GetByTaxonomy(BlogId).OrderByDescending(m => m.CreatedDate);
            int PageSizeClient;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeClient.ToString()), out PageSizeClient);
            PageSizeClient = (PageSizeClient < 1) ? 20 : PageSizeClient;
            IPagedList<T_News> Model = MvcPaging.PagingExtensions.ToPagedList(NewsList, currentPageIndex, PageSizeClient, NewsList.Count());
            return View("Blog", Model);

        }



        [SiteMapTitle("Title")]
        public ActionResult ListInCateModern(int id, int? page)
        {
            T_TagServices tagServices = new T_TagServices();
            ViewBag.CateTitle = tagServices.GetByID(id).Title;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            // Get all with paging
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<T_News> NewsList = _newServices.GetByTaxonomy(id);
            int PageSizeClient;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeClient.ToString()), out PageSizeClient);
            PageSizeClient = (PageSizeClient < 1) ? 20 : PageSizeClient;
            IPagedList<T_News> Model = MvcPaging.PagingExtensions.ToPagedList(NewsList, currentPageIndex, PageSizeClient, NewsList.Count());
            return View("ListInCateModern", Model);

        }

        public PartialViewResult GetTags(int id, string taxonomy)
        {
            T_News_TagServices _news_TagServices = new T_News_TagServices();
            IEnumerable<T_Tag> TagList = _news_TagServices.GetTagByNewsID(id, taxonomy);
            return PartialView(TagList);
        }
        public PartialViewResult GetNewsInCategory(int CateId, int Number = 5)
        {
            T_NewsServices _newServices = new T_NewsServices();
            //Get limit itme
            IEnumerable<T_News> NewsList = _newServices.GetByTaxonomy(CateId, Number);
            return PartialView("GetNewsInCategory", NewsList);
        }

        public PartialViewResult GetNewsInCategoryDisplayRight(int CateId, int Number = 5)
        {
            T_NewsServices _newServices = new T_NewsServices();
            //Get limit itme
            IEnumerable<T_News> NewsList = _newServices.GetRandomByTaxonomy(CateId, Number);
            return PartialView("GetNewsInCategoryDisplayRight", NewsList);
        }


        public PartialViewResult GetNewsInCategoryModern(int CateId, int Number = 8, int column = 4)
        {

            ViewBag.Column = column;

            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<T_News> NewsList = _newServices.GetByTaxonomy(CateId, Number);
            return PartialView("GetNewsInCategoryModern", NewsList);
        }

        public PartialViewResult GetLastestNews(int limit = 5)
        {
            T_NewsServices _newServices = new T_NewsServices();
            //Get limit itme
            IEnumerable<T_News> NewsList = _newServices.GetLastNews(limit);
            return PartialView("GetLastestNews", NewsList);

        }

        public PartialViewResult GetRelatedNews(int newsId, int limit = 5)
        {
            T_NewsServices _newServices = new T_NewsServices();
            //Get limit itme
            IEnumerable<T_News> NewsList = _newServices.GetRelatedNews(newsId, limit);
            return PartialView("GetRelatedNews", NewsList);

        }
        public PartialViewResult GetBlogCategoryMenu(int limit = 20)
        {
            int BlogId = 1;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_Blog_ID.ToString()), out BlogId);
            T_TagServices _tagServices = new T_TagServices();
            IEnumerable<T_Tag> Model = _tagServices.GetByTaxonomyInCate(Utilities.Constants.TAXONOMY_CATEGORY, BlogId).OrderBy(m => m.ID);
            return PartialView(Model);
        }

        public PartialViewResult GetCategoryLink(int id)
        {
            T_News_TagServices _news_TagServices = new T_News_TagServices();
            IEnumerable<T_Tag> TagList = _news_TagServices.GetTagByNewsID(id, TNVTH.Web.Utilities.Constants.TAXONOMY_CATEGORY);
            return PartialView(TagList);
        }
        public ActionResult GoiThietKeWebsite(int? page)
        {
            T_TagServices tagServices = new T_TagServices();
            int MauTKId = 1039;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_GoiThietKe_ID.ToString()), out MauTKId);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            // Get all with paging
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<T_News> NewsList = _newServices.GetByTaxonomy(MauTKId).OrderBy(m => m.Title);
            int PageSizeClient = 12;
            //Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeClient.ToString()), out PageSizeClient);
            //PageSizeClient = (PageSizeClient < 1) ? 20 : PageSizeClient;
            IPagedList<T_News> Model = MvcPaging.PagingExtensions.ToPagedList(NewsList, currentPageIndex, PageSizeClient, NewsList.Count());
            return View(Model);
        }
        public PartialViewResult GetGoiThietKe(int limit = 6)
        {

            T_TagServices tagServices = new T_TagServices();
            int MauTKId = 1039;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_GoiThietKe_ID.ToString()), out MauTKId);
            //ViewBag.MauTKId = MauTKId;
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<T_News> NewsList = _newServices.GetByTaxonomy(MauTKId, limit).OrderBy(m => Guid.NewGuid());
            return PartialView(NewsList);

        }

        public PartialViewResult GetOtherGoiThietKe(int? exceptId, int limit = 5)
        {

            T_TagServices tagServices = new T_TagServices();
            int GoiTKId = 1037;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_GoiThietKe_ID.ToString()), out GoiTKId);
            //ViewBag.GoiTKId = GoiTKId;
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<T_News> NewsList = _newServices.GetByTaxonomy(GoiTKId, limit).Where(m => m.ID != exceptId).OrderBy(m => Guid.NewGuid());
            return PartialView(NewsList);

        }

        public PartialViewResult GetMauThietKe(int limit = 6)
        {
            T_TagServices tagServices = new T_TagServices();
            int MauTKId = 1037;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_MauThietKe_ID.ToString()), out MauTKId);
            //ViewBag.MauTKId = MauTKId;
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<T_News> NewsList = _newServices.GetByTaxonomy(MauTKId, limit).OrderBy(m => Guid.NewGuid());
            return PartialView(NewsList);
        }



        public ActionResult SieuThiWeb()
        {
            //Get list mau thiet ke category
            int MauTKId = 1037;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_MauThietKe_ID.ToString()), out MauTKId);
            T_TagServices _tagServices = new T_TagServices();
            IEnumerable<T_Tag> MauThietKeCateList = _tagServices.GetByTaxonomyInCate(Utilities.Constants.TAXONOMY_CATEGORY, MauTKId).OrderBy(m => m.ID);
            ViewBag.MauThietKeCateList = MauThietKeCateList;

            //Get all Mau thiet kê
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<MauThietKeViewModel> MauThietKeList = _newServices.GetMauThietKe(MauTKId).OrderBy(m => m.MauThietKe.ID);
            return View(MauThietKeList);
        }

        public ActionResult ThietKeWeb()
        {
            //Get list mau thiet ke category
            int MauTKId = 1037;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_MauThietKe_ID.ToString()), out MauTKId);
            T_TagServices _tagServices = new T_TagServices();
            IEnumerable<T_Tag> MauThietKeCateList = _tagServices.GetByTaxonomyInCate(Utilities.Constants.TAXONOMY_CATEGORY, MauTKId).OrderBy(m => m.ID);
            ViewBag.MauThietKeCateList = MauThietKeCateList;

            //Get all Mau thiet kê
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<MauThietKeViewModel> MauThietKeList = _newServices.GetMauThietKe(MauTKId).OrderBy(m => m.MauThietKe.ID);
            return View(MauThietKeList);
        }

        public PartialViewResult GetOtherMauThietKe(int? exceptId, int limit = 5)
        {

            T_TagServices tagServices = new T_TagServices();
            int GoiTKId = 1037;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_MauThietKe_ID.ToString()), out GoiTKId);
            //ViewBag.GoiTKId = GoiTKId;
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<T_News> NewsList = _newServices.GetByTaxonomy(GoiTKId, limit).Where(m => m.ID != exceptId).OrderBy(m => Guid.NewGuid());
            return PartialView(NewsList);

        }
        public PartialViewResult GetOrderInfo()
        {
            return PartialView();
        }

        public PartialViewResult GetListDichVu(int? exceptId, int limit = 5)
        {
            int DichVuId = 1031;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.Conf_DichVu_ID.ToString()), out DichVuId);

            //Get all Dịch vụ
            T_NewsServices _newServices = new T_NewsServices();
            IEnumerable<T_News> DichVuList = _newServices.GetByTaxonomy(DichVuId, limit).Where(m => m.ID != exceptId).OrderBy(m => Guid.NewGuid());
            return PartialView(DichVuList);
        }
    }
}
