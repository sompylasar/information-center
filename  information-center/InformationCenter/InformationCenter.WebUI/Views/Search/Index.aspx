<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>
<%@ Import Namespace="InformationCenter.Data"%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
Поиск документов - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="indexMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
    </style>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        jQuery(function($) {
            function check() {
                //alert('check');
                if ($('#chkUseAdditional').is(":checked")) {
                    $('#frmSearchDocument_additional .additional').appendTo( $('#frmSearchDocument .search-fields-additional') );
                }
                else {
                    $('#frmSearchDocument .additional').appendTo( $('#frmSearchDocument_additional .search-fields-additional') );
                }
                var additional_in_form = $('#frmSearchDocument .additional').length;
                if ($('#frmSearchDocument_additional .additional').length || additional_in_form) {
                    $('#chkUseAdditional_span').show();
                    if (additional_in_form)
                        $('#frmSearchDocument_additional_in_form').show();
                    else
                        $('#frmSearchDocument_additional_in_form').hide();
                }
                else {
                    $('#chkUseAdditional_span').hide();
                    $('#frmSearchDocument_additional_in_form').hide();
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
            var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);
            fields = fields.OrderBy(field => field.Order);

            var additionalFields = fields.Skip(2);
            fields = fields.Except(additionalFields);
            
            var request = (SearchRequest)(ViewData["SearchRequest"] ?? new SearchRequest());
            var useAdditional = Convert.ToBoolean(ViewData["UseAdditionalFields"]);
        %>
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p>
            <div>
            <fieldset>
                <legend>Основные поля</legend>
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
                </table>
            </fieldset>
            </div>
        </p>
        <p>
            <span id="chkUseAdditional_span" style="display:none;"><label for="chkUseAdditional" id="chkUseAdditional_label">Использовать дополнительные поля</label><input type="checkbox" id="chkUseAdditional" name="more" value="true"<%= useAdditional ? " checked=\"checked\"" : "" %> /></span>
            <div id="frmSearchDocument_additional_in_form" style="display:none;">
            <fieldset>
                <legend>Дополнительные поля</legend>
                <table class="search-fields search-fields-additional">
                </table>
            </fieldset>
            </div>
        </p>
        <p><button type="submit">Найти!</button></p>
    </form>
    <div id="frmSearchDocument_additional" style="display:none;">
        <table class="search-fields search-fields-additional">
            <% foreach (FieldView field in additionalFields)
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
            <tr class="additional"><td><label for="txt_<%=field.ID %>"><%=field.Name %></label></td><td><input type="text" id="Text1" name="_<%=field.ID %>" value="<%=value %>" /></td></tr>
            <% } %>
        </table>
    </div>
</asp:Content>