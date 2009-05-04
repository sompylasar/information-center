<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.WebUI.Helpers"%>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Загрузка документа - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/jquery.autocomplete.min.js"></script>
    <link rel="stylesheet" type="text/css" href="/Content/jquery.autocomplete/styles.css" />
    <style type="text/css">
        .selected 
        {
        	color: #000000 !important;
        	border-color: #000000 !important;
        	background-color: #E3E3E3 !important;
        }
        .unselectable {
           -moz-user-select: none;
           -khtml-user-select: none;
           user-select: none;
        }
        ul.listbox-from, ul.listbox-from li
        {
        	display: block;
        	list-style-type: none;
        	width: 100%;
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
        ul.listbox-from li, ul.listbox-from li span, table.listbox-to td 
        {
        	cursor: default;
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
        
        td.listbox-section
        {
        	padding: 2px;
        	vertical-align: top;
        	/*min-width: 300px;
        	width: 300px;*/
        	border: 0;
        	height: 100%;
        }
        
        #fileToUpload, #txtDescriptionName 
        {
        	width: 50%;
        }
        
        #fields/*, #fields #fields-from-container, #fields #fields-to-container*/
        {
        	border: 0;
        	
        }
        #fields_row 
        {
        	height: 100%;
        }
        #fields #fields-from-container, #fields #fields-to-container
        {
        	height: 180px;
        	margin: 0px;
        }
        #fields #fields-from-container .listbox-from, #fields #fields-to-container .listbox-to-wrapper 
        {
        	display: block;
        	min-width: 150px;
        	height: 156px;
        	padding-right: 18px;
        	overflow-x: hidden;
        	overflow-y: auto;
        }
        #fields #fields-from-container .listbox-from
        {
        	margin-right: 1px;
        	padding-right: 7px;
        }
        #fields #fields-buttons-container
        {
        	padding: 0px 30px;
        	border: 0;
        	vertical-align: middle;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Заполнение описания</h2>
    <%=Html.Breadcrumbs().AddTextLink("Загрузка документа").AddActionLink("Выбор шаблона описания", "SelectTemplate").Last("Заполнение описания") %>
    
    <%
        var selectedFields = (IEnumerable<FieldView>)(ViewData["SelectedFields"] ?? new FieldView[0]);
        selectedFields = selectedFields.OrderBy(field => field.Order);

        var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);
        fields = fields.OrderBy(field => field.Order).Except(selectedFields);
    %>
    <script type="text/javascript">
        jQuery(function($) {
            $('.unselectable').attr({ 'unselectable':'on' }).bind('selectstart', function(){return false;});
        
            var isCtrlDown = false;
            $(document).keydown(function (event) {
                isCtrlDown = (event.ctrlKey || event.metaKey);
            }).keyup(function (event) {
                isCtrlDown = (event.ctrlKey || event.metaKey);
            });
            var check_selected = function () {
                if ($("#fields .listbox-to tr[selected]").length <= 0) 
                    $("#fields .button-remove").attr({ disabled: 'disabled' });
                else
                    $("#fields .button-remove").removeAttr('disabled');
                    
                if ($("#fields .listbox-from li[selected]").length <= 0)
                    $("#fields .button-add").attr({ disabled: 'disabled' });
                else
                    $("#fields .button-add").removeAttr('disabled');
            };
            var check_empty = function () {
                if ($("#fields .listbox-to tr").length <= 0) {
                    $("#fields .listbox-to-empty").show();
                    $("#fields .button-remove-all").attr({ disabled: 'disabled' });
                }
                else {
                    $("#fields .listbox-to-empty").hide();
                    $("#fields .button-remove-all").removeAttr('disabled');
                    $( $.makeArray( $("#fields .listbox-to tr") )
                        .sort(function (a,b) { return parseInt($(a).attr('order'))-parseInt($(b).attr('order')); }) )
                            .appendTo( $("#fields .listbox-to") );
                }
                    
                if ($("#fields .listbox-from li").length <= 0) {
                    $("#fields .listbox-from-empty").show();
                    $("#fields .button-add-all").attr({ disabled: 'disabled' });
                }
                else {
                    $("#fields .listbox-from-empty").hide();
                    $("#fields .button-add-all").removeAttr('disabled');
                    $( $.makeArray( $("#fields .listbox-from li") )
                        .sort(function (a,b) { return parseInt($(a).attr('order'))-parseInt($(b).attr('order')); }) )
                            .appendTo( $("#fields .listbox-from") );
                }
                
                check_selected();
            };
            var select_items = function (items) {
                $(items).attr('selected', 'selected').addClass('selected');
            }
            var deselect_items = function (items) {
                $(items).removeAttr('selected').removeClass('selected');
            }

            var extend_tr = function (tr) {
                var $target = $(tr);
                
                $target.find('td').unbind('.select-item').bind('click.select-item', function () {
                    if (false && !isCtrlDown) {
                        deselect_items( $target.siblings() );
                    }
                    
                    if ($target.is('[selected]')) deselect_items( $target );
                    else select_items( $target );
                    
                    check_selected();
                });
                
                $target.find('input').unbind('.select-item').bind('click.select-item', function (event) {event.stopPropagation();})
            };
            var extend_li = function (li) {
                var $target = $(li);
                
                $target.unbind('.select-item').bind('click.select-item', function () {
                    if (false && !isCtrlDown) {
                        deselect_items( $target.siblings() );
                    }
                    
                    if ($target.is('[selected]')) deselect_items( $target );
                    else select_items( $target );
                    
                    check_selected();
                });
            };
            var extend_autofill = function (textinput) {
                $(textinput).each(function () {
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
            };
            
            var add_one = function (li) {
                var $itemFrom = $(li);
                $itemFrom.remove();
                
                var $itemTo = $('<tr></tr>').attr({ 'order': $itemFrom.attr('order') });
                var $fieldName = $('<td></td>').addClass('fieldName').appendTo($itemTo);
                var $fieldValue = $('<td></td>').addClass('fieldValue').appendTo($itemTo);
                var fieldName = $itemFrom.attr('rel');
                
                $fieldName.html($itemFrom.html());
                $input = $('<input />').attr({ type: 'text', name: fieldName }).appendTo($fieldValue);
                
                if ($itemFrom.is('[nullable]')) {
                    $itemTo.attr('nullable', 'nullable');
                    $input.change(function () {
                        var value = ($(this).val() || '');
                        var $input_message = $input.next('.field-validation-error');
                        if ($input_message.length <= 0) { $input_message = $('<span class="field-validation-error"></span>'); $input.after($input_message); }
                        if (value == '') $input_message.html('Заполните поле.').show();
                        else $input_message.hide();
                    });
                }
                
                $itemTo.appendTo('#fields .listbox-to');
                
                extend_tr($itemTo);
                extend_autofill($input);
                
                check_empty();
            };
            var remove_one = function (tr) {
                var $itemFrom = $(tr);
                $itemFrom.remove();
                
                var $fieldName = $itemFrom.find('td.fieldName');
                var $fieldValue = $itemFrom.find('td.fieldValue');
                var fieldName = $fieldValue.find("input").attr('name');
                
                var $itemTo = $('<li></li>')
                    .attr({ rel: fieldName, 'order': $itemFrom.attr('order') })
                    .html($fieldName.html());
                if ($itemFrom.is('[nullable]'))
                    $itemTo.attr('nullable', 'nullable');
                
                $itemTo.appendTo('#fields .listbox-from');
                
                extend_li($itemTo);
                
                check_empty();
            };
            
            $("#fields .listbox-to tr").each(function () {  extend_tr(this); extend_autofill($(this).find(':text')); });
            $("#fields .listbox-from li").each(function () {  extend_li(this); });
            
            check_empty();
            check_selected();
            
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
            
            
            function validate_upload(uploadForm) {
                var valid = true;
                var $fileToUpload = $('#fileToUpload', uploadForm);
                var file = $.trim($fileToUpload.val());
                var $fileToUpload_message = $fileToUpload.next('.field-validation-error');
                if ($fileToUpload_message.length <= 0) { $fileToUpload_message = $('<span class="field-validation-error"></span>'); $fileToUpload.after($fileToUpload_message); }
                if (file == '') {
                    $fileToUpload_message.html('Выберите файл.').show();
                    valid = false;
                }
                else {
                    $fileToUpload_message.hide();
                }
                var $txtDescriptionName = $('#txtDescriptionName', uploadForm);
                var descriptionName = $.trim($txtDescriptionName.val() || '');
                var $txtDescriptionName_message = $txtDescriptionName.next('.field-validation-error');
                if ($txtDescriptionName_message.length <= 0) { $txtDescriptionName_message = $('<span class="field-validation-error"></span>'); $txtDescriptionName.after($txtDescriptionName_message); }
                if (descriptionName == '') {
                    $txtDescriptionName_message.html('Заполните название описания.').show();
                    valid = false;
                }
                else {
                    $txtDescriptionName_message.hide();
                }
                $('.listbox-to tr', uploadForm).each(function () {
                    var $item = $(this);
                    var $input = $item.find(':text');
                    
                    var $input_message = $input.next('.field-validation-error');
                    if ($input_message.length <= 0) { $input_message = $('<span class="field-validation-error"></span>'); $input.after($input_message); }
                    
                    $input.val( $.trim($input.val()) );
                    if (!$item.is('[nullable]') && $input.val() == '') {
                        $input_message.html('Заполните поле.').show();
                        valid = false;
                    }
                    else {
                        $input_message.hide();
                    }
                });
                return valid;
            }
            
            if ($.browser.mozilla) $('#fileToUpload').attr('size', 50);
            
            $('#frmUpload').submit(function () {
                return validate_upload(this);
            })
        });
    </script>

    <%= Html.ValidationSummary("Введенные данные некорректны. Проверьте их и повторите попытку.") %>
    
    <form action="/Upload/Start" id="frmUpload" method="post" enctype="multipart/form-data">
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <div>
            <table class="layout">
            <tr><td style="width:15%"><label for="fileToUpload">Документ:</label></td><td style="width:75%"><input type="file" id="fileToUpload" name="f" value="<%=TempData["UploadFileName"] ?? "" %>" /><%=Html.ValidationMessage("f")%></td></tr>
            <tr><td><label for="txtDescriptionName">Название описания:</label></td><td><input type="text" id="txtDescriptionName" name="DescriptionName" maxlength="256" value="<%=TempData["DescriptionName"] ?? ViewData["SelectedTemplateName"] %>" /><%= Html.ValidationMessage("DescriptionName")%></td></tr>
            </table>
        </div>
        <div>
            <table id="fields"><tr id="fields_row"><td class="listbox-section">
                <fieldset id="fields-from-container">
                    <legend>Доступные поля описания</legend>
                    <span class="listbox-from-empty"><%=(fields.Count() + selectedFields.Count() > 0 ? "(все доступные поля выбраны)" : "(список пуст)") %></span>
                    <ul class="listbox-from">
                        <% foreach (FieldView field in fields) { %>
                        <li rel="_<%=field.ID %>" order="<%=field.Order %>"<%=field.Nullable ? " nullable=\"nullable\"" : "" %>><span class="unselectable"><%=Html.Encode(field.Name) %> (<%=Html.Encode(field.FieldTypeView.FieldTypeName) %>)</span></li>
                        <% } %>
                    </ul>
                </fieldset>
            </td><td id="fields-buttons-container">
                <div>
                    <button type="button" class="button-add" title="Добавить выделенные">&gt;</button>
                    <button type="button" class="button-add-all" title="Добавить все">&gt;&gt;</button>
                </div>
                <br />
                <div>
                    <button type="button" class="button-remove" title="Убрать выделенные">&lt;</button>
                    <button type="button" class="button-remove-all" title="Убрать все">&lt;&lt;</button>
                </div>
            </td><td class="listbox-section">
                <fieldset id="fields-to-container">
                    <legend>Выбранные поля описания</legend>
                    <span class="listbox-to-empty">(добавьте необходимые поля)</span>
                    <div class="listbox-to-wrapper">
                        <table class="listbox-to">
                            <% foreach (FieldView field in selectedFields) { %>
                            <tr order="<%=field.Order %>"<%=field.Nullable ? " nullable=\"nullable\"" : "" %>><td class="fieldName"><span class="unselectable"><%=Html.Encode(field.Name) %> (<%=Html.Encode(field.FieldTypeView.FieldTypeName) %>)</span></td><td class="fieldValue"><input type="text" name="_<%=field.ID %>" value="<%=Html.Encode(((string)TempData["_"+field.ID]) ?? "") %>" /></td></tr>
                            <% } %>
                        </table>
                    </div>
                </fieldset>
            </td></tr></table>
        </div>
        <p><button type="submit">Загрузить</button></p>
    </form>
</asp:Content>