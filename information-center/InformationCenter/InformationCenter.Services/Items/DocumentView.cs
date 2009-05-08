using System;
using System.Linq;
using InformationCenter.Data;
using System.Collections.Generic;
using LogicUtils;

namespace InformationCenter.Services
{

    /// <summary>
    /// Представление документа.
    /// </summary>
    public class DocumentView : ViewItem, IEquatable<DocumentView>
    {

        #region Поля

        private Dictionary<DocDescription, DocDescriptionView> descriptions = null;

        #endregion

        #region Конструкторы

        internal DocumentView(Document Doc) : base(Doc) { }

        #endregion

        #region Свойства

        /// <summary>
        /// внутренний объект документа
        /// </summary>
        internal Document Document { get { return entity as Document; } }

        /// <summary>
        /// уникальный идентификатор
        /// </summary>
        public Guid ID { get { return Document.ID; } }

        /// <summary>
        /// название - имя
        /// </summary>
        public string FileName { get { return Document.FileName; } }

        /// <summary>
        /// данные документа
        /// </summary>
        public byte[] BinaryData { get { return Document.Data; } }

        /// <summary>
        /// все описания документа
        /// </summary>
        public DocDescriptionView[] Descriptions
        {
            get
            {
                if (descriptions == null) descriptions = new Dictionary<DocDescription, DocDescriptionView>();
                Document.DocDescription.Load();
                List<Pair<DocDescription, DocDescriptionView>> temp = new List<Pair<DocDescription,DocDescriptionView>>();
                temp.AddRange(descriptions.Select(d => new Pair<DocDescription, DocDescriptionView>(d.Key, d.Value)));
                descriptions.Clear();
                foreach (DocDescription doc in Document.DocDescription)
                {
                    Pair<DocDescription, DocDescriptionView> finded = temp.Find(t => t.Key == doc);
                    if (finded == null) descriptions.Add(doc, new DocDescriptionView(doc));
                    else descriptions.Add(finded.Key, finded.Value);
                }
                return descriptions.Values.ToArray();
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// получить хеш-код
        /// </summary>
        /// <returns>хеш-код</returns>
        public override int GetHashCode() { return ID.GetHashCode(); }

        public override bool Equals(object obj) { return Equals(obj as DocumentView); }

        public bool Equals(DocumentView other) { return object.ReferenceEquals(other, this) || (other != null && other.ID == ID); }

        /// <summary>
        /// преобразовать в строку
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString() { return FileName; }

        #endregion

    }

}