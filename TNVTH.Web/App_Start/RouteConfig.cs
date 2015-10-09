using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TNVTH.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            MvcSiteMapProvider.Web.Mvc.XmlSiteMapController.RegisterRoutes(routes);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{slug}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, slug = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "WithID",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }

            );
            routes.MapRoute(
                name: "GetSildeOne",
                url: "{controller}/{action}/{id}/{width}/{height}",
                defaults: new { controller = "Home", action = "List", id = UrlParameter.Optional, width = UrlParameter.Optional, height = UrlParameter.Optional }

            );
        }
    }
}