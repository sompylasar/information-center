using System;
using System.Web.Mvc;
using InformationCenter.Data;
using InformationCenter.Services;
using InformationCenter.WebUI.Models;
using System.Web;

namespace InformationCenter.WebUI.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        private ServiceCenterClient _client;
        private void InitServiceCenterClient()
        {
            _client = new ServiceCenterClient(Session["UserName"] as string, Session["Password"] as string);
        }

        public ActionResult Index()
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                ViewData["Fields"] = _client.ServiceCenter.SearchService.GetFields();
                ViewData["SearchRequest"] = new SearchRequest();
                ViewData["UseAdditionalFields"] = (Session["SearchUseAdditionalFields"] ?? false);

                actionResult = View();
            }
            else
            {
                ViewData["error"] = "������ ������ � ������ ������ ����������." 
                    + " "+_client.ServiceCenterException;
            }

            return actionResult;
        }

        public ActionResult Query(bool? more)
        {
            if (AuthHelper.NeedRedirectToAuth(this,"Index")) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                actionResult = View("Index");

                bool useAdditional = more ?? false;

                var request = new SearchRequest();

                var fields = _client.ServiceCenter.SearchService.GetFields();

                foreach (string fieldKey in HttpContext.Request.Params)
                {
                    var fieldValueStr = HttpContext.Request[fieldKey];
                    bool use = (HttpContext.Request["use" + fieldKey] == "true");
                    if (!use) continue;

                    if (fieldKey.StartsWith("_"))
                    {
                        Guid fieldId = new Guid(fieldKey.Substring(1));

                        TempData[fieldKey] = fieldValueStr;

                        FieldTypeView fieldTypeView = null;
                        Type fieldType = typeof(string);
                        foreach (FieldView field in fields)
                        {
                            if (field.ID == fieldId)
                            {
                                fieldTypeView = field.FieldTypeView;
                                fieldType = field.FieldTypeView.TypeOfField;
                                break;
                            }
                        }
                        
                        object fieldValue = fieldValueStr;
                        try
                        {
                            fieldValue = Convert.ChangeType(fieldValueStr, fieldType);

                            try
                            {
                                request.Items.Add(new SearchItem(fieldId, fieldValue));
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError(fieldKey, ex.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(fieldKey, ex.Message +  (fieldTypeView == null ? "" : " ��������� ���: "+fieldTypeView.FieldTypeName));
                        }
                    }
                }
                ViewData["Fields"] = fields;
                ViewData["SearchRequest"] = request;
                ViewData["UseAdditionalFields"] = useAdditional;

                if (ModelState.IsValid)
                {
                    Session["SearchPrevRequest"] = request;
                    Session["SearchUseAdditionalFields"] = useAdditional;

                    try
                    {
                        var results = _client.ServiceCenter.SearchService.Query(request);

                        ViewData["SearchResultItems"] = results;

                        actionResult = View("SearchResults");
                    }
                    catch (Exception ex)
                    {
                        ViewData["error"] = ex.Message;
                    }
                }
            }
            else
            {
                ViewData["error"] = "������ ������ � ������ ������ ����������.";
            }

            return actionResult;
        }
    }
}