using System;
using System.Data.SqlClient;

namespace InformationCenter.Services
{

    /// <summary>
    /// ��������� ����� ��� ���������� ��������.
    /// </summary>
    public class ServiceCenter : IDisposable
    {

        #region ����

        private ServiceSet set;
        private string connectionString = null;

        #endregion

        #region ������������

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="ConnectionString">������ ���������� � ������</param>
        public ServiceCenter(string ConnectionString)
        {
            connectionString = ConnectionString;
            using (SqlConnection con = new SqlConnection(ConnectionString)) con.Open();
        }

        #endregion

        #region ��������

        private ServiceSet InternalService
        {
            get
            {
                if (set == null) set = new ServiceSet(ConnectionString);
                return set;
            }
        }

        /// <summary>
        /// ������ ����������
        /// </summary>
        public string ConnectionString { get { return connectionString; } }      

        /// <summary>
        /// ������ ��� ������
        /// </summary>
        public ISearchService SearchService { get { return InternalService; } }

        /// <summary>
        /// ������ ��� ���������� ���������� (download)
        /// </summary>
        public IDownloadService DownloadService { get { return InternalService; } }

        /// <summary>
        /// ������ ��� ������� ���������� (upload)
        /// </summary>
        public IUploadService UploadService { get { return InternalService; } }

        /// <summary>
        /// ������ ��� ������ � ���������
        /// </summary>
        public IDocumentDescriptionService DocumentDescriptionService { get { return InternalService; } }

        #endregion

        #region ������

        /// <summary>
        /// ���������� �������
        /// </summary>
        public void Dispose()
        {
            if (set != null) set.Dispose();
            set = null; ;
        }

        /// <summary>
        /// ������������� � ������
        /// </summary>
        /// <returns>������</returns>
        public override string ToString() { return "�������������� ����� ����"; }

        #endregion

    }

}