<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Получение документа - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Получение документа</h2>
    <div>
        <p>Начинается скачивание документа с ID=<%=((DocumentView)ViewData["Document"]).ID %>...</p>
    </div>
</asp:Content>
