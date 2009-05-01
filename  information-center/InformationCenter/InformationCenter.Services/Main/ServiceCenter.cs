using InformationCenter.Services.ServicesImpl;
using System;

namespace InformationCenter.Services
{

    /// <summary>
    /// ��������� ����� ��� ���������� ��������.
    /// </summary>
    public class ServiceCenter : IDisposable
    {

        #region ����

        private UploadService uploadService;
        private SearchService searchService;
        private DownloadService downloadService;
        private string connectionString = null;

        #endregion

        #region ������������

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="ConnectionString">������ ���������� � ������</param>
        public ServiceCenter(string ConnectionString) { connectionString = ConnectionString; }

        #endregion

        #region �������

        /// <summary>
        /// ������� ��������� ������ �����������
        /// </summary>
        public event EventHandler ConnectionStringChanged;

        protected virtual void OnConnectionStringChanged()
        {
            DisposeResources();
            if (ConnectionStringChanged != null) ConnectionStringChanged(this, EventArgs.Empty);
        }

        #endregion

        #region ��������

        /// <summary>
        /// ������ ����������
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
        /// ������ ��� ������
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
        /// ������ ��� ���������� ���������� (download)
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
        /// ������ ��� ������� ���������� (upload)
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

        #region ������

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
        /// ���������� �������
        /// </summary>
        public void Dispose() { DisposeResources(); }

        #endregion

    }

}