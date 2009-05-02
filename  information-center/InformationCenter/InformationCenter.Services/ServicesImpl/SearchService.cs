using System;
using System.Collections.Generic;
using System.Linq;
using InformationCenter.Data;

namespace InformationCenter.Services.ServicesImpl
{

    /// <summary>
    /// Сервис для поиска документов.
    /// </summary>
    public class SearchService : IDisposable
    {

        #region Поля

        private InformationCenterEngine engine = null;
        private string connectionString = null;

        #endregion

        #region Конструкторы

        public SearchService(string ConnectionString) { connectionString = ConnectionString; }

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

        public string[] GetFileNames(bool WithExtensions) { return Engine.GetFileNames(WithExtensions); }

        public FieldView[] GetFields()
        {
            return Array.ConvertAll<Field, FieldView>(Engine.GetFields(), input => new FieldView(input));
        }

        public object GetValue(FieldValueView Value) { return Value == null ? null : Engine.GetFieldValue(Value.ID); }

        public object[] GetValuesOfField(FieldView FieldView) { return Engine.GetFieldValues(FieldView.ID); }

        #endregion

        private Exception ValidateRequest(SearchRequest Request)
        {
            foreach (SearchItem item in Request.Items)
            {
                if (item == null) return new ItemIsNullException(Request.Items, Request.Items.IndexOf(item));
                Field value = Engine.CreateQuery<Field>().Where(f => f.ID == item.FieldID).FirstOrDefault();
                if (value == null) return new FieldNotFoundException(item.FieldID);
                value.FieldTypeReference.Load();
                Type t = Type.GetType(value.FieldType.DotNetType);
                if (t == null) return new DotNetTypeNotExistsException(value.FieldType.DotNetType);
                if (item.FieldValue == null)
                {
                    if (!value.Nullable)
                        return new NullableValueNotAllowedException(new FieldView(value));
                }
                else
                {
                    Type realType = item.FieldValue.GetType();
                    if (realType != t) return new TypeMismatchException(t, realType);
                }
            }
            return null;
        }

        /// <summary>
        /// поиск документов
        /// </summary>
        /// <param name="Request">запрос с параметрами и условиями поиска</param>
        /// <returns>коллекция результатов поиска - массив описаний документов</returns>
        public DocDescriptionView[] Query(SearchRequest Request)
        {
            if (Request == null) throw new ArgumentNullException("Request");
            Exception ex = ValidateRequest(Request);
            if (ex != null) throw ex;
            return Engine.SearchDocDescription(Request).Select(d => new DocDescriptionView(d)).ToArray();

            /*
            var results = new List<SearchResultItem>();
            
            // TODO: fetch SearchResultItems from Mapper
            //results.Add(new SearchResultItem(new DocDescription()));

            // HACK: search implemented in middle layer
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

       // private bool AreValuesEqual(object requestValue, object storedValue)
       // {
            // TODO: properly test values for equality
        //    return (requestValue.ToString().Trim() == storedValue.ToString().Trim());
        //}

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
        public override string ToString() { return "Сервис поиска"; }

        #endregion

    }

}