<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����� ����
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
	����� ����
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>�������������� ����</h2>
        <%
            var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);

    %>
    <%= Html.ValidationSummary("��������� ������ �����������. ��������� �� � ��������� �������.") %>
    
    <form action="/Fields/EditField" id="frmSelectField" method="get" enctype="multipart/form-data">
        <p><span class="success"><%=ViewData["success"]%></span></p>
        <p><span class="error"><%=ViewData["error"]%></span></p>
        
        <p><label for="selField">����:</label><select id="selField" name="field">
            <% foreach (FieldView field in fields)
               {  %>
            <option value="<%=field.ID %>"><%=field.Name+" ("+field.FieldTypeView.FieldTypeName+")"%></option>
            <% } %>
            <% if (fields.Count() == 0)
               { %>
            <option value="0">(���� �����������)</option>
            <%} %>
        </select><%= Html.ValidationMessage("TemplateId") %></p>
        <div>
            
        </div>
        <p><button type="submit">�������</button></p>
    </form>
     <%=Html.ActionLink("�����", "Index", "Fields")%>

</asp:Content>

