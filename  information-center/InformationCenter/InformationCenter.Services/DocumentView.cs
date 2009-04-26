using System;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public class DocumentView
    {

        private Document doc = null;

        internal DocumentView(Document Doc)
        {
            if (Doc == null) throw new ArgumentNullException("Doc");
            doc = Doc;
        }

    }


}