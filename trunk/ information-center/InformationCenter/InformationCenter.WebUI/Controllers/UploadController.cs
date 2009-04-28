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

        private ServiceCenter serviceCenter = new ServiceCenter(AppSettings.CONNECTION_STRING); 


        public ActionResult Index()
        {
            return RedirectToAction("SelectTemplate");
        }

        public ActionResult SelectTemplate()
        {
            var templates = serviceCenter.UploadService.GetTemplates();

            //if (templates.Length <= 0) return RedirectToAction("FillDescription");

            ViewData["Templates"] = templates;

            return View();
        }

        public ActionResult FillDescription()
        {
            string templateIdStr = (Request["TemplateId"] ?? "");
            bool useEmptyTemplate = string.IsNullOrEmpty(templateIdStr);

            TemplateView selectedTemplate = null;

            if (!useEmptyTemplate)
            {
                Guid templateId = new Guid(templateIdStr);
                var templates = serviceCenter.UploadService.GetTemplates();
                
                foreach (TemplateView template in templates)
                {
                    if (template.ID == templateId)
                    {
                        selectedTemplate = template;
                        break;
                    }
                }
            }

            ViewData["Fields"] = serviceCenter.SearchService.GetFields();

            try
            {
                if (!useEmptyTemplate && selectedTemplate == null)
                    throw new Exception("”казанный шаблон не найден");
                
                ViewData["SelectedFields"] = (selectedTemplate == null 
                    ? new FieldView[0] 
                    : serviceCenter.UploadService.GetFieldsOfTemplate(selectedTemplate));

                return View();
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;

                return View();
            }
        }

        public ActionResult Start()
        {
            var file = HttpContext.Request.Files["f"];

            try
            {
                if (file == null || string.IsNullOrEmpty(file.ContentType))
                    throw new Exception("‘айл не загружен.");

                serviceCenter.UploadService.Upload(file.InputStream, file.FileName, file.ContentType, file.ContentLength);

                ViewData["error"] = "";
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;
            }

            return View("Finished");
        }
    }
}
