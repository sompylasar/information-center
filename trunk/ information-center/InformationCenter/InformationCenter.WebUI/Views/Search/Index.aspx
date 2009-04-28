<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>
<%@ Import Namespace="InformationCenter.Data"%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
Поиск документов - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="indexMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

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
    <form action="/Search/Query" method="post" id="frmSearchDocument" class="search-form">
        <%
            var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);
            fields = fields.OrderBy(field => field.Order);
            var request = (SearchRequest)(ViewData["SearchRequest"] ?? new SearchRequest());
            var useAdditional = Convert.ToBoolean(ViewData["UseAdditionalFields"]);
        %>
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p>
            <table class="search-fields">
                <% foreach (FieldView field in fields)
                   { 
                       object value = "";
                       foreach (SearchItem item in request.Items)
                       {
                           if (item.FieldID == field.ID)
                           {
                               value = item.FieldValue;
                               break;
                           }
                       }
                %>
                <tr><td><label for="txt_<%=field.ID %>"><%=field.Name %></label></td><td><input type="text" id="txt_<%=field.ID %>" name="_<%=field.ID %>" value="<%=value %>" /></td></tr>
                <% } %>
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
            <!--<tr class="additional"><td><label for="txtAdditionalDate">Дата:</label></td><td><input type="text" id="txtAdditionalDate" name="_Date" value="" /></td></tr>
            <tr class="additional"><td><label for="txtAdditionalPublisher">Издательство:</label></td><td><input type="text" id="txtAdditionalPublisher" name="_Publisher" value="" /></td></tr>-->
        </table>
    </div>
</asp:Content>