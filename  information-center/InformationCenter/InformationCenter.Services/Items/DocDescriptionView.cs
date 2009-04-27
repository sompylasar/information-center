using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public class DocDescriptionView : ViewItem
    {

        #region Конструкторы

        internal DocDescriptionView(DocDescription Desc) : base(Desc) { }

        #endregion

        #region Свойства

        protected DocDescription Description { get { return entity as DocDescription; } }

        #endregion

    }

}