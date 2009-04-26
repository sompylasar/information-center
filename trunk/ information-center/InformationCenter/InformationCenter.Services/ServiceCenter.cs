using InformationCenter.Services.ServicesImpl;
using System;

namespace InformationCenter.Services
{

    public class ServiceCenter : IDisposable
    {

        #region Поля

        private UploadService uploadService;
        private SearchService searchService;
        private DownloadService downloadService;
        private string connectionString = null;

        #endregion

        #region Конструкторы

        public ServiceCenter(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        #endregion

        #region События

        public event EventHandler ConnectionStringChanged;

        protected virtual void OnConnectionStringChanged()
        {
            DisposeResources();
            if (ConnectionStringChanged != null) ConnectionStringChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Свойства

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

        public SearchService SearchService
        {
            get
            {
                if (searchService == null) searchService = new SearchService(ConnectionString);
                return searchService;
            }
        }

        public DownloadService DownloadService
        {
            get
            {
                if (downloadService == null) downloadService = new DownloadService(ConnectionString);
                return downloadService;
            }
        }

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

        public void Dispose() { DisposeResources(); }

        #endregion

    }

}