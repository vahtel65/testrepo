using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

using Aspect.Domain;

namespace Aspect.Model.Classification
{
    public class ClassificationProvider : Aspect.Domain.CommonDomain, ITreeProvider
    {
        private List<Guid> deniedNodes = null;
        protected virtual List<Guid> GetDeniedNodes(List<Guid> roles)
        {
                if (deniedNodes == null)
                {
                    var d = from uv in RoleViewPermissions
                            where uv.PermissionEntityID == PermissionEntity.ClassificationTree && !uv.Read
                            && roles.Contains(uv.RoleID)
                            select uv.EntityID;
                    deniedNodes = d.ToList();
                }
                return deniedNodes;
        }
        public virtual List<ITreeNode> GetList(Guid parentID, Guid userID, List<Guid> roles)
        {
            //---
            /*if (deniedNodes == null)
            {
                var d = from uv in RoleViewPermissions
                        where uv.PermissionEntityID == PermissionEntity.ClassificationTree && !uv.Read
                        && roles.Contains(uv.RoleID)
                        select uv.EntityID;
                deniedNodes = d.ToList();
            }*/
            //---
            var q = from c in ClassificationTrees
                    where (c.ParentID == parentID || (!c.ParentID.HasValue && parentID.Equals(Guid.Empty)))                    
                    select c;            
            List<ClassificationTree> list = q.ToList();
            if (Guid.Empty.Equals(parentID))
            {
                // Выносим пункт "Изделия" на вверх дерева
                list = list.OrderBy(item => item.Name).ToList();
            }
            list = list.Where(l => !GetDeniedNodes(roles).Contains(l.ID)).ToList();
            return list.ConvertAll(delegate(ClassificationTree n) { return (ITreeNode)n; });
        }

        public virtual ITreeNode GetTreeNode(Guid id)
        {
            return ClassificationTrees.SingleOrDefault(s => s.ID == id);
        }
    }
}
