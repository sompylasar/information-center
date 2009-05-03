using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using InformationCenter.Services;
using FormatException=System.FormatException;

namespace InformationCenter.WebUI.Models
{
    public static class TemplateHelper
    {
        public static TemplateView GetTemplateByGUIDStr(string GuidStr, IEnumerable<TemplateView> Templates)
        {
            TemplateView selectedTemplate = null;

            Guid templateId;
            try
            {
                templateId = new Guid(GuidStr);
            }
            catch (FormatException ex)
            {
                templateId = Guid.Empty;
            }

            foreach (TemplateView template in Templates)
            {
                if (template.ID == templateId)
                {
                    selectedTemplate = template;
                    break;
                }
            }
            return selectedTemplate;
        }




        public static bool CheckTemplateName(string Name,  IEnumerable<TemplateView> Templates)
        {
            bool result = true;
            foreach (TemplateView template in Templates)
            {
                if (template.Name.ToLower() == Name.ToLower())
                {
                    result = false;
                    break;
                }

            }
            return result;
        }

        public static IEnumerable<FieldView> GetSelectedFields(HttpContextBase HttpCtx, IEnumerable<FieldView> AllFields)
        {
            List<FieldView> selectedFields = new List<FieldView>();
            foreach (string fieldKey in HttpCtx.Request.Params)
            {
                if (fieldKey.StartsWith("_"))
                {
                    Guid fieldId = new Guid(fieldKey.Substring(1));
                    foreach (FieldView f in AllFields)
                    {
                        if (f.ID == fieldId)
                        {
                            selectedFields.Add(f);
                            break;
                        }
                    }
                    
                }
            }
            return selectedFields;
        }
    }
}