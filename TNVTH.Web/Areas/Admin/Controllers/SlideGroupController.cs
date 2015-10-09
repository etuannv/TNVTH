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
    public class SlideGroupController : Controller
    {
        private readonly IT_SlideGroupServices _slideGroupServices;


        public SlideGroupController()
        {
            _slideGroupServices = new T_SlideGroupServices();
        }
        //
        // GET: /Admin/SlideGroup/List
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult List(string search, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            ViewData["search"] = search;
            IEnumerable<T_SlideGroup> Cate = _slideGroupServices.SlideGroupSearch(search);

            int PageSizeAdmin = 10;
            Int32.TryParse(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()), out PageSizeAdmin);
            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;

            IPagedList<T_SlideGroup> MyList = MvcPaging.PagingExtensions.ToPagedList(Cate, currentPageIndex, PageSizeAdmin, Cate.Count());
            return View(MyList);
        }


        // GET: /Admin/SlideGroup/AddNew
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult AddNew()
        {
            return View();
        }


        // POST: /Admin/SlideGroup/AddNew
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        public ActionResult AddNew(T_SlideGroup iSlideGroup)
        {
            ReturnValue<bool> result = new ReturnValue<bool>(false, "");
        
            if (ModelState.IsValid)
            {
                result = _slideGroupServices.AddNewSlideGroup(iSlideGroup);
            }
            if (result.RetValue)
            {
                return RedirectToAction("List", "SlideGroup");
            }
            else
            {
                // Get SlideGroup_List again
                ModelState.AddModelError("Error", result.Msg);
                return View(iSlideGroup);
            }
        }


        // GET: /Admin/SlideGroup/Delete
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_SlideGroup model = _slideGroupServices.GetByID((int)id);
            return View("Delete", model);
        }


        // POST: /Admin/SlideGroup/Delete
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Delete(int id)
        {
            T_SlideGroup SlideGroup = _slideGroupServices.GetByID((int)id);

            //Delete all slide of this group
            T_SlideServices SlideService = new T_SlideServices();
            SlideService.DeleteSlideBySlideGroup(SlideGroup.ID);

            //Delet this grou[p
            _slideGroupServices.DeleteSlideGroup(id);
            return RedirectToAction("List", "SlideGroup");
        }

        // GET: /Admin/SlideGroup/Edit
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_SlideGroup model = _slideGroupServices.GetByID((int)id);
            return View("Edit", model);
        }


        // POST: /Admin/SlideGroup/Edit
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Edit(T_SlideGroup iSlideGroup)
        {
            ReturnValue<bool> result = _slideGroupServices.UpdateSlideGroup(iSlideGroup);
            if (result.RetValue)
            {
                return RedirectToAction("List", "SlideGroup");
            }
            else
            {
                // Get SlideGroup_List again
                ModelState.AddModelError("Error", result.Msg);
                return View(iSlideGroup);
            }
        }


        [Authorize]
        [AcceptVerbs("GET")]
        public JsonResult SlideGroupSearch(string term)
        {
            return this.Json( _slideGroupServices.SlideGroupSearch(term), JsonRequestBehavior.AllowGet);
        }

    }
}
