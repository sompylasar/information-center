using System;

namespace LogicUtils
{

    /// <summary>
    /// События, которые неплохо было бы иметь для контроля коллекций.
    /// </summary>
    public interface ICommonEvents<T>
    {

        /// <summary>
        /// событие возникает перед добавлением нового элемента
        /// </summary>
        event EventHandler<CancelableEventArgs<T>> ValueAdding;

        /// <summary>
        /// событие возникает в результате отмены добавления элемента
        /// </summary>
        event EventHandler<EventArgs<T>> ValueAddCanceled;

        /// <summary>
        /// событие возникает после добавления нового элемента
        /// </summary>
        event EventHandler<EventArgs<T>> ValueAdded;

        /// <summary>
        /// событие возникает перед удалением элемента
        /// </summary>
        event EventHandler<CancelableEventArgs<T>> ValueRemoving;

        /// <summary>
        /// событие возникает в результате отмены удаления элемента
        /// </summary>
        event EventHandler<EventArgs<T>> ValueRemoveCanceled;

        /// <summary>
        /// событие возникает после удаления элемента
        /// </summary>
        event EventHandler<EventArgs<T>> ValueRemoved;

    }

}
