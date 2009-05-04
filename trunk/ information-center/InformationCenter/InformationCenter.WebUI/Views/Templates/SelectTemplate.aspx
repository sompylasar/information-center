<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>
<%@ Import Namespace="InformationCenter.WebUI.Helpers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	�������������� ������� - �������������� ����� ����
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� �������
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>�������������� ��������</h2>
    <%=Html.Breadcrumbs().AddActionLink("���������� ���������", "Index").Last("����� �������")%>
        <%
        var templates = (IEnumerable<TemplateView>)(ViewData["Templates"] ?? new TemplateView[0]);
    %>
    <%= Html.ValidationSummary("��������� ������ �����������. ��������� �� � ��������� �������.") %>
    
    <form action="/Templates/EditTemplate" id="frmSelectTemplate" method="get" enctype="multipart/form-data">
        <p><span class="success"><%=ViewData["success"]%></span></p>
        <p><span class="error"><%=ViewData["error"]%></span></p>
        
        <p><label for="selTemplate">������:</label><select id="selTemplate" name="tpl">
            <% foreach (TemplateView template in templates) {  %>
            <option value="<%=template.ID %>"><%=template.Name %></option>
            <% } %>
            <% if (templates.Count() == 0)
               { %>
            <option value="0">(������� �����������)</option>
            <%} %>
        </select><%= Html.ValidationMessage("TemplateId") %></p>
        <div>
            
        </div>
        <p><button type="submit">�������</button></p>
    </form>


</asp:Content>
