<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Загрузка документов - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        jQuery(function($) {
            
        });
    </script>

    <h2>Выбор шаблона описания</h2>
    
    <%
        var templates = (IEnumerable<TemplateView>)(ViewData["Templates"] ?? new TemplateView[0]);
    %>
    <%= Html.ValidationSummary("Введенные данные некорректны. Проверьте их и повторите попытку.") %>
    
    <form action="/Upload/FillDescription" id="frmSelectTemplate" method="get" enctype="multipart/form-data">
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p><label for="selTemplate">Шаблон:</label><select id="selTemplate" name="tpl">
            <option value="">(не использовать)</option>
            <% foreach (TemplateView template in templates) {  %>
            <option value="<%=template.ID %>"><%=template.Name %></option>
            <% } %>
        </select><%= Html.ValidationMessage("TemplateId") %></p>
        <div>
            
        </div>
        <p><button type="submit">Выбрать</button></p>
    </form>
</asp:Content>