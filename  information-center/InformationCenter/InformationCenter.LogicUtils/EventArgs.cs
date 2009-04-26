using System;

namespace LogicUtils
{

    #region TEMPLATE

    /// <summary> Аргументы параметризованных событий </summary>
    /// <typeparam name="T"> Тип параметра, передаваемого в событии. </typeparam>
    public class EventArgs<T> : EventArgs
    {

        /// <summary> Аргумент события </summary>
        protected T m_value;

        /// <summary> Конструктор с значением аргумента события </summary>
        /// <param name="value"> значение аргумента </param>
        public EventArgs(T value) { m_value = value; }
        
        /// <summary> Получить значение аргумента </summary>
        /// <value> значение аргумента </value>
        public T Value { get { return (m_value); } }

    }

    /// <summary>
    /// Аргументы для отменяемого события.
    /// </summary>
    /// <typeparam name="T">тип параметра, передаваемого в событии</typeparam>
    public class CancelableEventArgs<T> : EventArgs<T>
    {

        private bool cancel = false;

        public CancelableEventArgs(T Value) : base(Value) { }

        public bool Cancel { get { return cancel; } set { cancel = value; } }

    }

    /// <summary>
    /// Класс аргументов произвольного типа для чтения/записи.
    /// </summary>
    /// <typeparam name="T">тип содержащихся данных</typeparam>
    public class ReadWriteEventArgs<T> : EventArgs<T>
    {

        #region Конструкторы

        /// <summary>
        /// конструктор с данными
        /// </summary>
        /// <param name="Value">аргумент</param>
        public ReadWriteEventArgs(T Value) : base(Value) { }

        #endregion

        #region Методы

        public void SetValue(T Value) { m_value = Value; }

        #endregion

    }

    /// <summary>
    /// Класс аргументов для изменения значении какого-либо типа.
    /// </summary>
    /// <typeparam name="T">тип содержащихся данных</typeparam>
    public class ChangeEventArgs<T> : EventArgs
    {

        #region Поля

        private T val_old;
        private T val_new;

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="OldValue">старое значение</param>
        /// <param name="NewValue">новое значение</param>
        public ChangeEventArgs(T OldValue, T NewValue)
        {
            val_old = OldValue;
            val_new = NewValue;
        }

        #endregion

        #region Свойства

        /// <summary>
        /// получить старое значение
        /// </summary>
        public T OldValue { get { return val_old; } }

        /// <summary>
        /// получить новое значение
        /// </summary>
        public T NewValue { get { return val_new; } }

        #endregion

    }

    public delegate void ChangeDelegate<T>(object sender, ChangeEventArgs<T> args);

    #endregion

}