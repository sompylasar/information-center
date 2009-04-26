using LogicUtils;
using System.Linq;
using System.Data;
using System.Data.Common;
using InformationCenter.EFEngine;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public sealed class InformationCenterEngine : EF_Engine, IDbConnectionProviderBase
    {

        #region Поля

      //  private Person current = null;

        #endregion

        #region Конструкторы

        /// <summary>
        /// основной и единственный конструктор
        /// </summary>
        /// <param name="Context">обёртка над БД</param>
        public InformationCenterEngine(InformationCenterEntities Context) : base(Context) { }

        public InformationCenterEngine(string ConnectionString) : base(new InformationCenterEntities(ConnectionString)) { }

        #endregion

        #region Свойства

        public IDbConnection GetConnection(DbConnectionStringBuilder connectionStringBuilder) { return null; }

        public IDbConnection CurrentConnection { get { return Context.Connection; } }

        public string DbConnectionString
        {
            get
            {
                int a = Context.Connection.ConnectionString.IndexOf('\'');
                return Context.Connection.ConnectionString.Substring(a).Trim('\'');
            }
        }

        /// <summary>
        /// получить текущего пользователя
        /// </summary>
        /*public Person CurrentUser
        {
            get
            {
                DebugTimer t = new DebugTimer();
                t.Start();
                if (current == null) current = (Context as SchedulingEntities).CurrentUser().FirstOrDefault();
                t.ToTraceString("CurrentUser : {0}");
                return current;
            }
        }*/

        #endregion

    }

}