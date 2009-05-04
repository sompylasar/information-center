<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>
<%@ Import Namespace="InformationCenter.WebUI.Helpers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����� ����
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
	�������������� ����
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%FieldView Current = (FieldView)ViewData["SelectedField"];
        string FieldName = "";
        string FieldId = "";
        string FieldOrder = "";
        string FieldType = "";
        bool fieldCanBeBlack = false;
        if (Current != null)
        {
            FieldName = Current.Name;
            FieldId = Current.ID.ToString();
            FieldOrder = Current.Order.ToString();
            FieldType = Current.FieldTypeView.FieldTypeName;
            fieldCanBeBlack = Current.Nullable;
        }
%>
    
    <h2>�������������� ���� "<%=Html.Encode(FieldName)%>"</h2>
    <%=Html.Breadcrumbs().AddActionLink("����������", "Index", "Management").AddActionLink("���������� ������", "Index").AddActionLink("�����", "SelectField").Last("��������������")%>
    
    <%= Html.ValidationSummary("��������� ������ �����������. ��������� �� � ��������� �������.") %>
    
   
    <form action="/Fields/CommitChanges" id="frmField" method="post" enctype="multipart/form-data">
        <%
            var dataTypes = (IEnumerable<FieldTypeView>)(ViewData["DataTypes"] ?? new FieldTypeView[0]);
            

        %>
        
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p><span class="success"><%=ViewData["success"]%></span></p>
        <p><label for="fieldName">��� ����:</label><input type="text" name="fieldName" value="<%=Html.Encode(FieldName) %>" /></p>
        <p><label for="fieldOrder">�������:</label><input type="text" name="fieldOrder" value="<%=Html.Encode(FieldOrder) %>" /></p>
        <input type=hidden name="fieldId" value="<%=Html.Encode(FieldId) %>" />
        <p><label for="fieldCanBeBlank">���� ����� ���� ������������:</label><input type="checkbox" disabled="disabled" name="fieldCanBeBlank" <%=fieldCanBeBlack==true ? "checked=\"checked\"" : ""%>/></p>
        <p><label for="fieldType">��� ������: <%=Html.Encode(FieldType) %></label></p>
        <p><button type="submit">���������</button></p>

    </form>
    <form action="/Fields/DeleteField" id="Form2" method="post" enctype="multipart/form-data">
        <input type=hidden name="fieldId" value="<%=Html.Encode(FieldId) %>" />
        <p><button type="submit">������� ����</button></p>
    </form>
 
</asp:Content>