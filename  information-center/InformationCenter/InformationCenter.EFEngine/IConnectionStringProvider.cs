using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationCenter.EFEngine
{

    public interface IConnectionStringProvider
    {
        string ConnectionString { get; }
    }

}
