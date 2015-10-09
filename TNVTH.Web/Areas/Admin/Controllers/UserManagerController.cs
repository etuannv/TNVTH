using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TNVTH.Web.Models;
using TNVTH.Web.Services;
using TNVTH.Web.Utilities;
using WebMatrix.WebData;

namespace TNVTH.Web.Areas.Admin.Controllers
{
    public class UserManagerController : Controller
    {
        //
        // GET: /Admin/UserManager/
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult List(string search, int? page)
        {
            UserManagerServices services = new UserManagerServices();
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            IEnumerable<T_UserProfile> ListMenu = services.Search(search);
            int PageSizeAdmin = Convert.ToInt32(TNVTH.Web.GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.PageSizeAdmin.ToString()));
            PageSizeAdmin = (PageSizeAdmin < 1) ? 20 : PageSizeAdmin;
            IPagedList<T_UserProfile> MyList = MvcPaging.PagingExtensions.ToPagedList(ListMenu, currentPageIndex, PageSizeAdmin, ListMenu.Count());
            return View(MyList);
        }

        // GET: /Admin/Menu/AddNew
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult AddNew()
        {
            PopulateRole(null);
            return View();
        }


        private void PopulateRole(int? roleId)
        {
            UserManagerServices services = new UserManagerServices();
            ViewBag.roleId = new SelectList(services.GetRoleList(), "RoleId", "RoleName", roleId);
        }

        // POST: /Admin/Menu/AddNew
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        public ActionResult AddNew([Bind(Include = "Username,Fullname, Email, Mobile,Enabled")]T_UserProfile userProfile, string password, int RoleID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    WebSecurity.CreateUserAndAccount(userProfile.UserName, password);
                    UserManagerServices services = new UserManagerServices();
                    T_UserProfile CurrentUser = services.GetByUsername(userProfile.UserName);
                    CurrentUser.Fullname = userProfile.UserName;
                    CurrentUser.Email = userProfile.Email;
                    CurrentUser.Mobile = userProfile.Mobile;
                    CurrentUser.Enabled = userProfile.Enabled;
                    ReturnValue<bool> ret = services.UpdateT_UserProfile(CurrentUser);

                    //Update role
                    services.SetUserInrole(CurrentUser.UserId, RoleID);

                    if (ret.RetValue)
                    {
                        return RedirectToAction("List", "UserManager");
                    }
                }
            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("", "Tạo tải khoản không thành công");
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");

            }
            return View(userProfile);
        }


        // GET: /Admin/UserManager/Edit
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Edit(int? userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserManagerServices services = new UserManagerServices();
            T_UserProfile userProfile = services.GetByID((int)userId);
            webpages_Roles Role = services.GetRoleByUserId((int)userId);
            if(Role != null)
            {
                PopulateRole(Role.RoleId);
            }
            else
            {
                PopulateRole(null);
            }
            
            if (userProfile == null)
            {
                return HttpNotFound();
            }

            return View("Edit", userProfile);
        }


        // POST: /Admin/UserManager/Edit
        [Authorize]
        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditPost(int? userId, int RoleID)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserManagerServices services = new UserManagerServices();
            T_UserProfile userProfile = services.GetByID((int)userId);
            if (TryUpdateModel(userProfile, "",
               new string[] { "Username", "Fullname", "Email", "Mobile", "Enabled" }))
            {
                try
                {
                    ReturnValue<bool> ret = services.UpdateT_UserProfile(userProfile);
                     //Update role
                    services.SetUserInrole(userProfile.UserId, RoleID);
                    if (ret.RetValue)
                    {
                        return RedirectToAction("List", "UserManager");
                    }
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            webpages_Roles Role = services.GetRoleByUserId((int)userId);
            if (Role != null)
            {
                PopulateRole(Role.RoleId);
            }
            else
            {
                PopulateRole(null);
            }
            return View("Edit", userProfile);
        }


        // GET: /Admin/UserManager/Delete
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult Delete(int? userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserManagerServices services = new UserManagerServices();
            T_UserProfile userProfile = services.GetByID((int)userId);
            return View("Delete", userProfile);
        }


        // POST: /Admin/UserManager/Delete
        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs("POST")]
        public ActionResult Delete(int userId)
        {
            UserManagerServices services = new UserManagerServices();
            services.DeleteT_UserProfile(userId);
            //TODO: Update parent tree
            return RedirectToAction("List", "UserManager");
        }

        // GET: /Admin/UserManager/Delete
        [Authorize]
        [AcceptVerbs("GET")]
        public ActionResult ResetPwd(int? userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserManagerServices services = new UserManagerServices();
            T_UserProfile userProfile = services.GetByID((int)userId);
            var token = WebSecurity.GeneratePasswordResetToken(userProfile.UserName);
            // create a link with this token and send email
            // link directed to an action with form to capture password
            WebSecurity.ResetPassword(token, "12345678a@");
            return RedirectToAction("List", "UserManager");
        }
        

        [Authorize]
        public PartialViewResult GetRole(int? userId)
        {
            if (userId.HasValue)
            {
                UserManagerServices services = new UserManagerServices();
                webpages_Roles RoleList = services.GetRoleByUserId((int)userId);
                if (RoleList != null)
                {
                    return PartialView("GetRole", RoleList.RoleName);
                }
            }
            return PartialView();
        }
    }
}
