﻿using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public class FieldTypeView : ViewItem
    {

        #region Поля

        #endregion

        #region Конструкторы

        internal FieldTypeView(FieldType FieldType) : base(FieldType) { }

        #endregion

        #region Свойства

        protected FieldType FieldType { get { return entity as FieldType; } }

        public Guid ID { get { return FieldType.ID; } }

        public string SqlName { get { return FieldType.SqlName; } }

        public Type TypeOfField
        {
            get
            {
                Type t = Type.GetType(FieldType.DotNetType);
                if (t == null) throw new DotNetTypeNotExistsException(FieldType.DotNetType);
                return t;
            }
        }

        public string FieldTypeName { get { return FieldType.Name; } }

        #endregion

    }

}