<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
�������� ��������� - �������������� ����� ����
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����� ����
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>�������� ���������</h2>
    <div>
        <%if (string.IsNullOrEmpty((string)ViewData["error"]))
          { %>
        <p>�������� ��������� ��������� �������.</p>
        <%}else{ %>
        <p>�������� ��������� ��������� � �������.</p>
        <p class="error"><%=ViewData["error"] %></p>
        <%} %>
        <p><%=Html.ActionLink("��������� � ����� ��������", "Index", "Upload") %></p>
    </div>
</asp:Content>