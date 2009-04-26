using System;
using System.Linq;
using System.Collections;
using System.Data.Objects;
using System.Collections.Generic;

namespace InformationCenter.EFEngine
{

    /// <summary>
    /// Класс для объектного постраничного запроса.
    /// </summary>
    /// <typeparam name="T">тип возвращаемых запросом объектов</typeparam>
    public class PageableQuery<T> : IEnumerable<T[]>
    {

        #region Классы

        /// <summary>
        /// Постраничный перечислитель.
        /// </summary>
        private class PageableEnumerator : IEnumerator<T[]>
        {

            #region Поля

            private PageableQuery<T> query; // запрос, который нужно постранично выполнять
            private int skipped = 0;        // число пропущенных записей
            private T[] inner_array = null; // текущая страница

            #endregion

            #region Конструкторы

            /// <summary>
            /// конструктор с постраничным запросом
            /// </summary>
            /// <param name="PQuery">постраничный запрос</param>
            public PageableEnumerator(PageableQuery<T> PQuery)
            {
                if (PQuery == null) throw new ArgumentNullException("PQuery");
                query = PQuery;
            }

            #endregion

            #region Свойства

            /// <summary>
            /// текущий элемент
            /// </summary>
            public T[] Current { get { return inner_array; } }

            /// <summary>
            /// текущий элемент
            /// </summary>
            object IEnumerator.Current { get { return inner_array; } }

            /// <summary>
            /// определяет признак того, что текущий блок данных перечислителя является последним
            /// </summary>
            public bool IsLastBlock { get { return inner_array == null ? false : inner_array.Length < query.PageSize; } }

            #endregion

            #region Методы

            /// <summary>
            /// освободить ресурсы
            /// </summary>
            public void Dispose() { }

            /// <summary>
            /// передвинуться в перечислении на одну позицию
            /// </summary>
            /// <returns>true, если ещё есть элементы</returns>
            public bool MoveNext()
            {
                if (IsLastBlock) return false; // проверка полезна, так как иногда можно не выполнять следующую строку кода
                inner_array = query.Query.Skip(skipped).Take(query.PageSize).ToArray();
                skipped += query.PageSize;
                return inner_array.Length > 0;
            }

            /// <summary>
            /// сбросить перечислитель в начальное состояние
            /// </summary>
            public void Reset() { skipped = 0; inner_array = null; }

            #endregion

        }

        #endregion

        #region Поля

        private static int def_page_size = 20;  // размер страницы по умолчанию
        private IOrderedQueryable<T> query;     // запрос, который нужно постранично выполнить
        private int page_size;                  // размер страницы

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="Query">некоторый объектный запрос</param>
        /// <param name="PageSize">размер (объём) страницы в сущностях (объектах)</param>
        public PageableQuery(IOrderedQueryable<T> Query, int PageSize)
        {
            if (Query == null) throw new ArgumentNullException("Query");
            page_size = PageSize;
            query = Query;
        }

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="Query">некоторый объектный запрос</param>
        public PageableQuery(IOrderedQueryable<T> Query) : this(Query, def_page_size) { }

        #endregion

        #region Свойства

        /// <summary>
        /// объём страницы (в записях - сущностях) по умолчанию
        /// </summary>
        public static int DefaultPageSize { get { return def_page_size; } }

        /// <summary>
        /// установленный объём страницы
        /// </summary>
        public int PageSize { get { return page_size; } }

        /// <summary>
        /// объектный запрос, который требует постраничного выполнения
        /// </summary>
        public IOrderedQueryable<T> Query { get { return query; } }

        #endregion

        #region Методы

        /// <summary>
        /// получить перечислитель
        /// </summary>
        /// <returns>объект-перечислитель</returns>
        public IEnumerator<T[]> GetEnumerator() { return new PageableEnumerator(this); }

        /// <summary>
        /// получить перечислитель
        /// </summary>
        /// <returns>объект-перечислитель</returns>
        IEnumerator IEnumerable.GetEnumerator() { return new PageableEnumerator(this); }

        #endregion

    }

}