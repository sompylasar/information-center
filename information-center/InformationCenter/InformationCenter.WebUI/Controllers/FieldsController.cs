using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using InformationCenter.Services;
using InformationCenter.WebUI.Models;

namespace InformationCenter.WebUI.Controllers
{
    public class FieldsController : Controller
    {

        private ServiceCenterClient _client;
        private void InitServiceCenterClient()
        {
            _client = new ServiceCenterClient((string)(Session["UserName"]), (string)(Session["Password"]), (Session["IntegratedSecurity"] == null ? true : (bool)Session["IntegratedSecurity"]));
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SelectField()
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                IEnumerable<FieldView> fields = _client.ServiceCenter.SearchService.GetFields().OrderBy(fld => fld.Order);


                if (fields.Count() <= 0)
                    ViewData["error"] = "Ни одного поля не создано";
                else
                    ViewData["Fields"] = fields;

                actionResult = View();
            }
            else
            {
                ViewData["error"] = "Сервис редактирования полей в данный момент недоступен.";
            }

            return actionResult;
        }

        public ActionResult EditField()
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View();
            InitServiceCenterClient();
            if (_client.Available)
            {
                var fields = _client.ServiceCenter.SearchService.GetFields();
                var dataTypes = _client.ServiceCenter.DocumentDescriptionService.GetFieldTypes();

                string fieldIdStr = (Request["field"] ?? "");

                FieldView selectedField = FieldHelper.GetFieldByGUIDStr(fieldIdStr, fields);

                try
                {
                    if (selectedField == null)
                        throw new Exception("Указанное поле не найдено.");
                    ViewData["SelectedField"] = selectedField;

                    ViewData["DataTypes"] = dataTypes;
                }
                catch (Exception ex)
                {
                    actionResult = View("SelectField");
                    ViewData["error"] = ex.Message;
                }
            }
            else
            {
                ViewData["error"] = "Сервис редактирования полей в данный момент недоступен.";
            }

            return actionResult;
        }

        public ActionResult CommitChanges()
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("EditField");
            InitServiceCenterClient();
            
            string fieldName = HttpContext.Request["fieldName"];
            string fieldOrderStr = HttpContext.Request["fieldOrder"];
            if (!string.IsNullOrEmpty(fieldOrderStr))
                fieldOrderStr = fieldOrderStr.Trim();

            string fieldIdStr = (HttpContext.Request["fieldId"] ?? "");
            var dataTypes = _client.ServiceCenter.DocumentDescriptionService.GetFieldTypes();

            if (_client.Available)
            {
                var fields = _client.ServiceCenter.SearchService.GetFields();
                FieldView selectedField = FieldHelper.GetFieldByGUIDStr(fieldIdStr, fields);


                if (selectedField != null)
                {
                    int fieldOrder;

                    bool ParceResult = int.TryParse(fieldOrderStr, out fieldOrder);
                    if (!(ParceResult && fieldOrder >=0))
                        fieldOrder = -1;
                    bool FieldNameCheckResult = true;
                    if (selectedField.Name != fieldName)
                        FieldNameCheckResult = FieldHelper.CheckFieldName(fieldName, fields);

                    if (selectedField.Order != fieldOrder || selectedField.Name != fieldName)
                    {

                        if (selectedField.Order != fieldOrder && ParceResult && fieldOrder >= 0 ||
                            selectedField.Name != fieldName && FieldNameCheckResult)
                        {
                            if (selectedField.Order != fieldOrder)
                            {
                                
                                _client.ServiceCenter.DocumentDescriptionService.ChangeFieldOrder(selectedField, fieldOrder);
                                ViewData["success"] = "Поле успешно сохранено";
                            }

                            if (selectedField.Name != fieldName)
                            {
                                _client.ServiceCenter.DocumentDescriptionService.RenameField(selectedField, fieldName);
                                fields = _client.ServiceCenter.SearchService.GetFields();
                                selectedField = FieldHelper.GetFieldByGUIDStr(fieldIdStr, fields);
                                ViewData["success"] = "Поле успешно сохранено";
                            }

                        }
                        else
                        {
                            if (selectedField.Order != fieldOrder && !(ParceResult && fieldOrder >= 0))
                            {
                                ViewData["error"] =
                                    "Ошибка при вводе порядка поля. Ожидается положительное целое число или 0";
                            }
                            if (selectedField.Name != fieldName && !FieldNameCheckResult)
                            {
                                ViewData["error"] = "Поле с таким именем уже существует, задайте другое имя.";
                            }
                        }
                    }
                    else
                    {
                        ViewData["success"] = "Нет изменений";
                    }

                }
                ViewData["SelectedField"] = selectedField;
                ViewData["DataTypes"] = dataTypes;




            }
            else
            {
                ViewData["error"] = "Сервис редактирования полей в данный момент недоступен.";
            }

            return actionResult;

        }
        public ActionResult DeleteField()
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("SelectField");
            InitServiceCenterClient();

            string fieldIdStr = (HttpContext.Request["fieldId"] ?? "");

            if (_client.Available)
            {
                var fields = _client.ServiceCenter.SearchService.GetFields();
                FieldView selectedField = FieldHelper.GetFieldByGUIDStr(fieldIdStr, fields);


                if (selectedField != null)
                {
                    string tempFieldName = selectedField.Name;
                    _client.ServiceCenter.DocumentDescriptionService.DeleteField(selectedField);
                    ViewData["success"] = "Поле \"" + tempFieldName + "\" успешно удалено.";

                }
                else
                {
                    ViewData["error"] = "Указанное поле не найдено.";
                }

                fields = _client.ServiceCenter.SearchService.GetFields();


                if (fields.Count() <= 0)
                    ViewData["error"] = "Ни одного поля не создано";
                else
                    ViewData["Fields"] = fields;

            }
            else
            {
                ViewData["error"] = "Сервис редактирования шаблонов в данный момент недоступен.";
            }

            return actionResult;

        }

        public ActionResult AddField()
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("NewField");
            InitServiceCenterClient();

            string fieldName = HttpContext.Request["fieldName"];
            string dataTypeIdStr = HttpContext.Request["DataType"];
            string fieldOrderStr = HttpContext.Request["fieldOrder"];
            string fieldCanBeBlankStr = HttpContext.Request["fieldCanBeBlank"];
            bool fieldCanBeBlank = false;
            if (!string.IsNullOrEmpty(fieldCanBeBlankStr) && fieldCanBeBlankStr == "on")
                fieldCanBeBlank = true;

            if (!string.IsNullOrEmpty(fieldOrderStr))
                fieldOrderStr = fieldOrderStr.Trim();


            if (fieldName != null)
                fieldName = fieldName.Trim();

            

            if (_client.Available)
            {
                var fields = _client.ServiceCenter.SearchService.GetFields();
                var dataTypes = _client.ServiceCenter.DocumentDescriptionService.GetFieldTypes();
                if (!string.IsNullOrEmpty(fieldName))
                {
                    
                    
                    FieldTypeView fieldType = null;
                    if (!string.IsNullOrEmpty(dataTypeIdStr))
                        fieldType = FieldTypeHelper.GetFieldTypeByGUIDStr(dataTypeIdStr, dataTypes);

                    if (fieldType != null)
                    {

                        if (FieldHelper.CheckFieldName(fieldName, fields))
                        {
                            int fieldOrder;

                            if (int.TryParse(fieldOrderStr, out fieldOrder) && fieldOrder >= 0)
                            {
                                
                                _client.ServiceCenter.DocumentDescriptionService.AddField(fieldName, fieldType, fieldCanBeBlank, fieldOrder);

                                ViewData["success"] = "Поле успешно создано";

                            }
                            else
                            {
                                ViewData["error"] =
                                    "Ошибка при вводе порядка поля. Ожидается положительное целое число или 0";
                            }
                        }
                        else
                        {
                            ViewData["error"] = "Поле с таким именем уже существует, задайте другое имя";
                        }
                    }
                    else
                    {
                        ViewData["error"] = "Не выбран тип данных";
                    }
                }
                else
                {
                    ViewData["error"] = "Не задано имя поля";
                }

                ViewData["FieldName"] = fieldName;
                ViewData["FieldOrder"] = fieldOrderStr;
                ViewData["FieldDataType"] = dataTypeIdStr;
                ViewData["FieldCanBeBlank"] = fieldCanBeBlank;

                ViewData["DataTypes"] = dataTypes;

            }
            else
            {
                ViewData["error"] = "Сервис создания полей в данный момент недоступен.";
            }

            return actionResult;

        }

        public ActionResult NewField()
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("NewField");
            InitServiceCenterClient();

            if (_client.Available)
            {
                var dataTypes = _client.ServiceCenter.DocumentDescriptionService.GetFieldTypes();

                ViewData["DataTypes"] = dataTypes;


            }
            else
            {
                ViewData["error"] = "Сервис создания полей в данный момент недоступен.";
            }

            return actionResult;

        }
    }
}
