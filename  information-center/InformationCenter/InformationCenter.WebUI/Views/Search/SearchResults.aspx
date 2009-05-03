<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="LogicUtils"%>
<%@ Import Namespace="InformationCenter.WebUI.Models"%>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="searchResultsTitle" ContentPlaceHolderID="TitleContent" runat="server">
Результаты поиска - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="searchResultsMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="searchResultsContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Результаты поиска</h2>
    <%
        var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);
        var request = (SearchRequestView)(ViewData["SearchRequest"] ?? new SearchRequestView());

        var results = (IEnumerable<DocDescriptionView>)(ViewData["SearchResultItems"] ?? new DocDescriptionView[0]);
        int resultsCount = results.Count();
        
        string query = "", sep = "";
        foreach (SearchItemView searchItem in request.Items)
        {
            string fieldName = "", fieldValue = searchItem.FieldValue.ToString();
            foreach (FieldView field in fields)
            {
                if (searchItem.Field.ID == field.ID)
                {
                    fieldName = field.Name;
                    break;
                }
            }
            query += sep + fieldName + "=\"" + fieldValue + "\"";
            sep = " " + (request.Mode == SearchMode.And ? "И" : "ИЛИ") + " ";
        }
    %>
    <div>
        <p><label>Запрос:</label><span id="query"><%=Html.Encode(query) %></span></p>
        <p><span class="error"><%=resultsCount <= 0 ? "По Вашему запросу не найдено ни одного документа." : Html.Encode(ViewData["error"] ?? "") %></span></p>
        <p class="search-results">
            <span id="total"><%=resultsCount==0 ? "" : "Найдено документов: "+resultsCount.ToString() %></span>
            <ol>
                <%foreach (DocDescriptionView result in results){
                      string filename, contentType;
                      FileHelper.SplitFilename(result.Document.FileName, out filename, out contentType);
                      %>
                <li>
                    <div><span class="file-name"><%=Html.ActionLink(filename, "Index", "Download", new { id = result.Document.ID }, null)%></span><span class="file-type"><%=string.IsNullOrEmpty(contentType) ? "" : "("+Html.Encode(contentType)+")" %></span></div>
                    <div>
                        <label class="description-name"><%=Html.Encode(result.Name) %></label>
                        <ul class="description-fields">
                        <% foreach (FieldValueView descriptionField in result.DescriptionFields) { %>
                        <li><%=Html.Encode(descriptionField.Field.Name + "=\"" + descriptionField.Value + "\"") %></li>
                        <% } %>
                        </ul>
                    </div>
                    <div></div>
                </li>
                <%}%>
            </ol>
        </p>
        <p><a href="/Search/Index">Изменить условия поиска</a><br />
        <a href="/Search/New">Новый поиск</a></p>
    </div>
</asp:Content>
