<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="InformationCenter.WebUI.Models"%>
<%@ Import Namespace="InformationCenter.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Получение документа - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
    <% 
        DocumentView document = ((DocumentView)ViewData["Document"]);

        if (document != null)
        {

            string filename = "", contentType = "";
            FileHelper.SplitFilename(document.FileName, out filename, out contentType);

            ViewData["FileUrl"] =
               "/Download/Index/"
               + document.ID + "/"
               + Url.RawEncode(filename, Encoding.UTF8) + "";

    %>
    <!--<meta http-equiv="refresh" content="5;url=<%=ViewData["FileUrl"] %>" />-->
    <script type="text/javascript">
    jQuery(function ($) {
        $('head meta[http-equiv=refresh]').remove();
        $('#beginDownload').hide();
        
        var total_seconds = 2;
        var seconds = 0+total_seconds;
        
        function updateTimer() {
            if (seconds > 0) {
                $('#waitDownload').show();
                $('#waitDownloadTimer').html(seconds + ' сек.');
            }
            else {
                $('#waitDownload').hide();
                $('#beginDownload').show();
                
                top.location = '<%=ViewData["FileUrl"] %>';
            }
        }
        
        updateTimer();
        var interval = setInterval(function () {
            --seconds;
            updateTimer();
            if (seconds <= 0) clearInterval(interval);
        }, 1000);
    });
    </script>
    <%  } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Получение документа</h2>
    <%
        DocumentView document = ((DocumentView)ViewData["Document"]);
        if (document == null)
        {
    %><p class="error"><%=Html.Encode(ViewData["error"] ?? "Ошибка программы. Документ не задан.")%></p><%
        }
        else
        {
            string filename = "", contentType = "";
            FileHelper.SplitFilename(document.FileName, out filename, out contentType);
    %>
    <p id="waitDownload" style="display: none"><span id="waitDownloadTimer"></span> до начала скачивания документа &laquo;<%=Html.Encode(filename)%>&raquo;...</p>
    <p id="beginDownload">Запущено скачивание документа &laquo;<%=Html.Encode(filename)%>&raquo;</p>
    <%} %>
</asp:Content>
