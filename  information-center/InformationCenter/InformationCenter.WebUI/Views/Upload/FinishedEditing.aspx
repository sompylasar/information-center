<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

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
        <p class="success">Редактирование документа завершено успешно.</p>
        <%}else{ %>
        <p class="error">Редактирование документа завершено с ошибкой.</p>
        <p class="error"><%=Html.Encode(ViewData["error"]) %></p>
        <!--<p>Чтобы вернуться к редактированию, используйте кнопку &laquo;Назад&raquo; Вашего браузера.</p>-->
        <%} %>
        <p>
            <% if (!success) { %>
            <a href="javascript:window.history.go(-1)">Назад к редактированию</a><br />
            <% } %>
           
        </p>
    </div>
</asp:Content>