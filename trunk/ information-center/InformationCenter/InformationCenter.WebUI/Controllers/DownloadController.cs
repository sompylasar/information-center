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
    public class DownloadController : Controller
    {
        //
        // GET: /Download/

        private ServiceCenterClient _client;
        private void InitServiceCenterClient()
        {
            _client = new ServiceCenterClient(Session["UserName"] as string, Session["Password"] as string);
        }

        public ActionResult Index(Guid? id, string encodedFilename)
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                actionResult = View("Index");
                try
                {
                    if (id == null && Request["id"] == null)
                        throw new Exception("������������� ��������� �� �����.");

                    Guid documentId = (id == null ? new Guid(Request["id"]) : id.Value);

                    DocumentView document = _client.ServiceCenter.DownloadService.GetDocument(documentId);
                    if (document == null)
                        throw new Exception("�������� � ��������� ��������������� �� ������.");

                    ViewData["Document"] = document;
                    string filename, contentType;
                    FileHelper.SplitFilename(document.FileName, out filename, out contentType);
                    ViewData["Document.FileName"] = filename;
                    ViewData["Document.ContentType"] = filename;

                    if (string.IsNullOrEmpty(encodedFilename))
                    {
                        actionResult = View("Download");
                    }
                    else
                    {
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
                ViewData["error"] = "������ ������ ���������� � ������ ������ ����������.";
            }

            return actionResult;
        }
    }
}