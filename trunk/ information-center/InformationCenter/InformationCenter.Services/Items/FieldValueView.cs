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
                FieldView field = Field;
                if (field == null) throw new Exception("Ошибка");
                FieldTypeView ftv = field.FieldTypeView;
                if (ftv == null) throw new Exception("Ошибка");
                Type fieldType = ftv.TypeOfField;
                string fieldSqlType = ftv.SqlName;
                PropertyInfo valueEntityProp = FieldValue.GetType().GetProperty(fieldSqlType);
                if (valueEntityProp == null) throw new NotSupportedFieldTypeException(fieldType);
                PropertyInfo referenceProp = FieldValue.GetType().GetProperty(fieldSqlType + "Reference");
                ((EntityReference)referenceProp.GetValue(FieldValue, new object[] { })).Load();
                EntityObject valueEntity = (EntityObject)valueEntityProp.GetValue(FieldValue, new object[] {});
                if (valueEntity == null) throw new Exception("Ошибка");
                object v = valueEntity.GetType().GetProperty("Value").GetValue(valueEntity, new object[] { });
                if (v == null && !field.Nullable) throw new NullableValueNotAllowedException(field);
                if (v != null && v.GetType() != fieldType) throw new TypeMismatchException(fieldType, v.GetType());
                return value = v;
            }
        }

        #endregion

    }

}
