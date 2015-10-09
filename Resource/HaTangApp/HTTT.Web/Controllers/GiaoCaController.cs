using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HTTT.HaTangApp.Models;
using HTTT.HaTangApp.Models.Validation;

namespace HTTT.HaTangApp.Controllers
{
    public class GiaoCaController : Controller
    {
        private IHaTangAppServices _service;

        public GiaoCaController()
        {
            _service = new HaTangAppServices(new ModelStateWrapper(this.ModelState));
        }

        public GiaoCaController(IHaTangAppServices service)
        {
            _service = service;
        }
        //
        // GET: /GiaoCa/
        [Authorize]
        public ActionResult Index()
        {
            return View(_service.ListBienBanGiaoCa());
        }

        //
        // GET: /GiaoCa/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /GiaoCa/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /GiaoCa/Create

        [HttpPost]
        [Authorize]
        public ActionResult Create([Bind(Exclude = "Id")] BienBanGiaoCa bienBanGiaoCaToCreate)
        {
            try
            {
                // TODO: Add insert logic here
                if (_service.CreateBienBanGiaoCa(bienBanGiaoCaToCreate))
                {
                    return RedirectToAction("Index");
                }
                
                return View("Create", bienBanGiaoCaToCreate);
                
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /GiaoCa/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var bienBanGiaoCaToEdit = _service.GetBienBanGiaoCa(id);
            return View("Edit", bienBanGiaoCaToEdit);
        }

        //
        // POST: /GiaoCa/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(BienBanGiaoCa bienBanGiaoCaToEdit)
        {
            try
            {
                // TODO: Add update logic here
                if (_service.EditBienBanGiaoCa(bienBanGiaoCaToEdit))
                {
                    return RedirectToAction("Index");
                }
                return View("Edit");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /GiaoCa/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /GiaoCa/Delete/5

        [HttpPost]
        [Authorize]
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
