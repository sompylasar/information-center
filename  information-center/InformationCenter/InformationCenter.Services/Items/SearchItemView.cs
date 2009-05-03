using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationCenter.Services
{

    public class SearchItemView
    {

        #region Поля

        private FieldView field;
        private object val;

        #endregion

        #region Конструкторы

        public SearchItemView(FieldView Field, object Value)
        {
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

    }

}
