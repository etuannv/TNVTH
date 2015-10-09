using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
    public class ConfigController : Controller
    {
        private readonly IT_ConfigServices _configServices;


        public ConfigController()
        {
            
            _configServices = new T_ConfigServices();
        }
        //
        // GET: /Admin/Config/List
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult List(string search, int? page)
        {
            if (!Request.IsAuthenticated) RedirectToAction("Login", "Account");
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            ViewData["search"] = search;
            IEnumerable<T_Config> Cate = _configServices.ConfigSearch(search);
            int PageSizeAdmin = 10;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()), out PageSizeAdmin);
            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;

            IPagedList<T_Config> MyList = MvcPaging.PagingExtensions.ToPagedList(Cate, currentPageIndex, PageSizeAdmin, Cate.Count());
            return View(MyList);
        }


        // GET: /Admin/Config/AddNew
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult AddNew()
        {
            return View();
        }


        // POST: /Admin/Config/AddNew
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        [SiteMapCacheRelease]
        public ActionResult AddNew(T_Config iConfig)
        {
            ReturnValue<bool> result = new ReturnValue<bool>(false, "");
        
            if (ModelState.IsValid)
            {
                result = _configServices.AddNewConfig(iConfig);
            }
            if (result.RetValue)
            {
                return RedirectToAction("List", "Config");
            }
            else
            {
                // Get Config_List again
                ModelState.AddModelError("Error", result.Msg);
                return View(iConfig);
            }
        }


        // GET: /Admin/Config/Delete
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Config model = _configServices.GetByID((int)id);
            return View("Delete", model);
        }


        // POST: /Admin/Config/Delete
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        [SiteMapCacheRelease]
        public ActionResult Delete(int id)
        {
            T_Config Config = _configServices.GetByID((int)id);
            _configServices.DeleteConfig(id);
            //TODO: Update parent tree
            return RedirectToAction("List", "Config");
        }

        // GET: /Admin/Config/Edit
        [Authorize]
        [AcceptVerbs("GET")]
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Config model = _configServices.GetByID((int)id);
            return View("Edit", model);
        }


        // POST: /Admin/Config/Edit
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        [SiteMapCacheRelease]
        public ActionResult Edit(T_Config iConfig)
        {
            ReturnValue<bool> result = _configServices.UpdateConfig(iConfig);
            if (result.RetValue)
            {
                return RedirectToAction("List", "Config");
            }
            else
            {
                // Get Config_List again
                ModelState.AddModelError("Error", result.Msg);
                return View(iConfig);
            }
        }


        [Authorize]
        [AcceptVerbs("GET")]
        public JsonResult ConfigSearch(string term)
        {
            return this.Json( _configServices.ConfigSearch(term), JsonRequestBehavior.AllowGet);
        }

    }
}
