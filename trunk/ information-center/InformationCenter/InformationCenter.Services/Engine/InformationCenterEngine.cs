using System;
using System.Linq;
using System.Data;
using InformationCenter.EFEngine;
using InformationCenter.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Collections.Generic;
using InformationCenter.DBUtils;

namespace InformationCenter.Services
{

    /// <summary>
    /// Движок для доступа к объектам хранилища.
    /// </summary>
    public class InformationCenterEngine : EF_Engine, IConnectionStringProvider
    {

        #region Поля

        private User current = null;
        private DBWorker worker = null;
        private string storedConnection = null;

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="ConnectionString">строка подключения</param>
        public InformationCenterEngine(string ConnectionString) : base(new Entities(ConnectionString))
        {
            storedConnection = ConnectionString;
        }

        public InformationCenterEngine(Entities Entities) : base(Entities) { }

        public InformationCenterEngine(Entities Entities, string ConnectionString) : this(Entities)
        {
            storedConnection = ConnectionString;
        }

        #endregion

        #region Свойства

        protected Entities Entities { get { return Context as Entities; } }

        string IConnectionStringProvider.ConnectionString { get { return storedConnection; } }

        protected DBWorker Worker
        {
            get
            {
                if (worker == null) worker = new DBWorker(CurrentConnection.ConnectionString);
                return worker;
            }
        }

        /// <summary>
        /// используемое подключение к источнику данных
        /// </summary>
        public IDbConnection CurrentConnection { get { return Context.Connection; } }

        /// <summary>
        /// получить текущего пользователя
        /// </summary>
        public User Current
        {
            get
            {
                if (current == null) current = new User();
                return current;
            }
        }

        #endregion

        #region Методы

        #region Add

        /// <summary>
        /// добавить документ
        /// </summary>
        /// <param name="FileName">название документа (имя файла документа)</param>
        /// <param name="Data">данные документа</param>
        /// <returns>идентификатор документа</returns>
        /// <exception cref="">в случае провала генерирует исключение</exception>
        public Guid AddDocument(string FileName, byte[] Data)
        {
            ObjectParameter g = new ObjectParameter("id", typeof(Guid));
            Entities.AddDocument(FileName, Data, g).First();
            return (Guid)g.Value;
        }

        public void AddFieldToTemplate(Guid TemplateID, Guid FieldID)
        {
            Entities.AddFieldToTemplate(TemplateID, FieldID).First();
        }

        public bool AddDocumentDescription(string Name, Guid DocumentID, Dictionary<Field, object> FieldsWithValues)
        {
            return Worker.AddDocDescription(Name, DocumentID, FieldsWithValues);
        }

        public bool AddTemplate(string Name, IEnumerable<Field> Fields) { return Worker.AddTemplate(Name, Fields); }

        public Guid AddField(string Name, Guid FieldTypeID, bool Nullable, int Order)
        {
            ObjectParameter g = new ObjectParameter("id", typeof(Guid));
            Entities.AddField(Name, FieldTypeID, Nullable, Order, g).First();
            return (Guid)g.Value;
        }

        #endregion

        #region Remove/Delete

        /// <summary>
        /// удаляет документ по его идентификатору
        /// </summary>
        /// <param name="ID">идентификатор документа</param>
        public void DeleteDocument(Guid ID) { Entities.DeleteDocument(ID).First(); }

        /// <summary>
        /// удаляет поле по его идентификатору
        /// </summary>
        /// <param name="ID">идентификатор поля</param>
        public void DeleteField(Guid ID) { Entities.DeleteField(ID).First(); }

        /// <summary>
        /// удаляет шаблон по его идентификатору
        /// </summary>
        /// <param name="ID">идентификатор шаблона</param>
        public void DeleteTemplate(Guid ID) { Entities.DeleteTemplate(ID).First(); }

        /// <summary>
        /// удаляет поле из шаблона
        /// </summary>
        /// <param name="TemplateID">идентификатор шаблона</param>
        /// <param name="FieldID">идентификатор поля</param>
        public void RemoveFieldFromTemplate(Guid TemplateID, Guid FieldID) { Entities.RemoveFieldFromTemplate(TemplateID, FieldID).First(); }

        #endregion

        #region Get

        public IEnumerable<DocDescription> SearchDocDescription(SearchRequest Request) { return Worker.SearchDocDescription(Request); }

        /// <summary>
        /// получает документ по его идентификатору
        /// </summary>
        /// <param name="ID">идентификатор документа</param>
        /// <returns>документ</returns>
        public Document GetDocument(Guid ID) { return Entities.Document.Where(d => d.ID == ID).FirstOrDefault(); }

        public Document[] GetDocuments() { return Entities.Document.ToArray(); }
        
        public string[] GetFileNames(bool WithExtensions)
        {
            string[] set = Entities.Document.Select(d => d.FileName).ToArray();
            if (WithExtensions) return set;
            List<string> result = new List<string>();
            foreach (string s in set)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    int i = s.LastIndexOf('.'); 
                    if (i != -1)
                    {
                        result.Add(s.Substring(0, i));
                    }
                }
            }
            return result.ToArray();
        }

        public Field[] GetTemplateFields(Template Template) { return GetTemplateFields(Template.ID); }

        public Field[] GetTemplateFields(Guid TemplateID) { return Entities.GetTemplateFields(TemplateID).ToArray(); }

        public Template[] GetTemplates() { return Entities.GetTemplates().ToArray(); }

        /// <summary>
        /// все поля в хранилище
        /// </summary>
        /// <returns>массив полей</returns>
        public Field[] GetFields() { return Entities.GetFields().ToArray(); }

        public FieldType[] GetFieldTypes() { return Entities.FieldType.ToArray(); }

        public object[] GetFieldValues(Field Field) { return GetFieldValues(Field.ID); }

        public object[] GetFieldValues(Guid FieldID)
        {
            List<object> result = new List<object>();
            DoFetch("[dbo].[GetFieldValues]", reader => { while (reader.Read()) result.Add(reader["Value"]); }, new SqlParameter("@fieldId", FieldID));
            return result.ToArray();
        }

        public object GetFieldValue(FieldValue Value) { return GetFieldValue(Value.ID); }

        public object GetFieldValue(Guid FieldValueID)
        {
            object result = null;
            DoFetch("[dbo].[GetFieldConcreteValue]", reader => { if (reader.Read()) result = reader["Value"]; }, new SqlParameter("@fieldValueId", FieldValueID));
            return result;
        }

        public T[] GetFieldValues<T>(Guid FieldID)
        {
            Field f = Entities.Field.Where(val => val.ID == FieldID).FirstOrDefault();
            if (f == null) return new T[] { };
            Type realType = new FieldView(f).FieldTypeView.TypeOfField;
            if (realType != typeof(T)) throw new TypeMismatchException(typeof(T), realType);
            if (typeof(T) == typeof(string))
                return Entities.GetStringCollection(FieldID).Select(s => s.Value).Cast<T>().ToArray();
            if (typeof(T) == typeof(DateTime))
                return Entities.GetDateTimeCollection(FieldID).Select(s => s.Value).Cast<T>().ToArray();
            if (typeof(T) == typeof(float))
                return Entities.GetFloatCollection(FieldID).Select(s => s.Value).Cast<T>().ToArray();
            throw new NotSupportedFieldTypeException(typeof(T));
        }
                
        public string[] GetStringCollection() { return Entities.StringFieldValue.Select(v => v.Value).Distinct().ToArray(); }

        public double?[] GetFloatCollection() { return Entities.FloatFieldValue.Select(v => v.Value).Distinct().ToArray(); }

        public DateTime?[] GetDateTimeCollection() { return Entities.DateTimeFieldValue.Select(v => v.Value).Distinct().ToArray(); }

        #endregion

        #region Modify

        public void RenameTemplate(Guid TemplateID, string NewName) { Entities.RenameTemplate(TemplateID, NewName).First(); }

        public void RenameField(Guid FieldID, string NewName) { Entities.RenameField(FieldID, NewName).First(); }

        public void ChangeFieldOrder(Guid FieldID, int Order) { Entities.ChangeFieldOrderNumber(FieldID, Order); }

        #endregion

        #endregion

    }

}