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
                ViewData["SearchRequest"] = (Session["SearchPrevRequest"] ?? new SearchRequest());
                ViewData["UseAdditionalFields"] = (Session["SearchUseAdditionalFields"] ?? false);

                actionResult = View();
            }
            else
            {
                ViewData["error"] = "—ервис поиска в данный момент недоступен." 
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

                ViewData["Fields"] = _client.ServiceCenter.SearchService.GetFields();
                ViewData["SearchRequest"] = request;
                ViewData["UseAdditionalFields"] = useAdditional;

                foreach (string fieldKey in HttpContext.Request.QueryString)
                {
                    var fieldValue = HttpContext.Request[fieldKey];

                    if (fieldKey.StartsWith("_"))
                    {
                        Guid fieldId = new Guid(fieldKey.Substring(1));

                        try
                        {
                            request.Items.Add(new SearchItem(fieldId, fieldValue));
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(fieldId + "_error", ex.Message);
                        }
                    }
                }

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
                ViewData["error"] = "—ервис поиска в данный момент недоступен.";
            }

            return actionResult;
        }
    }
}
