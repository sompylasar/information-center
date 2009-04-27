using System;
using InformationCenter.EFEngine;
using InformationCenter.Data;

namespace InformationCenter.Services
{

    public class InformationCenterEngineLoader : EF_Engine_Loader
    {

        private static string meta = "metadata=res://*/Database.csdl|res://*/Database.ssdl|res://*/Database.msl;provider=System.Data.SqlClient;provider connection string=";

        public InformationCenterEngineLoader() : base() { }

        public override Type EntityContainerType { get { return typeof(Entities); } }

        public override Type EntityEngineType { get { return typeof(InformationCenterEngine); } }

        public InformationCenterEngine Load(string ConnectionString, out Exception Ex)
        {
            return (InformationCenterEngine)base.Load(meta, ConnectionString, out Ex);
        }

    }

}