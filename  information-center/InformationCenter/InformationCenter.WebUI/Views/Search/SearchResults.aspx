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
        var results = (IEnumerable<SearchResultItem>)(ViewData["SearchResultItems"] ?? new SearchResultItem[0]);
    %>
    <div>
        <p><span class="error"><%=results.Count() <= 0 ? "По Вашему запросу не найдено ни одного документа." : ViewData["error"]??""%></span></p>
        <p class="search-results">
            <ol>
                <%foreach (SearchResultItem result in results){%>
                <li>
                    <div><%=result.Header %></div>
                    <div><%=Html.ActionLink("Просмотр", "Index", "Download", new { id = result.ID }, null)%></div>
                </li>
                <%}%>
            </ol>
        </p>
        <p><a href="javascript:window.history.go(-1)">Назад к поиску</a><br />
        <%=Html.ActionLink("Новый поиск", "Index", "Search") %></p>
    </div>
</asp:Content>
