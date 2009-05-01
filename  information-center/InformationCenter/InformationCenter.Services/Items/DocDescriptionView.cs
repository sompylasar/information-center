using System;
using System.Linq;
using InformationCenter.Data;
using System.Collections.Generic;
using LogicUtils;

namespace InformationCenter.Services
{

    /// <summary>
    /// Описание документа.
    /// </summary>
    public class DocDescriptionView : ViewItem
    {

        #region Поля

        private DocumentView docView = null;
        private Dictionary<FieldValue, FieldValueView> fields = null;

        #endregion

        #region Конструкторы

        internal DocDescriptionView(DocDescription Desc) : base(Desc) { }
       
        #endregion

        #region Свойства

        protected DocDescription Description { get { return entity as DocDescription; } }

        /// <summary>
        /// уникальный идентификатор
        /// </summary>
        public Guid ID { get { return Description.ID; } }

        /// <summary>
        /// документ, описываемый объектом
        /// </summary>
        public DocumentView Document
        {
            get
            {
                Description.DocumentReference.Load();
                if (docView == null && Description.Document != null) docView = new DocumentView(Description.Document);
                else if (docView != null && Description.Document == null) docView = null;
                return docView;
            }
        }

        /// <summary>
        /// все поля описания
        /// </summary>
        public FieldValueView[] DescriptionFields
        {
            get
            {
                if (fields == null) fields = new Dictionary<FieldValue, FieldValueView>();
                Description.FieldValue.Load();
                List<Pair<FieldValue, FieldValueView>> temp = new List<Pair<FieldValue, FieldValueView>>();
                temp.AddRange(fields.Select(d => new Pair<FieldValue, FieldValueView>(d.Key, d.Value)));
                fields.Clear();
                foreach (FieldValue f in Description.FieldValue)
                {
                    Pair<FieldValue, FieldValueView> finded = temp.Find(t => t.Key == f);
                    if (finded == null) fields.Add(f, new FieldValueView(f));
                    else fields.Add(finded.Key, finded.Value);
                }
                return fields.Values.ToArray();
            }
        }

        #endregion

    }

}