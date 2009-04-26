using System;
using System.Collections.Generic;

namespace LogicUtils
{

    /// <summary>
    /// Типы событий, происходящих в коллекции.
    /// </summary>
    public enum EventTypes
    {
        AddCanceled,
        Added,
        RemoveCanceled,
        Removed,
        Moved
    }

    /// <summary>
    /// Типы отменяемых событий, происходящих в коллекции.
    /// </summary>
    public enum CancelableEventTypes
    {
        Add,
        Remove
    }

    /// <summary>
    /// Менеджер отложенного выполнения событий.
    /// </summary>
    /// <typeparam name="T">тип данных, с которыми работает менеджер</typeparam>
    public class DefferredEventsManager<T> : ICommonEvents<T>
    {

        #region Поля

        protected Dictionary<EventTypes, List<EventHandler<EventArgs<T>>>> events = null;
        protected Dictionary<CancelableEventTypes, List<EventHandler<CancelableEventArgs<T>>>> cancelable_events = null;

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор по умолчанию
        /// </summary>
        public DefferredEventsManager() { }

        #endregion

        #region События

        #region Add

        /// <summary>
        /// событие возникает перед добавлением нового элемента в коллекцию
        /// </summary>
        public event EventHandler<CancelableEventArgs<T>> ValueAdding
        {
            add { RegisterCancelableHandler(value, CancelableEventTypes.Add); }
            remove { UnRegisterCancelableHandler(value, CancelableEventTypes.Add); }
        }

        /// <summary>
        /// событие возникает в результате отмены добавления в коллекцию элемента
        /// </summary>
        public event EventHandler<EventArgs<T>> ValueAddCanceled
        {
            add { RegisterHandler(value, EventTypes.AddCanceled); }
            remove { UnRegisterHandler(value, EventTypes.AddCanceled); }
        }

        /// <summary>
        /// событие возникает после добавления нового элемента в коллекцию
        /// </summary>
        public event EventHandler<EventArgs<T>> ValueAdded
        {
            add { RegisterHandler(value, EventTypes.Added); }
            remove { UnRegisterHandler(value, EventTypes.Added); }
        }

        #endregion

        #region Remove

        /// <summary>
        /// событие возникает перед удалением элемента из коллекции
        /// </summary>
        public event EventHandler<CancelableEventArgs<T>> ValueRemoving
        {
            add { RegisterCancelableHandler(value, CancelableEventTypes.Remove); }
            remove { UnRegisterCancelableHandler(value, CancelableEventTypes.Remove); }
        }

        /// <summary>
        /// событие возникает в результате отмены удаления элемента из коллекции
        /// </summary>
        public event EventHandler<EventArgs<T>> ValueRemoveCanceled
        {
            add { RegisterHandler(value, EventTypes.RemoveCanceled); }
            remove { UnRegisterHandler(value, EventTypes.RemoveCanceled); }
        }

        /// <summary>
        /// событие возникает после удаления элемента из коллекции
        /// </summary>
        public event EventHandler<EventArgs<T>> ValueRemoved
        {
            add { RegisterHandler(value, EventTypes.Removed); }
            remove { UnRegisterHandler(value, EventTypes.Removed); }
        }

        #endregion

        #endregion

        #region Методы

        #region Отменяемые

        public virtual void RegisterCancelableHandler(EventHandler<CancelableEventArgs<T>> Handler, CancelableEventTypes EventType)
        {
            if (Handler != null)
            {
                if (cancelable_events == null) cancelable_events = new Dictionary<CancelableEventTypes, List<EventHandler<CancelableEventArgs<T>>>>();
                if (!cancelable_events.ContainsKey(EventType))
                    cancelable_events.Add(EventType, new List<EventHandler<CancelableEventArgs<T>>>());
                cancelable_events[EventType].Add(Handler);
            }
        }

        public virtual void UnRegisterCancelableHandler(EventHandler<CancelableEventArgs<T>> Handler, CancelableEventTypes EventType)
        {
            if (Handler != null && cancelable_events != null && cancelable_events.ContainsKey(EventType))
                cancelable_events[EventType].Remove(Handler);
        }

        public virtual void ExecuteCancelableHandler(CancelableEventArgs<T> Args, CancelableEventTypes EventType)
        {
            if (cancelable_events != null && cancelable_events.ContainsKey(EventType) && !Args.Cancel)
            {
                for (int i = 0; i < cancelable_events[EventType].Count; ++i)
                {
                    try { cancelable_events[EventType][i].Invoke(this, Args); }
                    catch (Exception Ex) { HandleError(EventType, Ex); }
                    if (Args.Cancel) return;
                }
            }
        }

        #endregion

        #region Не отменяемые

        public virtual void RegisterHandler(EventHandler<EventArgs<T>> Handler, EventTypes EventType)
        {
            if (Handler != null)
            {
                if (events == null) events = new Dictionary<EventTypes, List<EventHandler<EventArgs<T>>>>();
                if (!events.ContainsKey(EventType))
                    events.Add(EventType, new List<EventHandler<EventArgs<T>>>());
                events[EventType].Add(Handler);
            }
        }

        public virtual void UnRegisterHandler(EventHandler<EventArgs<T>> Handler, EventTypes EventType)
        {
            if (Handler != null && events != null && events.ContainsKey(EventType))
                events[EventType].Remove(Handler);
        }

        public virtual void ExecuteHandler(EventArgs<T> Args, EventTypes EventType)
        {
            if (events != null && events.ContainsKey(EventType))
            {
                for (int i = 0; i < events[EventType].Count; ++i)
                {
                    try { events[EventType][i].Invoke(this, Args); }
                    catch (Exception Ex) { HandleError(EventType, Ex); }
                }
            }
        }

        #endregion

        #region Обработка ошибок

        protected virtual void HandleError(CancelableEventTypes EventType, Exception Ex) { }

        protected virtual void HandleError(EventTypes EventType, Exception Ex) { }

        #endregion

        #endregion

    }

}