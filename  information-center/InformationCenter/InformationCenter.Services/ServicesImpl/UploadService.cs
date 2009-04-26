using System;
using System.IO;
using InformationCenter.Data;

namespace InformationCenter.Services.ServicesImpl
{

    public class UploadService : IDisposable
    {

        #region ����

        private InformationCenterEngine engine = null;
        private string connectionString = null;

        #endregion

        public UploadService(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

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

        public void ValidateFile(Stream stream, string fileName, string contentType, int contentLength)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (contentType == null) throw new ArgumentNullException("contentType");

            if (fileName.Trim() == "")
                throw new Exception("��� ����� �� �������");
            if (contentLength <= 0)
                throw new Exception("���� ����");
            if (contentType != "text/plain")
                throw new Exception("��� ����� " + contentType + " �������� � ��������");
        }

        public void SaveFile(Stream stream, string fileName, string contentType, int contentLength)
        {
            ValidateFile(stream, fileName, contentType, contentLength);

            // TODO: save file
        }


        #region IDisposable Members
        
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