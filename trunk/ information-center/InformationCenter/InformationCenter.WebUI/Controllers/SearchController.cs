using System;
using System.Web.Mvc;
using InformationCenter.Data;
using InformationCenter.Services;
using InformationCenter.WebUI.Models;

namespace InformationCenter.WebUI.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        private ServiceCenter serviceCenter = new ServiceCenter(AppSettings.CONNECTION_STRING); 

        public ActionResult Index()
        {
            ViewData["Fields"] = serviceCenter.SearchService.GetFields();
            ViewData["SearchRequest"] = (Session["SearchPrevRequest"] ?? new SearchRequest());
            ViewData["UseAdditionalFields"] = (Session["SearchUseAdditionalFields"] ?? false);

            return View();
        }

        public ActionResult Query(bool? more)
        {
            bool useAdditional = more ?? false;

            var request = new SearchRequest();

            ViewData["Fields"] = serviceCenter.SearchService.GetFields();
            ViewData["SearchRequest"] = request;
            ViewData["UseAdditionalFields"] = useAdditional;

            foreach (string fieldKey in HttpContext.Request.QueryString)
            {
                var fieldValue = HttpContext.Request[fieldKey];

                if (fieldKey.StartsWith("_"))
                {
                    Guid fieldId = new Guid(fieldKey.Substring(1));

                    request.Items.Add(new SearchItem(fieldId, fieldValue));
                }
            }

            Session["SearchPrevRequest"] = request;
            Session["SearchUseAdditionalFields"] = useAdditional;

            try
            {
                var results = serviceCenter.SearchService.Query(request);

                ViewData["SearchResultItems"] = results;

                return View("SearchResults");
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;

                return View("Index");
            }
        }
    }
}
