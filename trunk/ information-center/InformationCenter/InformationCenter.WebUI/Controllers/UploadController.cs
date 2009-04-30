using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                ViewData["error"] = "Сервис загрузки документов в данный момент недоступен.";
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
                ViewData["Templates"] = _client.ServiceCenter.UploadService.GetTemplates();
                ViewData["SelectedTemplate"] = selectedTemplate;

                try
                {
                    if (!useEmptyTemplate && selectedTemplate == null)
                        throw new Exception("Указанный шаблон не найден.");

                    ViewData["SelectedFields"] = (TempData["SelectedFields"] 
                        ?? (selectedTemplate == null 
                             ? new FieldView[0]
                             : _client.ServiceCenter.UploadService.GetFieldsOfTemplate(selectedTemplate)) );
                }
                catch (Exception ex)
                {
                    ViewData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "Сервис загрузки документов в данный момент недоступен.";
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
                actionResult = View("FillDescription");

                var file = HttpContext.Request.Files["f"];

                var fields = (IEnumerable<FieldView>)_client.ServiceCenter.SearchService.GetFields();
                var selectedFields = new List<FieldView>();
                foreach (string fieldKey in HttpContext.Request.Params)
                {
                    var fieldValue = HttpContext.Request[fieldKey];

                    if (fieldKey.StartsWith("_"))
                    {
                        Guid fieldId = new Guid(fieldKey.Substring(1));

                        TempData[fieldKey] = fieldValue;
                        foreach (FieldView field in fields)
                        {
                            if (field.ID.ToString() == fieldId.ToString())
                            {
                                selectedFields.Add(field);
                                Debug.WriteLine(field); 
                            }
                        }
                    }
                }

                ViewData["Fields"] = fields;
                ViewData["SelectedFields"] = selectedFields;

                try
                {
                    if (file == null)
                        throw new Exception("Файл не был отправлен.");

                    _client.ServiceCenter.UploadService.Upload(file.InputStream, 
                        file.FileName, file.ContentType, file.ContentLength);

                    actionResult = View("Finished");
                }
                catch (Exception ex)
                {
                    ViewData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "Сервис загрузки документов в данный момент недоступен.";
            }

            return actionResult;
        }
    }
}
