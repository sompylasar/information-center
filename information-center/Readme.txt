﻿Информационный центр ВУЗа

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

Что нужно сделать, чтобы всё это откомпилировать и посмотреть:

1. Поставить Microsoft VisualStudio 2008
2. Поставить Microsoft .NET Framework 3.5 sp1:
http://www.microsoft.com/downloads/details.aspx?FamilyID=AB99342F-5D1A-413D-8319-81DA479AB0D7&displaylang=en
3. Поставить Microsoft VisualStudio 2008 ServicePack1:
http://www.microsoft.com/downloads/details.aspx?FamilyId=FBEE1648-7106-44A7-9649-6D9F6D58056E&displaylang=en
4. Поставить ASP.NET MVC 1.0:
http://www.microsoft.com/downloads/details.aspx?FamilyID=53289097-73ce-43bf-b6a6-35e00103cb4b&displaylang=en
5. Поставить SQL Server 2005.
6. Присоединить к SQL Server базу данных "InformationCenter".
7. При необходимости добавить в Web.config в секцию configuration->connectionStrings следующую строку:
    <add name="InformationCenterDatabase" connectionString="data source=.\SQLEXPRESS;Initial Catalog=InformationCenter;Integrated Security=SSPI;" providerName="System.Data.SqlClient"/>
   Заполнить connectionString="" правильной строкой подключения к базе "InformationCenter" (для справки см. http://connectionstrings.com/ ).
   Примечание: в случае отсутствия в настройках такой строки веб-интерфейс предложит авторизоваться (на данный момент доступен только интегрированный тип входа); в противном случае, если строка задана верно, авторизация запрашиваться не будет.
8. Только после всего этого можно компилировать проект и молиться, чтобы все заработало =).

---------------------------------------------------------------------------------------------------