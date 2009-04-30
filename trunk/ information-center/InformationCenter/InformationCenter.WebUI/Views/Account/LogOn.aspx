<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
Вход - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Вход</h2>
    <p>
        Введите имя пользователя и пароль.<!-- Если у Вас нет учетной записи, <%= Html.ActionLink("зарегистрируйтесь", "Register") %>.-->
    </p>
    <%= Html.ValidationSummary("Вход запрещен. Исправьте ошибки и попробуйте снова.") %>

    <% using (Html.BeginForm()) { %>
        <div>
            <fieldset>
                <legend>Личные данные</legend>
                <p>
                    <label for="username">Имя пользователя:</label>
                    <%= Html.TextBox("username") %>
                    <%= Html.ValidationMessage("username") %>
                </p>
                <p>
                    <label for="password">Пароль:</label>
                    <%= Html.Password("password") %>
                    <%= Html.ValidationMessage("password") %>
                </p>
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
