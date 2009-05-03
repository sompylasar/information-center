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
        td.listbox-section
        {
        	padding: 2px;
        	vertical-align: top;
        	/*min-width: 300px;
        	width: 300px;*/
        	border: 0;
        	height: 100%;
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
            var deselect_all = function (items, except) {
                except = $(except);
                $(items)
                    .filter(function () {return ($.inArray(this, except) < 0);})
                    .removeAttr('selected').removeClass('selected');
            }
            var extend_tr = function (tr) {
                var $tr = $(tr);
                
                $tr.find('td').unbind('.select').bind('click.select', function () {
                    if (false && !isCtrlDown) {
                        deselect_all( $('#fields .listbox-to tr') , $tr );
                    }
                    
                    if ($tr.is('[selected]')) $tr.removeAttr('selected').removeClass('selected');
                    else $tr.attr('selected', 'selected').addClass('selected');
                    
                    check_selected();
                });
                
                $tr.find('input').unbind('.select').bind('click.select', function (event) {event.stopPropagation();})
            };
            var extend_li = function (li) {
                var $li = $(li);
                
                $li.unbind('.select').bind('click.select', function () {
                    if (false && !isCtrlDown) {
                        deselect_all( $('#fields .listbox-from li'), $li );
                    }
                    
                    if ($li.is('[selected]')) $li.removeAttr('selected').removeClass('selected');
                    else $li.attr('selected', 'selected').addClass('selected');
                    
                    check_selected();
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
                $('<input />').attr({ type: 'text', name: fieldName }).appendTo($fieldValue);
                
                $itemTo.appendTo('#fields .listbox-to');
                
                extend_tr($itemTo);
                
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
                
                $itemTo.appendTo('#fields .listbox-from');
                
                extend_li($itemTo);
                
                check_empty();
            };
            
            $("#fields .listbox-to tr").each(function () {  extend_tr(this); });
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
        });
    </script>

    <h2>Загрузка нового документа</h2>
    
    <%= Html.ValidationSummary("Введенные данные некорректны. Проверьте их и повторите попытку.") %>
    
    <form action="/Upload/Start" id="frmUpload" method="post" enctype="multipart/form-data">
        <%
            var selectedFields = (IEnumerable<FieldView>)(ViewData["SelectedFields"] ?? new FieldView[0]);
            selectedFields = selectedFields.OrderBy(field => field.Order);

            var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);
            fields = fields.OrderBy(field => field.Order).Except(selectedFields);
        %>
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p><span class="success"><%=ViewData["success"]%></span></p>
        <p><label for="fileToUpload">Документ:</label><input type="file" id="fileToUpload" name="f" /><%= Html.ValidationMessage("f") %></p>
        <div>
            <table id="fields"><tr id="fields_row"><td class="listbox-section">
                <fieldset id="fields-from-container">
                    <legend>Доступные поля описания</legend>
                    <span class="listbox-from-empty"><%=(fields.Count() + selectedFields.Count() > 0 ? "(все доступные поля выбраны)" : "(список пуст)") %></span>
                    <ul class="listbox-from">
                        <% foreach (FieldView field in fields) { %>
                        <li rel="_<%=field.ID %>" order="<%=field.Order %>"><span class="unselectable"><%=Html.Encode(field.Name) %> (<%=Html.Encode(field.FieldTypeView.FieldTypeName) %>)</span></li>
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
                            <tr order="<%=field.Order %>"><td class="fieldName"><span class="unselectable"><%=Html.Encode(field.Name) %> (<%=Html.Encode(field.FieldTypeView.FieldTypeName) %>)</span></td><td class="fieldValue"><input type="text" name="_<%=field.ID %>" value="<%=Html.Encode(((string)TempData["_"+field.ID]) ?? "") %>" /></td></tr>
                            <% } %>
                        </table>
                    </div>
                </fieldset>
            </td></tr></table>
        </div>
        <p><button type="submit">Загрузить</button></p>
        <p>
        <% if (ViewData["Templates"] != null && ((IEnumerable<TemplateView>)ViewData["Templates"]).Count() > 0)
           { %>
            <a href="javascript:window.history.go(-1)">Назад к выбору шаблона</a>
        <% } else { %>
            <%=Html.ActionLink("Выбор шаблона", "SelectTemplate", "Upload")%>
        <% } %>
        </p>
    </form>
</asp:Content>