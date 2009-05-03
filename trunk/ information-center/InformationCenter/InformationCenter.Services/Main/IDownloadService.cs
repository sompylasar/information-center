using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationCenter.Services
{

    /// <summary>
    /// Интерфейс сервиса для скачивания документов (download).
    /// </summary>
    public interface IDownloadService
    {

        /// <summary>
        /// получить представление документа по его идентификатору
        /// </summary>
        /// <param name="ID">идентификатор документа в хранилище</param>
        /// <returns>представление документа</returns>
        DocumentView GetDocument(Guid ID);

    }

}
