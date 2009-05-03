using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationCenter.Services
{

    public class SearchItemView : IEquatable<SearchItemView>
    {

        #region Поля

        private FieldView field;
        private object val;

        #endregion

        #region Конструкторы

        public SearchItemView(FieldView Field, object Value)
        {
            if (Field == null) throw new ArgumentNullException("Field");
            field = Field;
            val = Value;
        }

        #endregion

        #region Свойства

        public FieldView Field { get { return field; } }

        public object FieldValue
        {
            get { return val; }
            set { val = value; }
        }

        #endregion

        #region Методы

        /// <summary>
        /// получить хеш-код
        /// </summary>
        /// <returns>хеш-код</returns>
        public override int GetHashCode() { return field.GetHashCode(); }

        public override bool Equals(object obj) { return Equals(obj as SearchItemView); }

        public bool Equals(SearchItemView other) { return object.ReferenceEquals(other, this) || (other != null && other.Field == field); }

        /// <summary>
        /// преобразовать в строку
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString() { return string.Format("Поле '{0}' == {1}", field, val == null ? "null" : val); }

        #endregion

    }

}
