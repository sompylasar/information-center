<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����� ����
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%FieldView Current = (FieldView)ViewData["SelectedField"];
        string FieldName = "";
        string FieldId = "";
        if (Current != null)
        {
            FieldName = Current.Name;
            FieldId = Current.ID.ToString();
        }

%>
    
    <h2>�������������� ���� "<%=Html.Encode(FieldName)%>"</h2>
    
    <%= Html.ValidationSummary("��������� ������ �����������. ��������� �� � ��������� �������.") %>
    
   
    <form action="/Fields/CommitChanges" id="frmField" method="post" enctype="multipart/form-data">
        <%
            var dataTypes = (IEnumerable<FieldTypeView>)(ViewData["DataTypes"] ?? new FieldTypeView[0]);
        %>
        
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p><span class="success"><%=ViewData["success"]%></span></p>
        <p><label for="fileToUpload">��� ����:</label><input type="text" name="templateName" value="<%=Html.Encode(FieldName) %>" />
        <input type=hidden name="fieldId" value="<%=Html.Encode(FieldId) %>" />
        <div>
            <ul>
                <input type="radio" readonly
            </ul>
        </div>
        <p><button type="submit">���������</button></p>

    </form>
    <form action="/Templates/DeleteTemplate" id="Form2" method="post" enctype="multipart/form-data">
        <input type=hidden name="templateId" value="<%=Html.Encode(TemplateId) %>" />
        <p><button type="submit">������� ������</button></p>
    </form>
            <p>
        <% if (ViewData["Templates"] != null && ((IEnumerable<TemplateView>)ViewData["Templates"]).Count() > 0)
           { %>
            <a href="javascript:window.history.go(-1)">����� � ������ �������</a>
        <% } else { %>
            <%=Html.ActionLink("����� �������", "SelectTemplate", "Templates")%>
        <% } %>
        </p>
</asp:Content>