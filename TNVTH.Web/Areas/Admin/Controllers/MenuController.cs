using System;
using System.Collections.Generic;
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
    public class MenuController : Controller
    {
        private readonly IT_MenuServices _menuServices;

        public MenuController()
        {
            _menuServices = new T_MenuServices();
        }
        //
        // GET: /Admin/Menu/List
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult List(string taxonomy, string search, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            IEnumerable<T_Menu> ListMenu = _menuServices.GetAllForDisplay(null, search);
            int PageSizeAdmin = Convert.ToInt32(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()));
            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;
            IPagedList<T_Menu> MyList = MvcPaging.PagingExtensions.ToPagedList(ListMenu, currentPageIndex, PageSizeAdmin, ListMenu.Count());
            return View(MyList);
        }


        // GET: /Admin/Menu/AddNew
        [Authorize]
        [AcceptVerbs("GET")]
        [SiteMapCacheRelease]
        public ActionResult AddNew(string taxonomy)
        {
            ViewBag.ParentID = new SelectList(_menuServices.GetAllForDisplay().OrderBy(m => m.ParentPath), "Id", "Title");
            return View();
        }


        // POST: /Admin/Menu/AddNew
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        public ActionResult AddNew(T_Menu iMenu)
        {
            ReturnValue<bool> result = new ReturnValue<bool>(false, "");
            if (ModelState.IsValid)
            {
                iMenu.ParentPath = _menuServices.GetPath(iMenu.ParentID);
                result = _menuServices.AddNewMenu(iMenu);
            }
            if (result.RetValue)
            {
                return RedirectToAction("List", "Menu");
            }
            else
            {
                // Get Menu_List again
                ModelState.AddModelError("Error", result.Msg);
                IEnumerable<T_Menu> MenuList = _menuServices.GetAllForDisplay();
                ViewBag.ParentID = new SelectList(_menuServices.GetAllForDisplay().OrderBy(m => m.ParentPath), "Id", "Title");
                return View(iMenu);
            }
        }


        // GET: /Admin/Menu/Delete
        [Authorize]
        [AcceptVerbs("GET")]

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Menu Menu = _menuServices.GetByID((int)id);
            ViewBag.ParentID = new SelectList(_menuServices.GetAllForDisplay().OrderBy(m => m.ParentPath), "Id", "Title");
            return View("Delete", Menu);
        }


        // POST: /Admin/Menu/Delete
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Delete(int id)
        {
            T_Menu Menu = _menuServices.GetByID((int)id);
            ReturnValue<bool> result = _menuServices.DeleteMenu(id);
            if (result.RetValue)
            {
                return RedirectToAction("List", "Menu");
            }
            else
            {
                // Get Menu_List again
                ModelState.AddModelError("Error", result.Msg);
                IEnumerable<T_Menu> MenuList = _menuServices.GetAllForDisplay();
                ViewBag.ParentID = new SelectList(_menuServices.GetAllForDisplay().OrderBy(m => m.ParentPath), "Id", "Title");
                return View(Menu);
            }
        }

        // GET: /Admin/Menu/Edit
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            T_Menu Menu = _menuServices.GetByID((int)id);
            T_MenuServices services = new T_MenuServices();
            ViewBag.ParentID = new SelectList(services.GetAllForDisplay().OrderBy(m => m.ParentPath), "Id", "Title", Menu.ParentID);
            return View("Edit", Menu);
        }


        // POST: /Admin/Menu/Edit
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Edit(T_Menu iMenu)
        {
            //T_Menu Menu = _menuServices.GetByID(iMenu.ID);
            //Menu.Title = iMenu.Title;
            //Menu.Slug = iMenu.Slug;
            //Menu.Description = iMenu.Description;
            //Menu.ParentID = iMenu.ParentID;
            //Menu.ParentPath = _menuServices.GetPath(Menu.ParentID);
            //Menu.Link = iMenu.Link;
            iMenu.ParentPath = _menuServices.GetPath(iMenu.ParentID);
            ReturnValue<bool> result = _menuServices.UpdateMenu(iMenu);
            if (result.RetValue)
            {
                return RedirectToAction("List", "Menu");
            }
            else
            {
                // Get Menu_List again
                ModelState.AddModelError("Error", result.Msg);
                IEnumerable<T_Menu> MenuList = _menuServices.GetAllForDisplay();
                ViewBag.ParentID = new SelectList(_menuServices.GetAllForDisplay().OrderBy(m => m.ParentPath), "Id", "Title");
                return View(iMenu);
            }
        }

    }
}
