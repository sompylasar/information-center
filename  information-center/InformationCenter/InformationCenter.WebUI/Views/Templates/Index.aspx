<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.WebUI.Helpers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����� ����
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
	���������� ���������
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>���������� ���������</h2>
    <%=Html.Breadcrumbs().AddActionLink("����������", "Index", "Management").Last("���������� ���������")%>
    <div id="TemplateManagerActions">
        <ul id="TemplateActions">              
            <li><%= Html.ActionLink("�������� �������", "NewTemplate", "Templates")%></li>
            <li><%= Html.ActionLink("�������������� �������", "SelectTemplate", "Templates")%></li>
        </ul>
    </div>

</asp:Content>

