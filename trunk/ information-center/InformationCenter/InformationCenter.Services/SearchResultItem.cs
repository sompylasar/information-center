using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public class SearchResultItem
    {

        #region Поля

        private DocDescription desc = null;

        #endregion

        #region Конструкторы

        internal SearchResultItem(DocDescription Description)
        {
            if (desc == null) throw new ArgumentNullException("Description");
            desc = Description;
        }

        #endregion

        #region Свойства

        public Guid ID { get { return desc.ID; } }

        public string Header { get { return desc.Name; } }

        #endregion

    }

}