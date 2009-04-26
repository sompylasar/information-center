using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public class SearchResultItem
    {

        private DocDescription desc = null;

        internal SearchResultItem(DocDescription Description)
        {
            if (desc == null) throw new ArgumentNullException("Description");
            desc = Description;
        }

        public Guid ID { get { return desc.ID; } }

        public string Header { get { return desc.Name; } }

    }

}