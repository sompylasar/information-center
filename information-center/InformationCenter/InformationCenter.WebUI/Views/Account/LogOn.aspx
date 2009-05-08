<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
Вход - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
#loginType_selection input 
{
	float: left;
}
#loginType_selection label
{
	clear: right;
}
</style>
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Вход</h2>
    <p>
        <!--Введите имя пользователя и пароль.--><!-- Если у Вас нет учетной записи, <%= Html.ActionLink("зарегистрируйтесь", "Register") %>.-->
    </p>
    <%= Html.ValidationSummary("Вход запрещен. Исправьте ошибки и попробуйте снова.") %>

    <% using (Html.BeginForm()) { %>
        <div>
            <fieldset>
                <legend>Личные данные</legend>
                <p id="loginType_selection">
                    <label for="loginType">Тип входа:</label>
                    <%= Html.RadioButton("loginType", "integrated", true, new { id = "loginType_integrated" })%><label for="loginType_windows">Интегрированный (Windows)</label>
                    <!--<%= Html.RadioButton("loginType", "sql", false, new { id = "loginType_sql" })%><label for="loginType_sql">SQL Server</label>-->
                    <%= Html.ValidationMessage("loginType") %>
                </p>
                <!--
                <p>
                    <label for="username">Имя пользователя:</label>
                    <%= Html.TextBox("username") %>
                    <%= Html.ValidationMessage("username") %>
                </p>
                <p>
                    <label for="password">Пароль:</label>
                    <%= Html.Password("password") %>
                    <%= Html.ValidationMessage("password") %>
                </p>-->
                <p>
                    <%= Html.CheckBox("rememberMe") %> <label class="inline" for="rememberMe">Запомнить</label>
                </p>
                <p>
                    <input type="submit" value="Войти" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
