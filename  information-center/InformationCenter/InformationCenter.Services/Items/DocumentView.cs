using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public class DocumentView : ViewItem
    {

        #region Конструкторы

        internal DocumentView(Document Doc) : base(Doc) { }

        #endregion

        #region Свойства

        public Guid ID { get { return (entity as Document).ID; } }

        protected Document Document { get { return entity as Document; } }

        #endregion

    }


}