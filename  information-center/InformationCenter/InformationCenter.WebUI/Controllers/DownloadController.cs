using System;
using System.Web.Mvc;
using InformationCenter.Services;
using InformationCenter.WebUI.Models;

namespace InformationCenter.WebUI.Controllers
{
    public class DownloadController : Controller
    {
        //
        // GET: /Download/

        private ServiceCenter serviceCenter = new ServiceCenter(AppSettings.CONNECTION_STRING); 

        public ActionResult Index()
        {
            try
            {
                if (Request["id"] == null)
                    throw new Exception("Идентификатор документа не задан");

                Guid documentId = new Guid(Request["id"]);
                DocumentView document = serviceCenter.DownloadService.GetDocument(documentId);
                if (document == null)
                    throw new Exception("Документ с указанным идентификатором не найден");

                ViewData["Document"] = document;
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
