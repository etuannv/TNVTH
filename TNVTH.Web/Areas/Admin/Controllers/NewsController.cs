using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using MvcSiteMapProvider.Web.Mvc.Filters;
using TNVTH.Web.Models;
using TNVTH.Web.Services;
using TNVTH.Web.Utilities;
using TNVTH.Web.Areas.Admin.Models;

namespace TNVTH.Web.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {
        private readonly IT_NewsServices _newsServices;
        private readonly IT_TagServices _tagServices;
        private readonly IT_News_TagServices _news_TagServices;

        public NewsController()
        {
            _newsServices = new T_NewsServices();
            _tagServices = new T_TagServices();
            _news_TagServices = new T_News_TagServices();
        }
        //
        // GET: /Admin/News/List
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult List(int? cateId, string search, int? page)
        {
            if (cateId.HasValue)
            {
                Session["cateId"] = cateId;
            }
            else
            {
                if (Session["cateId"] != null) cateId = (int)Session["cateId"];
            }
            ViewBag.cateId = new SelectList(_tagServices.GetByTaxonomyForDisplay(TNVTH.Web.Utilities.Constants.TAXONOMY_CATEGORY), "ID", "Title", cateId);


            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            ViewData["search"] = search;
            IEnumerable<T_News>  ListNew = _newsServices.GetNews(cateId, search);
            
            int PageSizeAdmin = 10;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()), out PageSizeAdmin);
            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;

            IPagedList<T_News> MyList = MvcPaging.PagingExtensions.ToPagedList(ListNew, currentPageIndex, PageSizeAdmin, ListNew.Count());
            return View(MyList);
        }

        [Authorize]
        public PartialViewResult GetTags(int id, string taxonomy)
        {
            IEnumerable<T_Tag> TagList = _news_TagServices.GetTagByNewsID(id, taxonomy);
            return PartialView(TagList);
        }

        // GET: /Admin/News/AddNew
        [Authorize]
        [AcceptVerbs("GET")]
        [SiteMapCacheRelease]
        public ActionResult AddNew()
        {
            IEnumerable<T_Tag> CatList = _tagServices.GetByTaxonomyForDisplay(Utilities.Constants.TAXONOMY_CATEGORY);
            NewsViewModel model = new NewsViewModel();
            model.CategoryList = CatList.ToList();
            return View("AddNew", model);
        }


        // POST: /Admin/News/AddNew
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [SiteMapCacheRelease]
        public ActionResult AddNew(FormCollection form, string Published)
        {
            // Upload Image
            HttpPostedFileBase file = Request.Files["ImageData"];
            string AvataURL = UploadAvatar(file);
            // Insert News
            T_News AddNews = new T_News();
            AddNews.Title = form["Title"].ToString();
            AddNews.Slug = form["Slug"].ToString();
            AddNews.ContentNews = form["ContentNews"];
            AddNews.CreatedDate = DateTime.Now;
            
            if(!string.IsNullOrEmpty(Published)) AddNews.Status = Constants.NEWS_STATUS_PUBLIC;
            else AddNews.Status = form["Status"].ToString();
            AddNews.AvataImageUrl = AvataURL;
            AddNews.CreatedBy = User.Identity.Name;
            AddNews.Author = form["Author"];
            if (string.IsNullOrEmpty(form["Title"].ToString()) || string.IsNullOrEmpty(form["Slug"].ToString()))
            {
                IEnumerable<T_Tag> CatList = _tagServices.GetByTaxonomyForDisplay(Utilities.Constants.TAXONOMY_CATEGORY);

                NewsViewModel model = new NewsViewModel(AddNews);
                model.CategoryList = CatList.ToList();
                model.MyCategoryList = new List<T_Tag>();
                // Get Slide_List again
                ModelState.AddModelError("Error", "Tiêu đề, Slug không được để trống");
                return View(model);
            }

            T_News MyNews = _newsServices.AddNewNewsAndReturn(AddNews);
            
            // Set News Category
            int SelectedCate = Convert.ToInt16(form["Category"]);
            _news_TagServices.AddNewNews_Tag(MyNews.ID, SelectedCate);
            
            // Insert Tag and News_Tags
            AddListTag(form["tags"].ToString(), MyNews.ID);

            return RedirectToAction("List", "News");
        }

        private void AddListTag(string TagStringList, int iNewsID)
        {
            var TagList = TagStringList.Split( new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
            foreach(string item in TagList)
            {
                AddNewTagAndTagForNews(item, iNewsID);
            }
        }

        private void AddNewTagAndTagForNews(string item, int iNewsID)
        {
            //Add Tag and return ID
            T_Tag NewTag = _tagServices.AddNewTagAndReturn(item);
            //Add News_Tag
            _news_TagServices.AddNewNews_Tag(iNewsID, NewTag.ID);
        }

        private string UploadAvatar(HttpPostedFileBase file)
        {
            if (!string.IsNullOrEmpty(file.FileName))
            {
                string RandomString = Path.GetRandomFileName();
                RandomString = RandomString.Replace(".", ""); // Remove period.
                String NewFileName = RandomString + Common.ToUrlSlug(Path.GetFileNameWithoutExtension(file.FileName)) + Path.GetExtension(file.FileName);
                var uploadDir = "/Content/Uploads/FeatureImage";
                var imagePath = Path.Combine(Server.MapPath(uploadDir), NewFileName);
                var imageUrl = Path.Combine(uploadDir, NewFileName);
                file.SaveAs(imagePath);
                return imageUrl;
            }
            else
            {
                return "";
            }
        }


        // GET: /Admin/News/Edit
        [Authorize]
        [AcceptVerbs("GET")]
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            T_News News = _newsServices.GetByID((int)id);

            IEnumerable<T_Tag> MyTagList = _news_TagServices.GetTagByNewsID(News.ID, Constants.TAXONOMY_TAG);
            IEnumerable<T_Tag> MyCategoryList = _news_TagServices.GetTagByNewsID(News.ID, Constants.TAXONOMY_CATEGORY);
            IEnumerable<T_Tag> CatList = _tagServices.GetByTaxonomyForDisplay(Utilities.Constants.TAXONOMY_CATEGORY);

            NewsViewModel model = new NewsViewModel(News);
            model.CategoryList = CatList.ToList();
            model.MyTagList = MyTagList.ToList();
            model.MyCategoryList = MyCategoryList.ToList();
            ViewData["TagList"] = BuildTagList(MyTagList.ToList());
            return View("Edit", model);
        }
        public string BuildTagList(List<T_Tag> iTagList)
        {
            StringBuilder sb = new StringBuilder();
            if (iTagList.Count > 0)
            {
                foreach (var item in iTagList)
                {
                    sb.Append(item.Title);
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        // POST: /Admin/News/Edit
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [SiteMapCacheRelease]

        public ActionResult Edit(FormCollection form, string Published)
        {
            
            T_News EditNews = _newsServices.GetByID(Convert.ToInt32(form["ID"]));
            if (User.IsInRole(Utilities.Constants.ROLE_PUBLISHER) && EditNews.CreatedBy != User.Identity.Name)
            {
                return RedirectToAction("AccessDenied", "Start");
            }

            // Upload avata if it have
            HttpPostedFileBase file = Request.Files["ImageData"];
            string AvataURL = UploadAvatar(file);

            // Update News infomation
            EditNews.Title = form["Title"].ToString();
            EditNews.Slug = form["Slug"].ToString();
            EditNews.ContentNews = form["ContentNews"];
            EditNews.Author = form["Author"];
            EditNews.ModifiedBy =  User.Identity.Name;
            if (!string.IsNullOrEmpty(Published)) EditNews.Status = Constants.NEWS_STATUS_PUBLIC;
            else EditNews.Status = form["Status"].ToString();
            if (!String.IsNullOrEmpty(AvataURL))
            {
                EditNews.AvataImageUrl = AvataURL;
            }
            EditNews.ModifiedDate = DateTime.Now;
            EditNews.ModifiedBy = User.Identity.Name;
            if (string.IsNullOrEmpty(form["Title"].ToString()) || string.IsNullOrEmpty(form["Slug"].ToString()))
            {
                IEnumerable<T_Tag> MyTagList = _news_TagServices.GetTagByNewsID(EditNews.ID, Constants.TAXONOMY_TAG);
                IEnumerable<T_Tag> MyCategoryList = _news_TagServices.GetTagByNewsID(EditNews.ID, Constants.TAXONOMY_CATEGORY);
                IEnumerable<T_Tag> CatList = _tagServices.GetByTaxonomyForDisplay(Utilities.Constants.TAXONOMY_CATEGORY);

                NewsViewModel model = new NewsViewModel(EditNews);
                model.CategoryList = CatList.ToList();
                model.MyTagList = MyTagList.ToList();
                model.MyCategoryList = MyCategoryList.ToList();
                ViewData["TagList"] = BuildTagList(MyTagList.ToList());
                // Get Slide_List again
                ModelState.AddModelError("Error", "Tiêu đề, Slug không được để trống");
                ModelState.AddModelError("Error", "Phải chọn chuyên mục");
                return View(model);
            }
            ReturnValue<bool> result = _newsServices.UpdateNews(EditNews);

            // Delete all tags, category of this News
            _news_TagServices.DeleteAllTagByNewsID(EditNews.ID);

            // Set News Category
            int SelectedCate = Convert.ToInt16(form["Category"]);
            _news_TagServices.AddNewNews_Tag(EditNews.ID, SelectedCate);

            // Insert Tag and News_Tags
            AddListTag(form["Tags"].ToString(), EditNews.ID);
            return RedirectToAction("List", "News");
        }



        // GET: /Admin/Tag/Delete
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_News DelNews = _newsServices.GetByID((int)id);
            if (User.IsInRole(Utilities.Constants.ROLE_PUBLISHER) && DelNews.CreatedBy != User.Identity.Name)
            {
                return RedirectToAction("AccessDenied", "Start");
            }
            return View("Delete", DelNews);
        }


        // POST: /Admin/Tag/Delete
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Delete(int id)
        {
            T_News DelNews = _newsServices.GetByID((int)id);
            if (User.IsInRole(Utilities.Constants.ROLE_PUBLISHER) && DelNews.CreatedBy != User.Identity.Name)
            {
                return RedirectToAction("AccessDenied", "Start");
            }
            //Delete all tag and category
            _news_TagServices.DeleteAllTagByNewsID(DelNews.ID);

            // Delete this News
            _newsServices.DeleteNews(DelNews);

            // Delete Avatar file
            DeleteAvataFile(DelNews.AvataImageUrl);

            
            //TODO: Update parent tree
            return RedirectToAction("List", "News");
        }

        private void DeleteAvataFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
