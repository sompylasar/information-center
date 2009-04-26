using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LogicUtils;

namespace LogicUtils
{

    /// <summary>
    /// Класс шаблонной коллекции с событиями. 
    /// </summary>
    /// <typeparam name="T">тип элементов, хранящихся в коллекции</typeparam>
    public class EventTriggerCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEquatable<EventTriggerCollection<T>>, ICommonEvents<T>
    {

        #region Поля

        protected List<T> items = null;
        protected DefferredEventsManager<T> manager = null;

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор по умолчанию
        /// </summary>
        public EventTriggerCollection() { items = new List<T>(); manager = CreateManager(); }

        /// <summary>
        /// конструктор с ёмкостью
        /// </summary>
        /// <param name="Capacity">ёмкость хранилища</param>
        public EventTriggerCollection(int Capacity) { items = new List<T>(Capacity); manager = CreateManager(); }

        /// <summary>
        /// конструктор с начальной коллекцией
        /// </summary>
        /// <param name="Items">коллекция элементов</param>
        public EventTriggerCollection(IEnumerable<T> Items) { items = new List<T>(Items); manager = CreateManager(); }

        #endregion

        #region События

        #region Add

        /// <summary>
        /// событие возникает перед добавлением нового элемента в коллекцию
        /// </summary>
        public event EventHandler<CancelableEventArgs<T>> ValueAdding
        {
            add { manager.RegisterCancelableHandler(value, CancelableEventTypes.Add); }
            remove { manager.UnRegisterCancelableHandler(value, CancelableEventTypes.Add); }
        }

        /// <summary>
        /// событие возникает в результате отмены добавления в коллекцию элемента
        /// </summary>
        public event EventHandler<EventArgs<T>> ValueAddCanceled
        {
            add { manager.RegisterHandler(value, EventTypes.AddCanceled); }
            remove { manager.UnRegisterHandler(value, EventTypes.AddCanceled); }
        }

        /// <summary>
        /// событие возникает после добавления нового элемента в коллекцию
        /// </summary>
        public event EventHandler<EventArgs<T>> ValueAdded
        {
            add { manager.RegisterHandler(value, EventTypes.Added); }
            remove { manager.UnRegisterHandler(value, EventTypes.Added); }
        }

        #endregion

        #region Remove

        /// <summary>
        /// событие возникает перед удалением элемента из коллекции
        /// </summary>
        public event EventHandler<CancelableEventArgs<T>> ValueRemoving
        {
            add { manager.RegisterCancelableHandler(value, CancelableEventTypes.Remove); }
            remove { manager.UnRegisterCancelableHandler(value, CancelableEventTypes.Remove); }
        }

        /// <summary>
        /// событие возникает в результате отмены удаления элемента из коллекции
        /// </summary>
        public event EventHandler<EventArgs<T>> ValueRemoveCanceled
        {
            add { manager.RegisterHandler(value, EventTypes.RemoveCanceled); }
            remove { manager.UnRegisterHandler(value, EventTypes.RemoveCanceled); }
        }

        /// <summary>
        /// событие возникает после удаления элемента из коллекции
        /// </summary>
        public event EventHandler<EventArgs<T>> ValueRemoved
        {
            add { manager.RegisterHandler(value, EventTypes.Removed); }
            remove { manager.UnRegisterHandler(value, EventTypes.Removed); }
        }
        
        #endregion

        protected virtual void OnAdding(CancelableEventArgs<T> Args)
        { manager.ExecuteCancelableHandler(Args, CancelableEventTypes.Add); }

        protected virtual void OnAddCanceled(EventArgs<T> Args)
        { manager.ExecuteHandler(Args, EventTypes.AddCanceled); }

        protected virtual void OnAdded(EventArgs<T> Args)
        { manager.ExecuteHandler(Args, EventTypes.Added); }

        protected virtual void OnRemoving(CancelableEventArgs<T> Args)
        { manager.ExecuteCancelableHandler(Args, CancelableEventTypes.Remove); }

        protected virtual void OnRemoveCanceled(EventArgs<T> Args)
        { manager.ExecuteHandler(Args, EventTypes.RemoveCanceled); }

        protected virtual void OnRemoved(EventArgs<T> Args)
        { manager.ExecuteHandler(Args, EventTypes.Removed); }

        #endregion

        #region Свойства

        /// <summary>
        /// индексатор коллекции
        /// </summary>
        /// <param name="Index">индекс элемента</param>
        /// <returns>элемент по заданному индексу</returns>
        public T this[int Index]
        {
            get { return items[Index]; }
            set
            {
                T old_val = items[Index];
                items[Index] = value;
            }
        }

        public int Capacity { get { return items.Capacity; } set { items.Capacity = value; } }

        /// <summary>
        /// число элементов в коллекции
        /// </summary>
        public int Count { get { return items.Count; } }

        /// <summary>
        /// только для чтения - нет
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        #endregion

        #region Методы

        /// <summary>
        /// создать управляющий событиями объект
        /// </summary>
        /// <returns>управляющий событиями объект</returns>
        protected virtual DefferredEventsManager<T> CreateManager() { return new DefferredEventsManager<T>(); }

        /// <summary>
        /// добавить элемент в коллекцию
        /// </summary>
        /// <param name="Item">элемент</param>
        public void Add(T Item) { AddItem(Item); }

        /// <summary>
        /// добавить элемент в коллекцию
        /// </summary>
        /// <param name="Item">элемент</param>
        /// <returns>признак успешного добавления</returns>
        public bool AddItem(T Item)
        {
            CancelableEventArgs<T> args = new CancelableEventArgs<T>(Item);
            OnAdding(args);
            if (!args.Cancel)
            {
                items.Add(Item);
                OnAdded(new EventArgs<T>(Item));
            }
            else OnAddCanceled(new EventArgs<T>(Item));
            return !args.Cancel;
        }

        /// <summary>
        /// добавить массив элементов в коллекцию
        /// </summary>
        /// <param name="Items">набор элементов</param>
        public void AddRange(IEnumerable<T> Items)
        {
            foreach (T item in Items)
            {
                CancelableEventArgs<T> args = new CancelableEventArgs<T>(item);
                OnAdding(args);
                if (!args.Cancel)
                {
                    items.Add(item);
                    OnAdded(new EventArgs<T>(item));
                }
                else OnAddCanceled(new EventArgs<T>(item));
            }
        }

        public ReadOnlyCollection<T> AsReadOnly() { return items.AsReadOnly(); }

        public int BinarySearch(T Item) { return items.BinarySearch(Item); }

        public int BinarySearch(T Item, IComparer<T> Comparer) { return items.BinarySearch(Item, Comparer); }

        public int BinarySearch(int Index, int Count, T Item, IComparer<T> Comparer) { return items.BinarySearch(Index, Count, Item, Comparer); }

        /// <summary>
        /// очистить коллекцию
        /// </summary>
        public void Clear() { while (items.Count != 0) Remove(items[0]); }

        /// <summary>
        /// признак присутствия элемента в коллекции
        /// </summary>
        /// <param name="item">элемент</param>
        /// <returns>true, если есть</returns>
        public bool Contains(T item) { return items.Contains(item); }

        public List<U> ConvertAll<U>(Converter<T,U> Converter) { return items.ConvertAll<U>(Converter); }

        /// <summary>
        /// скопировать коллекцию в массив
        /// </summary>
        /// <param name="Array">массив, куда копировать</param>
        /// <param name="ArrayIndex">начальный индекс добавляемых из коллекции данных</param>
        public void CopyTo(T[] Array, int ArrayIndex) { items.CopyTo(Array, ArrayIndex); }

        public void CopyTo(T[] Array) { items.CopyTo(Array); }

        public void CopyTo(int Index, T[] Array, int ArrayIndex, int Count) { items.CopyTo(Index, Array, ArrayIndex, Count); }  

        public override bool Equals(object obj)
        {
            EventTriggerCollection<T> other = obj as EventTriggerCollection<T>;
            return Equals(other);
        }

        /// <summary>
        /// проверка эквивалентности двух коллекций
        /// </summary>
        /// <param name="other">другая коллекция</param>
        /// <returns>true, если множества эквивалентны</returns>
        public bool Equals(EventTriggerCollection<T> other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other)) return true;

            if (other.Count != Count) return false;
            int collection_item_counter = 0;
            foreach (T item in items) if (other.Contains(item)) ++collection_item_counter;
            return Count == collection_item_counter;
        }

        /// <summary>
        /// проверка существования в коллекции
        /// </summary>
        /// <param name="Match">условие поиска</param>
        /// <returns>true, если найдено соответствие</returns>
        public bool Exists(Predicate<T> Match) { return items.Exists(Match); }
        
        /// <summary>
        /// поиск в коллекции по условию
        /// </summary>
        /// <param name="Match">условие поиска</param>
        /// <returns>первый удовлетворяющий условию поиска элемент или null, если ничего не найдено</returns>
        public T Find(Predicate<T> Match) { return items.Find(Match); }

        /// <summary>
        /// поиск в коллекции по условию
        /// </summary>
        /// <param name="Match">условие поиска</param>
        /// <returns>все элементы коллекции, удовлетворяющие заданному условию поиска</returns>
        public virtual List<T> FindAll(Predicate<T> Match) { return items.FindAll(Match); }

        public int FindIndex(Predicate<T> Match) { return items.FindIndex(Match); }

        public int FindIndex(int StartIndex, Predicate<T> Match) { return items.FindIndex(StartIndex, Match); }

        public int FindIndex(int StartIndex, int Count, Predicate<T> Match) { return items.FindIndex(StartIndex, Count, Match); }

        public T FindLast(Predicate<T> Match) { return items.FindLast(Match); }

        public int FindLastIndex(Predicate<T> Match) { return items.FindLastIndex(Match); }

        public int FindLastIndex(int StartIndex, Predicate<T> Match) { return items.FindLastIndex(StartIndex, Match); }

        public int FindLastIndex(int StartIndex, int Count, Predicate<T> Match) { return items.FindLastIndex(StartIndex, Count, Match); }
        
        public void ForEach(Action<T> Action) { items.ForEach(Action); }

        /// <summary>
        /// получить перечислитель (для foreach)
        /// </summary>
        /// <returns>перечислитель</returns>
        public IEnumerator<T> GetEnumerator() { return items.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public override int GetHashCode() { return items.GetHashCode(); }

        public int IndexOf(T Item) { return items.IndexOf(Item); }

        public int IndexOf(T Item, int Index) { return items.IndexOf(Item, Index); }

        public int IndexOf(T Item, int Index, int Count) { return items.IndexOf(Item, Index, Count); }

        public void Insert(int Index, T Item)
        {
            CancelableEventArgs<T> args = new CancelableEventArgs<T>(Item);
            OnAdding(args);
            if (!args.Cancel)
            {
                items.Insert(Index, Item);
                OnAdded(new EventArgs<T>(Item));
            }
            else OnAddCanceled(new EventArgs<T>(Item));
        }

        public int LastIndexOf(T Item) { return items.LastIndexOf(Item); }

        public int LastIndexOf(T Item, int Index) { return items.LastIndexOf(Item, Index); }

        public int LastIndexOf(T Item, int Index, int Count) { return items.LastIndexOf(Item, Index, Count); }

        /// <summary>
        /// удалить из коллекции элемент
        /// </summary>
        /// <param name="Item">элемент</param>
        /// <returns>true, если удаление прошло успешно</returns>
        public bool Remove(T Item)
        {
            CancelableEventArgs<T> args = new CancelableEventArgs<T>(Item);
            OnRemoving(args);
            bool result = false;
            if (!args.Cancel)
            {
                result = items.Remove(Item);
                if (result) OnRemoved(new EventArgs<T>(Item));
            }
            else OnRemoveCanceled(new EventArgs<T>(Item));
            return result;
        }

        public int RemoveAll(Predicate<T> Match)
        {
            var result = FindAll(Match);
            int counter = 0;
            foreach (T item in result) if (Remove(item)) ++counter;
            return counter;
        }

        /// <summary>
        /// удалить из коллекции элемент в заданной позиции
        /// </summary>
        /// <param name="Index">индекс элемента</param>
        /// <returns>успешно ли прошло удаление</returns>
        public void RemoveAt(int Index) { if (Index >= 0 && Index < items.Count) Remove(items[Index]); }

        public void Sort() { items.Sort(); }

        public void Sort(Comparison<T> Comparison) { items.Sort(Comparison); }

        public void Sort(IComparer<T> Comparer) { items.Sort(Comparer); }

        public void Sort(int Index, int Count, IComparer<T> Comparer) { items.Sort(Index, Count, Comparer); }

        /// <summary>
        /// преобразовать в массив
        /// </summary>
        /// <returns>массив значений типа T</returns>
        public T[] ToArray() { return items.ToArray(); }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Capacity * 20);
            foreach (T item in items) sb.Append(item.ToString()).Append(Environment.NewLine);
            return sb.ToString();
        }

        public void TrimExcess() { items.TrimExcess(); }

        public bool TrueForAll(Predicate<T> Match) { return items.TrueForAll(Match); }

        #endregion

    }

}