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
            catch (FormatException)
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

        public static Dictionary<FieldView, object> GetFieldValues(IEnumerable<FieldView> fields, List<FieldView> selectedFields, HttpContextBase HttpContext, TempDataDictionary TempData, ModelStateDictionary ModelState)
        {
            var descriptionFieldsWithValues = new Dictionary<FieldView, object>();

            foreach (string fieldKey in HttpContext.Request.Params)
            {
                var fieldValueStr = (HttpContext.Request[fieldKey] ?? "").Trim();

                if (fieldKey.StartsWith("_"))
                {
                    Guid fieldId = new Guid(fieldKey.Substring(1));

                    TempData[fieldKey] = fieldValueStr;

                    FieldView field = null;
                    FieldTypeView fieldTypeView = null;
                    Type fieldType = typeof(string);
                    foreach (FieldView f in fields)
                    {
                        if (f.ID == fieldId)
                        {
                            field = f;
                            selectedFields.Add(f);

                            fieldTypeView = f.FieldTypeView;
                            fieldType = f.FieldTypeView.TypeOfField;

                            break;
                        }
                    }
                    if (field == null)
                    {
                        ModelState.AddModelError(fieldKey, "Поле с идентификатором " + fieldId + " не найдено");
                        continue;
                    }

                    try
                    {
                        object fieldValue = Convert.ChangeType(fieldValueStr, fieldType);

                        try
                        {
                            descriptionFieldsWithValues.Add(field, fieldValue);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(fieldKey, "Ошибка в поле " + field.Name + ": " + ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(fieldKey, "Ошибка в поле " + field.Name + ": " + ex.Message + (fieldTypeView == null ? "" : " Ожидаемый тип: " + fieldTypeView.FieldTypeName));
                    }
                }
            }
            return descriptionFieldsWithValues;
        }
    }
}