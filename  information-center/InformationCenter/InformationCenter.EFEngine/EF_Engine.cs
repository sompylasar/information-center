using System;
using LogicUtils;
using System.Data.Objects;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using InformationCenter.Data;

namespace InformationCenter.EFEngine
{

    public class EF_Engine : IRequestable, IDisposable
    {

        #region Поля

        private ObjectContextEx context = null;

        #endregion

        #region Конструкторы

        /// <summary>
        /// основной и единственный конструктор
        /// </summary>
        /// <param name="Context">обёртка над БД Skif_BP</param>
        public EF_Engine(ObjectContextEx Context)
        {
            if (Context == null) throw new ArgumentNullException("Context");
            context = Context;
        }

        #endregion

        #region Свойства

        /// <summary>
        /// режим разрешения конфликтов
        /// </summary>
        public RefreshMode ConflictModeResolveOnSave { get; set; }

        protected ObjectContextEx Context { get { return context; } }

        #endregion

        #region Методы

        protected string DbConnectionString
        {
            get
            {
                IConnectionStringProvider pr = this as IConnectionStringProvider;
                if (pr != null) return pr.ConnectionString;
                else
                {
                    int a = context.Connection.ConnectionString.IndexOf('\'');
                    return context.Connection.ConnectionString.Substring(a).Trim('\'');
                }
            }
        }

        public IQueryable<T> Get<T>(IEnumerable<Guid> Identifiers) where T : EntityObject
        {
            string tName = typeof(T).Name;
            string alias = tName + "_";
            string guids = STD.EnumerateGuidsInDbForm(Identifiers);
            StringBuilder sb = new StringBuilder(1024);
            if (guids != null)
                sb.Append("SELECT VALUE ").Append(alias).Append(" FROM ").Append(context.DefaultContainerName).
                    Append(".").Append(tName).Append(" AS ").Append(alias).Append(" WHERE ").Append(alias).
                    Append(".ID IN {").Append(guids).Append("}");
            else sb.Append("SELECT VALUE TOP (0) ").Append(alias).Append(" FROM ").Append(context.DefaultContainerName).
                Append(".").Append(tName).Append(" AS ").Append(alias);
            return context.CreateQuery<T>(sb.ToString());
        }

        /// <summary>
        /// сделать выборку по данным
        /// </summary>
        /// <param name="ProcedureName">имя хранимой процедуры</param>
        /// <param name="Delegate">делегат для работы с полученными данными</param>
        /// <param name="Params">параметры для процедуры</param>
        public void DoFetch(string ProcedureName, DbDataReaderDelegate Delegate, params DbParameter[] Params)
        {
            using (new DebugTimer("DoFetch(" + ProcedureName + ") : {0}"))
            {
                DbConnection cn = null;
                DbDataReader reader = null;
                Exception Exc = null;
                DbCommand cmd = null;
                try
                {
                    cn = new SqlConnection(DbConnectionString);
                    cn.Open();
                    cmd = cn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = ProcedureName;
                    if (Params != null && Params.Length != 0) cmd.Parameters.AddRange(Params);
                    reader = cmd.ExecuteReader();// (CommandBehavior.SequentialAccess);
                    Delegate(reader);
                    reader.Close();
                }
                catch (Exception Ex) { Exc = Ex; }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                    if (reader != null) reader.Dispose();
                    if (cmd != null) cmd.Dispose();
                    if (cn != null)
                    {
                        cn.Close();
                        cn.Dispose();
                    }
                }
                if (Exc != null) throw Exc;
            }
        }       

        public ObjectQuery<T> CreateQuery<T>() where T : EntityObject { return context.CreateQuery<T>("[" + typeof(T).Name + "]"); }

        public ObjectQuery<T> CreateQuery<T>(string QueryString, params ObjectParameter[] Parameters) { return context.CreateQuery<T>(QueryString, Parameters); }

        public ObjectResult<T> ExecuteFunction<T>(string FunctionName, params ObjectParameter[] Parameters) where T : IEntityWithChangeTracker
        {
            using (new DebugTimer("ExecuteFunction<" + typeof(T) + ">(" + FunctionName + ") : {0}"))
                return context.ExecuteFunctionA<T>(FunctionName, Parameters);
        }

        public T GetByKey<T>(EntityKey Key) { return (T)GetByKey(Key); }

        public object GetByKey(EntityKey Key) { return context.GetObjectByKey(Key); }

        public bool TryGetByKey<T>(EntityKey Key, out T Entity)
        {
            object temp = null;
            bool result = TryGetByKey(Key, out temp);
            Entity = (T)temp;
            return result;
        }

        public bool TryGetByKey(EntityKey Key, out object Entity) { return context.TryGetObjectByKey(Key, out Entity); }

        public void AddObject(string EntitySetName, object Entity)
        {
            if (Entity == null) throw new ArgumentNullException("Entity");
            context.AddObject(EntitySetName, Entity);
        }

        public void Refresh(RefreshMode RefreshMode, IEnumerable Collection)
        {
            if (Collection == null) throw new ArgumentNullException("Collection");
            context.Refresh(RefreshMode, Collection);
        }

        public void Refresh(RefreshMode RefreshMode, object Entity)
        {
            if (Entity == null) throw new ArgumentNullException("Entity");
            using (new DebugTimer("Refresh(" + RefreshMode.ToString() + ", " + Entity.ToString() + ") : {0}"))
                context.Refresh(RefreshMode, Entity);
        }

        public Exception SaveChanges()
        {
            UpdateException result = null;
            try
            {
                try
                {
                    context.SaveChanges();
                }
                catch (OptimisticConcurrencyException Ex)
                {
                    foreach (ObjectStateEntry ose in Ex.StateEntries)
                    {
                        context.Refresh(ConflictModeResolveOnSave, ose.Entity);
                    }
                }
            }
            catch (UpdateException Ex)
            {
                result = Ex;
            }
            return result;
        }

        public EntityKey CreateEntityKey(string EntitySetName, object EntityObject)
        {
            return context.CreateEntityKey(EntitySetName, EntityObject);
        }

        public void Dispose() { context.Dispose(); }

        #endregion

    }

}