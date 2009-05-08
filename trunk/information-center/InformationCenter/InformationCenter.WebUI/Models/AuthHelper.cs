using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace InformationCenter.WebUI.Models
{
    public static class AuthHelper
    {
        public static bool NeedRedirectToAuth(Controller controller)
        {
            if (!controller.Request.IsAuthenticated)
            {
                controller.Session["ReturnRedirect"] = new RedirectResult(controller.Request.RawUrl);
                return true;
            }
            return false;
        }
    }
}