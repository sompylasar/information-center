using System;
using System.Collections.Generic;
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
                if (template.Name == Name)
                {
                    result = false;
                    break;
                }

            }
            return result;
        }
    }
}