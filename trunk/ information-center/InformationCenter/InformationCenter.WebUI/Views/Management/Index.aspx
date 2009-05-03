<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	”правление
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>”правление</h2>
    <div id="ManagementActions">
        <ul id="Actions">              
            <li><%= Html.ActionLink("”правление шаблонами", "Index", "Templates")%></li>
            <li><%= Html.ActionLink("”правление пол€ми", "Index", "Fields")%></li>
        </ul>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainHeaderContent" runat="server">
</asp:Content>
