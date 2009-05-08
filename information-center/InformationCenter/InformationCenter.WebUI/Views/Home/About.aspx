<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
Об авторах - Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="indexMainHeader" ContentPlaceHolderID="MainHeaderContent" runat="server">
Информационный центр ВУЗа
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        ul.authors, ul.authors li
        {
        	list-style-type: none;
        }
        ul.authors { clear: both; }
        ul.authors li 
        {
        	display: block;
        	width: 160px;
        	float: left;
        	margin-bottom: 10px;
        	text-align: center;
        }
        ul.authors div.photo 
        {
        	height: 160px;
        }
        ul.authors div.photo img 
        {
        	border: 3px solid #5cb257;
        }
        ul.authors li span.name
        {
        	font-weight: bold;
        }
        ul.authors li span.position
        {
        	display: block;
        	clear: left;
        }
    </style>
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Об авторах</h2>
    <div style="clear:both"></div>
    <ul class="authors">
        <li>
            <div class="photo"><img src="/Content/Images/AboutPhotos/SemenikhinaOS_small.png" alt="Семенихина О.С. (фото)" /></div>
            <div><span class="name">Семенихина О.С.</span> - <span class="position">менеджер проекта</span></div>
        </li>
        <li>
            <div class="photo"><img src="/Content/Images/AboutPhotos/BabakIA_small.png" alt="Бабак И.А. (фото)" /></div>
            <div><span class="name">Бабак И.А.</span> - <span class="position">инженер-программист</span></div>
        </li>
        <li>
            <div class="photo"><img src="/Content/Images/AboutPhotos/KretovKA_small.png" alt="Кретов К.А. (фото)" /></div>
            <div><span class="name">Кретов К.А.</span> - <span class="position">инженер-программист</span></div>
        </li>
        <li>
            <div class="photo"><img src="/Content/Images/AboutPhotos/MaksimovIA_small.png" alt="Максимов И.А. (фото)" /></div>
            <div><span class="name">Максимов И.А.</span> - <span class="position">инженер-программист</span></div>
        </li>
        <li>
            <div class="photo"><img src="/Content/Images/AboutPhotos/ShlyapenkoDA_small.png" alt="Шляпенко Д.А. (фото)" /></div>
            <div><span class="name">Шляпенко Д.А.</span> - <span class="position">инженер-программист</span></div>
        </li>
    </ul>
    <div style="clear:both"></div>
</asp:Content>
