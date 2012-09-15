using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;

namespace Aspect.Domain
{
    public partial class Property : IBoundField
    {
        partial void OnLoaded()
        {
            //this.Alias = string.Format("p_{0}", this.Alias);
            this._Alias = string.Format("p_{0}", this.Alias);
        }

        public bool Selected { get; set; }
    }
}
