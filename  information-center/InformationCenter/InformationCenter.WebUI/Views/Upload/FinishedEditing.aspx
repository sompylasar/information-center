<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
�������������� ��������� - �������������� ����� ����
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����� ����
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>�������������� ���������</h2>
    
    <% bool success = string.IsNullOrEmpty((string)ViewData["error"]); %>
    
    <div>
        <%if (success) { %>
        <p class="success">�������������� ��������� ��������� �������.</p>
        <%}else{ %>
        <p class="error">�������������� ��������� ��������� � �������.</p>
        <p class="error"><%=Html.Encode(ViewData["error"]) %></p>
        <!--<p>����� ��������� � ��������������, ����������� ������ &laquo;�����&raquo; ������ ��������.</p>-->
        <%} %>
        <p>
            <% if (!success) { %>
            <a href="javascript:window.history.go(-1)">����� � ��������������</a><br />
            <% } %>
           
        </p>
    </div>
</asp:Content>