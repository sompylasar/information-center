using LogicUtils;
using System.Linq;
using System.Data;
using System.Data.Common;
using InformationCenter.EFEngine;
using InformationCenter.Data;
using System;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace InformationCenter.Services
{

    public sealed class InformationCenterEngine : EF_Engine, IDbConnectionProviderBase
    {

        #region Поля

        private User current = null;

        #endregion

        #region Конструкторы

        /// <summary>
        /// основной и единственный конструктор
        /// </summary>
        /// <param name="Context">обёртка над БД</param>
        public InformationCenterEngine(Entities Context) : base(Context) { }

        public InformationCenterEngine(string ConnectionString) : base(new Entities(ConnectionString)) { }

        #endregion

        #region Свойства

        private Entities Entities { get { return Context as Entities; } }

        public IDbConnection GetConnection(DbConnectionStringBuilder connectionStringBuilder) { return null; }

        public IDbConnection CurrentConnection { get { return Context.Connection; } }

        public string DbConnectionString
        {
            get
            {
                int a = Context.Connection.ConnectionString.IndexOf('\'');
                return Context.Connection.ConnectionString.Substring(a).Trim('\'');
            }
        }

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
            int result = Entities.AddDocument(FileName, Data, g).First().Value;
            return result == 0 ? (Guid)g.Value : Guid.Empty;
        }

        public Guid AddDocumentDescription(string Name, Guid DocumentID)
        {
            ObjectParameter g = new ObjectParameter("id", typeof(Guid));
            // TODO: создать временную таблицу с полями и передать ее имя
            int result = Entities.AddDocumentDescription(Name, DocumentID, "" /*tempFieldsTableName*/, g).First().Value;
            return result == 0 ? (Guid)g.Value : Guid.Empty;
        }

        public Guid AddField(string Name, Guid FieldTypeID)
        {
            ObjectParameter g = new ObjectParameter("id", typeof(Guid));
            int result = Entities.AddField(Name, FieldTypeID, g).First().Value;
            return result == 0 ? (Guid)g.Value : Guid.Empty;
        }

        public Guid AddTemplate(string Name)
        {
            ObjectParameter g = new ObjectParameter("id", typeof(Guid));
            int result = Entities.AddTemplate(Name, "", g).First().Value;
            return result == 0 ? (Guid)g.Value : Guid.Empty;
        }

        #endregion

        #region Delete

        public int DeleteDocument(Guid ID)
        {
            return Entities.DeleteDocument(ID).First().Value;
        }

        public int DeleteField(Guid ID)
        {
            return Entities.DeleteField(ID).First().Value;
        }

        public int DeleteTemplate(Guid ID)
        {
            return Entities.DeleteTemplate(ID).First().Value;
        }

        #endregion

        #region Get

        public Document GetDocument(Guid ID)
        {
            return Entities.Document.Where(d => d.ID == ID).FirstOrDefault();
        }
        
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