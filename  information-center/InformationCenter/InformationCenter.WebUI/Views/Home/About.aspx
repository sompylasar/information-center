<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
Об авторах - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="indexMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Об авторах</h2>
    <ul>
        <li>Семенихина О.С. - менеджер проекта</li>
        <li>Бабак И.А. - инженер-программист</li>
        <li>Кретов К.А. - инженер-программист</li>
        <li>Максимов И.А. - инженер-программист</li>
        <li>Шляпенко Д.А. - инженер-программист</li>
    </ul>
</asp:Content>
