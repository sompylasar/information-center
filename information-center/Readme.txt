Информационный центр ВУЗа

Адрес проекта: http://code.google.com/p/information-center/

---------------------------------------------------------------------------------------------------

Участники проекта.

1)Программисты:
	1. Бабак Иван
	2. Кретов Константин
	3. Максимов Иван
	4. Шляпенко Денис

2)Руководитель группы:
	Семенихина Оксана

3)Руководитель проекта:
	Григорьев Александр Сергеевич


Зоны ответственности участников проекта:

1. Бабак Иван - сервисы верхнего уровня (веб-интерфейс): Загрузка, Поиск, Выдача документов; дизайн веб-интерфейса.
InformationCenter.WebUI
InformationCenter.WebUI.Tests

2. Кретов Константин - сервисы верхнего уровня (веб-интерфейс): Управление, редактирование.
InformationCenter.WebUI
InformationCenter.WebUI.Tests

3. Максимов Иван - сервисы средних уровней (уровня сущностей БД и уровня сервисов информационного центра)
InformationCenter.Data - проект с автосгенерированым кодом объектного представления базы данных
InformationCenter.LogicUtils - утилитная библиотека
InformationCenter.EFEngine - проект с классами для работы в Entity Framework
InformationCenter.Services - реализация конкретных функций информационного центра

4. Шляпенко Денис - сервисы нижних уровней
InformationCenter.DBUtils - функции сопряжения с базой данных
InformationCenter.LogicUtils - библиотека утилит
InformationCenter (база данных) - структура базы данных, хранимые процедуры, триггеры и прочее
 
5. Семенихина Оксана - контроль выполнения, отчётность перед руководителем проекта, схемы, графики и прочее.

6. Григорьев Александр Сергеевич - проставление зачёта.

---------------------------------------------------------------------------------------------------

Что нужно сделать, чтобы откомпилировать и посмотреть проект:

1. Установить Microsoft VisualStudio 2008.
2. Установить Microsoft .NET Framework 3.5 sp1:
http://www.microsoft.com/downloads/details.aspx?FamilyID=AB99342F-5D1A-413D-8319-81DA479AB0D7&displaylang=en
3. Установить Service Pack 1 для Microsoft VisualStudio 2008:
http://www.microsoft.com/downloads/details.aspx?FamilyId=FBEE1648-7106-44A7-9649-6D9F6D58056E&displaylang=en
4. Установить ASP.NET MVC 1.0:
http://www.microsoft.com/downloads/details.aspx?FamilyID=53289097-73ce-43bf-b6a6-35e00103cb4b&displaylang=en
5. Установить SQL Server 2005.
6. Присоединить к SQL Server базу данных "InformationCenter" ( db/InformationCenter.mdf ).
7. При необходимости отредактировать файл Web.config в корневой папке проекта, добавив в секцию configuration->connectionStrings следующую строку:
    <add name="InformationCenterDatabase" connectionString="data source=.\SQLEXPRESS;Initial Catalog=InformationCenter;Integrated Security=SSPI;" providerName="System.Data.SqlClient"/>
   Заполнить connectionString="" правильной строкой подключения к базе "InformationCenter" (для справки см. http://connectionstrings.com/ ).
   Примечание: в случае отсутствия в настройках такой строки веб-интерфейс предложит авторизоваться (на данный момент доступен только интегрированный тип входа); в противном случае, если строка задана верно, авторизация запрашиваться не будет.
8. Открыть исходные коды проекта ( InformationCenter/InformationCenter.sln ) в VisualStudio и запустить отладку. В появившемся диалоге, спрашивающем, следует ли поменять Web.config для возможной отладки, выбрать продолжение без отладки.

---------------------------------------------------------------------------------------------------

Что нужно сделать, чтобы запустить проект на веб-сервере:

1. Определить версию установленного веб-сервера IIS (Microsoft Internet Information Services)
В секции Using ASP.NET MVC with Different Versions of IIS статьи http://www.asp.net/learn/mvc/tutorial-08-cs.aspx указано, какие версии IIS установлены в различных версиях ОС Windows).
2. При необходимости установить IIS версии не ниже 5.0:
http://www.microsoft.com/web/downloads/platform.aspx
3. Настроить веб-сервер IIS согласно прилагаемым к нему инструкциям.
4. Добавить веб-сайт, в корень которого скопировать файлы из папки InformationCenter.published
5. Следовать инструкциям по настройке установленной версии IIS для ASP.NET MVC (см. статью http://www.asp.net/learn/mvc/tutorial-08-cs.aspx )
6. Выполнить пункты 5-7 инструкции по компиляции и просмотру проекта.
7. Запустить веб-браузер и ввести адрес созданного веб-сайта.

---------------------------------------------------------------------------------------------------