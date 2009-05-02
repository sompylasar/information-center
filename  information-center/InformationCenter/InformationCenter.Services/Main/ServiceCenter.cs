using System;
using System.Data.SqlClient;

namespace InformationCenter.Services
{

    /// <summary>
    /// Сервисный центр для управления системой.
    /// </summary>
    public class ServiceCenter : IDisposable
    {

        #region Поля

        private ServiceSet set;
        private string connectionString = null;

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="ConnectionString">строка соединения к центру</param>
        public ServiceCenter(string ConnectionString)
        {
            connectionString = ConnectionString;
            using (SqlConnection con = new SqlConnection(ConnectionString)) con.Open();
        }

        #endregion

        #region Свойства

        private ServiceSet InternalService
        {
            get
            {
                if (set == null) set = new ServiceSet(ConnectionString);
                return set;
            }
        }

        /// <summary>
        /// строка соединения
        /// </summary>
        public string ConnectionString { get { return connectionString; } }      

        /// <summary>
        /// сервис для поиска
        /// </summary>
        public ISearchService SearchService { get { return InternalService; } }

        /// <summary>
        /// сервис для скачивания документов (download)
        /// </summary>
        public IDownloadService DownloadService { get { return InternalService; } }

        /// <summary>
        /// сервис для заливки документов (upload)
        /// </summary>
        public IUploadService UploadService { get { return InternalService; } }

        /// <summary>
        /// сервис для работы с шаблонами
        /// </summary>
        public IDocumentDescriptionService DocumentDescriptionService { get { return InternalService; } }

        #endregion

        #region Методы

        /// <summary>
        /// освоводить ресурсы
        /// </summary>
        public void Dispose()
        {
            if (set != null) set.Dispose();
            set = null; ;
        }

        /// <summary>
        /// преобразовать в строку
        /// </summary>
        /// <returns>строка</returns>
        public override string ToString() { return "Информационный центр ВУЗа"; }

        #endregion

    }

}