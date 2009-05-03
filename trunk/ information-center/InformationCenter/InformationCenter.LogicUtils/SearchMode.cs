using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace LogicUtils
{

    /// <summary>
    /// Режим поиска.
    /// </summary>
    [Description("Режим поиска")]
    public enum SearchMode
    {

        /// <summary>
        /// И
        /// </summary>
        [Description("И")]
        And,

        /// <summary>
        /// Или
        /// </summary>
        [Description("Или")]
        Or

    }

}
