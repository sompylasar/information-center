using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace InformationCenter.WebUI.Helpers
{
    public class BreadcrumbLink
    {
        public const string DEFAULT_SEPARATOR = "<span class=\"separator\">&nbsp;&rarr;&nbsp;</span>";

        private HtmlHelper _htmlHelper;
        private string _html;
        private string _separator = DEFAULT_SEPARATOR;

        public BreadcrumbLink(HtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
            _html = "";
        }

        public BreadcrumbLink AddHtml(string html)
        {
            _html += (_html != "" ? _separator : "")
                  + html;
            return this;
        }
        public BreadcrumbLink AddTextLink(string linkText)
        {
            return AddHtml("<a href=\"javascript:;\" class=\"text\">" + linkText + "</a>");
        }
        public BreadcrumbLink AddActionLink(string linkText, string actionName)
        {
            return AddHtml(_htmlHelper.ActionLink(linkText, actionName));
        }
        public BreadcrumbLink AddActionLink(string linkText, string actionName, string controllerName)
        {
            return AddHtml(_htmlHelper.ActionLink(linkText, actionName, controllerName));
        }
        public BreadcrumbLink SetSeparator(string separatorHtml)
        {
            _separator = separatorHtml;
            return this;
        }

        public string Last(string linkText)
        {
            return AddHtml("<a href=\"javascript:;\" class=\"text last\">" + linkText + "</a>").Render();
        }
        public string Render()
        {
            return "<div class=\"breadcrumbs\">" + _html + "</div>";
        }
    }

    public static class BreadcrumbsHtmlHelperExtensions
    {
        public static BreadcrumbLink Breadcrumbs(this HtmlHelper htmlHelper)
        {
            return new BreadcrumbLink(htmlHelper);
        }
    }
}