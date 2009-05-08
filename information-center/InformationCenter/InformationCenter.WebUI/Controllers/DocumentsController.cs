using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using InformationCenter.Services;
using InformationCenter.WebUI.Models;

namespace InformationCenter.WebUI.Controllers
{
    public class DocumentsController : Controller
    {
        //
        // GET: /Download/

        private ServiceCenterClient _client;
        private void InitServiceCenterClient()
        {
            _client = new ServiceCenterClient((string)Session["ConnectionString"]);
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Search");
        }

        public ActionResult Download(Guid? id, string encodedFilename)
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                try
                {
                    if (id == null && Request["id"] == null)
                        throw new Exception("Идентификатор документа не задан.");

                    Guid documentId = (id == null ? new Guid(Request["id"]) : id.Value);

                    DocumentView document = _client.ServiceCenter.DocumentsService.GetDocument(documentId);
                    if (document == null)
                        throw new Exception("Документ с указанным идентификатором не найден.");

                    ViewData["Document"] = document;

                    if (string.IsNullOrEmpty(encodedFilename))
                    {
                        actionResult = View("DownloadStarted");
                    }
                    else
                    {
                        string filename = "", contentType = "";
                        FileHelper.SplitFilename(document.FileName, out filename, out contentType);
                        actionResult = new DownloadResult(document.BinaryData, filename, contentType);
                    }
                }
                catch (Exception ex)
                {
                    ViewData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "Сервис выдачи документов в данный момент недоступен."
                    + " " + _client.ServiceCenterException.Message;
            }

            return actionResult;
        }


        public ActionResult Delete(Guid? id)
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                actionResult = View("DeleteFinished");
                try
                {
                    if (id == null && Request["id"] == null)
                        throw new Exception("Идентификатор документа не задан.");

                    Guid documentId = (id == null ? new Guid(Request["id"]) : id.Value);

                    DocumentView document = _client.ServiceCenter.DocumentsService.GetDocument(documentId);
                    if (document == null)
                        throw new Exception("Документ с указанным идентификатором не найден.");

                    string filename = "", contentType = "";
                    FileHelper.SplitFilename(document.FileName, out filename, out contentType);

                    _client.ServiceCenter.DocumentsService.DeleteDocument(document);

                    ViewData["success"] = "Документ \""+filename+"\" успешно удален.";
                }
                catch (Exception ex)
                {
                    ViewData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "Сервис работы с документами в данный момент недоступен."
                    + " " + _client.ServiceCenterException.Message;
            }

            return actionResult;
        }
    }
}
