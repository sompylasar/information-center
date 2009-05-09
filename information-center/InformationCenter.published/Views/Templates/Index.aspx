<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.WebUI.Helpers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
	Управление шаблонами
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Управление шаблонами</h2>
    <%=Html.Breadcrumbs().AddActionLink("Управление", "Index", "Management").Last("Управление шаблонами")%>
    <div id="TemplateManagerActions">
        <ul id="TemplateActions">              
            <li><%= Html.ActionLink("Создание шаблона", "NewTemplate", "Templates")%></li>
            <li><%= Html.ActionLink("Редактирование шаблона", "SelectTemplate", "Templates")%></li>
        </ul>
    </div>

</asp:Content>

