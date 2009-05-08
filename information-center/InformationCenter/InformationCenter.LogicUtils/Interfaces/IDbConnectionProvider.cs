using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace LogicUtils
{

    public interface IDbConnectionProviderBase
    {

        /// <summary>
        /// получить объект соединения
        /// </summary>
        /// <param name="connectionStringBuilder">контейнер для параметров соединения</param>
        /// <returns>объект соединения</returns>
        IDbConnection GetConnection(DbConnectionStringBuilder connectionStringBuilder);

        IDbConnection CurrentConnection { get; }

    }

    /// <summary>
    /// Интрерфейс для классов соединения с БД.
    /// </summary>
    public interface IDbConnectionProvider : IDbConnectionProviderBase
    {

        /// <summary>
        /// событие возникает, когда для соединения с БД требуется новый пароль
        /// </summary>
        event EventHandler<ReadWriteEventArgs<RefObject<string>>> NewPasswordRequired;

    }

}
