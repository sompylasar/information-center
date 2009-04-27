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

        protected Document Document { get { return entity as Document; } }

        #endregion

    }


}