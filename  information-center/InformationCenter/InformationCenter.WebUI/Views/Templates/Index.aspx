<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����� ����
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>���������� ���������</h2>
    <div id="TemplateManagerActions">
        <ul id="TemplateActions">              
            <li><%= Html.ActionLink("�������� �������", "NewTemplate", "Templates")%></li>
            <li><%= Html.ActionLink("�������������� �������", "SelectTemplate", "Templates")%></li>
        </ul>
    </div>
    
     <%=Html.ActionLink("�����", "Index", "Management")%>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainHeaderContent" runat="server">
</asp:Content>
