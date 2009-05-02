using System;
using System.Linq;
using InformationCenter.Data;

namespace InformationCenter.Services.ServicesImpl
{

    /// <summary>
    /// Сервис для загрузки документов на клиент.
    /// </summary>
    public class DownloadService : IDisposable
    {
        
        #region Поля

        private InformationCenterEngine engine = null;
        private string connectionString = null;

        #endregion

        #region Конструкторы

        public DownloadService(string ConnectionString) { connectionString = ConnectionString; }

        #endregion

        #region Свойства

        public string ConnectionString { get { return connectionString; } }

        protected InformationCenterEngine Engine
        {
            get
            {
                if (engine == null)
                {
                    Exception ex = null;
                    engine = new InformationCenterEngineLoader().Load(ConnectionString, out ex);
                    if (ex != null) throw ex;
                }
                return engine;
            }
        }

        #endregion

        #region Методы

        public DocumentView GetDocument(SearchResultItem Item)
        {
            if (Item == null) throw new ArgumentNullException("Item");
            return GetDocument(Item.Document.ID);
        }

        public DocumentView GetDocument(Guid ID)
        {
            Document doc = Engine.CreateQuery<Document>().Where(d => d.ID == ID).FirstOrDefault();
            return doc == null ? null : new DocumentView(doc);
        }
        
        /// <summary>
        /// освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            if (engine != null)
            {
                engine.Dispose();
                engine = null;
            }
        }

        /// <summary>
        /// преобразовать в строку
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString() { return "Сервис загрузки на клиент."; }

        #endregion

    }

}