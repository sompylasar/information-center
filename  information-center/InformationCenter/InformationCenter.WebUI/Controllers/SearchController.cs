using System;
using System.Web.Mvc;
using InformationCenter.Data;
using InformationCenter.Services;

namespace InformationCenter.WebUI.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        private ServiceCenter serviceCenter = new ServiceCenter(""); 

        private SearchRequest request = new SearchRequest();
        private bool useAdditional = false;

        public ActionResult Index()
        {
            ViewData["SearchRequest"] = request;
            ViewData["UseAdditionalFields"] = useAdditional;

            return View();
        }

        public ActionResult Query(bool? more)
        {
            useAdditional = more ?? false;
            
            ViewData["SearchRequest"] = request;
            ViewData["UseAdditionalFields"] = useAdditional;

            request.Items.Add(new SearchItem(new Guid(), HttpContext.Request.QueryString["t"] ?? ""));
            //request.Items.Add["Author"] = HttpContext.Request.QueryString["a"] ?? "";

            foreach (string fieldKey in HttpContext.Request.QueryString)
            {
                var fieldValue = HttpContext.Request[fieldKey];

                if (fieldKey.StartsWith("_"))
                {
                    string fieldName = fieldKey.Substring(1);
                   // request.Fields[fieldName] = fieldValue ?? "";
                }
            }

            try
            {
                var result = serviceCenter.SearchService.Query(request);

                ViewData["SearchResults"] = result;

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
