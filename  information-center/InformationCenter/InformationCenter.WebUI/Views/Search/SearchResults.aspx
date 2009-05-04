<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>
<%@ Import Namespace="LogicUtils"%>
<%@ Import Namespace="InformationCenter.WebUI.Models"%>
<%@ Import Namespace="InformationCenter.WebUI.Helpers"%>


<asp:Content ID="searchResultsTitle" ContentPlaceHolderID="TitleContent" runat="server">
Результаты поиска - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="searchResultsMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/jquery.pagination.js"></script>
    <link rel="stylesheet" type="text/css" href="/Content/jquery.pagination/jquery.pagination.css" />
    <style type="text/css">
        .search-pagination, .search-results 
        {
        	margin-top: 15px;
        	margin-bottom: 5px;
        }
        #query-container label {
        }
        #results-container {
        }
    </style>
</asp:Content>

<asp:Content ID="searchResultsContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Результаты поиска</h2>
    
    <%
        var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);
        var request = (SearchRequestView)(ViewData["SearchRequest"] ?? new SearchRequestView());

        var results = (IEnumerable<DocDescriptionView>)(ViewData["SearchResultItems"] ?? new DocDescriptionView[0]);
        int resultsCount = results.Count();
        
        string query = "", sep = "";
        foreach (SearchItemView searchItem in request.Items)
        {
            string fieldName = "", fieldValue = searchItem.FieldValue.ToString();
            foreach (FieldView field in fields)
            {
                if (searchItem.Field.ID == field.ID)
                {
                    fieldName = field.Name;
                    break;
                }
            }
            query += sep + fieldName + "=\"" + fieldValue + "\"";
            sep = " " + (request.Mode == SearchMode.And ? "И" : "ИЛИ") + " ";
        }
    %>
    <script type="text/javascript">
        jQuery(function ($) {
            var $results_list = $('#results-container .search-results-list');
            var $results = $results_list.find('li');
            var results_length = $results.length;
            
            var $pagination = $(".pagination");
            var extend_pagination = function () {
                $pagination.find("a[href^=#]").unbind('.pagination')
                    .bind('click.pagination', function () { return false; })
                    .each(function () {
                        var $a = $(this);
                        var page = parseInt($.trim($a.text()), 10);
                        if (!isNaN(page))
                            $a.attr({ 'href': '#page='+page, 'page': page });
                    });
            };
            
            var pagination_options = {
                items_per_page: 10,
                num_display_entries: 5,
                num_edge_entries: 2,
                prev_text: 'Предыдущая',
                next_text: 'Следующая',
                callback: function onPageSelect(page_index) {
                    var start_index = page_index*pagination_options.items_per_page;
                    var next_start_index = Math.min((page_index+1) * pagination_options.items_per_page, results_length);
                    
                    $results_list.attr('start', start_index+1);

                    $results.each(function (index) {
                        if (index >= start_index && index < next_start_index) $(this).show();
                        else $(this).hide();
                    });
                    
                    location.hash = '#page='+(page_index+1);
                    extend_pagination();
                }
            };
            
            $pagination.pagination(results_length, pagination_options);
            extend_pagination();
            
            if (results_length < pagination_options.items_per_page) 
                $pagination.parent().hide();
            
            var page_from_hash = /#page=([0-9]+)/.exec(location.hash)[1];
            var current_page = Math.max(1, Math.min(Math.ceil(results_length/pagination_options.items_per_page), page_from_hash));
            $pagination.find('a[page='+current_page+']').click();
        });
    </script>
    
    <div>
        <div id="query-container"><label for="query">Запрос:</label><span id="query"><%=Html.Encode(string.IsNullOrEmpty(query) ? "(пустой)" : query) %></span></div>
        <% string error = (resultsCount <= 0 ? "По Вашему запросу не найдено ни одного документа." : Html.Encode(ViewData["error"] ?? "")); %>
        <% if (!string.IsNullOrEmpty(error)) { %><p><span class="error"><%=error %></span></p><% } %>
        <div class="search-pagination"><label class="pagination-label">Страницы: </label><div class="pagination"></div><div style="clear:both;"></div></div>
        <div id="results-container" class="search-results">
            <span id="total"><%=resultsCount==0 ? "" : "Найдено документов: "+resultsCount.ToString() %></span>
            <ol class="search-results-list">
                <%foreach (DocDescriptionView result in results) {%>
                <li>
                    <% ViewData["Description"] = result; 
                      Html.RenderPartial("DocDescriptionView", ViewData); %>
                </li>
                <%}%>
            </ol>
        </div>
        <div class="search-pagination"><label class="pagination-label">Страницы: </label><div class="pagination"></div><div style="clear:both;"></div></div>
        <p><a href="/Search/Index">Изменить условия поиска</a><br />
        <a href="/Search/New">Новый поиск</a></p>
    </div>
</asp:Content>
