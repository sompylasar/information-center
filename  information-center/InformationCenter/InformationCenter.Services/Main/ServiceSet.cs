using System;
using System.Linq;
using InformationCenter.Data;
using System.Collections.Generic;
using System.IO;
using System.Data.Objects;

namespace InformationCenter.Services
{

    /// <summary>
    /// Сервис для загрузки документов на клиент.
    /// </summary>
    public class ServiceSet : IDownloadService, IUploadService, ISearchService, IDocumentDescriptionService, IDisposable
    {
        
        #region Поля

        private string connectionString = null;
        private InformationCenterEngine engine = null;
        private static int maxFileSizeInBytes = 100 * 1024 * 1024;

        #endregion

        #region Конструкторы

        public ServiceSet(string ConnectionString) { connectionString = ConnectionString; }

        #endregion

        #region Свойства

        /// <summary>
        /// максимально возможный размер загружаемого на сервер файла в байтах
        /// </summary>
        public int MaxFileSizeInBytes { get { return maxFileSizeInBytes; } }

        /// <summary>
        /// строка соединения
        /// </summary>
        public string ConnectionString { get { return connectionString; } }

        /// <summary>
        /// движок для доступа к данным
        /// </summary>
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

        #region Private
        
        private Exception ValidateRequest(SearchRequest Request)
        {
            foreach (SearchItem item in Request.Items)
            {
                if (item == null) return new ItemIsNullException(Request.Items, Request.Items.IndexOf(item));
                //Field value = Engine.CreateQuery<Field>().Where(f => f.ID == item.FieldID).FirstOrDefault();
                //if (value == null) return new FieldNotFoundException(item.FieldID);
                item.Field.FieldTypeReference.Load();
                Type t = Type.GetType(item.Field.FieldType.DotNetType);
                if (t == null) return new DotNetTypeNotExistsException(item.Field.FieldType.DotNetType);
                if (item.FieldValue == null)
                {
                    if (!item.Field.Nullable)
                        return new NullableValueNotAllowedException(new FieldView(item.Field));
                }
                else
                {
                    Type realType = item.FieldValue.GetType();
                    if (realType != t) return new TypeMismatchException(t, realType);
                }
            }
            return null;
        }

        private Exception ValidateFile(Stream Stream, string FileName, string ContentType, int ContentLength)
        {
            if (Stream == null) return new ArgumentNullException("stream");
            if (FileName == null) return new ArgumentNullException("fileName");
            if (FileName.Trim() == "") return new Exception("Имя файла не указано.");
            if (ContentLength <= 0) return new Exception("Файл пуст.");
            if (ContentLength > MaxFileSizeInBytes) return new FileSizeOverflowException(ContentLength, maxFileSizeInBytes, FileName);
            //if (ContentType != "text/plain") return new Exception("Тип файла " + ContentType + " запрещен к загрузке.");
            return null;
        }

        #endregion

        #region Get

        /// <summary>
        /// возвращает представление документа по его идентификатору
        /// </summary>
        /// <param name="ID">идентификатор документа</param>
        /// <returns>представление документа</returns>
        public DocumentView GetDocument(Guid ID)
        {
            Document doc = Engine.GetDocument(ID);
            return doc == null ? null : new DocumentView(doc);
        }

        /// <summary>
        /// получить все типы полей
        /// </summary>
        /// <returns>массив представлений типов полей</returns>
        public FieldTypeView[] GetFieldTypes()
        {
            return Array.ConvertAll<FieldType, FieldTypeView>(Engine.GetFieldTypes(), input => new FieldTypeView(input));
        }

        public string[] GetFileNames(bool WithExtensions) { return Engine.GetFileNames(WithExtensions); }

        public FieldView[] GetFields()
        {
            return Array.ConvertAll<Field, FieldView>(Engine.GetFields(), input => new FieldView(input));
        }

        public object GetValue(FieldValueView Value) { return Value == null ? null : Engine.GetFieldValue(Value.ID); }

        public object[] GetValuesOfField(FieldView FieldView) { return Engine.GetFieldValues(FieldView.ID); }

        public TemplateView[] GetTemplates()
        {
            return Array.ConvertAll<Template, TemplateView>(Engine.GetTemplates(), input => new TemplateView(input));
        }

        public FieldView[] GetFieldsOfTemplate(TemplateView TemplateView)
        {
            return Array.ConvertAll<Field, FieldView>(Engine.GetTemplateFields(TemplateView.ID), input => new FieldView(input));
        }

        /// <summary>
        /// поиск документов
        /// </summary>
        /// <param name="Request">запрос с параметрами и условиями поиска</param>
        /// <returns>коллекция результатов поиска - массив описаний документов</returns>
        public DocDescriptionView[] Query(SearchRequestView Request)
        {
            if (Request == null) throw new ArgumentNullException("Request");
            SearchRequest inner = Request.ToInnerRequest();
            Exception ex = ValidateRequest(inner);
            if (ex != null) throw ex;
            return Engine.SearchDocDescription(inner).Select(d => new DocDescriptionView(d)).ToArray();

            /*
            var results = new List<SearchResultItem>();
            var requestFields = new Dictionary<Guid, SearchItem>();
            foreach (SearchItem searchItem in Request.Items)
            {
                requestFields.Add(searchItem.FieldID, searchItem);
            }
            Document[] documents = Engine.GetDocuments();
            foreach (Document document in documents)
            {
                document.DocDescription.Load();
                DocDescription[] descriptions = document.DocDescription.ToArray();
                foreach (DocDescription description in descriptions)
                {
                    var descriptionView = new DocDescriptionView(description);
                    FieldValueView[] fields = descriptionView.DescriptionFields;

                    bool interested = false;
                    bool relevant = (Request.Mode == SearchMode.And ? true : false);
                    foreach (FieldValueView valueView in fields)
                    {
                        Guid fieldId = valueView.Field.ID;
                        if (!requestFields.ContainsKey(fieldId)) continue;

                        bool matchField = AreValuesEqual(requestFields[fieldId].FieldValue, valueView.Value);

                        interested = true;
                        relevant = (Request.Mode == SearchMode.And ? relevant && matchField : relevant || matchField);
                    }
                    if (interested && relevant)
                    {
                        results.Add(new SearchResultItem(descriptionView));
                    }
                } // foreach descriptions
            } // foreach documents


            return results.ToArray();
             */
        }

        #endregion

        #region Add

        public void AddField(string Name, FieldTypeView Type, bool Nullable, int Order)
        {
            Engine.AddField(Name, Type.ID, Nullable, Order);
        }

        public bool AddTemplate(string Name, IEnumerable<FieldView> FieldViews)
        {
            return Engine.AddTemplate(Name, FieldViews.Select(fv => fv.Field));
        }

        public bool AddDescription(Guid DocumentID, string Name, Dictionary<FieldView, object> FieldsWithValues)
        {
            if (FieldsWithValues == null) throw new ArgumentNullException("FieldsWithValues");
            if (Engine.CreateQuery<Document>().Where(d => d.ID == DocumentID).FirstOrDefault() == null)
                throw new DocumentNotFoundException(DocumentID);
            Dictionary<Field, object> param = new Dictionary<Field, object>();
            foreach (var pair in FieldsWithValues) param.Add(pair.Key.Field, pair.Value);
            return Engine.AddDocumentDescription(Name, DocumentID, param);
        }

        public void AddFieldToTemplate(Guid TemplateID, Guid FieldID)
        {
            Engine.AddFieldToTemplate(TemplateID, FieldID);
        }

        #endregion

        #region Remove/Delete

        public void RemoveFieldFromTemplate(TemplateView TemplateView, FieldView FieldView)
        {
            Engine.RemoveFieldFromTemplate(TemplateView.ID, FieldView.ID);
        } 

        public void DeleteTemplate(TemplateView Template)
        {
            Engine.DeleteTemplate(Template.ID);
            Engine.Refresh(RefreshMode.StoreWins, Template.Template);
        }

        public void DeleteField(FieldView Field)
        {
            Engine.DeleteField(Field.ID);
            Engine.Refresh(RefreshMode.StoreWins, Field.Field);
        }

        #endregion

        #region Modify

        public void RenameField(FieldView Field, string NewName)
        {
            Engine.RenameField(Field.ID, NewName);
            Engine.Refresh(RefreshMode.StoreWins, Field.Field);
        }

        public void RenameTemplate(TemplateView Template, string NewName)
        {
            Engine.RenameTemplate(Template.ID, NewName);
            Engine.Refresh(RefreshMode.StoreWins, Template.Template);
        }

        #endregion

        #region Other

        public Guid Upload(Stream Stream, string FileName, string contentType, int ContentLength)
        {
            Exception ex = ValidateFile(Stream, FileName, contentType, ContentLength);
            if (ex != null) throw ex;
            return Engine.AddDocument(FileName, new ByteBlockReader(Stream).ReadToEnd().ToArray());
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

        #endregion

    }

}