using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public partial class _dictUM
    {
        public static Guid defaultValue = new Guid("68CD2019-85F6-4E52-AEFE-09CA5C2B64F3");

        partial void OnLoaded()
        {
            
        }

        public string PublicName
        {
            get
            {
                return this.umn1;
                //return string.Format("{0}. {1}", this.umn1, this.umn2);
            }
        }
    }
}
