<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Загрузка завершена - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Загрузка завершена</h2>
    <div>
        <%if (string.IsNullOrEmpty((string)ViewData["error"]))
          { %>
        <p>Загрузка документа завершена успешно.</p>
        <%}else{ %>
        <p>Загрузка документа завершена с ошибкой.</p>
        <p class="error"><%=ViewData["error"] %></p>
        <%} %>
        <p><%=Html.ActionLink("Вернуться к форме загрузки", "Index", "Upload") %></p>
    </div>
</asp:Content>