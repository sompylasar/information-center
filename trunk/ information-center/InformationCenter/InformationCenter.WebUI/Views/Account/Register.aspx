<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
Регистрация - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="registerMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Создание новой учетной записи</h2>
    <p>
        Заполните поля формы.
    </p>
    <p>
        Пароль должен состоять из не менее <%=Html.Encode(ViewData["PasswordLength"])%> символов.
    </p>
    <%= Html.ValidationSummary("Произошла ошибка при создании учетной записи. Скорректируйте данные и попробуйте снова.") %>

    <% using (Html.BeginForm()) { %>
        <div>
            <fieldset>
                <legend>Личная информация</legend>
                <p>
                    <label for="username">Имя пользователя:</label>
                    <%= Html.TextBox("username") %>
                    <%= Html.ValidationMessage("username") %>
                </p>
                <p>
                    <label for="email">Электронный адрес:</label>
                    <%= Html.TextBox("email") %>
                    <%= Html.ValidationMessage("email") %>
                </p>
                <p>
                    <label for="password">Пароль:</label>
                    <%= Html.Password("password") %>
                    <%= Html.ValidationMessage("password") %>
                </p>
                <p>
                    <label for="confirmPassword">Подтверждение пароля:</label>
                    <%= Html.Password("confirmPassword") %>
                    <%= Html.ValidationMessage("confirmPassword") %>
                </p>
                <p>
                    <input type="submit" value="Зарегистрироваться" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
