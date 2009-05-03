<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Редактирование шаблона - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainHeaderContent" runat="server">
Редактирование шаблона
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Редактирование шаблонов</h2>
        <%
        var templates = (IEnumerable<TemplateView>)(ViewData["Templates"] ?? new TemplateView[0]);
    %>
    <%= Html.ValidationSummary("Введенные данные некорректны. Проверьте их и повторите попытку.") %>
    
    <form action="/Templates/EditTemplate" id="frmSelectTemplate" method="get" enctype="multipart/form-data">
        <p><span class="success"><%=ViewData["success"]%></span></p>
        <p><span class="error"><%=ViewData["error"]%></span></p>
        
        <p><label for="selTemplate">Шаблон:</label><select id="selTemplate" name="tpl">
            <% foreach (TemplateView template in templates) {  %>
            <option value="<%=template.ID %>"><%=template.Name %></option>
            <% } %>
        </select><%= Html.ValidationMessage("TemplateId") %></p>
        <div>
            
        </div>
        <p><button type="submit">Выбрать</button></p>
    </form>

</asp:Content>

