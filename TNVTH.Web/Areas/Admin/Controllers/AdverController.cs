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
    public class AdverController : Controller
    {
        private readonly IT_AdverServices _AdverServices;


        public AdverController()
        {
            _AdverServices = new T_AdverServices();
        }
        //
        // GET: /Admin/Adver/List
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult List(string search, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            ViewData["search"] = search;
            IEnumerable<T_Adver> Cate = _AdverServices.AdverSearch(search);
            int PageSizeAdmin = 10;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()), out PageSizeAdmin);
            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;
            IPagedList<T_Adver> MyList = MvcPaging.PagingExtensions.ToPagedList(Cate, currentPageIndex, PageSizeAdmin, Cate.Count());
            return View(MyList);
        }


        // GET: /Admin/Adver/AddNew
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult AddNew()
        {
            return View();
        }


        // POST: /Admin/Adver/AddNew
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        public ActionResult AddNew(T_Adver iAdver)
        {
            // Upload the image
            HttpPostedFileBase file = Request.Files["ImageData"];
            string PathReturn = UploadAdverImage(file);
            iAdver.ImagePath = PathReturn;
            ReturnValue<bool> result = new ReturnValue<bool>(false, "");
        
            if (ModelState.IsValid)
            {
                result = _AdverServices.AddNewAdver(iAdver);
            }
            if (result.RetValue)
            {
                return RedirectToAction("List", "Adver");
            }
            else
            {
                // Get Adver_List again
                ModelState.AddModelError("Error", result.Msg);
                return View(iAdver);
            }
        }

        private string UploadAdverImage(HttpPostedFileBase file)
        {
            if (!string.IsNullOrEmpty(file.FileName))
            {
                string RandomString = Path.GetRandomFileName();
                RandomString = RandomString.Replace(".", ""); // Remove period.

                String NewFileName = RandomString + file.FileName;
                var uploadDir = "/Content/Uploads/Adver";
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



        // GET: /Admin/Adver/Delete
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Adver model = _AdverServices.GetByID((int)id);
            return View("Delete", model);
        }


        // POST: /Admin/Adver/Delete
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Delete(int id)
        {
            T_Adver Adver = _AdverServices.GetByID((int)id);
            _AdverServices.DeleteAdver(id);
            //TODO: Update parent tree
            return RedirectToAction("List", "Adver");
        }

        // GET: /Admin/Adver/Edit
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Adver model = _AdverServices.GetByID((int)id);
            return View("Edit", model);
        }


        // POST: /Admin/Adver/Edit
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Edit(T_Adver iAdver)
        {
            // Upload image if it have
            HttpPostedFileBase file = Request.Files["ImageData"];
            string PathReturn = UploadAdverImage(file);
            if (!string.IsNullOrEmpty(PathReturn)) iAdver.ImagePath = PathReturn;

            iAdver.ModifiedDate = DateTime.Now;
            iAdver.ModifiedBy = "admin";
            ReturnValue<bool> result = _AdverServices.UpdateAdver(iAdver);
            if (result.RetValue)
            {
                return RedirectToAction("List", "Adver");
            }
            else
            {
                // Get Adver_List again
                ModelState.AddModelError("Error", result.Msg);
                return View(iAdver);
            }
        }


        [Authorize]
        [AcceptVerbs("GET")]
        public JsonResult AdverSearch(string term)
        {
            return this.Json( _AdverServices.AdverSearch(term), JsonRequestBehavior.AllowGet);
        }

    }
}
