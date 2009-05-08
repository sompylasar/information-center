using System;
using InformationCenter.Services;

namespace InformationCenter.WebUI.Models
{
    public class ServiceCenterClient
    {
        public bool Available { get { return (_serviceCenter != null); } }
        private ServiceCenter _serviceCenter;
        public ServiceCenter ServiceCenter
        {
            get
            {
                if (!Available)
                    throw new Exception("Сервисы недоступны." +
                                        (ServiceCenterException != null ? " " + ServiceCenterException.Message : ""));
                return _serviceCenter;
            }
            protected set { _serviceCenter = value; }
        }
        public Exception ServiceCenterException { get; private set; }

        public ServiceCenterClient(string connectionString)
        {
            ServiceCenterException = new Exception(""); // to avoid null-reference exception on getting ServiceCenterException
            try
            {
                connectionString = connectionString ?? AppSettingsHelper.GetConnectionString();
                if (connectionString == null)
                    throw new Exception("Не задана строка соединения c сервером базы данных.");

                ServiceCenter = new ServiceCenter(connectionString);
            }
            catch (Exception ex)
            {
                ServiceCenter = null;
                ServiceCenterException = ex;
            }
        }
    }
}