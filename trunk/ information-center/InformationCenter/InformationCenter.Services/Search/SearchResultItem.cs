using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public class SearchResultItem
    {

        #region Поля

        private DocDescriptionView desc = null;

        #endregion

        #region Конструкторы

        internal SearchResultItem(DocDescriptionView Description)
        {
            if (Description == null) throw new ArgumentNullException("Description");
            desc = Description;
        }

        #endregion

        #region Свойства

        public DocDescriptionView Description { get { return desc; } }
        
        public DocumentView Document { get { return desc.Document; } }

        #endregion

    }

}