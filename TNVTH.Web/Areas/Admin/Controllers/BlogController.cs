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
    public class BlogController : Controller
    {
        private readonly IT_BlogServices _blogServices;
        private readonly IT_TagServices _tagServices;
        private readonly IT_Blog_TagServices _blog_TagServices;

        public BlogController()
        {
            _blogServices = new T_BlogServices();
            _tagServices = new T_TagServices();
            _blog_TagServices = new T_Blog_TagServices();
        }
        //
        // GET: /Admin/Blog/List
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
            IEnumerable<T_Blog>  ListNew = _blogServices.GetBlog(cateId, search);
            
            int PageSizeAdmin = 10;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()), out PageSizeAdmin);
            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;

            IPagedList<T_Blog> MyList = MvcPaging.PagingExtensions.ToPagedList(ListNew, currentPageIndex, PageSizeAdmin, ListNew.Count());
            return View(MyList);
        }

        [Authorize]
        public PartialViewResult GetTags(int id, string taxonomy)
        {
            IEnumerable<T_Tag> TagList = _blog_TagServices.GetTagByBlogID(id, taxonomy);
            return PartialView(TagList);
        }

        // GET: /Admin/Blog/AddNew
        [Authorize]
        [AcceptVerbs("GET")]
        [SiteMapCacheRelease]
        public ActionResult AddNew()
        {
            IEnumerable<T_Tag> CatList = _tagServices.GetByTaxonomyForDisplay(Utilities.Constants.TAXONOMY_CATEGORY);
            BlogViewModel model = new BlogViewModel();
            model.CategoryList = CatList.ToList();
            return View("AddNew", model);
        }


        // POST: /Admin/Blog/AddNew
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
            // Insert Blog
            T_Blog AddBlog = new T_Blog();
            AddBlog.Title = form["Title"].ToString();
            AddBlog.Slug = form["Slug"].ToString();
            AddBlog.ContentBlog = form["ContentBlog"];
            AddBlog.CreatedDate = DateTime.Now;
            
            if(!string.IsNullOrEmpty(Published)) AddBlog.Status = Constants.NEWS_STATUS_PUBLIC;
            else AddBlog.Status = form["Status"].ToString();
            AddBlog.AvataImageUrl = AvataURL;
            AddBlog.CreatedBy = User.Identity.Name;
            AddBlog.Author = form["Author"];
            if (string.IsNullOrEmpty(form["Title"].ToString()) || string.IsNullOrEmpty(form["Slug"].ToString()))
            {
                IEnumerable<T_Tag> CatList = _tagServices.GetByTaxonomyForDisplay(Utilities.Constants.TAXONOMY_CATEGORY);

                BlogViewModel model = new BlogViewModel(AddBlog);
                model.CategoryList = CatList.ToList();
                model.MyCategoryList = new List<T_Tag>();
                // Get Slide_List again
                ModelState.AddModelError("Error", "Tiêu đề, Slug không được để trống");
                return View(model);
            }

            T_Blog MyBlog = _blogServices.AddNewBlogAndReturn(AddBlog);
            
            // Set Blog Category
            int SelectedCate = Convert.ToInt16(form["Category"]);
            _blog_TagServices.AddNewBlog_Tag(MyBlog.ID, SelectedCate);
            
            // Insert Tag and Blog_Tags
            AddListTag(form["tags"].ToString(), MyBlog.ID);

            return RedirectToAction("List", "Blog");
        }

        private void AddListTag(string TagStringList, int iBlogID)
        {
            var TagList = TagStringList.Split( new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
            foreach(string item in TagList)
            {
                AddNewTagAndTagForBlog(item, iBlogID);
            }
        }

        private void AddNewTagAndTagForBlog(string item, int iBlogID)
        {
            //Add Tag and return ID
            T_Tag NewTag = _tagServices.AddNewTagAndReturn(item);
            //Add Blog_Tag
            _blog_TagServices.AddNewBlog_Tag(iBlogID, NewTag.ID);
        }

        private string UploadAvatar(HttpPostedFileBase file)
        {
            if (!string.IsNullOrEmpty(file.FileName))
            {
                string RandomString = Path.GetRandomFileName();
                RandomString = RandomString.Replace(".", ""); // Remove period.
                String NewFileName = RandomString + Common.ToUrlSlug(file.FileName);
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


        // GET: /Admin/Blog/Edit
        [Authorize]
        [AcceptVerbs("GET")]
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            T_Blog Blog = _blogServices.GetByID((int)id);

            IEnumerable<T_Tag> MyTagList = _blog_TagServices.GetTagByBlogID(Blog.ID, Constants.TAXONOMY_TAG);
            IEnumerable<T_Tag> MyCategoryList = _blog_TagServices.GetTagByBlogID(Blog.ID, Constants.TAXONOMY_CATEGORY);
            IEnumerable<T_Tag> CatList = _tagServices.GetByTaxonomyForDisplay(Utilities.Constants.TAXONOMY_CATEGORY);

            BlogViewModel model = new BlogViewModel(Blog);
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

        // POST: /Admin/Blog/Edit
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [SiteMapCacheRelease]

        public ActionResult Edit(FormCollection form, string Published)
        {
            
            T_Blog EditBlog = _blogServices.GetByID(Convert.ToInt32(form["ID"]));
            if (User.IsInRole(Utilities.Constants.ROLE_PUBLISHER) && EditBlog.CreatedBy != User.Identity.Name)
            {
                return RedirectToAction("AccessDenied", "Start");
            }

            // Upload avata if it have
            HttpPostedFileBase file = Request.Files["ImageData"];
            string AvataURL = UploadAvatar(file);

            // Update Blog infomation
            EditBlog.Title = form["Title"].ToString();
            EditBlog.Slug = form["Slug"].ToString();
            EditBlog.ContentBlog = form["ContentBlog"];
            EditBlog.Author = form["Author"];
            EditBlog.ModifiedBy =  User.Identity.Name;
            if (!string.IsNullOrEmpty(Published)) EditBlog.Status = Constants.NEWS_STATUS_PUBLIC;
            else EditBlog.Status = form["Status"].ToString();
            if (!String.IsNullOrEmpty(AvataURL))
            {
                EditBlog.AvataImageUrl = AvataURL;
            }
            EditBlog.ModifiedDate = DateTime.Now;
            EditBlog.ModifiedBy = User.Identity.Name;
            if (string.IsNullOrEmpty(form["Title"].ToString()) || string.IsNullOrEmpty(form["Slug"].ToString()))
            {
                IEnumerable<T_Tag> MyTagList = _blog_TagServices.GetTagByBlogID(EditBlog.ID, Constants.TAXONOMY_TAG);
                IEnumerable<T_Tag> MyCategoryList = _blog_TagServices.GetTagByBlogID(EditBlog.ID, Constants.TAXONOMY_CATEGORY);
                IEnumerable<T_Tag> CatList = _tagServices.GetByTaxonomyForDisplay(Utilities.Constants.TAXONOMY_CATEGORY);

                BlogViewModel model = new BlogViewModel(EditBlog);
                model.CategoryList = CatList.ToList();
                model.MyTagList = MyTagList.ToList();
                model.MyCategoryList = MyCategoryList.ToList();
                ViewData["TagList"] = BuildTagList(MyTagList.ToList());
                // Get Slide_List again
                ModelState.AddModelError("Error", "Tiêu đề, Slug không được để trống");
                ModelState.AddModelError("Error", "Phải chọn chuyên mục");
                return View(model);
            }
            ReturnValue<bool> result = _blogServices.UpdateBlog(EditBlog);

            // Delete all tags, category of this Blog
            _blog_TagServices.DeleteAllTagByBlogID(EditBlog.ID);

            // Set Blog Category
            int SelectedCate = Convert.ToInt16(form["Category"]);
            _blog_TagServices.AddNewBlog_Tag(EditBlog.ID, SelectedCate);

            // Insert Tag and Blog_Tags
            AddListTag(form["Tags"].ToString(), EditBlog.ID);
            return RedirectToAction("List", "Blog");
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
            T_Blog DelBlog = _blogServices.GetByID((int)id);
            if (User.IsInRole(Utilities.Constants.ROLE_PUBLISHER) && DelBlog.CreatedBy != User.Identity.Name)
            {
                return RedirectToAction("AccessDenied", "Start");
            }
            return View("Delete", DelBlog);
        }


        // POST: /Admin/Tag/Delete
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Delete(int id)
        {
            T_Blog DelBlog = _blogServices.GetByID((int)id);
            if (User.IsInRole(Utilities.Constants.ROLE_PUBLISHER) && DelBlog.CreatedBy != User.Identity.Name)
            {
                return RedirectToAction("AccessDenied", "Start");
            }
            //Delete all tag and category
            _blog_TagServices.DeleteAllTagByBlogID(DelBlog.ID);

            // Delete this Blog
            _blogServices.DeleteBlog(DelBlog);

            // Delete Avatar file
            DeleteAvataFile(DelBlog.AvataImageUrl);

            
            //TODO: Update parent tree
            return RedirectToAction("List", "Blog");
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
