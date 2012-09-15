using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public partial class ClassificationTree : ITreeNode
    {
        public TreeNodeSection Section
        {
            get
            {
                return TreeNodeSection.Default;
            }
        }
    }
}
