using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    /// <summary>
    /// Представление значения поля.
    /// </summary>
    public class FieldValueView : ViewItem
    {

        #region Поля

        private FieldView fv = null;

        #endregion

        #region Конструкторы

        internal FieldValueView(FieldValue Value) : base(Value) { }

        #endregion

        #region Свойства

        /// <summary>
        /// уникальный идентификатор
        /// </summary>
        public Guid ID { get { return FieldValue.ID; } }

        protected FieldValue FieldValue { get { return entity as FieldValue; } }

        /// <summary>
        /// описание поля
        /// </summary>
        public FieldView Field
        {
            get
            {
                FieldValue.FieldReference.Load();
                if (fv == null && FieldValue.Field != null) fv = new FieldView(FieldValue.Field);
                else if (fv != null && FieldValue.Field == null) fv = null;
                return fv;
            }
        }

        #endregion

    }

}
