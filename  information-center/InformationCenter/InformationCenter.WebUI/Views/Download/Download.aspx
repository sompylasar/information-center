<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Data"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
��������� ��������� - �������������� ����� ����
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����� ����
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>��������� ���������</h2>
    <div>
        <p>���������� ���������� ��������� � ID=<%=((Document)ViewData["Document"]).ID %>...</p>
    </div>
</asp:Content>
