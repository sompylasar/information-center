<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
        Вы вошли как <b><%= Html.Encode(Page.User.Identity.Name) %></b>.
        [ <%= Html.ActionLink("Выйти", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%= Html.ActionLink("Войти", "LogOn", "Account") %> ]
<%
    }
%>
