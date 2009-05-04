using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Data.SqlClient;
using InformationCenter.Data;
using System.Data;
using LogicUtils;
using System.Diagnostics;

namespace InformationCenter.DBUtils
{
    public class DBWorker
    {
        //public DBWorker(SqlConnectionStringBuilder csb)
        //{
        //    if (csb == null)
        //        throw new ArgumentNullException("csb", "Некорректная строка подключения");

        //    if (String.IsNullOrEmpty(csb.ConnectionString.Trim()))
        //        throw new ArgumentException("Некорректная строка подключения", "csb");

        //    this.csb = new SqlConnectionStringBuilder(csb.ConnectionString);
        //    provider = new SqlConnectionProvider();
        //}
        public DBWorker(string connectionString)
        {
            if(String.IsNullOrEmpty(connectionString.Trim()))
                throw new ArgumentNullException("connectionString", "Некорректная строка подключения");

            ConnectionString = connectionString;
            provider = new SqlConnectionProvider();
        }

        public string ConnectionString
        {
            get { return connectionString; }
            set
            {
                connectionString = value;
                csb = new SqlConnectionStringBuilder(DbConnectionString);
            }
        }
        private string connectionString { get; set; }
        protected string DbConnectionString
        {
            get
            {
                int a = ConnectionString.IndexOf('\'');
                return ConnectionString.Substring(a).Trim('\'');
            }
        }

        public string EntityConnectionString { get; set; }
        private SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
        private IDbConnectionProvider provider {get; set; }

        #region Execute
        /// <summary>
        /// Выполнить запрос к БД
        /// </summary>
        /// <param name="Query">Текст запроса</param>
        /// <returns>Результат запроса</returns>
            internal DataTable ExecuteQuery(string Query)
        {
            if (Query.Trim() == string.Empty)
                throw new ArgumentException("Пустая строка", Query);

            return ExecuteQuery(new SqlCommand(Query));
        }
        /// <summary>
        /// Выполнить запрос к БД
        /// </summary>
        /// <param name="Cmd">Запрос</param>
        /// <returns>Результат запроса</returns>
        private DataTable ExecuteQuery(SqlCommand Cmd)
        {
            if (Cmd == null)
                throw new NullReferenceException("Cmd");

            DataTable result = new DataTable();
            var con = provider.GetConnection(csb) as SqlConnection;
            if (con != null)
            {
                con.Open();
                result = ExecuteQuery(Cmd, con);
                con.Close();
            }
            return result;
        }
        private DataTable ExecuteQuery(SqlCommand Cmd, SqlConnection Connection)
        {
            DataTable result = new DataTable();
            if (Connection != null)
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                Cmd.Connection = Connection;
                var reader = Cmd.ExecuteReader();
                bool columnInitialized = false;
                while (reader.Read())
                {
                    int count = reader.FieldCount;
                    if (!columnInitialized)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            result.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                        }
                        columnInitialized = true;
                    }

                    object[] values = new object[count];
                    reader.GetValues(values);
                    result.Rows.Add(values);
                }
                //var da = new SqlDataAdapter(Cmd);
                //da.Fill(result);
                
                //Connection.Close();
            }
            return result;
        }
        /// <summary>
        /// Выполнить запрос к БД
        /// </summary>
        /// <param name="Query">Текст запроса</param>
        /// <param name="Params">Параметры запроса</param>
        /// <returns>Результат запроса</returns>
        public DataTable ExecuteQuery(string Query, params SqlParameter[] Params)
        {
            if (Query.Trim() == string.Empty)
                throw new ArgumentException("Пустая строка", Query);
            if (Params == null)
                throw new NullReferenceException("Params");

            var cmd = new SqlCommand(Query);
            foreach (var param in Params)
            {
                cmd.Parameters.Add(param);
            }

            return ExecuteQuery(cmd);
        }
        public DataTable ExecuteQuery(string Query, SqlConnection Connection, params SqlParameter[] Params)
        {
            if (Query.Trim() == string.Empty)
                throw new ArgumentException("Пустая строка", Query);
            if (Params == null)
                throw new NullReferenceException("Params");
            if (Connection == null)
                throw new ArgumentNullException("Connection");

            var cmd = new SqlCommand(Query);
            foreach (var param in Params)
            {
                cmd.Parameters.Add(param);
            }

            return ExecuteQuery(cmd, Connection);
        }

        /// <summary>
        /// Выполнить запрос (не на выборку)
        /// </summary>
        /// <param name="Query">Текст запроса</param>
        /// <returns>Число изменённых строк</returns>
        public int ExecuteNonQuery(string Query)
        {
            if (Query.Trim() == string.Empty)
                throw new ArgumentException("Пустая строка", Query);

            return ExecuteNonQuery(new SqlCommand(Query));
        }
        /// <summary>
        /// Выполнить запрос (не на выборку)
        /// </summary>
        /// <param name="Query">Текст запроса</param>
        /// <param name="Params">Параметры</param>
        /// <returns>Число изменённых строк</returns>
        public int ExecuteNonQuery(string Query, params SqlParameter[] Params)
        {
            if (Query.Trim() == string.Empty)
                throw new ArgumentException("Пустая строка", Query);
            if (Params == null)
                throw new NullReferenceException("Params");

            var cmd = new SqlCommand(Query);
            foreach (var param in Params)
            {
                cmd.Parameters.Add(param);
            }

            return ExecuteNonQuery(cmd);
        }

        public int ExecuteNonQuery(string Query, SqlConnection Connection, params SqlParameter[] Params)
        {
            if (Query.Trim() == string.Empty)
                throw new ArgumentException("Пустая строка", Query);
            if (Params == null)
                throw new NullReferenceException("Params");
            if (Connection == null)
                throw new ArgumentNullException("Connection");

            var cmd = new SqlCommand(Query);
            foreach (var param in Params)
            {
                cmd.Parameters.Add(param);
            }

            return ExecuteNonQuery(cmd, Connection);
        }

        /// <summary>
        /// Выполнить запрос (не на выборку)
        /// </summary>
        /// <param name="Cmd">Запрос</param>
        /// <returns>Число изменённых строк</returns>
        public int ExecuteNonQuery(SqlCommand Cmd)
        {
            if (Cmd == null)
                throw new NullReferenceException("Cmd");

            int result = 0;
            var con = provider.GetConnection(csb) as SqlConnection;
            if (con != null)
            {
                con.Open();
                result = ExecuteNonQuery(Cmd, con);
                con.Close();
            }
            return result;
        }
        public int ExecuteNonQuery(SqlCommand Cmd, SqlConnection Connection)
        {
            if (Cmd == null)
                throw new ArgumentNullException("Cmd");
            if (Connection == null)
                throw new ArgumentNullException("Connection");

            int result = 0;

            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            Cmd.Connection = Connection;
            result = Cmd.ExecuteNonQuery();

            return result;
        }
#endregion
        
        /// <summary>
        /// Создаёт временную таблицу состоящую из полей описания
        /// </summary>
        /// <param name="templateName">Имя шаблона</param>
        /// <param name="fields">Коллекция полей описания</param>
        /// <returns>Добавило или нет</returns>
        public Guid AddTemplate(string templateName, IEnumerable<Field> fields)
        {
            Guid result = Guid.Empty;

            string tempTableName = "##FieldsTempTable_" + Guid.NewGuid().ToString("N");
            string query = GenQueryCreateTable(tempTableName, new ColumnDescription[]
                                                                  {
                                                                      new ColumnDescription
                                                                          {
                                                                              Name = "FieldID",
                                                                              Type = "UniqueIdentifier"
                                                                          }
                                                                  });

            int i = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();
            query += Environment.NewLine;
            foreach (var field in fields)
            {
                query += Environment.NewLine +
                         " INSERT INTO " + tempTableName +
                         @"(FieldID)
                    VALUES
                    (@id" + i.ToString() + ") ";

                parameters.Add(new SqlParameter("@id" + i.ToString(), field.ID) {DbType = DbType.Guid});

                ++i;
            }

            query += Environment.NewLine +
                     @"EXECUTE [AddTemplate] 
                   @name
                  ,@tempFieldsTableName
                  ,@tId";

            parameters.Add(new SqlParameter("@name", templateName));
            parameters.Add(new SqlParameter("@tempFieldsTableName", tempTableName));
            var pId = new SqlParameter("@tID", Guid.NewGuid())
                          {DbType = DbType.Guid, Direction = ParameterDirection.Output};
            parameters.Add(pId);

            try
            {
                ExecuteNonQuery(query, parameters.ToArray());

                if (pId.Value is Guid)
                    result = (Guid)pId.Value;
            }
            catch (Exception exc)
            {
                tempTableName = string.Empty;
                Debug.WriteLine("CreateFieldsTempTable > " + exc.Message);
            }
            finally
            {
                try
                {
                    query = "drop table " + tempTableName;
                    ExecuteNonQuery(new SqlCommand(query));
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        public Guid AddDocDescription(string docDescriptionName, Guid documentId, Dictionary<Field, object> fields)
        {
            Guid result = Guid.Empty;

            string tempTableName = "##FieldsTempTable_" + Guid.NewGuid().ToString("N");
            List<ColumnDescription> cds = new List<ColumnDescription>();
            cds.Add(new ColumnDescription
            {
                Name = "FieldID",
                Type = "UniqueIdentifier"
            });

            var ent = new Entities(ConnectionString);
            
            foreach (var fieldType in ent.FieldType.ToArray())
            {
                cds.Add(new ColumnDescription()
                    {
                        Name = "FieldValue" + fieldType.SqlName,
                        Type = fieldType.SqlType
                    });
            }
            string query = GenQueryCreateTable(tempTableName, cds);

            int i = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();
            query += Environment.NewLine;
            foreach (var field in fields)
            {
                query += Environment.NewLine +
                         " INSERT INTO " + tempTableName +
                         @"(FieldID, FieldValue"+field.Key.FieldType.SqlName+
                         ") VALUES (@id" + i.ToString() + ", @value" + i + ") ";

                parameters.Add(new SqlParameter("@id" + i.ToString(), field.Key.ID) { SqlDbType = SqlDbType.UniqueIdentifier});
                if (field.Value is Guid)
                    parameters.Add(new SqlParameter("@value" + i.ToString(), (Guid)field.Value) { SqlDbType = SqlDbType.UniqueIdentifier });
                else
                    parameters.Add(new SqlParameter("@value" + i.ToString(), field.Value));

                ++i;
            }

            query += Environment.NewLine +
                     @"EXECUTE [AddDocDescription] 
                   @name
                  ,@documentId
                  ,@tempFieldsTableName
                  ,@id";

            parameters.Add(new SqlParameter("@name", docDescriptionName));
            parameters.Add(new SqlParameter("@tempFieldsTableName", tempTableName));
            parameters.Add(new SqlParameter("@documentId", documentId) {DbType = DbType.Guid});
            var pId = new SqlParameter("@id", Guid.NewGuid())
                          {DbType = DbType.Guid, Direction = ParameterDirection.Output};
            parameters.Add(pId);


            try
            {
                ExecuteNonQuery(query, parameters.ToArray());

                if (pId.Value is Guid)
                    result = (Guid)pId.Value;
            }
            catch (Exception exc)
            {
                Debug.WriteLine("CreateFieldsTempTable > " + exc.Message);
                throw exc;
            }

            return result;
        }

        public IEnumerable<DocDescription> SearchDocDescription(SearchRequest Request)
        {
            List<DocDescription> result = new List<DocDescription>();

            string tempTableName = "##SearchItemsTempTable_" + Guid.NewGuid().ToString("N");
            List<ColumnDescription> cds = new List<ColumnDescription>();
            cds.Add(new ColumnDescription
            {
                Name = "FieldID",
                Type = "UniqueIdentifier"
            });

            var ent = new Entities(ConnectionString);

            foreach (var fieldType in ent.FieldType.ToArray().OrderBy(f=>f.SqlName))
            {
                cds.Add(new ColumnDescription()
                {
                    Name = "FieldValue" + fieldType.SqlName,
                    Type = fieldType.SqlType
                });
            }
            string query = GenQueryCreateTable(tempTableName, cds);

            int i = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();
            query += Environment.NewLine;
            foreach (var searchItem in Request.Items)
            {
                Field field = ent.Field.Where(f => f.ID == searchItem.Field.ID).FirstOrDefault();
                if(field == null)
                    throw new ArgumentException();

                query += Environment.NewLine +
                         " INSERT INTO " + tempTableName +
                         @"(FieldID, FieldValue"+field.FieldType.SqlName+@")
                    VALUES
                    (@id" + i + ", @value" +
                         i + ") ";

                parameters.Add(new SqlParameter("@id" + i.ToString(), field.ID) { SqlDbType = SqlDbType.UniqueIdentifier });
                if (searchItem.FieldValue is Guid)
                    parameters.Add(new SqlParameter("@value" + i, (Guid)searchItem.FieldValue) { SqlDbType = SqlDbType.UniqueIdentifier });
                else
                    parameters.Add(new SqlParameter("@value" + i, searchItem.FieldValue));

                ++i;
            }

            query += Environment.NewLine +
                     @"EXECUTE SearchDescriptions @tempSearchItemTable";

            parameters.Add(new SqlParameter("@tempSearchItemTable", tempTableName));

            DataTable table = null;
            try
            {
                table = ExecuteQuery(query, parameters.ToArray());
            }
            catch (Exception exc)
            {
                Debug.WriteLine("SearchDocDescription > " + exc.Message);
                throw exc;
            }

            //result.AddRange(ParseDataTable<DocDescription>(table,
            //    new[]
            //    {
            //        new ColumnDescription(){Name = "ID", PropName = "ID", Type = "UniqueIdentifier"},
            //        new ColumnDescription(){Name = "Name", PropName = "Name", Type = "NVarChar(256)"},
            //        new ColumnDescription(){Name = "DocumentID", PropName = "DocumentID", Type = "UniqueIdentifier"}
            //    }));
            result.AddRange(ParseDataTableToDocDescriptions(table, ent));
            return result.ToArray();
        }

        public void SelectFromTable(string tableName)
        {
            try
            {
                var table = ExecuteQuery("select * from "+ tableName);
                foreach (var item in table.AsEnumerable())
                {
                    Debug.WriteLine(item.Field<Guid>(0));
                }
            }
            catch (Exception exc)
            {
            }
        }

        private IEnumerable<DocDescription> ParseDataTableToDocDescriptions(DataTable table, Entities entities)
        {
            List<DocDescription> result = new List<DocDescription>();
            for (int i = 0; i < table.Rows.Count; ++i)
            {
                DataRow row = table.Rows[i];
                Guid id = row.Field<Guid>("ID");

                DocDescription element = entities.DocDescription.Where(dd => dd.ID == id).FirstOrDefault();
                if (element != null)
                    result.Add(element);
                else
                    throw new NullReferenceException();
            }
            return result;
        }

        private IEnumerable<T> ParseDataTable<T>(DataTable table, IEnumerable<ColumnDescription> columns) where T:new()
        {
            List<T> result = new List<T>();
            for (int i = 0; i < table.Rows.Count; ++i )
            {
                DataRow row = table.Rows[i];
                T element = new T();
                var props = typeof(T).GetProperties();
                foreach (var column in columns)
                {
                    var prop = typeof(T).GetProperty(column.PropName);
                    if (prop == null)
                        throw new NullReferenceException();
                    prop.GetSetMethod().Invoke(element, new[] { row[column.Name] });
                }

                result.Add(element);
            }
            return result;
        }

        public string CreateSearchTempTable(IEnumerable<SearchItem> searchItems)
        {
            string result = string.Empty;
            return result;
        }

        /// <summary>
        /// Сгенерить запрос на создание таблицы на основании коллекции свойств
        /// </summary>
        /// <param name="TempTableName"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        private static string GenQueryCreateTable(string TempTableName, IEnumerable<ColumnDescription> props)
        {
            string result = "CREATE TABLE " + TempTableName + "(";
            foreach (var prop in props)
            {
                result += prop.Name + " " + prop.Type;
                result += ", ";
            }
            result = result.TrimEnd(',', ' ');
            result += ")";

            return result;
        }



        private class ColumnDescription
        {
            public string Name { get; set; }
            public string PropName { get; set; }
            public string Type { get; set; }
        }
    }
}
