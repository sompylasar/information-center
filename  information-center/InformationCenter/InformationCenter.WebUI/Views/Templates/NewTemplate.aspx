<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
	Создание шаблона
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
        ul.listbox-from, ul.listbox-from li, ul.listbox-to, ul.listbox-to li
        {
        	display: block;
        	list-style-type: none;
        	width: 100%;
        	margin: 0px;
        	padding: 0px;
        	/*min-width: 288px;
        	width: 288px;*/
        }
        ul.listbox-from li, ul.listbox-to li
        {
        	min-height: 20px;
        	line-height: 20px;
        	margin: 0;
        	padding: 2px;
        	border: 1px solid #e8eef4;
        	white-space: nowrap;
        }
        ul.listbox-from li, ul.listbox-from li span, ul.listbox-to li, ul.listbox-to li span
        {
        	cursor: default;
        }
        ul.listbox-from li:hover, ul.listbox-to li:hover
        {
        	border: 1px solid #AAAAAA;
        }
        ul.listbox-from li.selected, ul.listbox-to li.selected
        {
        	color: #000000 !important;
        	border-color: #000000 !important;
        	background-color: #E3E3E3 !important;
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
            $('.unselectable').attr({ 'unselectable': 'on' }).bind('selectstart', function() { return false; });

            var isCtrlDown = false;
            $(document).keydown(function(event) {
                isCtrlDown = (event.ctrlKey || event.metaKey);
            }).keyup(function(event) {
                isCtrlDown = (event.ctrlKey || event.metaKey);
            });
            var check_selected = function() {
                if ($("#fields .listbox-to li[selected]").length <= 0)
                    $("#fields .button-remove").attr({ disabled: 'disabled' });
                else
                    $("#fields .button-remove").removeAttr('disabled');


                if ($("#fields .listbox-from li[selected]").length <= 0)
                    $("#fields .button-add").attr({ disabled: 'disabled' });
                else
                    $("#fields .button-add").removeAttr('disabled');
            };
            var check_empty = function() {
                if ($("#fields .listbox-to li").length <= 0) {
                    $("#fields .listbox-to-empty").show();
                    $("#fields .button-remove-all").attr({ disabled: 'disabled' });
                }
                else {
                    $("#fields .listbox-to-empty").hide();
                    $("#fields .button-remove-all").removeAttr('disabled');
                    $($.makeArray($("#fields .listbox-to li"))
                        .sort(function(a, b) { return parseInt($(a).attr('order')) - parseInt($(b).attr('order')); }))
                            .appendTo($("#fields .listbox-to"));
                }

                if ($("#fields .listbox-from li").length <= 0) {
                    $("#fields .listbox-from-empty").show();
                    $("#fields .button-add-all").attr({ disabled: 'disabled' });
                }
                else {
                    $("#fields .listbox-from-empty").hide();
                    $("#fields .button-add-all").removeAttr('disabled');
                    $($.makeArray($("#fields .listbox-from li"))
                        .sort(function(a, b) { return parseInt($(a).attr('order')) - parseInt($(b).attr('order')); }))
                            .appendTo($("#fields .listbox-from"));
                }

                check_selected();
            };
            var deselect_all = function(items, except) {
                except = $(except);
                $(items)
                    .filter(function() { return ($.inArray(this, except) < 0); })
                    .removeAttr('selected').removeClass('selected');
            }

            var extend_li = function(li, item) {
                var $li = $(li);
                var $item = $(item);

                $li.unbind('.select').bind('click.select', function() {
                    if (false && !isCtrlDown) {
                        deselect_all($item, $li);
                    }

                    if ($li.is('[selected]')) $li.removeAttr('selected').removeClass('selected');
                    else $li.attr('selected', 'selected').addClass('selected');

                    check_selected();
                });
            };

            var add_one = function(li) {
                var $itemFrom = $(li);

                var $itemTo = $('<li></li>')
                    .html($itemFrom.html());

                var rel_attr = $itemFrom.attr("rel");
                var order_attr = $itemFrom.attr("order");

                $itemFrom.remove();
                $itemTo.attr({ rel: rel_attr, order: order_attr });
                $itemTo.find("input").attr("name", rel_attr);


                $itemTo.appendTo('#fields .listbox-to');

                extend_li($itemTo, '#fields .listbox-to li');

                check_empty();
            };
            var remove_one = function(li) {
                var $itemFrom = $(li);
                var $itemTo = $('<li></li>')
                    .html($itemFrom.html());

                var rel_attr = $itemFrom.attr("rel");
                var order_attr = $itemFrom.attr("order");

                $itemFrom.remove();
                $itemTo.attr({ rel: rel_attr, order: order_attr });
                $itemTo.find("input").removeAttr("name");

                $itemTo.appendTo('#fields .listbox-from');

                extend_li($itemTo, '#fields .listbox-from li');

                check_empty();
            };

            $("#fields .listbox-to li").each(function() { extend_li(this, '#fields .listbox-to li'); });
            $("#fields .listbox-from li").each(function() { extend_li(this, '#fields .listbox-from li'); });

            check_empty();
            check_selected();

            $("#fields .button-add").click(function() {
                $("#fields .listbox-from > li[selected]").each(function() {
                    add_one(this);
                });
            });
            $("#fields .button-add-all").click(function() {
                $("#fields .listbox-from > li").each(function() {
                    add_one(this);
                });
            });
            $("#fields .button-remove").click(function() {
                $("#fields .listbox-to   li[selected]").each(function() {
                    remove_one(this);
                });
            });
            $("#fields .button-remove-all").click(function() {
                $("#fields .listbox-to   li").each(function() {
                    remove_one(this);
                });
            });
        });
    </script>
    <%TemplateView Current = (TemplateView)ViewData["SelectedTemplate"];
        string TemplateName = "";

        if (Current != null)
        {
            TemplateName = Current.Name;
        }

%>
    
    <h2>Создание шаблона</h2>
    
    <%= Html.ValidationSummary("Введенные данные некорректны. Проверьте их и повторите попытку.") %>
    
    <form action="/Templates/AddTemplate" id="frmTemplate" method="post" enctype="multipart/form-data">
        <%
            var selectedFields = (IEnumerable<FieldView>)(ViewData["SelectedFields"] ?? new FieldView[0]);
            selectedFields = selectedFields.OrderBy(field => field.Order);

            var fields = (IEnumerable<FieldView>)(ViewData["Fields"] ?? new FieldView[0]);
            fields = fields.OrderBy(field => field.Order).Except(selectedFields);
        %>
        
        <p><span class="error"><%=ViewData["error"]%></span></p>
        <p><span class="success"><%=ViewData["success"]%></span></p>
        <p><label for="fileToUpload">Имя шаблона:</label><input type="text" name="templateName" value="<%=Html.Encode(ViewData["TemplateName"]) %>" />
        <div>
            <table id="fields"><tr id="fields_row"><td class="listbox-section">
                <fieldset id="fields-from-container">
                    <legend>Доступные поля шаблона</legend>
                    <span class="listbox-from-empty"><%=(fields.Count() + selectedFields.Count() > 0 ? "(все доступные поля выбраны)" : "(список пуст)") %></span>
                    <ul class="listbox-from">
                        <% foreach (FieldView field in fields) { %>
                        <li rel="_<%=field.ID %>" order="<%=field.Order %>"><span class="unselectable"><%=Html.Encode(field.Name) %> (<%=Html.Encode(field.FieldTypeView.FieldTypeName) %>)</span><input style="Display:none;" type="text" /></li>
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
                    <legend>Выбранные поля шаблона</legend>
                    <span class="listbox-to-empty">(добавьте необходимые поля)</span>
                    <div class="listbox-to-wrapper">
                         <ul class="listbox-to">
                            <% foreach (FieldView field in selectedFields) { %>
                            <li rel="_<%=field.ID %>" order="<%=field.Order %>"><span class="unselectable"><%=Html.Encode(field.Name) %> (<%=Html.Encode(field.FieldTypeView.FieldTypeName) %>)</span><input style="Display:none;" type="text" name="_<%=field.ID %>" /></li>
                            <% } %>
                        </ul>
                    </div>
                </fieldset>
            </td></tr></table>
        </div>
        <p><button type="submit">Сохранить</button></p>
        <%=Html.ActionLink("Назад", "Index", "Templates")%>

    </form>
</asp:Content>