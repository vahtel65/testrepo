using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public partial class DictionaryTree : ITreeNode
    {
        partial void OnLoaded()
        {
            //this.Alias = string.Format("dp_{0}", this.Alias);
            this._Alias = string.Format("dp_{0}", this.Alias);
        }

        public TreeNodeSection Section
        {
            get
            {
                return TreeNodeSection.Dictionary;
            }
        }
    }
}