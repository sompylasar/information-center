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
            $results_list = $('#results-container .search-results-list');
            $results = $results_list.find('li');
            
            var results_length = $results.length;
            
            var pagination_options = {
                items_per_page: 10,
                num_display_entries: 5,
                num_edge_entries: 2,
                prev_text: 'Предыдущая',
                next_text: 'Следующая',
                callback: function onPageSelect(page_index, $pagination) {
                    var start_index = page_index*pagination_options.items_per_page;
                    var next_start_index = Math.min((page_index+1) * pagination_options.items_per_page, results_length);

                    $results.each(function (index) {
                        if (index >= start_index && index < next_start_index) $(this).show();
                        else $(this).hide();
                    });
                }
            };
            $(".pagination").pagination(results_length, pagination_options);
            
            if (results_length < pagination_options.items_per_page) 
                $(".pagination").parent().hide();
        });
    </script>
    
    <div>
        <div id="query-container"><label for="query">Запрос:</label><span id="query"><%=Html.Encode(string.IsNullOrEmpty(query) ? "(пустой)" : query) %></span></div>
        <% string error = (resultsCount <= 0 ? "По Вашему запросу не найдено ни одного документа." : Html.Encode(ViewData["error"] ?? "")); %>
        <% if (!string.IsNullOrEmpty(error)) { %><p><span class="error"><%=error %></span></p><% } %>
        <div><label class="pagination-label">Страницы: </label><div class="pagination"></div><div style="clear:both;"></div></div>
        <p id="results-container" class="search-results">
            <span id="total"><%=resultsCount==0 ? "" : "Найдено документов: "+resultsCount.ToString() %></span>
            <ol class="search-results-list">
                <%foreach (DocDescriptionView result in results){
                      string filename, contentType;
                      FileHelper.SplitFilename(result.Document.FileName, out filename, out contentType);
                      %>
                <li>
                    <div><span class="file-name"><%=Html.ActionLink(filename, "Index", "Download", new { id = result.Document.ID }, null)%></span><span class="file-type"><%=string.IsNullOrEmpty(contentType) ? "" : "("+Html.Encode(contentType)+")" %></span></div>
                    <div>
                        <label class="description-name"><%=Html.Encode(result.Name) %></label>
                        <ul class="description-fields">
                        <% foreach (FieldValueView descriptionField in result.DescriptionFields) { %>
                        <li><%=Html.Encode(descriptionField.Field.Name + "=\"" + descriptionField.Value + "\"") %></li>
                        <% } %>
                        </ul>
                    </div>
                    <div></div>
                </li>
                <%}%>
            </ol>
        </p>
        <div><label class="pagination-label">Страницы: </label><div class="pagination"></div><div style="clear:both;"></div></div>
        <p><a href="/Search/Index">Изменить условия поиска</a><br />
        <a href="/Search/New">Новый поиск</a></p>
    </div>
</asp:Content>
