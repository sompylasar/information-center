using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using InformationCenter.Services;
using FormatException = System.FormatException;

namespace InformationCenter.WebUI.Models
{
    public static class FieldHelper
    {
        public static FieldView GetFieldByGUIDStr(string GuidStr, IEnumerable<FieldView> Fields)
        {
            FieldView selectedField = null;

            Guid templateId;
            try
            {
                templateId = new Guid(GuidStr);
            }
            catch (FormatException)
            {
                templateId = Guid.Empty;
            }

            foreach (FieldView fields in Fields)
            {
                if (fields.ID == templateId)
                {
                    selectedField = fields;
                    break;
                }
            }
            return selectedField;
        }




        public static bool CheckFieldName(string Name, IEnumerable<FieldView> Fields)
        {
            bool result = true;
            foreach (FieldView fields in Fields)
            {
                if (fields.Name.ToLower() == Name.ToLower())
                {
                    result = false;
                    break;
                }

            }
            return result;
        }

        
    }
}