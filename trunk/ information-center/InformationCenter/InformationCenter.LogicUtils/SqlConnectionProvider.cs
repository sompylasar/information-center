using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace LogicUtils
{

    /// <summary>
    /// Класс для соединения с БД.
    /// </summary>
    public class SqlConnectionProvider : IDbConnectionProvider
    {

        #region Поля

        private const int defaultMinPoolSize = 5;
        private const int defaultMaxPoolSize = 100;

        #endregion

        #region События

        /// <summary>
        /// событие возникает, когда для соединения с БД требуется новый пароль
        /// </summary>
        public event EventHandler<ReadWriteEventArgs<RefObject<string>>> NewPasswordRequired;

        #endregion

        #region Свойства

        public IDbConnection CurrentConnection { get { return null; } }

        #endregion
        
        #region Методы

        #region PRIVATE

        private bool TestConnection(SqlConnection connection)
        {
            bool result = false;

            if (connection != null)
            {
                for (int i = 0; i <= 1; i++)
                {
                    try
                    {
                        connection.Open();
                        result = true;
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (i == 0 && ((ex.Number == 18487) || (ex.Number == 18488)))
                        {
                            string newPass = InvokeNewPasswordRequired();
                            SqlConnection.ChangePassword(connection.ConnectionString, newPass);

                            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connection.ConnectionString);
                            csb.Password = newPass;
                            connection.ConnectionString = csb.ConnectionString;
                        }
                        else
                        {
                            if (connection != null)
                            {
                                connection = null;
                            }
                            throw ex;
                        }
                    }
                }
            }
            return result;
        }

        private string InvokeNewPasswordRequired()
        {
            string result = string.Empty;
            var newPassObj = new RefObject<string>();
            if (NewPasswordRequired != null)
            {
                NewPasswordRequired(this, new ReadWriteEventArgs<RefObject<string>>(newPassObj));
                result = newPassObj.Value;
            }
            return result;
        }

        #endregion

        #region PUBLIC

        /// <summary>
        /// получить объект соединения
        /// </summary>
        /// <param name="connectionStringBuilder">контейнер для параметров соединения</param>
        /// <returns>объект соединения</returns>
        public IDbConnection GetConnection(DbConnectionStringBuilder csb)
        {
            if (csb == null) throw new ArgumentNullException("csb");
            SqlConnectionStringBuilder c = csb as SqlConnectionStringBuilder;
            if (c == null) throw new ArgumentException("csb");
            SqlConnection connection = null;
            try
            {
                c.MinPoolSize = defaultMinPoolSize;
                c.Pooling = true;
                connection = new SqlConnection(csb.ConnectionString);
            }
            catch (Exception ex)
            {
                if (connection != null) connection = null;
                throw ex;
            }
            if (!TestConnection(connection)) connection = null;
            else connection.Close();
            return connection;
        }

        #endregion
        
        //private bool GetConnect(string S, string B, string L, string P)
        //{
        //    ConnectionBase CB = new ConnectionBase();
        //    try
        //    {
        //        CB.Connect(S, B, L, P);
        //    }
        //    catch (SqlException Ex)
        //    {
        //        if ((Ex.Number == 18487) || (Ex.Number == 18488))
        //        {
        //            ChangePassword fCP = new ChangePassword("Test");
        //            while ((fCP.GetNewPass() == true) && (CB.ChangePass(S, B, L, P, fCP.NewPass) == false))
        //            {

        //            }
        //            if (fCP.IsSet == true)
        //            {
        //                GetConnect(S, B, L, fCP.NewPass);
        //                return (true);
        //            }

        //            else
        //            {
        //                MessageBox.Show(Ex.Message);
        //                return (false);
        //            }
        //        }

        //        else
        //        {
        //            MessageBox.Show(Ex.Message);
        //            return (false);
        //        }
        //    }

        //    cnn = CB.Connection;
        //    return (true);
        //}
        //public bool ChangePass(String fServer, String fBase, String fLogin, String fOldPass, String fNewPass)
        //{
        //    ChangePassword chp = new ChangePassword(fLogin);
        //    try
        //    {
        //        SqlConnection.ChangePassword(GetConnectonString(fServer, fBase, fLogin, fOldPass), fNewPass);
        //        return (true);
        //    }
        //    catch (SqlException sqex)
        //    {
        //        MessageBox.Show(sqex.Message);
        //        return (false);
        //    }
        //}

        #endregion

    }
}