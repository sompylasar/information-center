<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
        string userName = Page.User.Identity.Name;
%>
        Вы вошли как <b><%= Html.Encode(string.IsNullOrEmpty(userName) ? "суперсекретный пользователь" : userName) %></b>.
        [ <%= Html.ActionLink("Выйти", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%= Html.ActionLink("Войти", "LogOn", "Account") %> ]
<%
    }
%>
