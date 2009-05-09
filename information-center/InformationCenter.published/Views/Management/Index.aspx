<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Управление
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Управление</h2>
    <div id="ManagementActions">
        <ul id="Actions">              
            <li><%= Html.ActionLink("Управление шаблонами", "Index", "Templates")%></li>
            <li><%= Html.ActionLink("Управление полями", "Index", "Fields")%></li>
        </ul>
    </div>

</asp:Content>

