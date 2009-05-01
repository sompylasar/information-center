using System;
using System.Collections.Generic;
using System.Linq;
using InformationCenter.Data;

namespace InformationCenter.Services.ServicesImpl
{

    public class SearchService : IDisposable
    {

        #region Поля

        private InformationCenterEngine engine = null;
        private string connectionString = null;

        #endregion

        #region Конструкторы

        public SearchService(string ConnectionString)
        {
            connectionString = ConnectionString;
            var e = Engine; // дернуть Engine, чтобы он проверил соединение по указанному connectionString
        }

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

        public SearchResultItem[] Query(SearchRequest Request)
        {
            if (Request == null) throw new ArgumentNullException("Request");

            Exception ex = ValidateRequest(Request);
            if (ex != null) throw ex;

            var results = new List<SearchResultItem>();
            
            // TODO: fetch SearchResultItems from Mapper
            //results.Add(new SearchResultItem(new DocDescription()));

            return results.ToArray();
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