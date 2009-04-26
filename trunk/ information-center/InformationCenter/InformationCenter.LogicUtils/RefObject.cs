using System;

namespace LogicUtils
{

    /// <summary>
    /// Класс-упаковщик.
    /// </summary>
    /// <typeparam name="T">тип данных для упаковки</typeparam>
    public class RefObject<T>
    {

        #region Конструкторы

        /// <summary>
        /// конструктор по умолчанию
        /// </summary>
        public RefObject() { }

        /// <summary>
        /// конструктор с параметром для упаковки
        /// </summary>
        /// <param name="value">чего упаковывать</param>
        public RefObject(T value) { Value = value; }

        #endregion

        #region Свойства

        /// <summary>
        /// получить/установить упакованные данные
        /// </summary>
        public T Value { get; set; }

        #endregion

    }

}