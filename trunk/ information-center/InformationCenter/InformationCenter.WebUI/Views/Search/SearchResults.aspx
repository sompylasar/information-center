<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Data"%>
<%@ Import Namespace="InformationCenter.WebUI.Models"%>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="searchResultsTitle" ContentPlaceHolderID="TitleContent" runat="server">
���������� ������ - �������������� ����� ����
</asp:Content>

<asp:Content ID="searchResultsMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
�������������� ����� ����
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="searchResultsContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>���������� ������</h2>
    <%
        var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);
        var request = (SearchRequest)(ViewData["SearchRequest"] ?? new SearchRequest());

        var results = (IEnumerable<DocDescriptionView>)(ViewData["SearchResultItems"] ?? new DocDescriptionView[0]);
        int resultsCount = results.Count();
        
        string query = "", sep = "";
        foreach (SearchItem searchItem in request.Items)
        {
            string fieldName = "", fieldValue = searchItem.FieldValue.ToString();
            foreach (FieldView field in fields)
            {
                if (searchItem.FieldID == field.ID)
                {
                    fieldName = field.Name;
                    break;
                }
            }
            query += sep + fieldName + "=\"" + fieldValue + "\"";
            sep = " " + (request.Mode == SearchMode.And ? "�" : "���") + " ";
        }
    %>
    <div>
        <p><label>������:</label><span id="query"><%=Html.Encode(query) %></span></p>
        <p><span class="error"><%=resultsCount <= 0 ? "�� ������ ������� �� ������� �� ������ ���������." : Html.Encode(ViewData["error"] ?? "") %></span></p>
        <p class="search-results">
            <span id="total"><%=resultsCount==0 ? "" : "������� ����������: "+resultsCount.ToString() %></span>
            <ol>
                <%foreach (DocDescriptionView result in results){
                      string filename, contentType;
                      FileHelper.SplitFilename(result.Document.FileName, out filename, out contentType);
                      %>
                <li>
                    <div><%=Html.Encode(filename) %> (<%=Html.Encode(contentType) %>)</div>
                    <div>
                        <label><%=Html.Encode(result.Name) %></label>
                        <% foreach (FieldValueView descriptionField in result.DescriptionFields) { %>
                        <ul><%=Html.Encode(descriptionField.Field.Name + "=\"" + descriptionField.Value + "\"") %></ul>
                        <% } %>
                    </div>
                    <div><%=Html.ActionLink("��������", "Index", "Download", new { id = result.Document.ID }, null)%></div>
                </li>
                <%}%>
            </ol>
        </p>
        <p><a href="javascript:window.history.go(-1)">����� � ������</a><br />
        <a href="/Search/Index">����� �����</a></p>
    </div>
</asp:Content>
