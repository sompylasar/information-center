using System.Data.SqlClient;

namespace InformationCenter.WebUI.Models
{
    public static class AppSettings
    {
        private const string DATA_SOURCE = @".\SQLEXPRESS";
        private const string INITIAL_CATALOG = "InformationCenter";

        public static string BuildConnectionString(string userName, string password)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = DATA_SOURCE;
            connectionStringBuilder.InitialCatalog = INITIAL_CATALOG;
            connectionStringBuilder.IntegratedSecurity = true;
            connectionStringBuilder.UserID = userName ?? "";
            connectionStringBuilder.Password = password ?? "";
            return connectionStringBuilder.ToString();
        }
    }
}