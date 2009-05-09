<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
        string userName = Html.Encode(Page.User.Identity.Name);
%>
        Вы вошли как <%= (string.IsNullOrEmpty(userName) ? "суперсекретный пользователь" : "<strong>"+userName+"</strong>") %>.
        [ <%= Html.ActionLink("Выйти", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%= Html.ActionLink("Войти", "LogOn", "Account") %> ]
<%
    }
%>
