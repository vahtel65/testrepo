using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspect.Domain;
using Aspect.Model.UserDomain;
using Aspect.Model.Classification;

namespace Aspect.UI.Web.Popup
{
    public partial class SelectTreeClass : Basic.PageBase
    {            
        protected void Page_Load(object sender, EventArgs e)
        {
            using (ITreeProvider provider = new ClassificationProvider())
            {
                this.BindClassView(provider, Guid.Empty, null, new Guid("55c7b455-0638-4acb-ac2e-5b4992e48462"));
            }
        }

        protected void BindClassView(ITreeProvider provider, Guid parentID, Ext.Net.TreeNode parentNode, Guid selectedID)
        {
            List<ITreeNode> list = new List<ITreeNode>();
            if (!parentID.Equals(Guid.Empty) && parentNode == null)
            {
                ITreeNode entity = provider.GetTreeNode(parentID);
                if (entity == null) return;
                list.Add(entity);
            }
            else
            {
                list = provider.GetList(parentID, User.ID, Roles);
                //List<ITreeNode> list = provider.GetList(parentID);
            }
            foreach (ITreeNode item in list)
            {
                Ext.Net.TreeNode treeNode;
                treeNode = new Ext.Net.TreeNode(item.ID.ToString(), item.Name, Ext.Net.Icon.Folder);
                treeNode.Cls = "TreeNode-Default";                                
                if (parentNode != null) parentNode.Nodes.Add(treeNode);
                else (ClassView.Root[0] as Ext.Net.TreeNode).Nodes.Add(treeNode);

                if (item.ID == selectedID)
                {
                    //this.selectedNodeID = this.RequestClassificationTreeID;
                    //this.selectedNodeText = item.Name;
                    ClassView.SelectNode(treeNode.NodeID);
                    Ext.Net.TreeNodeBase tmpParent = treeNode.ParentNode;
                    while (tmpParent != null)
                    {
                        tmpParent.Expanded = true;
                        tmpParent = tmpParent.ParentNode;
                    }

                    //ClassView.SelectNode(treeNode.NodeID);
                    //ClassView.ExpandChildNodes(treeNode.NodeID);
                }
                this.BindClassView(provider, item.ID, treeNode, selectedID);
            }
        }
    }
}
