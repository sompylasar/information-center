using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using InformationCenter.Data;
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
            return View();
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
                    
                    //if (selectedTemplate.Name != templateName)
                    //{
                    //    if (TemplateHelper.CheckTemplateName(templateName, allTemplates))
                    //        selectedTemplate.Name = templateName;
                    //}
                    if (selectedFields.Count > 0)
                    {
                        IEnumerable<FieldView> oldFields =
                            _client.ServiceCenter.DocumentDescriptionService.GetFieldsOfTemplate(selectedTemplate);
                        foreach (FieldView field in oldFields)
                        {
                            _client.ServiceCenter.DocumentDescriptionService.RemoveFieldFromTemplate(selectedTemplate,
                                                                                                     field);
                        }
                        foreach (FieldView field in selectedFields)
                        {
                            _client.ServiceCenter.DocumentDescriptionService.AddFieldToTemplate(selectedTemplate.ID,
                                                                                                field.ID);
                        }
                        ViewData["success"] = "Шаблон успешно сохранен";
                        
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


    }
}
