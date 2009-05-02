using System;
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
                    ViewData["error"] = "Ќи одного шаблона не создано";
                else
                    ViewData["Templates"] = templates;

                actionResult = View();
            }
            else
            {
                ViewData["error"] = "—ервис редактировани€ шаблонов в данный момент недоступен.";
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
                bool TemplateSelected = string.IsNullOrEmpty(templateIdStr);

                TemplateView selectedTemplate = null;

                if (!TemplateSelected)
                {
                    Guid templateId;
                    try
                    {
                        templateId = new Guid(templateIdStr);
                    }
                    catch (FormatException ex)
                    {
                        templateId = Guid.Empty;
                    }
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

                try
                {
                    if (selectedTemplate == null)
                        throw new Exception("”казанный шаблон не найден.");
                    ViewData["SelectedTemplate"] = selectedTemplate;

                    ViewData["SelectedFields"] = (TempData["SelectedFields"]
                        ?? (selectedTemplate == null
                             ? new FieldView[0]
                             : _client.ServiceCenter.DocumentDescriptionService.GetFieldsOfTemplate(selectedTemplate)));
                }
                catch (Exception ex)
                {
                    ViewData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "—ервис редактировани€ шаблонов в данный момент недоступен.";
            }

            return actionResult;
        }

        public ActionResult CommitChanges()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "CommitChanges")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View();
            InitServiceCenterClient();
            if (_client.Available)
            {
                var fields = (IEnumerable<FieldView>)_client.ServiceCenter.SearchService.GetFields();
                var selectedFields = new List<FieldView>();
                foreach (string fieldKey in HttpContext.Request.Params)
                {
                    var fieldValueStr = HttpContext.Request[fieldKey];

                    if (fieldKey.StartsWith("_"))
                    {
                        Guid fieldId = new Guid(fieldKey.Substring(1));

                        TempData[fieldKey] = fieldValueStr;

                        FieldView field = null;
                        
                        Type fieldType = typeof(string);
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
                            ModelState.AddModelError(fieldKey, "ѕоле с идентификатором " + fieldId + " не найдено");
                            continue;
                        }

                        
                    }
                }

                ViewData["Fields"] = fields;
                ViewData["SelectedFields"] = selectedFields;


            }
            else
            {
                ViewData["error"] = "—ервис редактировани€ шаблонов в данный момент недоступен.";
            }

            return actionResult;

        }


    }
}
