using System;
using System.Linq;
using InformationCenter.Data;

namespace InformationCenter.Services.ServicesImpl
{

    public class DownloadService : IDisposable
    {
        
        #region ����

        private InformationCenterEngine engine = null;
        private string connectionString = null;

        #endregion

        #region ������������

        public DownloadService(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        #endregion

        #region ��������

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

        #region ������

        public DocumentView Fetch(SearchResultItem Item)
        {
            if (Item == null) throw new ArgumentNullException("Item");
            return Fetch(Item.ID);
        }

        public DocumentView Fetch(Guid ID)
        {
            Document doc = Engine.CreateQuery<Document>().Where(d => d.ID == ID).FirstOrDefault();
            return doc == null ? null : new DocumentView(doc);
        }
        
        public void Dispose()
        {
            if (engine != null)
            {
                engine.Dispose();
                engine = null;
            }
        }

        #endregion

    }

}