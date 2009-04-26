using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using InformationCenter.Services;
using InformationCenter.Data;

namespace InformationCenter.DBUtils
{
    public class DBWorker
    {
        public DBWorker(string connectionString)
        {
            if(String.IsNullOrEmpty(connectionString.Trim()))
                throw new ArgumentNullException("connectionString", "Некорректная строка подключения");

            csb = new SqlConnectionStringBuilder(connectionString);
        }

        public string ConnectionString { get { return csb.ConnectionString; } }
        private SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();

        public string CreateSearchTempTable(IEnumerable<SearchItem> searchItems)
        {
            string result = string.Empty;
            return result;
        }

        public string CreateFieldsTempTable(IEnumerable<Field> fields)
        {
            string result = string.Empty;
            return result;
        }
    }
}
