using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HTTT.HaTangApp.Models;
using HTTT.HaTangApp.Models.Validation;
using HTTT.HaTangApp.Models.ViewModel;
using System.Web.Profile;
namespace HTTT.HaTangApp.Areas.Admin.Controllers
{
    public class UserManageController : Controller
    {
         private IHaTangAppServices _service;

        public UserManageController()
        {
            _service = new HaTangAppServices(new ModelStateWrapper(this.ModelState));
        }

        public UserManageController(IHaTangAppServices service)
        {
            _service = service;
        }

        //
        // GET: /Admin/UserManage/

        public ActionResult ListUsers()
        {

            return View(_service.ListUsers());
        }

        //
        // GET: /Admin/UserManage/Details/5

        public ActionResult Details(Guid userId)
        {
            return View(_service.GetUser(userId));
        }

        //
        // GET: /Admin/UserManage/Create

        public ActionResult CreateUser()
        {
            return View();
        } 

        //
        // POST: /Admin/UserManage/Create

        [HttpPost]
        public ActionResult CreateUser(UserViewModel userToCreate)
        {
            try
            {
                if (_service.CreateUser(userToCreate))
                {
                    return RedirectToAction("ListUsers");
                }
                else
                {
                    return View(userToCreate);
                }
            }
            catch
            {
                return View(userToCreate);
            }
        }
        
        //
        // GET: /Admin/UserManage/Edit/5
 
        public ActionResult Edit(Guid id)
        {
            aspnet_Users cUser = _service.GetUser(id);
            UserViewModel pUser = new UserViewModel(cUser);
            return View(pUser);
        }

        //
        // POST: /Admin/UserManage/Edit/5

        [HttpPost]
        public ActionResult Edit(Guid id, UserViewModel userToEdit)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Admin/UserManage/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Admin/UserManage/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
