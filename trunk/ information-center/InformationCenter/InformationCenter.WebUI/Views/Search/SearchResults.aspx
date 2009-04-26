<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Data"%>

<asp:Content ID="searchResultsTitle" ContentPlaceHolderID="TitleContent" runat="server">
Результаты поиска - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="searchResultsMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="searchResultsContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Результаты поиска</h2>
    <%
        var results = (SearchResults)(ViewData["SearchResults"] ?? new SearchResults());
    %>
    <div>
        <p><span class="error"><%=results.Items.Count <= 0 ? "По Вашему запросу не найдено ни одного документа." : ""%></span></p>
        <p class="search-results">
            <ol>
                <%foreach (SearchResultItem result in results.Items){%>
                <li>
                    <div><%=result.Header %></div>
                    <div><%=Html.ActionLink(result.Url, "Index", "Download", new { id = result.ID }, null)%></div>
                </li>
                <%}%>
            </ol>
        </p>
        <p><%=Html.ActionLink("Вернуться к форме поиска", "Index", "Search") %></p>
    </div>
</asp:Content>
