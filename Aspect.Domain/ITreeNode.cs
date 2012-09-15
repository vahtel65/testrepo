using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public enum TreeNodeSection
    {
        Default,
        Custom,
        Wares,
        Dictionary
    }

    public interface ITreeNode
    {        
        Guid ID { get; set; }

        Guid? ParentID { get; set; }

        string Name { get; set; }

        TreeNodeSection Section { get; }
    }
}
