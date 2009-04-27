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
                if (Connection.State == ConnectionState.Open)
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
            string tempTableName = "#FieldsTempTable_" + Guid.NewGuid().ToString("N");
            string query = GenQueryCreateTable(tempTableName, new ColumnDescription[]
            {
                new ColumnDescription{Name = "FieldID", Type = SqlDbType.UniqueIdentifier}
            });

            var con = provider.GetConnection(csb) as SqlConnection;
            if (con != null)
            {
                try
                {
                    con.Open();
                    ExecuteNonQuery(new SqlCommand(query), con);
                    query = "select * from " + tempTableName;
                    DataTable table = ExecuteQuery(new SqlCommand(query), con);
                    foreach (var field in fields)
                    {
                        table.Rows.Add(field.ID);
                    }
                    table.AcceptChanges();
                    DataTable table1 = ExecuteQuery(new SqlCommand(query), con);
                    con.Close();

                    foreach (var item in table1.AsEnumerable())
                    {
                        Debug.WriteLine(item.Field<Guid>(0));
                    }
                    //query += Environment.NewLine + " insert into " + tempTableName + " (";
                }
                catch (Exception exc)
                {
                    tempTableName = string.Empty;
                    Debug.WriteLine("CreateFieldsTempTable > " + exc.Message);
                }
                finally
                {
                    con.Dispose();
                }

            }
            return tempTableName;
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
