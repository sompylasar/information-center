<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
Выбор сервиса - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="indexMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Выбор сервиса</h2>
    <ul>
        <li><%=Html.ActionLink("Поиск", "Index", "Search")%></li>
        <li><%=Html.ActionLink("Загрузка", "Index", "Upload")%></li>
    </ul>
</asp:Content>
