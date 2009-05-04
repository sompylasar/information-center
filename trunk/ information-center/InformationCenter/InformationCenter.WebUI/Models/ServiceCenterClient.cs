using System;
using InformationCenter.Services;

namespace InformationCenter.WebUI.Models
{
    public class ServiceCenterClient
    {
        public bool Available { get { return (ServiceCenter != null); } }
        public ServiceCenter ServiceCenter { get; private set; }
        public Exception ServiceCenterException { get; private set; }

        public ServiceCenterClient(string userName, string password, bool integratedSecurity)
        {
            try
            {
                ServiceCenter =
                    new ServiceCenter(AppSettings.BuildConnectionString(userName, password, integratedSecurity));
            }
            catch (Exception ex)
            {
                ServiceCenter = null;
                ServiceCenterException = ex;
            }
        }
    }
}