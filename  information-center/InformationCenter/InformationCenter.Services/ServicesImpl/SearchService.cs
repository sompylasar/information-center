using System;
using System.Linq;
using InformationCenter.Data;

namespace InformationCenter.Services.ServicesImpl
{

    public class SearchService : IDisposable
    {

        #region ����

        private InformationCenterEngine engine = null;
        private string connectionString = null;

        #endregion

        #region ������������

        public SearchService(string ConnectionString)
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

        public FieldView[] GetFields()
        {
            return Array.ConvertAll<Field, FieldView>(Engine.GetFields(), input => new FieldView(input));
        }

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

            var result = new SearchResultItem[1];
            
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