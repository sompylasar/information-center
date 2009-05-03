using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using InformationCenter.Services;
using InformationCenter.WebUI.Models;

namespace InformationCenter.WebUI.Controllers
{
    public class TemplatesController : Controller
    {
        //
        // GET: /Templates/
        private ServiceCenterClient _client;
        private void InitServiceCenterClient()
        {
            _client = new ServiceCenterClient((string)(Session["UserName"]), (string)(Session["Password"]), (Session["IntegratedSecurity"] == null ? true : (bool)Session["IntegratedSecurity"]));
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewTemplate()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "EditTemplate")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View();
            InitServiceCenterClient();
            if (_client.Available)
            {
                    var fields = _client.ServiceCenter.SearchService.GetFields().ToList();
                  
                    ViewData["SelectedFields"] = null;
                    ViewData["Fields"] = fields;
                
            }
            else
            {
                ViewData["error"] = "Сервис редактирования шаблонов в данный момент недоступен.";
            }

            return actionResult;
        }

        public ActionResult SelectTemplate()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "SelectTemplate")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                var templates = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();


                if (templates.Length <= 0)
                    ViewData["error"] = "Ни одного шаблона не создано";
                else
                    ViewData["Templates"] = templates;

                actionResult = View();
            }
            else
            {
                ViewData["error"] = "Сервис редактирования шаблонов в данный момент недоступен.";
            }

            return actionResult;
        }

        public ActionResult EditTemplate()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "EditTemplate")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View();
            InitServiceCenterClient();
            if (_client.Available)
            {
                string templateIdStr = (Request["tpl"] ?? "");

                TemplateView selectedTemplate = TemplateHelper.GetTemplateByGUIDStr(templateIdStr, _client.ServiceCenter.DocumentDescriptionService.GetTemplates());

                try
                {
                    if (selectedTemplate == null)
                        throw new Exception("Указанный шаблон не найден.");
                    ViewData["SelectedTemplate"] = selectedTemplate;

                    
                    
                    IEnumerable<FieldView> selectedFields = ((IEnumerable<FieldView>)TempData["SelectedFields"]
                        ??  _client.ServiceCenter.DocumentDescriptionService.GetFieldsOfTemplate(selectedTemplate));
                     
                    var fields = _client.ServiceCenter.SearchService.GetFields().ToList();

                    foreach (var field in selectedFields)
                    {
                        fields.Remove(field);
                    }
                    ViewData["SelectedFields"] = selectedFields;
                    ViewData["Fields"] = fields;
                }
                catch (Exception ex)
                {
                    actionResult = View("SelectTemplate");
                    ViewData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "Сервис редактирования шаблонов в данный момент недоступен.";
            }

            return actionResult;
        }

        public ActionResult CommitChanges()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "CommitChanges")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("EditTemplate");
            InitServiceCenterClient();
            string templateName = (string)HttpContext.Request["templateName"];

            string templateIdStr = (HttpContext.Request["templateId"] ?? "");

            if (_client.Available)
            {
                IEnumerable<TemplateView> allTemplates = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();
                TemplateView selectedTemplate = TemplateHelper.GetTemplateByGUIDStr(templateIdStr, allTemplates);

                if (selectedTemplate != null)
                {
                    var fields = (IEnumerable<FieldView>) _client.ServiceCenter.SearchService.GetFields();
                    var selectedFields = new List<FieldView>();
                    foreach (string fieldKey in HttpContext.Request.Params)
                    {
                        var fieldValueStr = HttpContext.Request[fieldKey];

                        if (fieldKey.StartsWith("_"))
                        {
                            Guid fieldId = new Guid(fieldKey.Substring(1));

                            TempData[fieldKey] = fieldValueStr;

                            FieldView field = null;

                            Type fieldType = typeof (string);
                            foreach (FieldView f in fields)
                            {
                                if (f.ID == fieldId)
                                {
                                    field = f;
                                    selectedFields.Add(f);
                                    break;
                                }
                            }
                            if (field == null)
                            {
                                ModelState.AddModelError(fieldKey, "Поле с идентификатором " + fieldId + " не найдено");
                                continue;
                            }

                        }
                    }

                    
                    if (selectedFields.Count > 0)
                    {
                        bool Err = false;
                        if (selectedTemplate.Name != templateName)
                        {
                            if (TemplateHelper.CheckTemplateName(templateName, allTemplates))
                            {
                                _client.ServiceCenter.DocumentDescriptionService.RenameTemplate(selectedTemplate, templateName);
                            }
                            else
                            {
                                Err = true;
                                ViewData["error"] = "Шаблон с таким именем уже существует, задайте другое имя";
                            }
                        }
                        if (!Err)
                        {
                            IEnumerable<FieldView> oldFields =
                                _client.ServiceCenter.DocumentDescriptionService.GetFieldsOfTemplate(selectedTemplate);
                            foreach (FieldView field in oldFields)
                            {
                                _client.ServiceCenter.DocumentDescriptionService.RemoveFieldFromTemplate(
                                    selectedTemplate,
                                    field);
                            }
                            foreach (FieldView field in selectedFields)
                            {
                                _client.ServiceCenter.DocumentDescriptionService.AddFieldToTemplate(selectedTemplate.ID,
                                                                                                    field.ID);
                            }

                            ViewData["success"] = "Шаблон успешно сохранен";
                        }

                    }
                    else
                    {
                        ViewData["error"] = "Не выбрано ни одного поля";
                    }

                    ViewData["SelectedTemplate"] = selectedTemplate;
                    ViewData["SelectedFields"] = selectedFields;
                    ViewData["Fields"] = fields;

                }
                




            }
            else
            {
                ViewData["error"] = "Сервис редактирования шаблонов в данный момент недоступен.";
            }

            return actionResult;

        }

        public ActionResult AddTemplate()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "AddTemplate")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("NewTemplate");
            InitServiceCenterClient();

            string templateName = (string)HttpContext.Request["templateName"];
            if (templateName != null)
                templateName = templateName.Trim();
            List<FieldView> fields = _client.ServiceCenter.SearchService.GetFields().ToList();
            if (_client.Available)
            {
                IEnumerable<TemplateView> allTemplates = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();

                IEnumerable<FieldView> selectedFields = TemplateHelper.GetSelectedFields(HttpContext, fields);
                if (templateName != null && templateName != "")
                {
                    
                    if (selectedFields.Count() > 0)
                    {
                        if (TemplateHelper.CheckTemplateName(templateName, allTemplates))
                        {
                            if (_client.ServiceCenter.DocumentDescriptionService.AddTemplate(templateName,
                                                                                             selectedFields))
                            {
                                ViewData["success"] = "Шаблон успешно создан";
                            }
                            else
                            {
                                ViewData["error"] = "Ошибка сохранения шаблона";
                            }

                        }
                        else
                        {
                            ViewData["error"] = "Шаблон с таким именем уже существует, задайте другое имя";
                        }
                    }
                    else
                    {
                        ViewData["error"] = "Не выбрано ни одного поля";
                    }
                }
                else
                {
                    ViewData["error"] = "Не задано имя шаблона";
                }

                foreach (var field in selectedFields)
                {
                    fields.Remove(field);
                }

                ViewData["TemplateName"] = templateName;
                ViewData["SelectedFields"] = selectedFields;
                ViewData["Fields"] = fields;


            }
            else
            {
                ViewData["error"] = "Сервис редактирования шаблонов в данный момент недоступен.";
            }

            return actionResult;

        }

        public ActionResult DeleteTemplate()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "DeleteTemplate")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("SelectTemplate");
            InitServiceCenterClient();

            string templateIdStr = (HttpContext.Request["templateId"] ?? "");

            if (_client.Available)
            {
                IEnumerable<TemplateView> allTemplates = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();
                TemplateView selectedTemplate = TemplateHelper.GetTemplateByGUIDStr(templateIdStr, allTemplates);
                if (selectedTemplate != null)
                {
                    string tempTmlName = selectedTemplate.Name;
                    _client.ServiceCenter.DocumentDescriptionService.DeleteTemplate(selectedTemplate);
                    ViewData["success"] = "Шаблон \"" + tempTmlName + "\" успешно удален.";

                }
                else
                {
                    ViewData["error"] = "Указанный шаблон не найден.";
                }
                
                allTemplates = _client.ServiceCenter.DocumentDescriptionService.GetTemplates();
                if (allTemplates.Count() <= 0)
                    ViewData["error"] = "Ни одного шаблона не создано";
                else
                    ViewData["Templates"] = allTemplates;

            }
            else
            {
                ViewData["error"] = "Сервис редактирования шаблонов в данный момент недоступен.";
            }

            return actionResult;

        }


    }
}
