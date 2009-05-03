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
                    ViewData["error"] = "�� ������ ���� �� �������";
                else
                    ViewData["Fields"] = fields;

                actionResult = View();
            }
            else
            {
                ViewData["error"] = "������ �������������� ����� � ������ ������ ����������.";
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
                        throw new Exception("��������� ���� �� �������.");
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
                ViewData["error"] = "������ �������������� ����� � ������ ������ ����������.";
            }

            return actionResult;
        }

    }
}
