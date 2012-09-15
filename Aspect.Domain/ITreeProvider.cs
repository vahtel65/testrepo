using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public interface ITreeProvider : IDisposable
    {
        List<ITreeNode> GetList(Guid parentID, Guid userID, List<Guid> roles);

        ITreeNode GetTreeNode(Guid id);
    }
}