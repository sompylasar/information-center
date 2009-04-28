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
        private static int maxFileSizeInBytes = 100 * 1024 * 1024;

        #endregion

        #region ������������

        public UploadService(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        #endregion

        #region ��������

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

        #region ������

        public TemplateView[] GetTemplates()
        {
            return Array.ConvertAll<Template, TemplateView>(Engine.GetTemplates(), input => new TemplateView(input));
        }

        public FieldView[] GetFieldsOfTemplate(TemplateView TemplateView)
        {
            return Array.ConvertAll<Field, FieldView>(Engine.GetTemplateFields(TemplateView.ID), input => new FieldView(input));
        }

        private Exception ValidateFile(Stream Stream, string FileName, string ContentType, int ContentLength)
        {
            if (Stream == null) return new ArgumentNullException("stream");
            if (FileName == null) return new ArgumentNullException("fileName");
            if (ContentType == null) return new ArgumentNullException("contentType");
            if (FileName.Trim() == "") return new Exception("��� ����� �� �������");
            if (ContentLength <= 0) return new Exception("���� ����");
            if (ContentLength > MaxFileSizeInBytes) return new FileSizeOverflowException(ContentLength, maxFileSizeInBytes, FileName);
            if (ContentType != "text/plain") return new Exception("��� ����� " + ContentType + " �������� � ��������");
            return null;
        }

        public Guid Upload(Stream Stream, string FileName, string contentType, int ContentLength)
        {
            Exception ex = ValidateFile(Stream, FileName, contentType, ContentLength);
            if (ex != null) throw ex;
            return Engine.AddDocument(FileName, new ByteBlockReader(Stream).ReadToEnd().ToArray());
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