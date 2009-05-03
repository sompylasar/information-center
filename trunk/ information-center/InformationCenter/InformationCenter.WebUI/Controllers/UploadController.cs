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
            Session["SelectedTemplate"] = null;
            Session["UploadSelectedFields"] = null;

            if (AuthHelper.NeedRedirectToAuth(this, "SelectTemplate")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                var templates = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();

                // skip template selection if no templates found
                if (templates.Length <= 0) return RedirectToAction("FillDescription");

                ViewData["Templates"] = templates;

                ViewData["error"] = TempData["Error"];

                actionResult = View("SelectTemplate");
            }
            else
            {
                ViewData["error"] = "Сервис загрузки документов в данный момент недоступен."
                    + " " + _client.ServiceCenterException.Message;
            }

            return actionResult;
        }

        public ActionResult TemplateSelected()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "SelectTemplate")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                actionResult = RedirectToAction("SelectTemplate");

                string selectedTemplateIdStr = (Request["tpl"] ?? "");
                bool isTemplateSelected = (!string.IsNullOrEmpty(Request["tpl"]));
                TemplateView selectedTemplate = null;

                if (isTemplateSelected)
                {
                    try
                    {
                        Guid templateId = new Guid(selectedTemplateIdStr);
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
                    catch(FormatException)
                    {}
                }

                try
                {
                    if (isTemplateSelected && selectedTemplate == null)
                        throw new Exception("Шаблон с идентификатором "+selectedTemplateIdStr+" не найден.");

                    actionResult = RedirectToAction("FillDescription");
                    Session["SelectedTemplate"] = selectedTemplate;
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "Сервис загрузки документов в данный момент недоступен."
                    + " " + _client.ServiceCenterException.Message;
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
                actionResult = View("FillDescription");

                TemplateView selectedTemplate = (TemplateView)Session["SelectedTemplate"];

                ViewData["Fields"] = _client.ServiceCenter.SearchService.GetFields();
                ViewData["Templates"] = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();
                ViewData["SelectedTemplate"] = selectedTemplate;
                ViewData["SelectedTemplateName"] = (selectedTemplate != null ? selectedTemplate.Name : "");

                if (TempData["ModelState"] != null)
                {
                    foreach (var state in (ModelStateDictionary)TempData["ModelState"])
                    {
                        ModelState.Add(state);
                    }
                    TempData["ModelState"] = null;
                }

                try
                {
                    ViewData["SelectedFields"] = Session["UploadSelectedFields"] ?? (selectedTemplate == null 
                             ? new FieldView[0]
                             : _client.ServiceCenter.DocumentDescriptionService.GetFieldsOfTemplate(selectedTemplate));
                }
                catch (Exception ex)
                {
                    ViewData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "Сервис загрузки документов в данный момент недоступен."
                    + " " + _client.ServiceCenterException.Message;
            }

            return actionResult;
        }

        public ActionResult Start()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "UploadForm")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                actionResult = RedirectToAction("FillDescription");

                var file = HttpContext.Request.Files["f"];
                if (file == null)
                {
                    ModelState.AddModelError("f", "Файл не был загружен.");
                }
                string descriptionName = (Request["DescriptionName"] ?? "").Trim();
                if (string.IsNullOrEmpty(descriptionName))
                {
                    ModelState.AddModelError("DescriptionName", "Название описания не должно быть пустым.");
                }
                TempData["DescriptionName"] = descriptionName;

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
                Session["UploadSelectedFields"] = selectedFields;

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

                            _client.ServiceCenter.UploadService.AddDescription(documentId, descriptionName, descriptionFieldsWithValues);
                        }

                        actionResult = View("Finished");
                        Session["SelectedTemplate"] = null;
                        Session["UploadSelectedFields"] = null;
                    }
                    catch (Exception ex)
                    {
                        TempData["error"] = ex.Message;
                    }
                }
                else
                {
                    TempData["ModelState"] = ModelState;
                }
            }
            else
            {
                ViewData["error"] = "Сервис загрузки документов в данный момент недоступен."
                    + " " + _client.ServiceCenterException.Message;
            }

            return actionResult;
        }
    }
}
