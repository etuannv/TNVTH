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
using System.Data.Entity.Infrastructure;

namespace TNVTH.Web.Areas.Admin.Controllers
{
    public class HotLinkController : Controller
    {
        private readonly IT_HotLinkServices _HotLinkServices;


        public HotLinkController()
        {
            
            _HotLinkServices = new T_HotLinkServices();
        }
        //
        // GET: /Admin/HotLink/List
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult List(string type, string search, int? page)
        {
            if (string.IsNullOrEmpty(type)) type = Utilities.HotLinkType.Video.ToString();
            ViewBag.HotType = type;
            if (!Request.IsAuthenticated) RedirectToAction("Login", "Account");
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            ViewData["search"] = search;
            IEnumerable<T_HotLink> Cate = _HotLinkServices.HotLinkSearch(type, search);
            int PageSizeAdmin = 10;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()), out PageSizeAdmin);
            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;

            IPagedList<T_HotLink> MyList = MvcPaging.PagingExtensions.ToPagedList(Cate, currentPageIndex, PageSizeAdmin, Cate.Count());
            return View(MyList);
        }


        // GET: /Admin/HotLink/AddNew
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult AddNew()
        {
            T_HotLink Model = new T_HotLink();
            return View(Model);
        }


        // POST: /Admin/HotLink/AddNew
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        [SiteMapCacheRelease]
        public ActionResult AddNew([Bind(Include = "Title,Link,Target,Type,Enabled")]T_HotLink iHotLink)
        {
            ReturnValue<bool> result = new ReturnValue<bool>(false, "");
            try
            {
                if (ModelState.IsValid)
                {
                    result = _HotLinkServices.AddNewHotLink(iHotLink);
                    if (result.RetValue)
                    {
                        return RedirectToAction("List", "HotLink");
                    }
                    else
                    {
                        // Get HotLink_List again
                        ModelState.AddModelError("Error", result.Msg);
                        return View(iHotLink);
                    }
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");

            }
            //PopulateCategory(CateID);
            return View(iHotLink);
        }


        // GET: /Admin/HotLink/Delete
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_HotLink model = _HotLinkServices.GetByID((int)id);
            return View("Delete", model);
        }


        // POST: /Admin/HotLink/Delete
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        [SiteMapCacheRelease]
        public ActionResult Delete(int id)
        {
            T_HotLink HotLink = _HotLinkServices.GetByID((int)id);
            _HotLinkServices.DeleteHotLink(id);
            //TODO: Update parent tree
            return RedirectToAction("List", "HotLink");
        }

        // GET: /Admin/HotLink/Edit
        [Authorize]
        [AcceptVerbs("GET")]
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_HotLink model = _HotLinkServices.GetByID((int)id);
            return View("Edit", model);
        }


        // POST: /Admin/HotLink/Edit
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        [SiteMapCacheRelease]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_HotLink HotLinkToUpdate = _HotLinkServices.GetByID((int)id);
            if (TryUpdateModel(HotLinkToUpdate, "",
               new string[] { "Title", "Link", "Target", "Type", "Enabled" }))
            {
                try
                {
                    _HotLinkServices.UpdateHotLink(HotLinkToUpdate);
                    return RedirectToAction("List", "HotLink");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View("Edit", HotLinkToUpdate);
        }


        [Authorize]
        [AcceptVerbs("GET")]
        public JsonResult HotLinkSearch(string type, string term)
        {
            return this.Json( _HotLinkServices.HotLinkSearch(type, term), JsonRequestBehavior.AllowGet);
        }

    }
}
