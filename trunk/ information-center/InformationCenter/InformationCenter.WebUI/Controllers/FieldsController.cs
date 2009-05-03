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
            if (AuthHelper.NeedRedirectToAuth(this, "SelectField")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                var fields = _client.ServiceCenter.SearchService.GetFields();


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
            if (AuthHelper.NeedRedirectToAuth(this, "EditField")) return RedirectToAction("LogOn", "Account");

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
            if (AuthHelper.NeedRedirectToAuth(this, "CommitChanges")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("EditField");
            InitServiceCenterClient();
            
            string fieldName = HttpContext.Request["fieldName"];

            string fieldIdStr = (HttpContext.Request["fieldId"] ?? "");
            var dataTypes = _client.ServiceCenter.DocumentDescriptionService.GetFieldTypes();

            if (_client.Available)
            {
                var fields = _client.ServiceCenter.SearchService.GetFields();
                FieldView selectedField = FieldHelper.GetFieldByGUIDStr(fieldIdStr, fields);


                if (selectedField != null)
                {

                    if (selectedField.Name != fieldName)
                    {
                        if (FieldHelper.CheckFieldName(fieldName, fields))
                        {
                            _client.ServiceCenter.DocumentDescriptionService.RenameField(selectedField, fieldName);
                            fields = _client.ServiceCenter.SearchService.GetFields();
                            selectedField = FieldHelper.GetFieldByGUIDStr(fieldIdStr, fields);
                            ViewData["success"] = "Поле успешно сохранено";
                        }
                        else
                        {
                            ViewData["error"] = "Поле с таким именем уже существует, задайте другое имя.";
                        }

                    }
                    else
                    {
                        ViewData["success"] = "Нет изменений";
                    }


                    ViewData["SelectedField"] = selectedField;
                    ViewData["DataTypes"] = dataTypes;

                }


            }
            else
            {
                ViewData["error"] = "Сервис редактирования полей в данный момент недоступен.";
            }

            return actionResult;

        }
        public ActionResult DeleteField()
        {
            if (AuthHelper.NeedRedirectToAuth(this, "DeleteField")) return RedirectToAction("LogOn", "Account");

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
            if (AuthHelper.NeedRedirectToAuth(this, "AddField")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("NewField");
            InitServiceCenterClient();

            string fieldName = HttpContext.Request["fieldName"];
            string dataTypeIdStr = HttpContext.Request["DataType"];

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
                            _client.ServiceCenter.DocumentDescriptionService.AddField(fieldName, fieldType);
                            
                                ViewData["success"] = "Поле успешно создано";
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
            if (AuthHelper.NeedRedirectToAuth(this, "NewField")) return RedirectToAction("LogOn", "Account");

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
