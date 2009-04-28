using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public class FieldView : ViewItem
    {

        #region Поля

        private FieldTypeView ftView = null;
        
        #endregion

        #region Конструкторы

        internal FieldView(Field Field) : base(Field) { }

        #endregion

        #region Свойства

        public Guid ID { get { return Field.ID; } }

        protected Field Field { get { return entity as Field; } }

        public int Order { get { return Field.OrderNumber; } }

        public bool Nullable { get { return Field.Nullable; } }

        public string Name { get { return Field.Text; } }

        public FieldTypeView FieldTypeView
        {
            get
            {
                if (ftView == null)
                {
                    if (!Field.FieldTypeReference.IsLoaded) Field.FieldTypeReference.Load();
                    ftView = new FieldTypeView(Field.FieldType);
                }
                return ftView;
            }
        }

        #endregion

        #region Методы

        public object[] GetSourceValues()
        {
            return new object[] { };
        }

        #endregion

    }

}