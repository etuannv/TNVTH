using System.Web;
using System.Web.Mvc;

namespace TNVTH.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new TNVTH.Web.Filters.InitializeSimpleMembershipAttribute());
        }
    }
}