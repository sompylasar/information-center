<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>

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
        if (Current != null)
        {
            FieldName = Current.Name;
            FieldId = Current.ID.ToString();
            FieldOrder = Current.Order.ToString();
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
        <p><label for="fieldName">��� ����:</label><input type="text" name="fieldName" value="<%=Html.Encode(FieldName) %>" /></p>
        <p><label for="fieldOrder">�������:</label><input type="text" name="fieldOrder" value="<%=Html.Encode(FieldOrder) %>" /></p>
        <input type=hidden name="fieldId" value="<%=Html.Encode(FieldId) %>" />
        <div>
            <ul style="list-style-type: none;">
            <%
                foreach (FieldTypeView dataType in dataTypes)
            {

%>
                <li><input name="dataType" type="radio" value="<%=dataType.ID%>" <%=Current.FieldTypeView.ID == dataType.ID ? "checked=\"checked\"" : ""%> disabled="disabled"><%=dataType.FieldTypeName%></input></li>
                <%
            }%>
            </ul>
        </div>
        <p><button type="submit">���������</button></p>

    </form>
    <form action="/Fields/DeleteField" id="Form2" method="post" enctype="multipart/form-data">
        <input type=hidden name="fieldId" value="<%=Html.Encode(FieldId) %>" />
        <p><button type="submit">������� ����</button></p>
    </form>
            <p>
       
            <%=Html.ActionLink("����� ����", "SelectField", "Fields")%>

        </p>
</asp:Content>