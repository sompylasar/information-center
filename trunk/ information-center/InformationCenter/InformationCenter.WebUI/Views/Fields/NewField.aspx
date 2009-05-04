<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>
<%@ Import Namespace="InformationCenter.WebUI.Helpers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����� ����
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
	�������� ����
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <h2>�������� ����</h2>
    <%=Html.Breadcrumbs().AddActionLink("����������", "Index", "Management").AddActionLink("���������� ������", "Index").Last("��������")%>
    
    <% string fieldName = (string)ViewData["FieldName"] ?? "";
       string fieldOrder = (string)ViewData["FieldOrder"] ?? "";
       string fieldDataTypeStr = (string)ViewData["FieldDataType"] ?? "";
       bool fieldCanBeBlank = (bool?)ViewData["FieldCanBeBlank"] ?? false;

         %>
    
    <%= Html.ValidationSummary("��������� ������ �����������. ��������� �� � ��������� �������.") %>
    
   
    <form action="/Fields/AddField" id="frmField" method="post" enctype="multipart/form-data">
        <%
            var dataTypes = (IEnumerable<FieldTypeView>)(ViewData["DataTypes"] ?? new FieldTypeView[0]);

        %>
        
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p><span class="success"><%=ViewData["success"]%></span></p>
        <p><label for="fieldName">��� ����:</label><input type="text" name="fieldName" value="<%=fieldName %>" /></p>
        <p><label for="fieldOrder">�������:</label><input type="text" name="fieldOrder" value="<%=fieldOrder %>" /></p>
        <p><label for="fieldCanBeBlank">���� ����� ���� ������������:</label><input type="checkbox" name="fieldCanBeBlank" <%=fieldCanBeBlank ? "checked=\"checked\"" : ""%>/></p>
        <div>
            <ul style="list-style-type: none;">
            <%
                foreach (FieldTypeView dataType in dataTypes)
            {

%>
                <li><input name="DataType" type="radio" value="<%=dataType.ID%>" <%=dataType.ID.ToString()==fieldDataTypeStr ? "checked=\"checked\"" :"" %>><%=dataType.FieldTypeName%></input></li>
                <%
            }%>
            </ul>
        </div>
        <p><button type="submit">���������</button></p>

    </form>

</asp:Content>