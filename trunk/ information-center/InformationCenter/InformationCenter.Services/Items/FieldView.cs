using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    /// <summary>
    /// Представление поля.
    /// </summary>
    public class FieldView : ViewItem, IEquatable<FieldView>
    {

        #region Поля

        private FieldTypeView ftView = null;

        #endregion

        #region Конструкторы

        internal FieldView(Field Field) : base(Field) { }

        #endregion

        #region Свойства

        /// <summary>
        /// внутренний объект
        /// </summary>
        internal Field Field { get { return entity as Field; } }

        /// <summary>
        /// уникальный идентификатор
        /// </summary>
        public Guid ID { get { return Field.ID; } }

        /// <summary>
        /// порядковый номер
        /// </summary>
        public int Order { get { return Field.OrderNumber; } }

        /// <summary>
        /// может ли принимать значение null
        /// </summary>
        public bool Nullable { get { return Field.Nullable; } }

        /// <summary>
        /// название поля
        /// </summary>
        public string Name { get { return Field.Text; } }

        /// <summary>
        /// представление типа поля
        /// </summary>
        public FieldTypeView FieldTypeView
        {
            get
            {
                Field.FieldTypeReference.Load();
                if (ftView == null && Field.FieldType != null) ftView = new FieldTypeView(Field.FieldType);
                else if (ftView != null && Field.FieldType == null) ftView = null;
                return ftView;
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// получить хеш-код
        /// </summary>
        /// <returns>хеш-код</returns>
        public override int GetHashCode() { return ID.GetHashCode(); }

        public override bool Equals(object obj) { return Equals(obj as FieldView); }      

        public bool Equals(FieldView other) { return object.ReferenceEquals(other, this) || (other != null && other.ID == ID); }

        /// <summary>
        /// преобразовать в строку
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString() { return Name; }

        #endregion

    }

}