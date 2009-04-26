using System;
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

        private Exception ValidateRequest(SearchRequest Request)
        {
            foreach (SearchItem item in Request.Items)
            {
                if (item == null) return new ItemIsNullException();
                Field value = Engine.CreateQuery<Field>().Where(f => f.ID == item.FieldID).FirstOrDefault();
                if (value == null) return new FieldNotFoundException();
                value.FieldTypeReference.Load();
                Type t = Type.GetType(value.FieldType.DotNetType);
                if (t == null) return new DotNetTypeNotFoundException();
                if (item.FieldValue == null)
                {
                    if (!value.FieldType.Nullable)
                        return new NullableValueNotAllowedException();
                }
                else
                {
                    if (item.FieldValue.GetType() != t)
                        return new TypeMismatchException();
                }
            }
            return null;
        }

        public SearchResult Query(SearchRequest Request)
        {
            if (Request == null) throw new ArgumentNullException("Request");

            Exception ex = ValidateRequest(Request);
            if (ex != null) throw ex;

            var result = new SearchResult();
            
            // TODO: fetch SearchResults or SearchResultItems from Mapper
            //result.Items.Add(new SearchResultItem(1, "A", "clickme1"));
            //result.Items.Add(new SearchResultItem(2, "BC", "clickme2"));
            //result.Items.Add(new SearchResultItem(3, "CD", "clickme3"));
            //result.Items.Add(new SearchResultItem(4, "D", "clickme4"));

            //var search = ((string)(request.Fields["Title"] ?? "")).Trim().ToLowerInvariant();

            //result.Items.RemoveAll(x => 
            //                       search == "" || !x.Header.ToLowerInvariant().Contains(search)
            //    );

            return result;
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