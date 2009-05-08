using System;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace InformationCenter.WebUI.Models
{
    public static class AuthHelper
    {
        public static bool NeedRedirectToAuth(Controller controller)
        {
            Exception exception;
            bool isConnectionStringFallback;
            TryLogOnToServiceCenter(controller, out exception, out isConnectionStringFallback);

            if (exception != null)
            {
                controller.Session["ReturnRedirect"] = new RedirectResult(controller.Request.Url.AbsoluteUri);
                return true;
            }
            return false;
        }

        public static bool TryLogOnToServiceCenter(Controller controller, out Exception exception, out bool isConnectionStringFallback)
        {
            isConnectionStringFallback = false;
            try
            {
                HttpSessionStateBase session = controller.Session;

                bool useConnectionStringConfig = ((bool?)session["UseConnectionStringConfig"]) ?? true;

                if (!useConnectionStringConfig || !AppSettingsHelper.IsConnectionStringConfigured())
                {
                    //throw new Exception("В настройках Web.config не задана строка соединения \"" + AppSettingsHelper.ConnectionStringSettingsName + "\".");

                    isConnectionStringFallback = true;

                    session["ConnectionString"] = AppSettingsHelper.BuildConnectionString(
                        @".\SQLEXPRESS", "InformationCenter",
                        (string)session["UserName"], (string)session["Password"], ((bool?)session["IntegratedSecurity"]) ?? true);

                    AppSettingsHelper.ConnectionString = ((string)session["ConnectionString"]);
                }
                else
                {
                    session["ConnectionString"] = null;
                }
            
                var client = new ServiceCenterClient((string)session["ConnectionString"]);

                if (!client.Available) 
                    throw client.ServiceCenterException;

                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                if (isConnectionStringFallback)
                {
                    ex = new Exception("В настройках Web.config не задана строка соединения \"" +
                                        AppSettingsHelper.ConnectionStringSettingsName + "\""
                                        + ", и соединение с введенными значениями не удалось.", ex);
                }

                exception = new Exception("Ошибка подключения к сервисам." + " " + ex.Message
                    + (AppSettingsHelper.IsConnectionStringConfigured() ? " Проверьте правильность строки соединения в настройках Web.config." : ""));
            }
            return false;
        }
    }
}