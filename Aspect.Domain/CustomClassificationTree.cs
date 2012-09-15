using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public partial class CustomClassificationTree : ITreeNode
    {
        public TreeNodeSection Section
        {
            get
            {
                return TreeNodeSection.Custom;
            }
        }

        string ITreeNode.Name
        {
            get
            {
                if (this.CustomClassificationNodeID.HasValue)
                    return this.CustomClassificationNode.Name;
                else if (this.ClassificationTreeID.HasValue)
                    return this.ClassificationTree.Name;
                else return this.Name;
            }
            set
            {
                
            }
        }
    }
}
