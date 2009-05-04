using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using System.Text;
using InformationCenter.Data;
using System.Data.Objects;

namespace InformationCenter.Services
{

    /// <summary>
    /// Представление значения поля.
    /// </summary>
    public class FieldValueView : ViewItem, IEquatable<FieldValueView>
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

                EntityObject entity = GetCurrentEntity(field, ftv, fieldType);
                object v = entity.GetType().GetProperty("Value").GetValue(entity, new object[] { });
                
                if (v == null && !field.Nullable) throw new NullableValueNotAllowedException(field);
                if (v != null && v.GetType() != fieldType) throw new TypeMismatchException(fieldType, v.GetType());
                return value = v;
            }
            set
            {
                FieldView field = Field;
                if (field == null) throw new Exception("Ошибка");
                FieldTypeView ftv = field.FieldTypeView;
                if (ftv == null) throw new Exception("Ошибка");
                Type fieldType = ftv.TypeOfField;
                
                if (value == null && !field.Nullable) throw new NullableValueNotAllowedException(field);
                if (value != null && value.GetType() != fieldType) throw new TypeMismatchException(fieldType, value.GetType());
                
                EntityObject entity = GetCurrentEntity(field, ftv, fieldType);
                entity.GetType().GetProperty("Value").SetValue(entity, value, new object[] { });
                
                ((ObjectQuery)((IEntityWithRelationships)this).RelationshipManager.GetAllRelatedEnds().First().CreateSourceQuery()).Context.
                    Refresh(System.Data.Objects.RefreshMode.ClientWins, entity);
            }
        }

        #endregion

        #region Методы

        private EntityObject GetCurrentEntity(FieldView field, FieldTypeView ftv, Type fieldType)
        {
            string fieldSqlType = ftv.SqlName;
            PropertyInfo valueEntityProp = FieldValue.GetType().GetProperty(fieldSqlType);
            if (valueEntityProp == null) throw new NotSupportedFieldTypeException(fieldType);
            PropertyInfo referenceProp = FieldValue.GetType().GetProperty(fieldSqlType + "Reference");
            ((EntityReference)referenceProp.GetValue(FieldValue, new object[] { })).Load();
            EntityObject valueEntity = (EntityObject)valueEntityProp.GetValue(FieldValue, new object[] { });
            if (valueEntity == null) throw new Exception("Ошибка");
            return valueEntity;    
        }

        /// <summary>
        /// получить хеш-код
        /// </summary>
        /// <returns>хеш-код</returns>
        public override int GetHashCode() { return ID.GetHashCode(); }

        public override bool Equals(object obj) { return Equals(obj as FieldValueView); }

        public bool Equals(FieldValueView other) { return object.ReferenceEquals(other, this) || (other != null && other.ID == ID); }

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
