<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����� ����
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
	���������� ������
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>���������� ������</h2>
    <div id="TemplateManagerActions">
        <ul id="TemplateActions">              
            <li><%= Html.ActionLink("�������� ����", "NewField", "Fields")%></li>
            <li><%= Html.ActionLink("�������������� ����", "SelectField", "Fields")%></li>
        </ul>
    </div>
    
    <%=Html.ActionLink("�����", "Index", "Management")%>

</asp:Content>

