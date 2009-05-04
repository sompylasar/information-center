<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Редактирование завершено - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Редактирование завершено</h2>
    
    <% bool success = string.IsNullOrEmpty((string)ViewData["error"]); %>
    
    <div>
        <%if (success) { %>
        <p class="success">Редактирование описания завершено успешно.</p>
        <%}else{ %>
        <p class="error">Редактирование описания завершено с ошибкой.</p>
        <p class="error"><%=Html.Encode(ViewData["error"]) %></p>
        <%} %>
        <div>
            <% ViewData["Description"] = ViewData["Description"]; 
               Html.RenderPartial("DocDescriptionView", ViewData); %>
        </div>
        <p>
            <% if (!success) { %>
            <%= Html.ActionLink("Назад к редактированию", "EditDescription", new { id = ((DocDescriptionView)ViewData["Description"]).ID })%>
            <% } %>
        </p>
    </div>
</asp:Content>