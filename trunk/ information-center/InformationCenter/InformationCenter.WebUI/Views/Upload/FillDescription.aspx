<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Загрузка документов - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .selected 
        {
        	color: #000000 !important;
        	border-color: #000000 !important;
        	background-color: #E3E3E3 !important;
        }
        ul.listbox-from, ul.listbox-from li
        {
        	display: block;
        	list-style-type: none;
        	margin: 0px;
        	padding: 0px;
        	/*min-width: 288px;
        	width: 288px;*/
        }
        ul.listbox-from li, table.listbox-to td
        {
        	min-height: 20px;
        	line-height: 20px;
        	margin: 0;
        	padding: 2px;
        	border: 1px solid #e8eef4;
        	white-space: nowrap;
        }
        ul.listbox-from li:hover, table.listbox-to tr:hover td
        {
        	border: 1px solid #AAAAAA;
        }
        ul.listbox-from li.selected, table.listbox-to tr.selected td
        {
        	color: #000000 !important;
        	border-color: #000000 !important;
        	background-color: #E3E3E3 !important;
        }
        td.listbox-section
        {
        	padding: 2px;
        	vertical-align: top;
        	/*min-width: 300px;
        	width: 300px;*/
        }
        td.listbox-section label
        {
        	display: block;
            padding: 2px;
            padding-left: 0px;
        	padding-bottom: 5px;
        	font-weight: bold;
        }
        table.listbox-to
        {
        	border: 0;
        	padding: 0;
        	margin: 0;
        	border-collapse: separate;
        	border-spacing: 0px;
        }
        table.listbox-to td.fieldName
        {
        	border-right: 0px;
        }
        table.listbox-to tr.selected td.fieldName, table.listbox-to tr:hover td.fieldName
        {
        	border-right: 0px;
        }
        #fields, #fields td#fields-from-container, #fields td#fields-to-container
        {
        	border: 0;
        }
        #fields td#fields-from-container
        {
        	max-height: 300px;
        	overflow: auto;
        }
        #fields td#fields-buttons-container
        {
        	padding: 0px 30px;
        	border: 0;
        	vertical-align: middle;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        jQuery(function($) {
            var isCtrlDown = false;
            $(document).keydown(function (event) {
                isCtrlDown = (event.ctrlKey || event.metaKey);
            }).keyup(function (event) {
                isCtrlDown = (event.ctrlKey || event.metaKey);
            });
            var check_empty = function () {
                if ($("#fields .listbox-to tr").length <= 0) {
                    $("#fields .listbox-to-empty").show();
                    
                    $("#fields .button-remove-all").attr({ disabled: 'disabled' });
                }
                else {
                    $("#fields .listbox-to-empty").hide();
                    
                    $("#fields .button-remove-all").removeAttr('disabled');
                }
                    
                if ($("#fields .listbox-from li").length <= 0) {
                    $("#fields .listbox-from-empty").show();
                    
                    $("#fields .button-add-all").attr({ disabled: 'disabled' });
                }
                else {
                    $("#fields .listbox-from-empty").hide();
                    
                    $("#fields .button-add-all").removeAttr('disabled');
                }
            };
            var check_buttons = function () {
                if ($("#fields .listbox-to tr[selected]").length <= 0) 
                    $("#fields .button-remove").attr({ disabled: 'disabled' });
                else
                    $("#fields .button-remove").removeAttr('disabled');
                    
                if ($("#fields .listbox-from li[selected]").length <= 0)
                    $("#fields .button-add").attr({ disabled: 'disabled' });
                else
                    $("#fields .button-add").removeAttr('disabled');
            };
            var extend_tr = function (tr) {
                var $tr = $(tr);
                
                $tr.find('td').unbind('.select').bind('click.select', function () {
                    if (false && !isCtrlDown) {
                        $('#fields .listbox-to tr')
                            .filter(function () {return (this != $tr[0]);})
                            .removeAttr('selected').removeClass('selected');
                    }
                    
                    if ($tr.is('[selected]')) $tr.removeAttr('selected').removeClass('selected');
                    else $tr.attr('selected', 'selected').addClass('selected');
                    
                    check_buttons();
                });
            };
            var extend_li = function (li) {
                var $li = $(li);
                
                $li.unbind('.select').bind('click.select', function () {
                    if (false && !isCtrlDown) {
                        $('#fields .listbox-from li')
                            .filter(function () {return (this != $li[0]);})
                            .removeAttr('selected').removeClass('selected');
                    }
                    
                    if ($li.is('[selected]')) $li.removeAttr('selected').removeClass('selected');
                    else $li.attr('selected', 'selected').addClass('selected');
                    
                    check_buttons();
                });
            };
            
            var add_one = function (li) {
                var $itemFrom = $(li);
                $itemFrom.remove();
                
                var $itemTo = $('<tr></tr>');    
                var $fieldName = $('<td></td>').addClass('fieldName').appendTo($itemTo);
                var $fieldValue = $('<td></td>').addClass('fieldValue').appendTo($itemTo);
                var fieldName = $itemFrom.attr('rel');
                
                $fieldName.html($itemFrom.html());
                $('<input />').attr({ type: 'text', name: fieldName }).appendTo($fieldValue);
                
                $itemTo.appendTo('#fields .listbox-to');
                
                extend_tr($itemTo);
                
                check_empty();
            };
            var remove_one = function (tr) {
                var $src_item = $(tr);
                $src_item.remove();
                
                var $fieldName = $src_item.find('td.fieldName');
                var $fieldValue = $src_item.find('td.fieldValue');
                var fieldName = $fieldValue.attr('name');
                
                var $itemTo = $('<li></li>').attr({ rel: fieldName }).html($fieldName.html());
                
                $itemTo.appendTo('#fields .listbox-from');
                
                extend_li($itemTo);
                
                check_empty();
            };
            
            $("#fields .listbox-to tr").each(function () {  extend_tr(this); });
            $("#fields .listbox-from li").each(function () {  extend_li(this); });
            
            check_empty();
            check_buttons();
            
            $("#fields .button-add").click(function() {
                $("#fields .listbox-from > li[selected]").each(function () {
                    add_one(this);
                });
            });
            $("#fields .button-add-all").click(function() {
                $("#fields .listbox-from > li").each(function () {
                    add_one(this);
                });
            });
            $("#fields .button-remove").click(function() {
                $("#fields .listbox-to   tr[selected]").each(function () {
                    remove_one(this);
                });
            });
            $("#fields .button-remove-all").click(function() {
                $("#fields .listbox-to   tr").each(function () {
                    remove_one(this);
                });
            });
        });
    </script>

    <h2>Загрузка документов</h2>
    
    <%= Html.ValidationSummary("Введенные данные некорректны. Проверьте их и повторите попытку.") %>
    
    <form action="/Upload/Start" id="frmUpload" method="post" enctype="multipart/form-data">
        <%
            var selectedFields = (IEnumerable<FieldView>)(ViewData["SelectedFields"] ?? new FieldView[0]);
            selectedFields = selectedFields.OrderBy(field => field.Order);

            var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);
            fields = fields.OrderBy(field => field.Order).Except(selectedFields);
        %>
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p><label for="fileToUpload">Документ:</label><input type="file" id="fileToUpload" name="f" /><%= Html.ValidationMessage("f") %></p>
        <div>
            <label for="fields"></label>
            <table id="fields"><tr><td id="fields-from-container" class="listbox-section">
                <label>Поля описания:</label>
                <span class="listbox-from-empty">(список пуст)</span>
                <ul class="listbox-from">
                    <% foreach (FieldView field in fields) { %>
                    <li rel="_<%=field.ID %>"><span><%=field.Name %></span></li>
                    <% } %>
                </ul>
            </td><td id="fields-buttons-container">
                <div>
                    <button type="button" class="button-add">&gt;</button>
                    <button type="button" class="button-add-all">&gt;&gt;</button>
                </div>
                <br />
                <div>
                    <button type="button" class="button-remove">&lt;</button>
                    <button type="button" class="button-remove-all">&lt;&lt;</button>
                </div>
            </td><td id="fields-to-container" class="listbox-section">
                <label>Выбранные поля описания:</label>
                <span class="listbox-to-empty">(список пуст)</span>
                <table class="listbox-to" style="width: 300px;">
                    <% foreach (FieldView field in selectedFields) { %>
                    <tr><td class="fieldName"><%=field.Name %></td><td class="fieldValue"><input type="text" name="_<%=field.ID %>" value="" /></td></tr>
                    <% } %>
                </table>
            </td></tr></table>
        </div>
        <p><button type="submit">Загрузить</button></p>
    </form>
</asp:Content>