using System;
using System.Collections.Generic;
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

        public UploadService(string ConnectionString) { connectionString = ConnectionString; }

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

        public TemplateView[] GetTemplates()
        {
            return Array.ConvertAll<Template, TemplateView>(Engine.GetTemplates(), input => new TemplateView(input));
        }

        public FieldView[] GetFieldsOfTemplate(TemplateView TemplateView)
        {
            return Array.ConvertAll<Field, FieldView>(Engine.GetTemplateFields(TemplateView.ID), input => new FieldView(input));
        }

        public void RemoveFieldFromTemplate(TemplateView TemplateView, FieldView FieldView)
        {
            Engine.RemoveFieldFromTemplate(TemplateView.ID, FieldView.ID);
        }

        private Exception ValidateFile(Stream Stream, string FileName, string ContentType, int ContentLength)
        {
            if (Stream == null) return new ArgumentNullException("stream");
            if (FileName == null) return new ArgumentNullException("fileName");
            //if (ContentType == null) return new ArgumentNullException("contentType");
            if (FileName.Trim() == "") return new Exception("Имя файла не указано.");
            if (ContentLength <= 0) return new Exception("Файл пуст.");
            if (ContentLength > MaxFileSizeInBytes) return new FileSizeOverflowException(ContentLength, maxFileSizeInBytes, FileName);
            //if (ContentType != "text/plain") return new Exception("Тип файла " + ContentType + " запрещен к загрузке.");
            return null;
        }

        public Guid Upload(Stream Stream, string FileName, string contentType, int ContentLength)
        {
            Exception ex = ValidateFile(Stream, FileName, contentType, ContentLength);
            if (ex != null) throw ex;
            return Engine.AddDocument(FileName, new ByteBlockReader(Stream).ReadToEnd().ToArray());
        }

        public Guid AddDescription(Guid DocumentID, string Name, Dictionary<FieldView, object> FieldsWithValues)
        {
            // TODO: validate document ID
            // TODO: validate types of values

            Dictionary<Field, object> fields = new Dictionary<Field, object>();
            foreach (KeyValuePair<FieldView, object> valuePair in FieldsWithValues)
            {
                fields.Add(valuePair.Key.Field, valuePair.Value);
            }

            Guid descriptionId = Engine.AddDocumentDescription(Name, DocumentID, fields);

            return descriptionId;
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