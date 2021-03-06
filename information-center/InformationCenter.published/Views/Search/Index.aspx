﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.WebUI.Helpers"%>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
Поиск документов - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="indexMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/jquery.autocomplete.min.js"></script>
    <link rel="stylesheet" type="text/css" href="/Content/jquery.autocomplete/styles.css" />
    <style type="text/css">
    </style>
    <script type="text/javascript">
        jQuery(function($) {
            function check_additional() {
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
            function check_filled(inputs) {
                $(inputs).each(function () {
                    $input = $(this);
                    var fieldKey = $input.attr('name');
                    $use_cb = $(':checkbox[name=use'+fieldKey+']');
                    $input.next('span.field-validation-error').hide();
                    if ($input.val() != '') $use_cb.attr('checked', 'checked');
                });
            }
            
            $(':text').each(function () {
                var $input = $(this);
                $input.autocomplete({ 
                    serviceUrl: '/Search/Autocomplete/'+$input.attr('name').replace(/^_/, ''),
                    minChars: 0, 
                    delimiter: /(,|;)\s*/, // regex or character
                    width: 202,
                    maxHeight: 400,
                    deferRequestBy: 50, //miliseconds
                    onSelect: function (value, data) { 
                        if (!data) 
                            $input.val(''); 
                    }
                });
            });

            $('#chkUseAdditional')
                .change(function () {
                    check_additional();
                });
                
            check_additional();
            
            $(':text[name^=_]')
                .blur(function () { check_filled(this); })
                .focus(function () { $(this).addClass('focus'); })
                .blur(function () { $(this).removeClass('focus'); });
            
            $('#frmSearchDocument').submit(function () {
                check_filled( $('.focus:text[name^=_]', this) );
            });
        });
    </script>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Поиск документов</h2>
    
    <form action="/Search/Query" method="post" id="frmSearchDocument" class="search-form">
        <%
            var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);
            fields = fields.OrderBy(field => field.Order);

            var additionalFields = fields.Skip(2);
            fields = fields.Except(additionalFields);

            var request = (SearchRequestView)(ViewData["SearchRequest"] ?? new SearchRequestView());
            var useAdditional = (ViewData["UseAdditionalFields"] == null ? false : (bool)ViewData["UseAdditionalFields"]);
        %>
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p>
            <div>
            <fieldset>
                <legend>Основные поля</legend>
                <table class="search-fields">
                    <% foreach (FieldView field in fields)
                       { 
                           object value = (TempData["_"+field.ID] ?? "");
                           bool found = false;
                           foreach (SearchItemView item in request.Items)
                           {
                               if (item.Field.ID == field.ID)
                               {
                                   value = item.FieldValue;
                                   found = true;
                                   break;
                               }
                           }
                    %>
                    <tr>
                        <td><input type="checkbox" name="use_<%=field.ID %>" id="cbx_<%=field.ID %>" value="true"<%= found ? " checked=\"checked\"" : "" %> /></td>
                        <td><label for="cbx_<%=field.ID %>"><%=Html.Encode(field.Name) %> (<%=Html.Encode(field.FieldTypeView.FieldTypeName) %>)</label></td>
                        <td><input type="text" id="txt_<%=field.ID %>" name="_<%=field.ID %>" value="<%=Html.Encode(value) %>" /><%=Html.ValidationMessage("_"+field.ID) %></td>
                    </tr>
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
        <p><button type="submit"><strong>Найти!</strong></button> <button type="button" onclick="window.location.href='/Search/New'">Очистить поля</button></p>
    </form>
    <div id="frmSearchDocument_additional" style="display:none;">
        <table class="search-fields search-fields-additional">
            <% foreach (FieldView field in additionalFields)
               {
                   object value = (TempData["_" + field.ID] ?? "");
                   bool found = false;
                   foreach (SearchItemView item in request.Items)
                   {
                       if (item.Field.ID == field.ID)
                       {
                           value = item.FieldValue;
                           found = true;
                           break;
                       }
                   }
            %>
            <tr class="additional">
                <td><input type="checkbox" name="use_<%=field.ID %>" id="cbx_<%=field.ID %>" value="true"<%= found ? " checked=\"checked\"" : "" %> /></td>
                <td><label for="cbx_<%=field.ID %>"><%=Html.Encode(field.Name) %> (<%=Html.Encode(field.FieldTypeView.FieldTypeName) %>)</label></td>
                <td><input type="text" id="txt_<%=field.ID %>" name="_<%=field.ID %>" value="<%=Html.Encode(value) %>" /><%=Html.ValidationMessage("_"+field.ID) %></td>
            </tr>
            <% } %>
        </table>
    </div>
</asp:Content>