using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using TNVTH.Web.Models;
using TNVTH.Web.Services;
using System.Linq;
using System.Text;
using System;
using MvcPaging;
using YoutubeExtractor;
using TNVTH.Web.Helpers;

namespace TNVTH.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["hasSlide"] = true;
            return View();
        }
        public ActionResult DichVu()
        {
            return View();
        }

        public ActionResult LienHe()
        {
            return View();
        }

        public PartialViewResult GetSlide()
        {
            T_SlideServices _slideServices = new T_SlideServices();
            int SlideGroupID = 0;
            int.TryParse(GlobalConfig.Instance.GetValue(Utilities.Config.SlideChinhGroupID.ToString()), out SlideGroupID);
            IEnumerable<T_Slide> slideImages = _slideServices.GetSlideByGroupID(SlideGroupID);
            return PartialView("GetSlide", slideImages);
        }
        public PartialViewResult GetCarousel(int id)
        {
            T_SlideServices _slideServices = new T_SlideServices();
            IEnumerable<T_Slide> slideImages = _slideServices.GetSlideByGroupID(id);
            return PartialView("GetCarousel", slideImages.ToList());
        }

        public PartialViewResult GetSlideOne(int id, string width = "200px", string height = "200px")
        {
            T_SlideServices _slideServices = new T_SlideServices();
            ViewBag.Subfix = TNVTH.Web.Utilities.Common.GetUniqueString();
            ViewBag.SlideWidth = width;
            ViewBag.SlideHeight = height;
            IEnumerable<T_Slide> slideImages = _slideServices.GetSlideByGroupID(id);
            return PartialView("GetSlideOne", slideImages.ToList());
        }

        public ViewResult SearchPage(string search, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int PageSizeClient;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeClient.ToString()), out PageSizeClient);
            PageSizeClient = (PageSizeClient < 1) ? 20 : PageSizeClient;


            ViewBag.term = search;
            //Search for news
            T_NewsServices _newsServices = new T_NewsServices();
            IEnumerable<T_News> NewsList = _newsServices.Search(search);
            IPagedList<T_News> Model = MvcPaging.PagingExtensions.ToPagedList(NewsList, currentPageIndex, PageSizeClient, NewsList.Count());
            if (Model != null)
            {
                if (Model.TotalItemCount < 1 && !string.IsNullOrEmpty(search))
                {
                    ViewBag.NoResult = "Không tìm thấy !";
                }
                else
                {
                    ViewBag.NoResult = "";
                }
            }
            return View(Model);
        }

        public string GetTopMenu()
        {
            T_MenuServices service = new T_MenuServices();
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class='nav navbar-nav pull-right mainNav'>");
            IEnumerable<T_Menu> Level1 = service.GetChildren(null);
            //Do level 1
            int ccount = 1;
            foreach (var item1 in Level1)
            {
                if (service.HasChild(item1.ID))
                {
                    sb.AppendFormat("<li class='c{0} dropdown'><a href='{1}' class='dropdown-toggle' data-toggle='dropdown'>{2} <b class='caret'></b></a>", ccount, item1.Link, item1.Title);
                    sb.Append(MakeLevel2(item1.ID));
                    sb.Append("</li>");
                }
                else
                {
                    sb.AppendFormat("<li class='c{0}'><a href='{1}'>{2}</a></li>", ccount, item1.Link, item1.Title);
                }
                ccount++;
            }

            sb.Append(" </ul>");
            sb.Append("      </div>");
            sb.Append("            </div>");
            return sb.ToString();
        }

        private string MakeLevel2(int parentId)
        {
            T_MenuServices service = new T_MenuServices();
            IEnumerable<T_Menu> Level2 = service.GetChildren(parentId);
            StringBuilder sb = new StringBuilder();
            sb.Append("        <ul class='dropdown-menu'>");
            //Do level 1
            foreach (var item2 in Level2)
            {
                sb.AppendFormat("<li><a href='{0}'>{1}</a></li>", item2.Link, item2.Title);
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        public PartialViewResult GetFooter()
        {
            return PartialView("GetFooter");
        }

        public PartialViewResult GetCategoryMenuFooter(string taxonomy, int limit = 5)
        {
            T_TagServices _tagServices = new T_TagServices();
            IEnumerable<T_Tag> Model = _tagServices.GetByTaxonomy(taxonomy, limit);
            return PartialView(Model);
        }
        public PartialViewResult GetHotLinkRight(string type, int limit = 5)
        {
            T_HotLinkServices _hotLinkServices = new T_HotLinkServices();
            IEnumerable<T_HotLink> Model = _hotLinkServices.GetByType(type, limit).OrderByDescending(m=> m.ID);
            return PartialView("HotLinkRight", Model);
        }
        public PartialViewResult GetAdvertisment(string positionKey)
        {
            List<T_Slide> SlideList = new List<T_Slide>();
            T_ConfigServices _configServices = new T_ConfigServices();
            T_Config Model = _configServices.GetByKey(positionKey);
            if (Model != null)
            {
                int GroupSildeID = 0;
                bool result = int.TryParse(Model.Value, out GroupSildeID);

                if (result && GroupSildeID > 0)
                {
                    T_SlideServices slideServices = new T_SlideServices();
                    SlideList = slideServices.GetSlideByGroupID(GroupSildeID).Where(m=> m.Enable == true).ToList();

                }
            }
            return PartialView("GetAdvertisment", SlideList);
        }


        [HttpGet]
        public void AdvertismentClick(int? id)
        {
            if (id.HasValue)
            { 
                T_SlideServices slideServices = new T_SlideServices();
                slideServices.AdvertismentClick((int)id);
            }
        }
    }
}
