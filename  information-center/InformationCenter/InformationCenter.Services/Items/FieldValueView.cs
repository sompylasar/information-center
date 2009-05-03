﻿using System;
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
        private DocDescriptionView desc = null;
        private object value = null;

        #endregion

        #region Конструкторы

        internal FieldValueView(FieldValue Value) : base(Value) { }

        #endregion

        #region Свойства

        /// <summary>
        /// внутренний объект
        /// </summary>
        internal FieldValue FieldValue { get { return entity as FieldValue; } }

        /// <summary>
        /// уникальный идентификатор
        /// </summary>
        public Guid ID { get { return FieldValue.ID; } }
        
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

        /// <summary>
        /// представление описания документа
        /// </summary>
        public DocDescriptionView DocDescription
        {
            get
            {
                FieldValue.DocDescriptionReference.Load();
                if (desc == null && FieldValue.DocDescription != null) desc = new DocDescriptionView(FieldValue.DocDescription);
                else if (desc != null && FieldValue.DocDescription == null) desc = null;
                return desc;
            }
        }

        /// <summary>
        /// значение поля
        /// </summary>
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

        #region Методы

        /// <summary>
        /// преобразовать в строку
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString()
        {
            object v = Value; 
            return v == null ? "null" : v.ToString();
        }

        #endregion

    }

}