using System;
using System.Web.Mvc;
using InformationCenter.Services;

namespace InformationCenter.WebUI.Controllers
{
    public class DownloadController : Controller
    {
        //
        // GET: /Download/

        private ServiceCenter serviceCenter = new ServiceCenter(""); 

        public ActionResult Index(Guid id)
        {
            try
            {
                ViewData["Document"] = serviceCenter.DownloadService.Fetch(id);

                return View("Download");
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;

                return View();
            }
        }
    }
}
