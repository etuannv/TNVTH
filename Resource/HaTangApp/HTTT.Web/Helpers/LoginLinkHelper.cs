using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Helpers
{
    /// <summary>
    /// This helper method renders a login link section.
    /// 
    /// This helper method is used in the Site.Master View 
    /// Master Page to display the website menu.
    /// </summary>
    public static class LoginLinkHelper
    {
        public static string LoginLink(this HtmlHelper helper)
        {
            string currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
            string currentActionName = (string)helper.ViewContext.RouteData.Values["action"];
            bool isAuthenticated = helper.ViewContext.HttpContext.Request.IsAuthenticated;

            var sb = new StringBuilder();
            if (isAuthenticated)
            {
                sb.Append("Logged in as <span>");
                sb.Append(helper.ViewContext.HttpContext.User.Identity.Name);
                sb.Append("</span>");
            }

            sb.Append("<div id=\"loginlink\"");

            if (currentControllerName.Equals("Account", StringComparison.CurrentCultureIgnoreCase) && (currentActionName.Equals("Login", StringComparison.CurrentCultureIgnoreCase) || currentActionName.Equals("Logout", StringComparison.CurrentCultureIgnoreCase)))
                sb.Append(" class=\"selected\">");
            else
                sb.Append(">");

            if (isAuthenticated)
            {
                sb.Append(helper.ActionLink("Logout", "Logout", "Account"));
            }
            else
            {
                sb.Append(helper.ActionLink("Login", "Logon", "Account"));
            }
            sb.Append("</div>");
            return sb.ToString();
        }
    }
}