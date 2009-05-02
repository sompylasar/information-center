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
        public DBWorker(SqlConnectionStringBuilder csb)
        {
            if (csb == null)
                throw new ArgumentNullException("csb", "Некорректная строка подключения");

            if (String.IsNullOrEmpty(csb.ConnectionString.Trim()))
                throw new ArgumentException("Некорректная строка подключения", "csb");

            this.csb = new SqlConnectionStringBuilder(csb.ConnectionString);
            provider = new SqlConnectionProvider();
        }
        public DBWorker(string connectionString)
        {
            if(String.IsNullOrEmpty(connectionString.Trim()))
                throw new ArgumentNullException("connectionString", "Некорректная строка подключения");

            csb = new SqlConnectionStringBuilder(connectionString);
            provider = new SqlConnectionProvider();
        }

        public string ConnectionString { get { return csb.ConnectionString; } }
        private SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
        private IDbConnectionProvider provider {get; set; }
        
        #region Execute
        /// <summary>
        /// Выполнить запрос к БД
        /// </summary>
        /// <param name="Query">Текст запроса</param>
        /// <returns>Результат запроса</returns>
        private DataTable ExecuteQuery(string Query)
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
        public bool AddTemplate(string templateName, IEnumerable<Field> fields)
        {
            bool result = false;

            string tempTableName = "##FieldsTempTable_" + Guid.NewGuid().ToString("N");
            string query = GenQueryCreateTable(tempTableName, new ColumnDescription[]
                                                                  {
                                                                      new ColumnDescription
                                                                          {
                                                                              Name = "FieldID",
                                                                              Type = SqlDbType.UniqueIdentifier
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
                result = ExecuteNonQuery(query, parameters.ToArray()) > 0;
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

        public bool AddDocDescription(string docDescriptionName, Guid documentId, Dictionary<Field, object> fields)
        {
            bool result = false;

            string tempTableName = "##FieldsTempTable_" + Guid.NewGuid().ToString("N");
            List<ColumnDescription> cds = new List<ColumnDescription>();
            cds.Add(new ColumnDescription
            {
                Name = "FieldID",
                Type = SqlDbType.UniqueIdentifier
            });

            foreach (var fieldType in fields.Keys.Select(f => { if (!f.FieldTypeReference.IsLoaded) f.FieldTypeReference.Load(); return f.FieldType; }).Distinct())
            {
                //cds.Add(new ColumnDescription()
                //    {
                //        Name = "FieldValue",
                //        Type = Enum.Parse(typeof(SqlDbType))
                //    });
            }
            string query = GenQueryCreateTable(tempTableName, cds);

            int i = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();
            query += Environment.NewLine;
            foreach (var field in fields)
            {
                query += Environment.NewLine +
                         " INSERT INTO " + tempTableName +
                         @"(FieldID, FieldValue)
                    VALUES
                    (@id" + i.ToString() + ", @value" + i + ") ";

                parameters.Add(new SqlParameter("@id" + i.ToString(), field.Key.ID) {DbType = DbType.Guid});
                parameters.Add(new SqlParameter("@value" + i.ToString(), field.Key.ID));

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
            var pId = new SqlParameter("@tID", Guid.NewGuid())
                          {DbType = DbType.Guid, Direction = ParameterDirection.Output};
            parameters.Add(pId);


            try
            {
                result = ExecuteNonQuery(query, parameters.ToArray()) > 0;
            }
            catch (Exception exc)
            {
                tempTableName = string.Empty;
                Debug.WriteLine("CreateFieldsTempTable > " + exc.Message);
                throw exc;
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

        public IEnumerable<DocDescription> SearchDocDescription(SearchRequest Request)
        {
            List<DocDescription> result = new List<DocDescription>();

            string tempTableName = "##SearchItemsTempTable_" + Guid.NewGuid().ToString("N");
            string query = GenQueryCreateTable(tempTableName, new ColumnDescription[]
                                                                  {
                                                                      new ColumnDescription
                                                                          {
                                                                              Name = "FieldID",
                                                                              Type = SqlDbType.UniqueIdentifier
                                                                          },
                                                                      new ColumnDescription
                                                                          {
                                                                              Name = "FieldValue",
                                                                              Type = SqlDbType.UniqueIdentifier
                                                                          }
                                                                  });

            int i = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();
            query += Environment.NewLine;
            foreach (var searchItem in Request.Items)
            {
                query += Environment.NewLine +
                         " INSERT INTO " + tempTableName +
                         @"(FieldID, FieldValue)
                    VALUES
                    (@id" + i + ", @value" +
                         i + ") ";

                parameters.Add(new SqlParameter("@id" + i, searchItem.FieldID) {DbType = DbType.Guid});
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
                tempTableName = string.Empty;
                Debug.WriteLine("CreateFieldsTempTable > " + exc.Message);
                throw exc;
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

            result.AddRange(ParseDataTable<DocDescription>(table,
                new[]
                {
                    new ColumnDescription(){Name = "ID", PropName = "ID", Type = SqlDbType.UniqueIdentifier},
                    new ColumnDescription(){Name = "Name", PropName = "ID", Type = SqlDbType.NVarChar},
                    new ColumnDescription(){Name = "DocumentID", PropName = "DocumentID", Type = SqlDbType.UniqueIdentifier}
                }));
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

        private IEnumerable<T> ParseDataTable<T>(DataTable table, IEnumerable<ColumnDescription> columns) where T:new()
        {
            List<T> result = new List<T>();
            for (int i = 0; i < table.Rows.Count; ++i )
            {
                DataRow row = table.Rows[i];
                T element = new T();
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
                SqlDbType type = prop.Type;
                result += prop.Name + " " + Enum.GetName(typeof(SqlDbType), type);
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
            public SqlDbType Type { get; set; }
        }
    }
}
