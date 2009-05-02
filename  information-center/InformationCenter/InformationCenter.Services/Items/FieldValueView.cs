using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
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
        private object value = null;

        #endregion

        #region Конструкторы

        internal FieldValueView(FieldValue Value) : base(Value) { }

        #endregion

        #region Свойства

        /// <summary>
        /// уникальный идентификатор
        /// </summary>
        public Guid ID { get { return FieldValue.ID; } }

        internal FieldValue FieldValue { get { return entity as FieldValue; } }

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

        public object Value
        {
            get 
            {  
                // TODO: check the code below
                if (value == null)
                {
                    FieldView field = Field;
                    if (field != null)
                    {
                        Type fieldType = field.FieldTypeView.TypeOfField;
                        string fieldSqlType = field.FieldTypeView.SqlName;

                        PropertyInfo valueEntityProp = FieldValue.GetType().GetProperty(fieldSqlType);
                        if (valueEntityProp != null)
                        {
                            PropertyInfo referenceProp = FieldValue.GetType().GetProperty(fieldSqlType + "Reference");
                            if (referenceProp != null)
                            {
                                EntityReference reference =
                                    (EntityReference) referenceProp.GetValue(FieldValue, new object[] {});
                                reference.Load();
                            } // if referenceProp
                            EntityObject valueEntity = (EntityObject) valueEntityProp.GetValue(FieldValue, new object[] {});
                            if (valueEntity != null)
                            {
                                PropertyInfo valueProp = valueEntity.GetType().GetProperty("Value");
                                if (valueProp != null)
                                {
                                    object v = valueProp.GetValue(valueEntity, new object[] {});
                                    if (v.GetType() == fieldType)
                                    {
                                        value = v;
                                    }
                                }
                            } // if obj
                        }
                    }
                }
                return value;
            }
        }

        #endregion

    }

}
