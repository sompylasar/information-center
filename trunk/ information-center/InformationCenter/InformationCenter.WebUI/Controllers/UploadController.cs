using System;
using System.Collections.Generic;
using System.Web.Mvc;
using InformationCenter.Services;
using InformationCenter.WebUI.Models;

namespace InformationCenter.WebUI.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        private ServiceCenterClient _client;
        private void InitServiceCenterClient()
        {
            _client = new ServiceCenterClient(Session["UserName"] as string, Session["Password"] as string);
        }

        public ActionResult Index()
        {
            //if (NeedRedirectToAuth("Index")) return RedirectToAction("LogOn", "Account");

            return RedirectToAction("SelectTemplate");
        }

        public ActionResult SelectTemplate()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "SelectTemplate")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                var templates = _client.ServiceCenter.UploadService.GetTemplates();

                // skip template selection if no templates found
                if (templates.Length <= 0) return RedirectToAction("FillDescription");

                ViewData["Templates"] = templates;

                actionResult = View();
            }
            else
            {
                ViewData["error"] = "—ервис загрузки документов в данный момент недоступен.";
            }

            return actionResult;
        }

        public ActionResult FillDescription()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "SelectTemplate")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                actionResult = View();

                string templateIdStr = (Request["tpl"] ?? "");
                bool useEmptyTemplate = string.IsNullOrEmpty(templateIdStr);

                TemplateView selectedTemplate = null;

                if (!useEmptyTemplate)
                {
                    Guid templateId = new Guid(templateIdStr);
                    var templates = _client.ServiceCenter.UploadService.GetTemplates();

                    foreach (TemplateView template in templates)
                    {
                        if (template.ID == templateId)
                        {
                            selectedTemplate = template;
                            break;
                        }
                    }
                }

                ViewData["Fields"] = _client.ServiceCenter.SearchService.GetFields();

                try
                {
                    if (!useEmptyTemplate && selectedTemplate == null)
                        throw new Exception("”казанный шаблон не найден.");

                    ViewData["SelectedFields"] = (selectedTemplate == null
                                                      ? new FieldView[0]
                                                      : _client.ServiceCenter.UploadService.GetFieldsOfTemplate(selectedTemplate));
                }
                catch (Exception ex)
                {
                    ViewData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "—ервис загрузки документов в данный момент недоступен.";
            }

            return actionResult;
        }

        public ActionResult Start()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "SelectTemplate")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                actionResult = View("Finished");

                var file = HttpContext.Request.Files["f"];

                try
                {
                    if (file == null || string.IsNullOrEmpty(file.ContentType))
                        throw new Exception("‘айл не загружен.");

                    _client.ServiceCenter.UploadService.Upload(file.InputStream, file.FileName, file.ContentType,
                                                       file.ContentLength);
                }
                catch (Exception ex)
                {
                    ViewData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "—ервис загрузки документов в данный момент недоступен.";
            }

            return actionResult;
        }
    }
}
