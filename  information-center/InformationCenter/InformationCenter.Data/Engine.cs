using System;
using System.IO;
using System.Xml;
using LogicUtils;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Data.Common;
using System.Data.Objects;
using System.Linq.Expressions;
using System.Xml.Serialization;
using System.Data.EntityClient;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;

namespace InformationCenter.Data
{

    /// <summary>
    /// Интерфейс для обращения к движку источника данных.
    /// </summary>
    public interface IRequestable
    {

        /// <summary>
        /// получить из хранилища все сущности заданного типа
        /// </summary>
        /// <typeparam name="T">тип сущностей</typeparam>
        /// <returns>объект-запрос для получения коллекции сущностей, по которому можно сделать ещё запросы</returns>
        ObjectQuery<T> CreateQuery<T>() where T : EntityObject;

        /// <summary>
        /// выполнить некоторый строковый запрос
        /// </summary>
        /// <typeparam name="T">тип возвращаемых данных</typeparam>
        /// <param name="QueryString">строка запроса</param>
        /// <param name="Parameters">параметры запроса</param>
        /// <returns>объект-запрос для получения коллекции сущностей, по которому можно сделать ещё запросы</returns>
        ObjectQuery<T> CreateQuery<T>(string QueryString, params ObjectParameter[] Parameters);

        IQueryable<T> Get<T>(IEnumerable<Guid> Identifiers) where T : EntityObject;

        /// <summary>
        /// выполнить некоторую команду
        /// </summary>
        /// <typeparam name="T">тип возвращаемых данных</typeparam>
        /// <param name="FunctionName">имя команды</param>
        /// <param name="Parameters">параметры команды</param>
        /// <returns>коллекция значений</returns>
        ObjectResult<T> ExecuteFunction<T>(string FunctionName, params ObjectParameter[] Parameters) where T : IEntityWithChangeTracker;
        
        /// <summary>
        /// получить сущнось по ключу
        /// </summary>
        /// <typeparam name="T">тип сущности</typeparam>
        /// <param name="Key">ключ</param>
        /// <returns>сущность</returns>
        T GetByKey<T>(EntityKey Key);
        
        /// <summary>
        /// получить объект по ключу
        /// </summary>
        /// <param name="Key">ключ</param>
        /// <returns>объект</returns>
        object GetByKey(EntityKey Key);

        /// <summary>
        /// попытаться получить сущность по ключу
        /// </summary>
        /// <typeparam name="T">тип сущности</typeparam>
        /// <param name="Key">ключ</param>
        /// <param name="Entity">выходное значение</param>
        /// <returns>true, если сущность получена, false, если сущность с таким ключом не найдена</returns>
        bool TryGetByKey<T>(EntityKey Key, out T Entity);
        
        /// <summary>
        /// попытаться получить объект по ключу
        /// </summary>
        /// <param name="Key">ключ</param>
        /// <param name="Entity">выходное значение</param>
        /// <returns>true, если объект получен, false, если объект с таким ключом не найден</returns>
        bool TryGetByKey(EntityKey Key, out object Entity);
        
        /// <summary>
        /// добавить сущность в хранилище
        /// </summary>
        /// <param name="EntitySetName">строковое значение типа набора сущностей</param>
        /// <param name="Entity">сама сущность</param>
        void AddObject(string EntitySetName, object Entity);
        
        /// <summary>
        /// обновить коллекцию объектов
        /// </summary>
        /// <param name="refreshMode">тип обновления</param>
        /// <param name="collection">коллекция объектов, которые нужно обновить</param>
        void Refresh(RefreshMode refreshMode, IEnumerable collection);

        /// <summary>
        /// обновить объект
        /// </summary>
        /// <param name="refreshMode">тип обновлния</param>
        /// <param name="entity">обновляемый объект</param>
        void Refresh(RefreshMode refreshMode, object entity);

        Exception SaveChanges();

        RefreshMode ConflictModeResolveOnSave { get; set; }

        EntityKey CreateEntityKey(string EntitySetName, object EntityObject);

        void DoFetch(string ProcedureName, DbDataReaderDelegate Delegate, params DbParameter[] Params);
    
    }

    /// <summary>
    /// Интерфейс для работы с хранилищем объектов. Наверное, будет пухлым.
    /// </summary>
    public interface IEngine : IRequestable, IDisposable
    {

        User Current { get; }

    }

}