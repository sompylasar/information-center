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
        /// уникальный идентификатор
        /// </summary>
        public Guid ID { get { return Field.ID; } }

        protected Field Field { get { return entity as Field; } }

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

        public override int GetHashCode() { return ID.GetHashCode(); }

        public override bool Equals(object obj) { return Equals(obj as FieldView); }      

        public bool Equals(FieldView other) { return object.ReferenceEquals(other, this) || (other != null && other.ID == ID); }

        #endregion

    }

}