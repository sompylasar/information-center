using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
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
        private bool IsPrevSearchSaved()
        {
            return (Session["PrevSearchRequest"] is SearchRequestView);
        }
        private void SavePrevSearch(SearchRequestView request, bool useAdditionalFields)
        {
            Session["PrevSearchRequest"] = request;
            Session["PrevSearchUseAdditionalFields"] = useAdditionalFields;
        }
        private void ClearPrevSearch()
        {
            SavePrevSearch(new SearchRequestView(), false);
        }

        public ActionResult Index()
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                if (!IsPrevSearchSaved())
                    ClearPrevSearch();

                ViewData["Fields"] = _client.ServiceCenter.SearchService.GetFields();
                ViewData["SearchRequest"] = Session["PrevSearchRequest"];
                ViewData["UseAdditionalFields"] = Session["PrevSearchUseAdditionalFields"];

                actionResult = View();
            }
            else
            {
                ViewData["error"] = "Сервис поиска в данный момент недоступен." 
                    + " "+_client.ServiceCenterException.Message;
            }

            return actionResult;
        }

        public ActionResult New()
        {
            ClearPrevSearch();

            List<string> keys = new List<string>(TempData.Keys);
            foreach (string fieldKey in keys)
            {
                if (fieldKey.StartsWith("_"))
                    TempData[fieldKey] = null;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Query(bool? more)
        {
            if (AuthHelper.NeedRedirectToAuth(this)) return RedirectToAction("LogOn", "Account");

            ActionResult actionResult = View("Error");
            InitServiceCenterClient();
            if (_client.Available)
            {
                actionResult = View("Index");

                var request = new SearchRequestView();
                bool useAdditionalFields = (more ?? false);
                var fields = _client.ServiceCenter.SearchService.GetFields();

                foreach (string fieldKey in HttpContext.Request.Params)
                {
                    if (fieldKey.StartsWith("_"))
                    {
                        string fieldValueStr = (HttpContext.Request[fieldKey] ?? "").Trim();
                        bool use = (HttpContext.Request["use" + fieldKey] == "true");

                        TempData[fieldKey] = fieldValueStr;

                        if (!use) continue;

                        Guid fieldId = new Guid(fieldKey.Substring(1));

                        FieldView field = null;
                        FieldTypeView fieldTypeView = null;
                        Type fieldType = typeof(string);
                        foreach (FieldView f in fields)
                        {
                            if (f.ID == fieldId)
                            {
                                field = f;
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
                                request.Items.Add(new SearchItemView(field, fieldValue));
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError(fieldKey, ex.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(fieldKey,
                                                     ex.Message +
                                                     (fieldTypeView == null
                                                          ? ""
                                                          : " Ожидаемый тип: " + fieldTypeView.FieldTypeName));
                        }
                    }
                }

                ViewData["Fields"] = fields;
                ViewData["SearchRequest"] = request;
                ViewData["UseAdditionalFields"] = useAdditionalFields;

                SavePrevSearch(request, useAdditionalFields);

                if (ModelState.IsValid)
                {
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
            var suggestions = new List<string>();
            var callbackData = new List<object>();

            if (!AuthHelper.NeedRedirectToAuth(this))
            {
                InitServiceCenterClient();
                if (_client.Available)
                {
                    try
                    {
                        FieldView field =
                            _client.ServiceCenter.SearchService.GetFields().Where(f => f.ID == id).FirstOrDefault();
                        if (field != null)
                        {
                            object[] values = _client.ServiceCenter.SearchService.GetValuesOfField(field).Distinct().ToArray();
                            foreach (var value in values)
                            {
                                string valueStr = value.ToString().Trim();
                                if (valueStr.ToUpperInvariant().IndexOf(query.ToUpperInvariant()) < 0) continue;
                                suggestions.Add(valueStr);
                                callbackData.Add(true);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            /*
            if (suggestions.Count <= 0)
            {
                suggestions.Add("(нет вариантов)");
                callbackData.Add(false);
            }*/

            object autocomplete = new
            {
                query = query,
                suggestions = suggestions
                ,data = callbackData
            };

            return Json(autocomplete);
        }
    }
}
