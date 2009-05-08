using System;
using System.Linq;
using InformationCenter.Data;
using System.Collections.Generic;
using LogicUtils;

namespace InformationCenter.Services
{

    /// <summary>
    /// Представление поля.
    /// </summary>
    public class FieldView : ViewItem, IEquatable<FieldView>
    {

        #region Поля

        private FieldTypeView ftView = null;
        private Dictionary<FieldValue, FieldValueView> fields = null;

        #endregion

        #region Конструкторы

        internal FieldView(Field Field) : base(Field) { }

        #endregion

        #region Свойства

        /// <summary>
        /// внутренний объект
        /// </summary>
        internal Field Field { get { return entity as Field; } }

        /// <summary>
        /// уникальный идентификатор
        /// </summary>
        public Guid ID { get { return Field.ID; } }

        /// <summary>
        /// порядковый номер
        /// </summary>
        public int Order { get { return Field.OrderNumber; } }

        /// <summary>
        /// может ли принимать значение null
        /// </summary>
        public bool Nullable { get { return Field.Nullable; } }

        /// <summary>
        /// название поля
        /// </summary>
        public string Name { get { return Field.Text; } }

        /// <summary>
        /// представление типа поля
        /// </summary>
        public FieldTypeView FieldTypeView
        {
            get
            {
                Field.FieldTypeReference.Load();
                if (ftView == null && Field.FieldType != null) ftView = new FieldTypeView(Field.FieldType);
                else if (ftView != null && Field.FieldType == null) ftView = null;
                return ftView;
            }
        }

        /// <summary>
        /// все представления значений полей описания
        /// </summary>
        public FieldValueView[] FieldValues
        {
            get
            {
                if (fields == null) fields = new Dictionary<FieldValue, FieldValueView>();
                Field.FieldValue.Load();
                List<Pair<FieldValue, FieldValueView>> temp = new List<Pair<FieldValue, FieldValueView>>();
                temp.AddRange(fields.Select(d => new Pair<FieldValue, FieldValueView>(d.Key, d.Value)));
                fields.Clear();
                foreach (FieldValue f in Field.FieldValue)
                {
                    Pair<FieldValue, FieldValueView> finded = temp.Find(t => t.Key == f);
                    if (finded == null) fields.Add(f, new FieldValueView(f));
                    else fields.Add(finded.Key, finded.Value);
                }
                return fields.Values.ToArray();
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// получить хеш-код
        /// </summary>
        /// <returns>хеш-код</returns>
        public override int GetHashCode() { return ID.GetHashCode(); }

        public override bool Equals(object obj) { return Equals(obj as FieldView); }      

        public bool Equals(FieldView other) { return object.ReferenceEquals(other, this) || (other != null && other.ID == ID); }

        /// <summary>
        /// преобразовать в строку
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString() { return Name; }

        #endregion

    }

}