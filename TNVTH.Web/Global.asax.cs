using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TNVTH.Misc.Web;
using TNVTH.Web.Services;

namespace TNVTH.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Global");
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();
            RegisterCustomeBinder();
            MvcSiteMapProvider.DI.Composer.Compose();
            Application["OnlineVisitors"] = 0;
            Application["TotalVisitors"] = Convert.ToInt32(GlobalConfig.Instance.GetValue(TNVTH.Web.Utilities.Config.VisitCount.ToString()));

        }
        public static void RegisterCustomeBinder()
        {
            ModelBinders.Binders.Add(typeof(DateTime), new CustomeDateBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new CustomeDateBinder());
        }

        public void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            Application["OnlineVisitors"] = (int)Application["OnlineVisitors"] + 1;
            Application["TotalVisitors"] = (int)Application["TotalVisitors"] + 1;
            Application.UnLock();
            //Increase total count
            GlobalConfig.Instance.SetValue(TNVTH.Web.Utilities.Config.VisitCount.ToString(), ((int)Application["TotalVisitors"] + 1).ToString());
        }

        public void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["OnlineVisitors"] = (int)Application["OnlineVisitors"] - 1;
            Application.UnLock();
        }

        protected void Application_BeginRequest()
        {
            if (Request.IsSecureConnection)
            {
                Response.Redirect(Request.Url.AbsoluteUri.Replace("https://", "http://"));
            }
        }
    }
}