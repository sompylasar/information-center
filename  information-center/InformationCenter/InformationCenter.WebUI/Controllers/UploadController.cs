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
            _client = new ServiceCenterClient((string)(Session["UserName"]), (string)(Session["Password"]), (Session["IntegratedSecurity"] == null ? true : (bool)Session["IntegratedSecurity"]));
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
                var templates = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();

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
                    var templates = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();

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
                ViewData["Templates"] = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();
                ViewData["SelectedTemplate"] = selectedTemplate;
                TempData["SelectedTemplate"] = selectedTemplate;

                try
                {
                    if (!useEmptyTemplate && selectedTemplate == null)
                        throw new Exception("Указанный шаблон не найден.");

                    ViewData["SelectedFields"] = (TempData["SelectedFields"] 
                        ?? (selectedTemplate == null 
                             ? new FieldView[0]
                             : _client.ServiceCenter.DocumentDescriptionService.GetFieldsOfTemplate(selectedTemplate)) );
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
                if (file == null)
                {
                    ModelState.AddModelError("f", "Файл не был отправлен");
                }

                var descriptionFieldsWithValues = new Dictionary<FieldView,object>();

                var fields = (IEnumerable<FieldView>)_client.ServiceCenter.SearchService.GetFields();
                var selectedFields = new List<FieldView>();
                foreach (string fieldKey in HttpContext.Request.Params)
                {
                    var fieldValueStr = (HttpContext.Request[fieldKey] ?? "").Trim();

                    if (fieldKey.StartsWith("_"))
                    {
                        Guid fieldId = new Guid(fieldKey.Substring(1));

                        TempData[fieldKey] = fieldValueStr;

                        FieldView field = null;
                        FieldTypeView fieldTypeView = null;
                        Type fieldType = typeof(string);
                        foreach (FieldView f in fields)
                        {
                            if (f.ID == fieldId)
                            {
                                field = f;
                                selectedFields.Add(f);

                                fieldTypeView = f.FieldTypeView;
                                fieldType = f.FieldTypeView.TypeOfField;
                                
                                break;
                            }
                        }
                        if (field == null)
                        {
                            ModelState.AddModelError(fieldKey, "Поле с идентификатором " + fieldId + " не найдено");
                            continue;
                        }

                        try
                        {
                            object fieldValue = Convert.ChangeType(fieldValueStr, fieldType);

                            try
                            {
                                descriptionFieldsWithValues.Add(field, fieldValue);
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError(fieldKey, "Ошибка в поле "+field.Name+": " + ex.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(fieldKey, "Ошибка в поле " + field.Name + ": " + ex.Message + (fieldTypeView == null ? "" : " Ожидаемый тип: " + fieldTypeView.FieldTypeName));
                        }
                    }
                }

                ViewData["Fields"] = fields;
                ViewData["SelectedFields"] = selectedFields;

                if (ModelState.IsValid)
                {
                    try
                    {
                        if (file != null)
                        {
                            Guid documentId = _client.ServiceCenter.UploadService.Upload(file.InputStream,
                                                                       FileHelper.CombineFilename(file.FileName,
                                                                                                  file.ContentType),
                                                                       file.ContentType, file.ContentLength);

                            string descriptionName = (Request["DescriptionName"] ?? (TempData["SelectedTemplate"] != null ? ((TemplateView)TempData["SelectedTemplate"]).Name : ""));
                            _client.ServiceCenter.UploadService.AddDescription(documentId, descriptionName, descriptionFieldsWithValues);
                        }

                        actionResult = View("Finished");
                    }
                    catch (Exception ex)
                    {
                        ViewData["error"] = ex.Message;
                    }
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
