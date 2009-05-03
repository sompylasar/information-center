using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using InformationCenter.Services;
using FormatException = System.FormatException;

namespace InformationCenter.WebUI.Models
{
    public static class FieldTypeHelper
    {
        public static FieldTypeView GetFieldTypeByGUIDStr(string GuidStr, IEnumerable<FieldTypeView> FieldTypes)
        {
            FieldTypeView selectedFieldType = null;

            Guid Id;
            try
            {
                Id = new Guid(GuidStr);
            }
            catch (FormatException ex)
            {
                Id = Guid.Empty;
            }

            foreach (FieldTypeView fieldTypes in FieldTypes)
            {
                if (fieldTypes.ID == Id)
                {
                    selectedFieldType = fieldTypes;
                    break;
                }
            }
            return selectedFieldType;
        }
    }
}