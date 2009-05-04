<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services" %>

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
        <p class="success">�������������� �������� ��������� �������.</p>
        <%}else{ %>
        <p class="error">�������������� �������� ��������� � �������.</p>
        <p class="error"><%=Html.Encode(ViewData["error"]) %></p>
        <%} %>
        <div>
            <% ViewData["Description"] = ViewData["Description"]; 
               Html.RenderPartial("DocDescriptionView", ViewData); %>
        </div>
        <p>
            <% if (!success) { %>
            <%= Html.ActionLink("����� � ��������������", "EditDescription", new { id = ((DocDescriptionView)ViewData["Description"]).ID })%>
            <% } %>
        </p>
    </div>
</asp:Content>