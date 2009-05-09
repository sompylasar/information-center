<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.WebUI.Models"%>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Удаление документа - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Удаление документа</h2>
    <p class="error"><%=Html.Encode(ViewData["error"]) %></p>
    <p class="success"><%=Html.Encode(ViewData["success"]) %></p>
</asp:Content>
