using System;
using System.Web.Mvc;
using InformationCenter.Services;

namespace InformationCenter.WebUI.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        private ServiceCenter serviceCenter = new ServiceCenter(""); 


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Start()
        {
            var file = HttpContext.Request.Files["f"];

            try
            {
                if (file == null)
                    throw new Exception("Файл не загружен.");

                serviceCenter.UploadService.Upload(file.InputStream, file.FileName, file.ContentType, file.ContentLength);

                ViewData["error"] = "";
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;
            }

            return View("Finished");
        }
    }
}
