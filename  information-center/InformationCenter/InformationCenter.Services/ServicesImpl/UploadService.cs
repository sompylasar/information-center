using System;
using System.IO;
using InformationCenter.Data;

namespace InformationCenter.Services.ServicesImpl
{

    public class UploadService : IDisposable
    {

        #region Поля

        private InformationCenterEngine engine = null;
        private string connectionString = null;
        private static int maxFileSizeInBytes = 100 * 1024 * 1024;

        #endregion

        #region Конструкторы

        public UploadService(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        #endregion

        #region Свойства

        public static int MaxFileSizeInBytes { get { return maxFileSizeInBytes; } }

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

        private Exception ValidateFile(Stream stream, string fileName, string contentType, int contentLength)
        {
            if (stream == null) return new ArgumentNullException("stream");
            if (fileName == null) return new ArgumentNullException("fileName");
            if (contentType == null) return new ArgumentNullException("contentType");
            if (fileName.Trim() == "") return new Exception("Имя файла не указано");
            if (contentLength <= 0) return new Exception("Файл пуст");
            if (!File.Exists(fileName)) return new FileNotFoundException("", fileName);
            if (contentLength > MaxFileSizeInBytes) return new FileSizeOverflowException();
            if (contentType != "text/plain") return new Exception("Тип файла " + contentType + " запрещен к загрузке");
            return null;
        }

        public void Upload(Stream stream, string fileName, string contentType, int contentLength)
        {
            Exception ex = ValidateFile(stream, fileName, contentType, contentLength);
            if (ex != null) throw ex;
            int result = Engine.AddDocument(new ByteBlockReader(stream).ReadToEnd().ToArray());
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