//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using MvcPaging;
//using TNVTH.Web.Models;
//using TNVTH.Web.Services;
//using TNVTH.Web.Utilities;
//using TNVTH.Web.Areas.Admin.Models;

//namespace TNVTH.Web.Areas.Admin.Controllers
//{
//    public class ImageController : Controller
//    {
//        private readonly IT_ImageServices _service;
//        private readonly IT_AlbumServices _albumServices;


//        public ImageController()
//        {
//            if (_service == null) _service = new T_ImageServices();
//            if (_albumServices == null) _albumServices = new T_AlbumServices();
//        }
//        //
//        // GET: /Admin/Image/List
//        [Authorize]
//        [AcceptVerbs("GET")]
//        public ActionResult List(string search, int? page)
//        {
//            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
//            ViewData["search"] = search;
//            IEnumerable<T_Image> Cate = _service.ImageSearch(search);
//            int PageSizeAdmin = 10;
//            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()), out PageSizeAdmin);
//            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;
//            IPagedList<T_Image> MyList = MvcPaging.PagingExtensions.ToPagedList(Cate, currentPageIndex, PageSizeAdmin, Cate.Count());
//            return View(MyList);
//        }


//        // GET: /Admin/Image/AddNew
//        [Authorize]
//        [AcceptVerbs("GET")]
//        public ActionResult AddNew()
//        {
//            ImageViewModel Model = new ImageViewModel();
//            Model.ImageGroupList = _albumServices.GetAll();
//            return View(Model);
//        }


//        // POST: /Admin/Image/AddNew
//        [Authorize]
//        [AcceptVerbs("POST")]
//        [ValidateAntiForgeryToken]
//        public ActionResult AddNew(T_Image iImage)
//        {
//            // Upload the image
//            HttpPostedFileBase file = Request.Files["ImageData"];
//            string PathReturn = UploadImageImage(file);
//            iImage.ImagePath = PathReturn;
//            ReturnValue<bool> result = new ReturnValue<bool>(false, "");
        
//            if (ModelState.IsValid)
//            {
//                result = _service.AddNewImage(iImage);
//            }
//            if (result.RetValue)
//            {
//                return RedirectToAction("List", "Image");
//            }
//            else
//            {
//                ImageViewModel Model = new ImageViewModel(iImage, _albumServices.GetAll());
//                // Get Image_List again
//                ModelState.AddModelError("Error", result.Msg);
//                return View(Model);
//            }
//        }

//        private string UploadImageImage(HttpPostedFileBase file)
//        {
//            if (!string.IsNullOrEmpty(file.FileName))
//            {
//                string RandomString = Path.GetRandomFileName();
//                RandomString = RandomString.Replace(".", ""); // Remove period.

//                String NewFileName = RandomString + file.FileName;
//                var uploadDir = "/Content/Uploads/Image";
//                var ImageData = Path.Combine(Server.MapPath(uploadDir), NewFileName);
//                var imageUrl = Path.Combine(uploadDir, NewFileName);
//                file.SaveAs(ImageData);
//                return imageUrl;
//            }
//            else
//            {
//                return "";
//            }
//        }


//        // GET: /Admin/Image/Delete
//        [Authorize]
//        [AcceptVerbs("GET")]
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            T_Image model = _service.GetByID((int)id);
//            return View("Delete", model);
//        }


//        // POST: /Admin/Image/Delete
//        [Authorize]
//        [ValidateAntiForgeryToken]
//        [AcceptVerbs("POST")]
//        public ActionResult Delete(int id)
//        {
//            T_Image Image = _service.GetByID((int)id);
//            _service.DeleteImage(id);
//            //TODO: Update parent tree
//            return RedirectToAction("List", "Image");
//        }

//        // GET: /Admin/Image/Edit
//        [Authorize]
//        [AcceptVerbs("GET")]
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            ImageViewModel Model = new ImageViewModel(_service.GetByID((int)id), _albumServices.GetAll());
//            return View("Edit", Model);
//        }


//        // POST: /Admin/Image/Edit
//        [Authorize]
//        [ValidateAntiForgeryToken]
//        [AcceptVerbs("POST")]
//        public ActionResult Edit(T_Image iImage)
//        {
//            // Upload image if it have
//            HttpPostedFileBase file = Request.Files["ImageData"];
//            string PathReturn = UploadImageImage(file);
//            if (!string.IsNullOrEmpty(PathReturn)) iImage.ImagePath = PathReturn;

//            ReturnValue<bool> result = _service.UpdateImage(iImage);
//            if (result.RetValue)
//            {
//                return RedirectToAction("List", "Image");
//            }
//            else
//            {
//                // Get Image_List again
//                ModelState.AddModelError("Error", result.Msg);
//                return View(iImage);
//            }
//        }


//        [Authorize]
//        [AcceptVerbs("GET")]
//        public JsonResult ImageSearch(string term)
//        {
//            return this.Json( _service.ImageSearch(term), JsonRequestBehavior.AllowGet);
//        }

//    }
//}
