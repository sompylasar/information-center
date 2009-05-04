using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using InformationCenter.Services;
using InformationCenter.WebUI.Models;
using System.Web.Routing;

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
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                TemplateView selectedTemplate = (TemplateView)Session["SelectedTemplate"];
                ViewData["SelectedTemplate"] = selectedTemplate;
                ViewData["SelectedTemplateName"] = (selectedTemplate != null ? selectedTemplate.Name : "");

                Session["SelectedTemplate"] = null;
                Session["UploadSelectedFields"] = null;

                var templates = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();

                // skip template selection if no templates found
                if (templates.Length <= 0) return RedirectToAction("FillDescription");

                ViewData["Templates"] = templates;

                ViewData["error"] = TempData["error"];

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
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

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
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                actionResult = View("FillDescription");

                ViewData["Fields"] = _client.ServiceCenter.SearchService.GetFields();
                ViewData["Templates"] = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();
                TemplateView selectedTemplate = (TemplateView)Session["SelectedTemplate"];
                ViewData["SelectedTemplate"] = selectedTemplate;
                ViewData["SelectedTemplateName"] = (selectedTemplate != null ? selectedTemplate.Name : "");

                var previousModelState = (ModelStateDictionary)TempData["ModelState"];
                if (previousModelState != null)
                {
                    foreach (KeyValuePair<string, ModelState> kvp in previousModelState)
                        if (!ModelState.ContainsKey(kvp.Key))
                            ModelState.Add(kvp.Key, kvp.Value);
                    TempData["ModelState"] = ModelState;
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
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

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

                //var descriptionFieldsWithValues = new Dictionary<FieldView,object>();

                var fields = (IEnumerable<FieldView>)_client.ServiceCenter.SearchService.GetFields();
                var selectedFields = new List<FieldView>();


                var descriptionFieldsWithValues = FieldTypeHelper.GetFieldValues(fields, selectedFields, HttpContext, TempData, ModelState);


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
        public ActionResult EditDescription(Guid? id)
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                try
                {
                    string descriptionIdStr = Request["id"];
                    if (id == null && string.IsNullOrEmpty(descriptionIdStr))
                        throw new Exception("Идентификатор описания не задан.");
                    Guid descriptionId = (id == null ? new Guid(descriptionIdStr) : id.Value);

                    DocDescriptionView description = _client.ServiceCenter.DocumentDescriptionService.GetDescription(descriptionId);
                    if (description == null)
                        throw new Exception("Описание с указанным идентификатором не найдено.");

                    DocumentView document = description.Document;

                    var previousModelState = (ModelStateDictionary)TempData["ModelState"];
                    if (previousModelState != null)
                    {   
                        foreach (KeyValuePair<string, ModelState> kvp in previousModelState)
                            if (!ModelState.ContainsKey(kvp.Key))
                                ModelState.Add(kvp.Key, kvp.Value);
                        TempData["ModelState"] = ModelState;
                    }
                    ViewData["error"] = TempData["error"];
                    ViewData["success"] = TempData["success"];

                    if (ModelState.IsValid)
                    {
                        List<FieldView> selectedFields = new List<FieldView>();
                        foreach (FieldValueView fieldValue in description.DescriptionFieldValues)
                        {
                            selectedFields.Add(fieldValue.Field);
                            ViewData["_" + fieldValue.Field.ID] = fieldValue.Value;
                        }
                        ViewData["SelectedFields"] = selectedFields;
                    }
                    else
                    {
                        foreach (FieldValueView fieldValue in description.DescriptionFieldValues)
                        {
                            ViewData["_" + fieldValue.Field.ID] = TempData["_" + fieldValue.Field.ID];
                        }
                        ViewData["SelectedFields"] = TempData["SelectedFields"];
                    }
                    ViewData["Description"] = description;
                    ViewData["DescriptionName"] = (TempData["DescriptionName"] ?? description.Name);
                    ViewData["Document"] = document;
                    
                    ViewData["Fields"] = _client.ServiceCenter.SearchService.GetFields();

                    TempData["SelectedFields"] = ViewData["SelectedFields"];
                    TempData["DescriptionName"] = ViewData["DescriptionName"];

                    actionResult = View("EditDescription");
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
        public ActionResult UpdateDescription(Guid? id)
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                try
                {
                    string descriptionIdStr = Request["id"];
                    if (id == null && string.IsNullOrEmpty(descriptionIdStr))
                        throw new Exception("Идентификатор описания не задан.");
                    Guid descriptionId = (id == null ? new Guid(descriptionIdStr) : id.Value);

                    DocDescriptionView description = _client.ServiceCenter.DocumentDescriptionService.GetDescription(descriptionId);
                    if (description == null)
                        throw new Exception("Описание с указанным идентификатором не найдено.");

                    DocumentView document = description.Document;


                    string descriptionName = (Request["DescriptionName"] ?? "").Trim();
                    if (string.IsNullOrEmpty(descriptionName))
                    {
                        ModelState.AddModelError("DescriptionName", "Название описания не должно быть пустым.");
                    }
                    TempData["DescriptionName"] = descriptionName;


                    var fields = (IEnumerable<FieldView>)_client.ServiceCenter.SearchService.GetFields();
                    
                    var selectedFields = new List<FieldView>();
                    var descriptionFieldsWithValues = FieldTypeHelper.GetFieldValues(fields, selectedFields, HttpContext, TempData, ModelState);

                    TempData["SelectedFields"] = selectedFields;


                    foreach (KeyValuePair<FieldView,object> fieldValue in descriptionFieldsWithValues)
                    {
                        TempData["_" + fieldValue.Key.ID] = fieldValue.Value;
                    }


                    actionResult = RedirectToAction("EditDescription", new { id = descriptionId });

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            ViewData["Description"] = description;

                            _client.ServiceCenter.DocumentDescriptionService.DeleteDocumentDescription(description);

                            descriptionId = _client.ServiceCenter.UploadService.AddDescription(document.ID, descriptionName, descriptionFieldsWithValues);
                            description = _client.ServiceCenter.DocumentDescriptionService.GetDescription(descriptionId);

                            ViewData["Description"] = description;


                            TempData.Clear();
                            Session["SelectedTemplate"] = null;
                            Session["UploadSelectedFields"] = null;


                            TempData["success"] = "Редактирование описания завершено успешно.";

                            actionResult = View("FinishedEditing");
                            //actionResult = RedirectToAction("EditDescription", new { id = descriptionId });
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
    }
}
