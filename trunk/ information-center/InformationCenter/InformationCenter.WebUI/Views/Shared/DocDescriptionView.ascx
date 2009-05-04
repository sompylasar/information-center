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
    <div><% if (document != null) 
            { %><span class="file-name"><%=Html.ActionLink(filename, "Index", "Download", new { id = document.ID }, null)%></span><span class="file-type"><%=string.IsNullOrEmpty(contentType) ? "" : "(" + Html.Encode(contentType) + ")"%></span><% 
            } %></div>
    <div>
        <span class="description-name"><label><%=Html.Encode(description.Name) %></label><%=Html.ActionLink("[редактировать]", "EditDescription", "Upload", new { id = description.ID }, null)%></span>
        <ul class="description-fields">
        <% foreach (FieldValueView descriptionField in description.DescriptionFieldValues) { %>
            <li><%=Html.Encode(descriptionField.Field.Name + "=\"" + descriptionField.Value + "\"") %></li>
        <% } %>
        </ul>
    </div>
</div>
<% } %>