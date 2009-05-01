using InformationCenter.Services.ServicesImpl;
using System;

namespace InformationCenter.Services
{

    /// <summary>
    /// Сервисный центр для управления системой.
    /// </summary>
    public class ServiceCenter : IDisposable
    {

        #region Поля

        private UploadService uploadService;
        private SearchService searchService;
        private DownloadService downloadService;
        private string connectionString = null;

        #endregion

        #region Конструкторы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="ConnectionString">строка соединения к центру</param>
        public ServiceCenter(string ConnectionString) { connectionString = ConnectionString; }

        #endregion

        #region События

        /// <summary>
        /// событие изменения строки подключения
        /// </summary>
        public event EventHandler ConnectionStringChanged;

        protected virtual void OnConnectionStringChanged()
        {
            DisposeResources();
            if (ConnectionStringChanged != null) ConnectionStringChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Свойства

        /// <summary>
        /// строка соединения
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
            set
            {
                if (connectionString != value)
                {
                    connectionString = value;
                    OnConnectionStringChanged();
                }
            }
        }

        /// <summary>
        /// сервис для поиска
        /// </summary>
        public SearchService SearchService
        {
            get
            {
                if (searchService == null) searchService = new SearchService(ConnectionString);
                return searchService;
            }
        }

        /// <summary>
        /// сервис для скачивания документов (download)
        /// </summary>
        public DownloadService DownloadService
        {
            get
            {
                if (downloadService == null) downloadService = new DownloadService(ConnectionString);
                return downloadService;
            }
        }

        /// <summary>
        /// зарвис для заливки документов (upload)
        /// </summary>
        public UploadService UploadService
        {
            get
            {
                if (uploadService == null) uploadService = new UploadService(ConnectionString);
                return uploadService;
            }
        }

        #endregion

        #region Методы

        private void DisposeResources()
        {
            if (uploadService != null) uploadService.Dispose();
            if (downloadService != null) downloadService.Dispose();
            if (searchService != null) searchService.Dispose();
            uploadService = null;
            downloadService = null;
            searchService = null;
        }

        /// <summary>
        /// освоводить ресурсы
        /// </summary>
        public void Dispose() { DisposeResources(); }

        #endregion

    }

}