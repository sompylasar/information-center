using System;
using InformationCenter.EFEngine;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public class InformationCenterEngineLoader : EF_Engine_Loader
    {

        #region Поля

        private static string meta = "metadata=res://*/Database.csdl|res://*/Database.ssdl|res://*/Database.msl;provider=System.Data.SqlClient;provider connection string=";

        #endregion

        #region Конструкторы

        public InformationCenterEngineLoader() : base() { }

        #endregion

        #region Свойства

        public override Type EntityContainerType { get { return typeof(Entities); } }

        public override Type EntityEngineType { get { return typeof(InformationCenterEngine); } }

        #endregion

        #region Методы

        public InformationCenterEngine Load(string ConnectionString, out Exception Ex)
        {
            return (InformationCenterEngine)base.Load(meta, ConnectionString, out Ex);
        }

        #endregion

    }

}