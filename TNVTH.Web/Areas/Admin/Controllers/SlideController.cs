using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using TNVTH.Web.Models;
using TNVTH.Web.Services;
using TNVTH.Web.Utilities;
using TNVTH.Web.Areas.Admin.Models;

namespace TNVTH.Web.Areas.Admin.Controllers
{
    public class SlideController : Controller
    {
        private readonly IT_SlideServices _SlideServices;
        private readonly IT_SlideGroupServices _slideGroupServices;


        public SlideController()
        {
            _SlideServices = new T_SlideServices();
            _slideGroupServices = new T_SlideGroupServices();
        }
        //
        // GET: /Admin/Slide/List
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult List(string search, int? GroupID, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            ViewData["search"] = search;
            if (GroupID.HasValue)
            {
                Session["SlideGroup"] = GroupID;
            }
            else
            {
                if (Session["SlideGroup"] != null) GroupID = (int)Session["SlideGroup"];
            }
            ViewBag.GroupID = new SelectList(_slideGroupServices.GetAll().OrderBy(m => m.Title), "Id", "Title", GroupID);

            IEnumerable<T_Slide> Cate = _SlideServices.SlideSearch(search, GroupID);
            int PageSizeAdmin = 10;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()), out PageSizeAdmin);
            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;
            IPagedList<T_Slide> MyList = MvcPaging.PagingExtensions.ToPagedList(Cate, currentPageIndex, PageSizeAdmin, Cate.Count());
            return View(MyList);
        }


        // GET: /Admin/Slide/AddNew
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult AddNew()
        {
            SlideViewModel Model = new SlideViewModel();
            Model.SlideGroupList = _slideGroupServices.GetAll();
            return View(Model);
        }


        // POST: /Admin/Slide/AddNew
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        public ActionResult AddNew(T_Slide iSlide)
        {
            // Upload the image
            HttpPostedFileBase file = Request.Files["ImageData"];
            string PathReturn = UploadSlideImage(file);
            iSlide.ImagePath = PathReturn;
            ReturnValue<bool> result = new ReturnValue<bool>(false, "");

            if (ModelState.IsValid)
            {
                result = _SlideServices.AddNewSlide(iSlide);
            }
            if (result.RetValue)
            {
                return RedirectToAction("List", "Slide");
            }
            else
            {
                SlideViewModel Model = new SlideViewModel(iSlide, _slideGroupServices.GetAll());
                // Get Slide_List again
                ModelState.AddModelError("Error", result.Msg);
                return View(Model);
            }
        }

        private string UploadSlideImage(HttpPostedFileBase file)
        {
            if (!string.IsNullOrEmpty(file.FileName))
            {
                string RandomString = Path.GetRandomFileName();
                RandomString = RandomString.Replace(".", ""); // Remove period.

                String NewFileName = RandomString + file.FileName;
                var uploadDir = "/Content/Uploads/Slide";
                var ImageData = Path.Combine(Server.MapPath(uploadDir), NewFileName);
                var imageUrl = Path.Combine(uploadDir, NewFileName);
                file.SaveAs(ImageData);
                return imageUrl;
            }
            else
            {
                return "";
            }
        }


        // GET: /Admin/Slide/Delete
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Slide model = _SlideServices.GetByID((int)id);
            return View("Delete", model);
        }


        // POST: /Admin/Slide/Delete
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Delete(int id)
        {
            T_Slide Slide = _SlideServices.GetByID((int)id);
            DeleteFile(Slide.ImagePath);
            _SlideServices.DeleteSlide(id);
            //TODO: Update parent tree
            return RedirectToAction("List", "Slide");
        }

        private void DeleteFile(string path)
        {
            try
            {
                string FilePath = Server.MapPath(path);
                if (System.IO.File.Exists(FilePath))
                {
                    System.IO.File.Delete(FilePath);
                }
            }
            catch
            { }
        }

        // GET: /Admin/Slide/Edit
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SlideViewModel Model = new SlideViewModel(_SlideServices.GetByID((int)id), _slideGroupServices.GetAll());
            return View("Edit", Model);
        }


        // POST: /Admin/Slide/Edit
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Edit(T_Slide iSlide)
        {
            // Upload image if it have
            HttpPostedFileBase file = Request.Files["ImageData"];
            string PathReturn = UploadSlideImage(file);
            if (!string.IsNullOrEmpty(PathReturn)) iSlide.ImagePath = PathReturn;

            ReturnValue<bool> result = _SlideServices.UpdateSlide(iSlide);
            if (result.RetValue)
            {
                return RedirectToAction("List", "Slide");
            }
            else
            {
                // Get Slide_List again
                ModelState.AddModelError("Error", result.Msg);
                return View(iSlide);
            }
        }


    }
}
