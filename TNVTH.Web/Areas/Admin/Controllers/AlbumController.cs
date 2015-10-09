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
    public class AlbumController : Controller
    {
        private readonly IT_AlbumServices _service;


        public AlbumController()
        {
            _service = new T_AlbumServices();
        }
        //
        // GET: /Admin/Album/List
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult List(string search, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            ViewData["search"] = search;
            IEnumerable<T_Album> Cate = _service.AlbumSearch(search);

            int PageSizeAdmin = 10;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()), out PageSizeAdmin);
            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;

            IPagedList<T_Album> MyList = MvcPaging.PagingExtensions.ToPagedList(Cate, currentPageIndex, PageSizeAdmin, Cate.Count());
            return View(MyList);
        }

        public PartialViewResult GetAlbumImage(int id, int? page)
        {
            ViewData["AlbumID"] = id;
            List<string> FileList = new List<string>();
            T_Album album = _service.GetByID(id);
            var albumFolder =  Constants.ALBUM_FOLDER_PATH + album.Slug;
            if (System.IO.Directory.Exists(Server.MapPath(albumFolder)))
            {
                foreach (var item in System.IO.Directory.GetFiles(Server.MapPath(albumFolder)))
                {
                    FileList.Add(Path.Combine(albumFolder, System.IO.Path.GetFileName(item)));
                }
            }
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            //int PageSizeAdmin = Convert.ToInt32(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()));
            //PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;
            IPagedList<string> MyList = FileList.ToPagedList(currentPageIndex, 20, FileList.Count);

            return PartialView(MyList);
        }



        // GET: /Admin/Album/AddNew
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult AddNew()
        {
            return View();
        }


        // POST: /Admin/Album/AddNew
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        public ActionResult AddNew(T_Album iAlbum)
        {
            ReturnValue<bool> result = new ReturnValue<bool>(false, "");

            if (ModelState.IsValid)
            {
                result = _service.AddNewAlbum(iAlbum);
            }
            if (result.RetValue)
            {
                return RedirectToAction("List", "Album");
            }
            else
            {
                // Get Album_List again
                ModelState.AddModelError("Error", result.Msg);
                return View(iAlbum);
            }
        }


        // GET: /Admin/Album/Delete
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Album model = _service.GetByID((int)id);
            return View("Delete", model);
        }


        // POST: /Admin/Album/Delete
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Delete(int id)
        {
            T_Album Album = _service.GetByID((int)id);
            _service.DeleteAlbum(id);

            //Check folder exist for this album
            var albumFolder = Constants.ALBUM_FOLDER_PATH + Album.Slug;
            if (System.IO.Directory.Exists(Server.MapPath(albumFolder)))
            {
                DeleteDirectory(Server.MapPath(albumFolder));
            }

            return RedirectToAction("List", "Album");
        }

        public void DeleteDirectory(string target_dir)
        {
            try
            {
                string[] files = Directory.GetFiles(target_dir);
                string[] dirs = Directory.GetDirectories(target_dir);

                foreach (string file in files)
                {
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                    System.IO.File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                Directory.Delete(target_dir, true);
            }
            catch
            { }
        }


        // GET: /Admin/Album/Edit
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Album model = _service.GetByID((int)id);
            //Delte directory
            return View("Edit", model);
        }


        // POST: /Admin/Album/Edit
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Edit(T_Album iAlbum)
        {
            ReturnValue<bool> result = _service.UpdateAlbum(iAlbum);
            if (result.RetValue)
            {
                return RedirectToAction("List", "Album");
            }
            else
            {
                // Get Album_List again
                ModelState.AddModelError("Error", result.Msg);
                return View(iAlbum);
            }
        }

        [Authorize]
        [AcceptVerbs("GET")]
        public JsonResult AlbumSearch(string term)
        {
            return this.Json(_service.AlbumSearch(term), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs("GET")]
        public ActionResult UploadImage(int id)
        {
            T_Album Model = _service.GetByID(id);
            return View(Model);
        }

        [Authorize]
        [AcceptVerbs("POST")]
        public ActionResult UploadImage(int ID, string Slug)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                UploadAlbumImage(file, Slug);
            }
            return RedirectToAction("Edit", "Album", new { @id = ID });
        }

        private string UploadAlbumImage(HttpPostedFileBase file, string albumName)
        {
            if (!string.IsNullOrEmpty(file.FileName))
            {

                //Check folder exist for this album
                var albumFolder = Constants.ALBUM_FOLDER_PATH + albumName;
                if (!System.IO.Directory.Exists(Server.MapPath(albumFolder)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(albumFolder));
                }


                //Generate image file name
                string RandomString = Path.GetRandomFileName();
                RandomString = RandomString.Replace(".", ""); // Remove period.
                String NewFileName = RandomString + "_" + file.FileName;


                var ImageData = Path.Combine(Server.MapPath(albumFolder), NewFileName);
                var imageUrl = Path.Combine(albumFolder, NewFileName);
                file.SaveAs(ImageData);
                return imageUrl;
            }
            else
            {
                return "";
            }
        }

        // POST: /Admin/Album/DeleteImage
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult DeleteImage(int id, string FilePath)
        {
            try
            {
                T_Album album = _service.GetByID(id);
                var albumFolder = Constants.ALBUM_FOLDER_PATH + album.Slug;
                string ServerPath = Server.MapPath(albumFolder);

                DirectoryInfo directory = new DirectoryInfo(ServerPath);
                FileInfo[] Result = directory.GetFiles(FilePath + ".*");

                if (Result.Length > 0)
                {
                    Result[0].Delete();
                }
                return RedirectToAction("Edit", "Album", new { @id = id });
            }
            catch
            { return RedirectToAction("Edit", "Album", new { @id = id }); }
        }
    }
}
