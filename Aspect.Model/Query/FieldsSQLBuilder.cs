using System;
using System.Collections.Generic;
//using System.Collections;
//using System.Linq;
using System.Text;

namespace Aspect.Model.Query
{
    internal abstract class FieldsSQLBuilder
    {
        /// <summary>
        /// From clause
        /// </summary>
        public StringBuilder From { get; protected set; }

        /// <summary>
        /// Join clause
        /// </summary>
        public StringBuilder Join { get; protected set; }
    }
}
