<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>
<%@ Import Namespace="InformationCenter.WebUI.Helpers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
	Создание поля
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <h2>Создание поля</h2>
    <%=Html.Breadcrumbs().AddActionLink("Управление полями", "Index").Last("Создание поля")%>
    
    <%= Html.ValidationSummary("Введенные данные некорректны. Проверьте их и повторите попытку.") %>
    
   
    <form action="/Fields/AddField" id="frmField" method="post" enctype="multipart/form-data">
        <%
            var dataTypes = (IEnumerable<FieldTypeView>)(ViewData["DataTypes"] ?? new FieldTypeView[0]);

        %>
        
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p><span class="success"><%=ViewData["success"]%></span></p>
        <p><label for="fileToUpload">Имя поля:</label><input type="text" name="fieldName" value="" /></p>
        <p><label for="fieldOrder">Порядок:</label><input type="text" name="fieldOrder" value="0" /></p>
        <div>
            <ul style="list-style-type: none;">
            <%
                foreach (FieldTypeView dataType in dataTypes)
            {

%>
                <li><input name="DataType" type="radio" value="<%=dataType.ID%>" ><%=dataType.FieldTypeName%></input></li>
                <%
            }%>
            </ul>
        </div>
        <p><button type="submit">Сохранить</button></p>

    </form>

</asp:Content>