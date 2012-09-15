using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

using Aspect.Domain;


namespace Aspect.Model.Classification
{
    public class CustomClassificationProvider : ClassificationProvider/*Aspect.Domain.CommonDomain*/, ITreeProvider
    {
        private List<Guid> deniedNodes = null;
        protected override List<Guid> GetDeniedNodes(List<Guid> roles)
        {
            if (deniedNodes == null)
            {
                var d = from uv in RoleViewPermissions
                        where uv.PermissionEntityID == PermissionEntity.CustomClassificationTree && !uv.Read
                        && roles.Contains(uv.RoleID)
                        select uv.EntityID;
                deniedNodes = d.ToList();
            }
            return deniedNodes;
        }
        public override List<ITreeNode> GetList(Guid parentID, Guid userID, List<Guid> roles)
        {
            if (CustomClassificationTrees.SingleOrDefault(d => d.ID == parentID) == null && !parentID.Equals(Guid.Empty))
            {
                return base.GetList(parentID, userID,roles);
            }
            var q = from c in CustomClassificationTrees
                    where c.ParentID == parentID || (!c.ParentID.HasValue && parentID.Equals(Guid.Empty))
                    select c;
            List<CustomClassificationTree> list = new List<CustomClassificationTree>();
            List<CustomClassificationTree> nodes = q.ToList();
            List<Guid> baseDeniedNodes = base.GetDeniedNodes(roles);
            list.AddRange(nodes.Where(l => !l.ClassificationTreeID.HasValue && !GetDeniedNodes(roles).Contains(l.ID)));
            list.AddRange(nodes.Where(l => l.ClassificationTreeID.HasValue && !baseDeniedNodes.Contains(l.ClassificationTreeID.Value)));
            return list.ConvertAll(delegate(CustomClassificationTree n) 
            {
                if (n.ClassificationTreeID.HasValue)
                {
                    return (ITreeNode)n.ClassificationTree;
                }
                return (ITreeNode)n; 
            });
        }
        public override ITreeNode GetTreeNode(Guid id)
        {
            return CustomClassificationTrees.SingleOrDefault(s => s.ID == id);
        }
        public void DeleteProducts(Guid userID, Guid treeID, List<Guid> ids)
        {
            CustomClassificationTree tree = CustomClassificationTrees.SingleOrDefault(c => c.ID == treeID);
            if (tree != null)
            {
                CustomClassificationNode node = tree.CustomClassificationNode;
                if (!tree.CustomClassificationNodeID.HasValue)
                {
                    return;
                }
                foreach (Guid id in ids)
                {
                    //check if product already exists in the node
                    List<CustomClassificationNodeProduct> list = CustomClassificationNodeProducts.Where(c => c.CustomClassificationNodeID == node.ID && c.ProductID == id).ToList();
                    foreach (CustomClassificationNodeProduct item in list)
                    {
                        Aspect.Utility.TraceHelper.Log(userID, "Продукт: {0}. Удаление из пользовательского класса. В CustomClassificationNode {1}. ", item.ProductID, item.CustomClassificationNodeID);
                    }

                    CustomClassificationNodeProducts.DeleteAllOnSubmit(list);
                    this.SubmitChanges();
                    /*if (CustomClassificationNodeProducts.Where(c => c.CustomClassificationNodeID == node.ID && c.ProductID == id).Count() == 0)
                    {
                        CustomClassificationNodeProduct entity = new CustomClassificationNodeProduct()
                        {
                            ID = Guid.NewGuid(),
                            ProductID = id,
                            CustomClassificationNodeID = node.ID
                        };
                        CustomClassificationNodeProducts.InsertOnSubmit(entity);
                        this.SubmitChanges();
                    }*/
                }
            }
        }
        
    }
}
