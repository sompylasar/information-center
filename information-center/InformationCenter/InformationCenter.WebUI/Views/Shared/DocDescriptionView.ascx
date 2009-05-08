<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="InformationCenter.Services" %>
<%@ Import Namespace="InformationCenter.WebUI.Models" %>
<%
    DocDescriptionView description = (DocDescriptionView)ViewData["Description"];
    if (description != null)
    { 
        DocumentView document = description.Document;
        string filename = "", contentType = "";
        if (document != null)
            FileHelper.SplitFilename(document.FileName, out filename, out contentType);
%>
<div class="description">
    <div>
    <% if (document != null) { %>
        <span class="file-name"><%=Html.ActionLink(filename, "Download", "Documents", new { id = document.ID }, null)%></span><span class="file-type"><%=string.IsNullOrEmpty(contentType) ? "" : "(" + Html.Encode(contentType) + ")"%></span>
        <span class="delete"><%=Html.ActionLink("[удалить]", "Delete", "Documents", new { id = document.ID }, new { onclick="return confirm('Вы действительно хотите удалить документ \""+Html.Encode(filename)+"\"?');" })%></span>
    <% } else { %>
        (описание не связано с документом)
    <% } %>
    </div>
    <div>
        <span class="description-name"><label><%=Html.Encode(description.Name) %></label><span class="edit"><%=Html.ActionLink("[редактировать]", "EditDescription", "Upload", new { id = description.ID }, null)%></span></span>
        <ul class="description-fields">
        <% foreach (FieldValueView descriptionField in description.DescriptionFieldValues) { %>
            <li><%=Html.Encode(descriptionField.Field.Name + "=\"" + descriptionField.Value + "\"") %></li>
        <% } %>
        </ul>
    </div>
</div>
<% } %>