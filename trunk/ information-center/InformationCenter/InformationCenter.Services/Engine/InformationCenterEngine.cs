using System;
using System.Linq;
using System.Data;
using InformationCenter.EFEngine;
using InformationCenter.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace InformationCenter.Services
{

    /// <summary>
    /// Движок для доступа к объектам хранилища.
    /// </summary>
    public sealed class InformationCenterEngine : EF_Engine
    {

        #region Поля

        private User current = null;

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="ConnectionString">строка подключения</param>
        public InformationCenterEngine(string ConnectionString) : base(new Entities(ConnectionString)) { }

        public InformationCenterEngine(Entities Context) : base(Context) { }

        #endregion

        #region Свойства

        private Entities Entities { get { return Context as Entities; } }

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

        public Guid AddDocument(string FileName, byte[] Data)
        {
            ObjectParameter g = new ObjectParameter("id", typeof(Guid));
            Entities.AddDocument(FileName, Data, g).First();
            return (Guid)g.Value;
        }

        public Guid AddDocumentDescription(string Name, Guid DocumentID, Dictionary<Field, object> fieldsWithValues)
        {
            ObjectParameter g = new ObjectParameter("id", typeof(Guid));
           // DBWorker
            // TODO: создать временную таблицу с полями и передать ее имя


            int result = Entities.AddDocumentDescription(Name, DocumentID, "" /*tempFieldsTableName*/, g).First().Value;
            return result == 0 ? (Guid)g.Value : Guid.Empty;
        }

        public Guid AddField(string Name, Guid FieldTypeID)
        {
            ObjectParameter g = new ObjectParameter("id", typeof(Guid));
            Entities.AddField(Name, FieldTypeID, g).First();
            return (Guid)g.Value;
        }

        public Guid AddTemplate(string Name)
        {
            ObjectParameter g = new ObjectParameter("id", typeof(Guid));
            Entities.AddTemplate(Name, "", g).First();
            return (Guid)g.Value;
        }

        #endregion

        #region Delete

        public void DeleteDocument(Guid ID) { Entities.DeleteDocument(ID).First(); }

        public void DeleteField(Guid ID) { Entities.DeleteField(ID).First(); }

        public void DeleteTemplate(Guid ID) { Entities.DeleteTemplate(ID).First(); }

        public void RemoveFieldFromTemplate(Guid TemplateID, Guid FieldID) { Entities.RemoveFieldFromTemplate(TemplateID, FieldID).First(); }

        #endregion

        #region Get

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

        public Field[] GetFields() { return Entities.GetFields().ToArray(); }

        public object[] GetFieldValues(Field Field) { return GetFieldValues(Field.ID); }

        public object[] GetFieldValues(Guid FieldID)
        {
            List<object> result = new List<object>();
            DoFetch("[dbo].[GetFieldValues]", reader => { while (reader.Read()) result.Add(reader[0]); }, new SqlParameter("@fieldId", FieldID));
            return result.ToArray();
        }

        public object GetFieldValue(FieldValue Value) { return GetFieldValue(Value.ID); }

        public object GetFieldValue(Guid FieldValueID)
        {
            object result = null;
            DoFetch("[dbo].[GetFieldConcreteValue]", reader => { if (reader.Read()) result = reader[0]; }, new SqlParameter("@@fieldValueId", FieldValueID));
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

        #endregion

    }

}