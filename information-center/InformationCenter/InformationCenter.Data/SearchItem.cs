using System;

namespace InformationCenter.Data
{

    public class SearchItem
    {

        #region Поля

        private Field field;
        private object val;

        #endregion

        #region Конструкторы

        public SearchItem(Field Field, object Value)
        {
            field = Field;
            val = Value;
        }

        #endregion

        #region Свойства

        public Field Field { get { return field; } }

        public object FieldValue
        {
            get { return val; }
            set { val = value; }
        }

        #endregion

    }

}