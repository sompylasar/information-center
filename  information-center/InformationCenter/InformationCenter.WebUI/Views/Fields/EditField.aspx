<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
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
    
    <h2>Редактирование поля "<%=Html.Encode(FieldName)%>"</h2>
    
    <%= Html.ValidationSummary("Введенные данные некорректны. Проверьте их и повторите попытку.") %>
    
   
    <form action="/Fields/CommitChanges" id="frmField" method="post" enctype="multipart/form-data">
        <%
            var dataTypes = (IEnumerable<FieldTypeView>)(ViewData["DataTypes"] ?? new FieldTypeView[0]);

        %>
        
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p><span class="success"><%=ViewData["success"]%></span></p>
        <p><label for="fileToUpload">Имя поля:</label><input type="text" name="fieldName" value="<%=Html.Encode(FieldName) %>" />
        <input type=hidden name="fieldId" value="<%=Html.Encode(FieldId) %>" />
        <div>
            <ul>
            <%
                foreach (FieldTypeView dataType in dataTypes)
            {

%>
                <input type="radio" readonly value="<%=dataType.FieldTypeName%>" <%=Current.FieldTypeView == dataType ? "checked" : ""%> />
                <%
            }%>
            </ul>
        </div>
        <p><button type="submit">Сохранить</button></p>

    </form>
    <form action="/Templates/DeleteTemplate" id="Form2" method="post" enctype="multipart/form-data">
        <input type=hidden name="fieldId" value="<%=Html.Encode(FieldId) %>" />
        <p><button type="submit">Удалить поле</button></p>
    </form>
            <p>
       
            <%=Html.ActionLink("Выбор поля", "SelectField", "Fields")%>

        </p>
</asp:Content>