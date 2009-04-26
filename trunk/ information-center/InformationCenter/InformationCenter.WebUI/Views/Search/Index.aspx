<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>
<%@ Import Namespace="InformationCenter.Data"%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
Поиск документов - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="indexMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        jQuery(function($) {
            function check() {
                //alert('check');
                if ($('#chkUseAdditional').is(":checked")) {
                    $('#frmSearchDocument_additional .additional').appendTo( $('#frmSearchDocument .search-fields') );
                }
                else {
                    $('#frmSearchDocument .additional').appendTo( $('#frmSearchDocument_additional .search-fields') );
                }
            }

            $('#chkUseAdditional')
                .change(function () {
                    check();
                });

            check();
        });
    </script>
    
    <h2>Поиск документов</h2>
    <form action="/Search/Query" method="get" id="frmSearchDocument" class="search-form">
        <% 
            var request = (SearchRequest)(ViewData["SearchRequest"] ?? new SearchRequest());
            var useAdditional = Convert.ToBoolean(ViewData["UseAdditionalFields"]);
        %>
        <p><span class="field-validation-error"><%=ViewData["error"]%></span></p>
        <p>
            <table class="search-fields">
                <tr><td><label for="txtSearchTitle">Название:</label></td><td><input type="text" id="txtSearchTitle" name="t" value="" /></td></tr>
                <tr><td><label for="txtSearchAuthor">Автор:</label></td><td><input type="text" id="txtSearchAuthor" name="a" value="" /></td></tr>
                <tr>
                    <td><label for="chkUseAdditional">Использовать дополнительные поля</label></td>
                    <td><input type="checkbox" id="chkUseAdditional" name="more" value="true"<%= useAdditional ? " checked=\"checked\"" : "" %> /></td>
                </tr>
            </table>
        </p>
        <p><button type="submit">Найти!</button></p>
    </form>
    <div class="hidden" id="frmSearchDocument_additional">
        <table class="search-fields">
            <tr class="additional"><td><label for="txtAdditionalDate">Дата:</label></td><td><input type="text" id="txtAdditionalDate" name="_Date" value="" /></td></tr>
            <tr class="additional"><td><label for="txtAdditionalPublisher">Издательство:</label></td><td><input type="text" id="txtAdditionalPublisher" name="_Publisher" value="" /></td></tr>
        </table>
    </div>
</asp:Content>