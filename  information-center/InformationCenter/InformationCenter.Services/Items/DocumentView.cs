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
    public class DocumentView : ViewItem
    {

        #region Конструкторы

        internal DocumentView(Document Doc) : base(Doc) { }
        private Dictionary<DocDescription, DocDescriptionView> descriptions = null;

        #endregion

        #region Свойства

        /// <summary>
        /// уникальный идентификатор
        /// </summary>
        public Guid ID { get { return (entity as Document).ID; } }

        protected Document Document { get { return entity as Document; } }

        /// <summary>
        /// название - имя
        /// </summary>
        public string Name { get { return Document.FileName; } }

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

    }


}