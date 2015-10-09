using Autofac;
using Autofac.Integration.Mvc;
using TNVCMS.Data.DataAcess;
using TNVCMS.Data.DatabaseModel;
using System.Web.Mvc;
using TNVCMS.Domain.Services;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Loader;
using MvcSiteMapProvider.Xml;
using System.Web.Hosting;
using System.Web.Routing;
using MvcSiteMapProvider.Web.Mvc;
using DI.Autofac.Modules;

namespace TNVCMS.Web.App_Start
{
    public class DependencyConfig
    {
        public static void Configure(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(T_NewsServices).Assembly).AsImplementedInterfaces().InstancePerHttpRequest();
            builder.RegisterAssemblyTypes(typeof(T_TagServices).Assembly).AsImplementedInterfaces().InstancePerHttpRequest();
            builder.RegisterAssemblyTypes(typeof(T_News_TagServices).Assembly).AsImplementedInterfaces().InstancePerHttpRequest();

            builder.RegisterControllers(typeof(MvcApplication).Assembly)
                   .PropertiesAutowired();

            builder.RegisterAssemblyTypes(typeof(ContextAdaptor).Assembly)
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            builder.RegisterType<TNVCMSEntities>()
                .As<IDbContext>()
                .InstancePerHttpRequest();

            builder.RegisterType<ContextAdaptor>()
                .As<IObjectSetFactory, IObjectContext>()
                .InstancePerHttpRequest();

            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerHttpRequest();


            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
                .InstancePerHttpRequest();

            // Register modules
            builder.RegisterModule(new MvcSiteMapProviderModule()); // Required
            builder.RegisterModule(new MvcModule()); // Required by MVC. Typically already part of your setup (double check the contents of the module).

            var container = builder.Build();

            // Setup global sitemap loader (required)
            MvcSiteMapProvider.SiteMaps.Loader = container.Resolve<ISiteMapLoader>();

            // Check all configured .sitemap files to ensure they follow the XSD for MvcSiteMapProvider (optional)
            var validator = container.Resolve<ISiteMapXmlValidator>();
            validator.ValidateXml(HostingEnvironment.MapPath("~/Mvc.sitemap"));

            // Register the Sitemaps routes for search engines (optional)
            XmlSiteMapController.RegisterRoutes(RouteTable.Routes);
                        
            var dependencyResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(dependencyResolver);
        }
    }
}