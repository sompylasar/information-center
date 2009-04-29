using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using InformationCenter.Services;
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
        private IDbConnectionProvider provider {get; set;}

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

        /// <summary>
        /// Создаёт временную таблицу состоящую из полей описания
        /// </summary>
        /// <param name="fields">Коллекция полей описания</param>
        /// <returns>Имя временной таблицы</returns>
        public string CreateFieldsTempTable(IEnumerable<Field> fields)
        {
            string tempTableName = "##FieldsTempTable_" + Guid.NewGuid().ToString("N");
            string query = GenQueryCreateTable(tempTableName, new ColumnDescription[]
            {
                new ColumnDescription{Name = "FieldID", Type = SqlDbType.UniqueIdentifier}
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
                
                parameters.Add(new SqlParameter("@id" + i.ToString(), field.ID) { DbType = DbType.Guid });
                
                ++i;
            }

            query += Environment.NewLine + //" select * from " + tempTableName;
            @"EXECUTE [AddTemplate] 
                   'Шаблонъ'
                  ,@tempFieldsTableName
                  ,@tId";

            //var
            parameters.Add(new SqlParameter("@tempFieldsTableName", tempTableName));
            var pId = new SqlParameter("@tID", Guid.NewGuid()) { DbType = DbType.Guid, Direction = ParameterDirection.Output };
            parameters.Add(pId);
            
            var con = provider.GetConnection(csb) as SqlConnection;
            if (con != null)
            {
                try
                {
                    //con.Open();
                    ExecuteNonQuery(query, parameters.ToArray());
                    //var table1 = ExecuteQuery(query, parameters.ToArray());
                    //query = "select * from " + tempTableName;
                    //DataTable table = ExecuteQuery(new SqlCommand(query), con);
                    //foreach (var field in fields)
                    //{
                    //    table.Rows.Add(field.ID);
                    //}
                    //table.AcceptChanges();
                    //DataTable table1 = ExecuteQuery(new SqlCommand(query), con);
                    //con.Close();

                    //foreach (var item in table1.AsEnumerable())
                    //{
                    //    Debug.WriteLine(item.Field<Guid>(0));
                    //}
                    //query += Environment.NewLine + " insert into " + tempTableName + " (";
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
                        //if (con.State != ConnectionState.Open)
                        //    con.Open();
                        query = "drop table " + tempTableName;
                        ExecuteNonQuery(new SqlCommand(query));//, con);
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        //con.Dispose();
                    }
                }

            }
            return tempTableName;
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
            public SqlDbType Type { get; set; }
        }
    }
}
