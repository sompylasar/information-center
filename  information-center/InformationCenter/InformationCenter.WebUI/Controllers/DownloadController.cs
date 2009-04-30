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

        private ServiceCenterClient _client;
        private void InitServiceCenterClient()
        {
            _client = new ServiceCenterClient(Session["UserName"] as string, Session["Password"] as string);
        }

        public ActionResult Index()
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                actionResult = View("Index");
                try
                {
                    if (Request["id"] == null)
                        throw new Exception("Идентификатор документа не задан");

                    Guid documentId = new Guid(Request["id"]);
                    DocumentView document = _client.ServiceCenter.DownloadService.GetDocument(documentId);
                    if (document == null)
                        throw new Exception("Документ с указанным идентификатором не найден");

                    ViewData["Document"] = document;

                    actionResult = View("Download");
                }
                catch (Exception ex)
                {
                    ViewData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "Сервис выдачи документов в данный момент недоступен.";
            }

            return actionResult;
        }
    }
}
