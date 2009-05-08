using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;

namespace InformationCenter.Data
{

    public class ObjectContextEx : ObjectContext
    {

        #region Конструкторы

        public ObjectContextEx(string ConnectionString) : base(ConnectionString) { }

        public ObjectContextEx(string ConnectionString, string DefaultContainerName) : base(ConnectionString, DefaultContainerName) { }

        public ObjectContextEx(EntityConnection Connection) : base(Connection) { }

        public ObjectContextEx(EntityConnection Connection, string DefaultContainerName) : base(Connection, DefaultContainerName) { }

        #endregion

        #region Методы

        public ObjectResult<T> ExecuteFunctionA<T>(string FunctionName, params ObjectParameter[] Parameters) where T : IEntityWithChangeTracker
        {
            return base.ExecuteFunction<T>(FunctionName, Parameters);
        }

        #endregion

    }

}