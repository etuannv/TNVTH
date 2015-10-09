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
    public class TagController : Controller
    {
        private readonly IT_TagServices _tagServices;


        public TagController()
        {
            _tagServices = new T_TagServices();
        }
        //
        // GET: /Admin/Tag/List
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult List(string taxonomy, string search, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            ViewData["taxonomy"] = taxonomy;
            ViewData["search"] = search;
            IEnumerable<T_Tag> Cate = _tagServices.GetByTaxonomyForDisplay(taxonomy, null, search);
            int PageSizeAdmin = Convert.ToInt32(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()));
            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;
            IPagedList<T_Tag> MyList = MvcPaging.PagingExtensions.ToPagedList(Cate, currentPageIndex, PageSizeAdmin, Cate.Count());
            return View(MyList);
        }


        // GET: /Admin/Tag/AddNew
        [Authorize]
        [AcceptVerbs("GET")]
        [SiteMapCacheRelease]
        public ActionResult AddNew(string taxonomy)
        {
            ViewData["taxonomy"] = taxonomy;
            IEnumerable<T_Tag> TagList = _tagServices.GetByTaxonomyForDisplay(taxonomy);
            TagViewModel model = new TagViewModel();
            model.TagList = TagList;
            return View("AddNew", model);
        }


        // POST: /Admin/Tag/AddNew
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        [SiteMapCacheRelease]
        public ActionResult AddNew(TagViewModel iTagVM)
        {
            ViewData["taxonomy"] = iTagVM.Taxonomy;
            ReturnValue<bool> result = new ReturnValue<bool>(false, "");
            if (ModelState.IsValid)
            {
                T_Tag NewTag = iTagVM.GetTag();
                NewTag.CreatedBy = User.Identity.Name;
                NewTag.ParentPath = _tagServices.GetPath(NewTag.ParentID);
                result = _tagServices.AddNewTag(NewTag);
            }
            if (result.RetValue)
            {
                return RedirectToAction("List", "Tag", new { @taxonomy = iTagVM.Taxonomy });
            }
            else
            {
                // Get Tag_List again
                ModelState.AddModelError("Error", result.Msg);
                iTagVM.TagList = _tagServices.GetByTaxonomy(iTagVM.Taxonomy);
                return View(iTagVM);
            }
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
            T_Tag Tag = _tagServices.GetByID((int)id);
            ViewData["taxonomy"] = Tag.Taxonomy;
            IEnumerable<T_Tag> TagList = _tagServices.GetByTaxonomyForDisplay(Tag.Taxonomy);
            TagViewModel model = new TagViewModel(Tag, TagList);
            return View("Delete", model);
        }


        // POST: /Admin/Tag/Delete
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Delete(int id)
        {
            T_Tag Tag = _tagServices.GetByID((int)id);
            if (User.IsInRole(Utilities.Constants.ROLE_PUBLISHER) && Tag.CreatedBy != User.Identity.Name)
            {
                return RedirectToAction("AccessDenied", "Start");
            }
            ViewData["taxonomy"] = Tag.Taxonomy;
            ReturnValue<bool> result = _tagServices.DeleteTag(id);
            if (result.RetValue)
            {
                return RedirectToAction("List", "Tag", new { @taxonomy = Tag.Taxonomy });
            }
            else
            {
                IEnumerable<T_Tag> TagList = _tagServices.GetByTaxonomyForDisplay(Tag.Taxonomy);
                TagViewModel model = new TagViewModel(Tag, TagList);
                // Get Tag_List again
                ModelState.AddModelError("Error", result.Msg);
                return View(model);
            }
        }

        // GET: /Admin/Tag/Edit
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Tag Tag = _tagServices.GetByID((int)id);
            ViewData["taxonomy"] = Tag.Taxonomy;
            IEnumerable<T_Tag> TagList = _tagServices.GetByTaxonomyForDisplay(Tag.Taxonomy, new[] { Tag });

            TagViewModel model = new TagViewModel(Tag, TagList);
            return View("Edit", model);
        }


        // POST: /Admin/Tag/Edit
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Edit(TagViewModel iTagVM)
        {
            T_Tag Tag = _tagServices.GetByID(iTagVM.ID);
            if (User.IsInRole(Utilities.Constants.ROLE_PUBLISHER) && Tag.CreatedBy != User.Identity.Name)
            {
                return RedirectToAction("AccessDenied", "Start");
            }
            ViewData["taxonomy"] = Tag.Taxonomy;
            Tag.Title = iTagVM.Title;
            Tag.Slug = iTagVM.Slug;
            Tag.Description = iTagVM.Description;
            Tag.ParentID = iTagVM.ParentID;
            Tag.ParentPath = _tagServices.GetPath(Tag.ParentID);
            Tag.ModifiedBy = User.Identity.Name;
            ReturnValue<bool> result = _tagServices.UpdateTag(Tag);
            if (result.RetValue)
            {
                return RedirectToAction("List", "Tag", new { @taxonomy = iTagVM.Taxonomy });
            }
            else
            {
                // Get Tag_List again
                ModelState.AddModelError("Error", result.Msg);
                iTagVM.TagList = _tagServices.GetByTaxonomy(iTagVM.Taxonomy);
                return View(iTagVM);
            }
        }

        public JsonResult GetTag(string id)
        {
            List<KeyValuePair<int, string>> data = _tagServices.TagSearch(id).ToList();
            return this.Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
