using System;
using System.Collections.Generic;
using System.Linq;
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
            _client = new ServiceCenterClient(
                (string)(Session["UserName"]),
                (string)(Session["Password"]),
                (Session["IntegratedSecurity"] == null ? true : (bool)Session["IntegratedSecurity"]));
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
                ViewData["error"] = "Сервис поиска в данный момент недоступен." 
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

                        bool found = false;
                        FieldTypeView fieldTypeView = null;
                        Type fieldType = typeof(string);
                        foreach (FieldView f in fields)
                        {
                            if (f.ID == fieldId)
                            {
                                found = true;

                                fieldTypeView = f.FieldTypeView;
                                fieldType = f.FieldTypeView.TypeOfField;

                                break;
                            }
                        }
                        if (!found)
                        {
                            ModelState.AddModelError(fieldKey, "Поле с идентификатором " + fieldId + " не найдено");
                            continue;
                        }

                        try
                        {
                            object fieldValue = Convert.ChangeType(fieldValueStr, fieldType);

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
                            ModelState.AddModelError(fieldKey, ex.Message +  (fieldTypeView == null ? "" : " Ожидаемый тип: "+fieldTypeView.FieldTypeName));
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
                ViewData["error"] = "Сервис поиска в данный момент недоступен.";
            }

            return actionResult;
        }


        public ActionResult Autocomplete(Guid? id)
        {
            string query = (Request["query"] ?? "");
            List<string> suggestions = new List<string>();
            List<string> data = new List<string>();

            if (!AuthHelper.NeedRedirectToAuth(this, "Index"))
            {
                InitServiceCenterClient();
                if (_client.Available)
                {
                    FieldView field = _client.ServiceCenter.SearchService.GetFields().Where(f => f.ID == id).FirstOrDefault();
                    if (field != null)
                    {
                        object[] values = _client.ServiceCenter.SearchService.GetValuesOfField(field);
                        foreach (var value in values)
                        {
                            suggestions.Add(value.ToString());
                            data.Add("");
                        }
                    }
                }
            }

            object autocomplete = new
            {
                query = query,
                suggestions = suggestions,
                data = data
            };

            return Json(autocomplete);
        }
    }
}
