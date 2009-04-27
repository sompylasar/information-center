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

      //  private Person current = null;

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
        /*public Person CurrentUser
        {
            get
            {
                DebugTimer t = new DebugTimer();
                t.Start();
                if (current == null) current = (Context as SchedulingEntities).CurrentUser().FirstOrDefault();
                t.ToTraceString("CurrentUser : {0}");
                return current;
            }
        }*/

        #endregion

        #region Методы

        public Guid AddDocument(string FileName, byte[] Data)
        {
            ObjectParameter g = new ObjectParameter("id", null);
            int result = Entities.AddDocument(FileName, Data, g).First().Value;
            return (Guid)g.Value;
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

        public string[] GetStringCollection() { return Entities.StringFieldValue.Select(v => v.Value).Distinct().ToArray(); }

        public double?[] GetFloatCollection() { return Entities.FloatFieldValue.Select(v => v.Value).Distinct().ToArray(); }

        public DateTime?[] GetDateTimeCollection() { return Entities.DateTimeFieldValue.Select(v => v.Value).Distinct().ToArray(); }

        #endregion

    }

}