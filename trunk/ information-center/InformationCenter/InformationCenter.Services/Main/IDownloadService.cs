using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationCenter.Services
{

    public interface IDownloadService
    {
        DocumentView GetDocument(Guid ID);
    }

}
