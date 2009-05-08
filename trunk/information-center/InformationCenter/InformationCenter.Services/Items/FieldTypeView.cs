using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    /// <summary>
    /// Представление типа поля.
    /// </summary>
    public class FieldTypeView : ViewItem, IEquatable<FieldTypeView>
    {

        #region Поля

        #endregion

        #region Конструкторы

        internal FieldTypeView(FieldType FieldType) : base(FieldType) { }

        #endregion

        #region Свойства

        /// <summary>
        /// внутренний объект
        /// </summary>
        internal FieldType FieldType { get { return entity as FieldType; } }

        /// <summary>
        /// по этому свойству (полю в БД) осуществляется завязка на конкретную таблицу
        /// </summary>
        internal string SqlName { get { return FieldType.SqlName; } }

        internal string SqlType { get { return FieldType.SqlType; } }

        /// <summary>
        /// уникальный идентификатор
        /// </summary>
        public Guid ID { get { return FieldType.ID; } }

        /// <summary>
        /// тип поля в терминах классов .NET
        /// </summary>
        public Type TypeOfField
        {
            get
            {
                Type t = Type.GetType(FieldType.DotNetType);
                if (t == null) throw new DotNetTypeNotExistsException(FieldType.DotNetType);
                return t;
            }
        }

        /// <summary>
        /// строковое представление типа поля
        /// </summary>
        public string FieldTypeName { get { return FieldType.Name; } }

        #endregion

        #region Методы

        /// <summary>
        /// получить хеш-код
        /// </summary>
        /// <returns>хеш-код</returns>
        public override int GetHashCode() { return ID.GetHashCode(); }

        public override bool Equals(object obj) { return Equals(obj as FieldTypeView); }

        public bool Equals(FieldTypeView other) { return object.ReferenceEquals(other, this) || (other != null && other.ID == ID); }

        /// <summary>
        /// преобразовать в строку
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString() { return FieldTypeName; }

        #endregion

    }

}