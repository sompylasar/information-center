<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="searchResultsTitle" ContentPlaceHolderID="TitleContent" runat="server">
Результаты поиска - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="searchResultsMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="searchResultsContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Результаты поиска</h2>
    <%
        var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);
        var request = (SearchRequest)(ViewData["SearchRequest"] ?? new SearchRequest());
        string query = "", sep = "";
        foreach (SearchItem searchItem in request.Items)
        {
            string fieldName = "", fieldValue = searchItem.FieldValue.ToString();
            foreach (FieldView field in fields)
            {
                if (searchItem.FieldID == field.ID)
                {
                    fieldName = field.Name;
                    break;
                }
            }
            query += sep + fieldName + "=\"" + fieldValue + "\"";
            sep = " " + (request.Mode == SearchMode.And ? "И" : "ИЛИ") + " ";
        }
        
        var results = (IEnumerable<SearchResultItem>)(ViewData["SearchResultItems"] ?? new SearchResultItem[0]);
        int resultsCount = results.Count();
    %>
    <div>
        <p><label>Запрос:</label><span id="query"><%=Html.Encode(query) %></span></p>
        <p><span class="error"><%=resultsCount <= 0 ? "По Вашему запросу не найдено ни одного документа." : Html.Encode(ViewData["error"] ?? "") %></span></p>
        <p class="search-results">
            <span id="total"><%=resultsCount==0 ? "" : "Найдено документов: "+resultsCount.ToString() %></span>
            <ol>
                <%foreach (SearchResultItem result in results){%>
                <li>
                    <div><%=Html.Encode(result.Header) %></div>
                    <div><%=Html.ActionLink("Просмотр", "Index", "Download", new { id = result.ID }, null)%></div>
                </li>
                <%}%>
            </ol>
        </p>
        <p><a href="javascript:window.history.go(-1)">Назад к поиску</a><br />
        <a href="/Search/Index">Новый поиск</a></p>
    </div>
</asp:Content>
