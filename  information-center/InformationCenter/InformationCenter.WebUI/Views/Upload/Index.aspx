<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Загрузка документов - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        jQuery(function($) {
            var extend_tr = function (tr) {
                var $tr = $(tr);
                
                $tr.find('td').unbind('.select').bind('click.select', function () {
                    if ($tr.is('[selected]')) $tr.removeAttr('selected').css({ backgroundColor: 'transparent' });
                    else $tr.attr('selected', 'selected').css({ backgroundColor: '#E3E3E3' });
                });
            };
            var add_one = function (option) {
                var $option = $(option);
                $option.remove();
                    
                var $tr = $('<tr></tr>');
                var $key = $('<td></td>').addClass('key').appendTo($tr);
                var $value = $('<td></td>').addClass('value').appendTo($tr);
                
                $key.html($option.html());
                $('<input />').attr({ type: 'text', name: $option.val() }).appendTo($value);
                
                $tr.appendTo("#fields .listbox-to");
                
                extend_tr($tr);
            };
            var remove_one = function (tr) {
                var $tr = $(tr);
                $tr.remove();
                
                var $key = $tr.find('td.key');
                var $value = $tr.find('td.value');
                
                var $option = $('<option></option>').attr({ value: $value.attr('name') }).html($key.html());
                
                $option.appendTo("#fields .listbox-from");
            };
            
            $("#fields .listbox-to tr").each(function () {  extend_tr(this); });
            
            $("#fields .button-add").click(function() {
                $("#fields .listbox-from > option[selected]").each(function () {
                    add_one(this);
                });
            });
            $("#fields .button-add-all").click(function() {
                $("#fields .listbox-from > option").each(function () {
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
        <p><label for="fileToUpload">Документ:</label><input type="file" id="fileToUpload" name="f" value="123" /><%= Html.ValidationMessage("f") %></p>
        <div>
            <label for="fields"></label>
            <table id="fields"><tr><td>
                <table class="listbox-to">
                    <tr><td class="key">Author</td><td class="value"><input type="text" name="_Author" value="" /></td></tr>
                    <tr><td class="key">Title</td><td class="value"><input type="text" name="_Title" value="" /></td></tr>
                </table>
            </td><td>
                <div>
                    <button type="button" class="button-add">&lt;</button>
                    <button type="button" class="button-add-all">&lt;&lt;</button>
                </div>
                <div>
                    <button type="button" class="button-remove">&gt;</button>
                    <button type="button" class="button-remove-all">&gt;&gt;</button>
                </div>
            </td><td>
                <select multiple="multiple" class="listbox-from">
                    <option value="_Date">Date</option>
                    <option value="_Publisher">Publisher</option>
                </select>
            </td></tr></table>
        </div>
        <p><button type="submit">Загрузить</button></p>
    </form>
</asp:Content>


