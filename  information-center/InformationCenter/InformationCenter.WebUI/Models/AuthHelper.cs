using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace InformationCenter.WebUI.Models
{
    public static class AuthHelper
    {
        public static bool NeedRedirectToAuth(Controller controller)
        {
            return NeedRedirectToAuth(controller, null);
        }

        public static bool NeedRedirectToAuth(Controller controller, string actionNameToReturnTo)
        {
            if (!controller.Request.IsAuthenticated)
            {
                controller.Session["ReturnRedirect"] = new RedirectResult("/" 
                    + (controller.RouteData.Values["controller"] != null 
                        ? (string)controller.RouteData.Values["controller"]
                            + "/" + (string)(actionNameToReturnTo ?? controller.RouteData.Values["action"])
                        : ""));
                return true;
            }
            return false;
        }
    }
}