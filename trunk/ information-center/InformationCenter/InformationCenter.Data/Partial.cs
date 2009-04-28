using System;
using System.Data.Objects.DataClasses;

namespace InformationCenter.Data
{

    public partial class User : EntityObject
    {

        public override string ToString()
        {
            return "Иванов Иван Иванович";
        }


    }

}