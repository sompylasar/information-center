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
                    ViewData["error"] = "Ќи одного пол€ не создано";
                else
                    ViewData["Fields"] = fields;

                actionResult = View();
            }
            else
            {
                ViewData["error"] = "—ервис редактировани€ полей в данный момент недоступен.";
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
                        throw new Exception("”казанное поле не найдено.");
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
                ViewData["error"] = "—ервис редактировани€ полей в данный момент недоступен.";
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
                            ViewData["success"] = "ѕоле успешно сохранено";
                        }
                        else
                        {
                            ViewData["error"] = "ѕоле с таким именем уже существует, задайте другое им€.";
                        }

                    }
                    else
                    {
                        ViewData["success"] = "Ќет изменений";
                    }


                    ViewData["SelectedField"] = selectedField;
                    ViewData["DataTypes"] = dataTypes;

                }


            }
            else
            {
                ViewData["error"] = "—ервис редактировани€ полей в данный момент недоступен.";
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
                    ViewData["success"] = "ѕоле \"" + tempFieldName + "\" успешно удалено.";

                }
                else
                {
                    ViewData["error"] = "”казанное поле не найден.";
                }

                fields = _client.ServiceCenter.SearchService.GetFields();


                if (fields.Count() <= 0)
                    ViewData["error"] = "Ќи одного пол€ не создано";
                else
                    ViewData["Fields"] = fields;

            }
            else
            {
                ViewData["error"] = "—ервис редактировани€ шаблонов в данный момент недоступен.";
            }

            return actionResult;

        }
    }
}
