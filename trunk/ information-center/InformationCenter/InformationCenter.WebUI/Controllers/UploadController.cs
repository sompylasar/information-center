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

                ViewData["Fields"] = _client.ServiceCenter.SearchService.GetFields();
                ViewData["Templates"] = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();
                TemplateView selectedTemplate = (TemplateView)Session["SelectedTemplate"];
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
            if (AuthHelper.NeedRedirectToAuth(this, "UploadForm")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View();
            InitServiceCenterClient();
            if (_client.Available)
            {
                try
                {

                    Guid documentId = (id == null ? new Guid(Request["id"]) : id.Value);

                    DocumentView document = _client.ServiceCenter.DownloadService.GetDocument(documentId);
                    if (document == null)
                        throw new Exception("Документ с указанным идентификатором не найден.");

                    List<FieldView> selectedFields = new List<FieldView>();

                    foreach (FieldValueView fieldValue in document.Descriptions[0].DescriptionFieldValues)
                    {
                        selectedFields.Add(fieldValue.Field);
                        TempData["_" + fieldValue.Field.ID] = fieldValue.Value;
                    }

                    ViewData["DocumentId"] = document.ID;
                    ViewData["UploadFileName"] = document.FileName;
                    ViewData["DescriptionName"] = document.Descriptions[0].Name;
                    ViewData["SelectedFields"] = selectedFields;
                    ViewData["Fields"] = _client.ServiceCenter.SearchService.GetFields();

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
        public ActionResult UpdateDescription()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "UploadForm")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                actionResult = View("EditDescription");
 
                string documentIdStr = (Request["DocumentId"] ?? "").Trim();
                if (string.IsNullOrEmpty(documentIdStr))
                {
                    ModelState.AddModelError("DocumentId", "Не выбран документ для редактирования.");
                }

                string descriptionName = (Request["DescriptionName"] ?? "").Trim();
                if (string.IsNullOrEmpty(descriptionName))
                {
                    ModelState.AddModelError("DescriptionName", "Название описания не должно быть пустым.");
                }
                TempData["DescriptionName"] = descriptionName;

                DocumentView document = null;
                try
                {
                    Guid documentId = new Guid(documentIdStr);

                    document = _client.ServiceCenter.DownloadService.GetDocument(documentId);
                    if (document == null)
                        ModelState.AddModelError("Document", "Указанный документ не найден.");

                }
                catch (Exception)
                {
                    ModelState.AddModelError("DocumentId", "Неверный формат идентификатора документа.");
                }
                


                var fields = (IEnumerable<FieldView>)_client.ServiceCenter.SearchService.GetFields();
                var selectedFields = new List<FieldView>();


                var descriptionFieldsWithValues = FieldTypeHelper.GetFieldValues(fields, selectedFields, HttpContext, TempData, ModelState);


                ViewData["DocumentId"] = documentIdStr;
                if (document != null)
                    ViewData["UploadFileName"] = document.FileName;
                ViewData["Fields"] = fields;
                ViewData["SelectedFields"] = selectedFields;
                Session["UploadSelectedFields"] = selectedFields;

                if (ModelState.IsValid)
                {
                    try
                    {
                        _client.ServiceCenter.DocumentDescriptionService.DeleteDocumentDescription(document.Descriptions[0]);

                        _client.ServiceCenter.UploadService.AddDescription(document.ID, descriptionName, descriptionFieldsWithValues);



                        actionResult = View("FinishedEditing");
                        TempData.Clear();
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
