using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TNVTH.Web.Areas.Admin.Controllers
{
    public class StartController : Controller
    {
        //
        // GET: /Admin/Home/

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account", new { area = "" });
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

    }
}
