using System;
using System.Linq;
using InformationCenter.Data;

namespace InformationCenter.Services.ServicesImpl
{

    /// <summary>
    /// ������ ��� �������� ���������� �� ������.
    /// </summary>
    public class DownloadService : IDisposable
    {
        
        #region ����

        private InformationCenterEngine engine = null;
        private string connectionString = null;

        #endregion

        #region ������������

        public DownloadService(string ConnectionString) { connectionString = ConnectionString; }

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

        /// <summary>
        /// �������� ������������� ��������� �� ��� ��������������
        /// </summary>
        /// <param name="ID">������������� ���������</param>
        /// <returns>������������� ���������</returns>
        public DocumentView GetDocument(Guid ID)
        {
            Document doc = Engine.CreateQuery<Document>().Where(d => d.ID == ID).FirstOrDefault();
            return doc == null ? null : new DocumentView(doc);
        }
        
        /// <summary>
        /// ���������� �������
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
        /// ������������� � ������
        /// </summary>
        /// <returns>������</returns>
        public override string ToString() { return "������ �������� �� ������."; }

        #endregion

    }

}