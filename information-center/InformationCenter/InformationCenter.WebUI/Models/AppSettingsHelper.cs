using System;
using System.Configuration;
using System.Data.SqlClient;

namespace InformationCenter.WebUI.Models
{
    public static class AppSettingsHelper
    {
        public static string ConnectionStringSettingsName = "InformationCenterDatabase";
        public static string ConnectionString = null;

        public static string BuildConnectionString(
            string serverName, string databaseName,
            string userName, string password, 
            bool integratedSecurity)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = serverName;
            connectionStringBuilder.InitialCatalog = databaseName;
            if (integratedSecurity)
            {
                connectionStringBuilder.IntegratedSecurity = true;
            }
            else
            {
                connectionStringBuilder.UserID = userName;
                connectionStringBuilder.Password = password;
            }
            return connectionStringBuilder.ToString();
        }

        private static string GetConnectionStringFromSettings()
        {
            try
            {
                var connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringSettingsName];
                return (connectionStringSettings != null ? connectionStringSettings.ConnectionString : null);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool IsConnectionStringConfigured()
        {
            return (GetConnectionStringFromSettings() != null);
        }

        public static string GetConnectionString()
        {
            return (GetConnectionStringFromSettings() ?? ConnectionString);
        }
    }
}