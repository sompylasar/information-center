using System;

namespace InformationCenter.Services
{

    public class SearchItem
    {

        #region Поля

        private Guid id;
        private object val;

        #endregion

        #region Конструкторы

        public SearchItem(Guid FieldID, object Value)
        {
            id = FieldID;
            val = Value;
        }

        #endregion

        #region Свойства

        public Guid FieldID { get { return id; } }

        public object FieldValue
        {
            get { return val; }
            set { val = value; }
        }

        #endregion

    }

}