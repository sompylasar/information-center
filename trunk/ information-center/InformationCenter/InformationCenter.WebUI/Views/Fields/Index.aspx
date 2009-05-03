<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
	Управление полями
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Управление полями</h2>
    <div id="TemplateManagerActions">
        <ul id="TemplateActions">              
            <li><%= Html.ActionLink("Создание поля", "NewField", "Fields")%></li>
            <li><%= Html.ActionLink("Редактирование поля", "SelectField", "Fields")%></li>
        </ul>
    </div>
    
    <%=Html.ActionLink("Назад", "Index", "Management")%>

</asp:Content>

