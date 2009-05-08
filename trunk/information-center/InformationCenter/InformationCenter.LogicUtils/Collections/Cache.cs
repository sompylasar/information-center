using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicUtils
{

    /// <summary>
    /// Интерфейс для поддержки ключей.
    /// </summary>
    public interface IKeyable
    {

        /// <summary>
        /// получить идентификатор
        /// </summary>
        Guid ID { get; }

    }

    /// <summary>
    /// Типизированный кэш.
    /// Кэширование осуществляется по ID объектов.
    /// </summary>
    /// <typeparam name="T">Сущность для кэширования</typeparam>
    public class Cache<T> where T : class, IKeyable
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public Cache()
        {
            CachingObjects = new Dictionary<Guid, T>();
        }

        /// <summary>
        /// Добавить в кэш, если ещё нету
        /// </summary>
        /// <param name="entity">Что добавить</param>
        public void Add(T entity)
        {
            if (!CachingObjects.Keys.Contains(entity.ID))
                CachingObjects.Add(entity.ID, entity);
        }
        /// <summary>
        /// Вернуть объект из кэша
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Объект, если он закэширован, иначе - default</returns>
        public T Get(Guid id)
        {
            if (CachingObjects.Keys.Contains(id))
                return CachingObjects[id];
            return null;
        }
        /// <summary>
        /// Вернуть объекты из кэша
        /// </summary>
        /// <param name="ids">Идентификаторы объектов</param>
        /// <returns>Коллекция закешированных объектов. Размер может не совпадать с ids в том случае, если каких-то объектов нет в кэше</returns>
        public IEnumerable<T> Get(IEnumerable<Guid> ids)
        {
            List<T> result = new List<T>();
            foreach (var id in ids)
            {
                T entity = Get(id);
                
                if (entity != null)
                    result.Add(entity);
            }
            return result.ToArray();
        }
        /// <summary>
        /// Содержится ли объект в кэше
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>   true    - объект закэширован
        ///             false   - объект незакэширован</returns>
        public bool Contains(Guid id)
        {
            return CachingObjects.Keys.Contains(id);
        }
        /// <summary>
        /// Содержится ли объект в кэше
        /// </summary>
        /// <returns>   true    - объект закэширован
        ///             false   - объект незакэширован</returns>
        public bool Contains(T entity)
        {
            return this.Contains(entity.ID);
        }
        /// <summary>
        /// Пропустить объект через кэш.
        /// Если entity не закэширован, то он будет добавлен в кэш.
        /// </summary>
        /// <param name="entity">Объект</param>
        /// <returns>Если entity уже закэширован, то будет возвращён объект из кэша, иначе - объект entity</returns>
        public T CacheIt(T entity)
        {
            if (this.Contains(entity))
                return this.Get(entity.ID);

            this.Add(entity);
            return entity;
        }

        /// <summary>
        /// Слить вместе
        /// </summary>
        /// <param name="other">С чем слить</param>
        /// <returns>Результат слияния</returns>
        public Cache<T> Merge(Cache<T> other)
        {
            Cache<T> result = new Cache<T>();
            foreach (var item in CachingObjects)
            {
                result.CachingObjects.Add(item.Key, item.Value);
            }
            foreach (var item in other.CachingObjects)
            {
                result.Add(item.Value);
            }
            return result;
        }
        /// <summary>
        /// Пересечь кэши
        /// </summary>
        /// <param name="other">С чем пересечь</param>
        /// <returns>Результат пересечения</returns>
        public Cache<T> Intersect(Cache<T> other)
        {
            Cache<T> result = new Cache<T>();

            foreach (var item in other.CachingObjects)
            {
                if (this.Contains(item.Key))
                    result.Add(item.Value);
            }
            return result;
        }
        /// <summary>
        /// Вычесть из указанной коллекции идентификаторов те, которые уже закешированы
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public IEnumerable<Guid> SubstractFrom(IEnumerable<Guid> Ids)
        {
            var result = new List<Guid>();
            foreach (var id in Ids)
            {
                if (!Contains(id))
                {
                    result.Add(id);
                }
            }
            return result.ToArray();
        }
        /// <summary>
        /// Очистить кэш
        /// </summary>
        public void Clear()
        {
            CachingObjects.Clear();
        }

        /// <summary>
        /// Рабочая коллекция кэшируемых объектов
        /// </summary>
        protected Dictionary<Guid, T> CachingObjects { get; set; }
    }

    /// <summary>
    /// Результат запроса, повлекшего синхронизацию кэша и хранилища
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SyncInfo<T>: IEnumerable<T> where T:IKeyable
    {
        /// <summary>
        /// Собственно сама коллекция (результат запроса)
        /// </summary>
        public IEnumerable<T> Items
        {
            get { return items.ToArray(); }
        }
        private List<T> items = new List<T>();
        /// <summary>
        /// Идентификаторы только что подгруженных из хранилища объектов
        /// </summary>
        public IEnumerable<Guid> NewIds
        {
            get { return newIds.ToArray(); }
        }
        private List<Guid> newIds = new List<Guid>();

        /// <summary>
        /// Добавить объект
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            items.Add(item);
        }
        /// <summary>
        /// Добавить объекты
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<T> items)
        {
            this.items.AddRange(items);
        }
        /// <summary>
        /// Добавить объект как новый
        /// </summary>
        /// <param name="item"></param>
        public void AddNewItem(T item)
        {
            newIds.Add(item.ID);
            Add(item);
        }
        /// <summary>
        /// Добавить объекты как новые
        /// </summary>
        /// <param name="items"></param>
        public void AddNewRange(IEnumerable<T> items)
        {
            newIds.AddRange(items.Select(s => s.ID));
            AddRange(items);
        }

        public void Merge(SyncInfo<T> syncInfo)
        {
            newIds.AddRange(syncInfo.NewIds);
            AddRange(syncInfo);
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion
    }
}
