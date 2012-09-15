using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspect.UI.Web.Basic;

using Aspect.Model.DictionaryDomain;
using Aspect.Domain;

namespace Aspect.UI.Web.Popup
{
    public partial class UserColumns : ContentPageBase
    {

        protected FieldPlaceHolderEnum FieldPlaceHolder
        {
            get
            {
                return (FieldPlaceHolderEnum)Enum.Parse(typeof(FieldPlaceHolderEnum), Request[RequestKeyFieldPlaceHolder]);
            }
        }
        protected Guid FieldPlaceHolderID
        {
            get
            {
                return Aspect.Domain.FieldPlaceHolder.GetFieldPlaceHolderID(this.FieldPlaceHolder);
            }
        }

        protected Guid RequestProductID
        {
            get
            {
                try
                {
                    return new Guid(this.Request["productid"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }

        protected List<Guid> PreviouseProperties
        {
            get
            {
                return (List<Guid>)ViewState["PreviouseProperties"];
            }
            set
            {
                ViewState["PreviouseProperties"] = value;
            }
        }

        protected List<Guid> PreviouseDictionaries
        {
            get
            {                
                List<Guid> list = (List<Guid>)ViewState["PreviouseDictionaries"];
                return list == null ? new List<Guid>() : list;
            }
            set
            {
                ViewState["PreviouseDictionaries"] = value;
            }
        }

        protected LinkButton Save;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (ContentDomain provider = Aspect.Model.Common.GetContentDomain(ClassifiacationTypeView))
                {                    
                    if (ClassifiacationTypeView == Aspect.Model.ClassifiacationTypeView.Dictionary)
                    {
                        DictionaryTree item = provider.GetDictionaryTreeNode(RequestClassificationTreeID);
                        TreeNode node = new TreeNode(item.Name, item.ID.ToString());
                        node.SelectAction = TreeNodeSelectAction.Expand;
                        DictionaryTreeView.Nodes.Add(node);
                        this.BindDictionaryTreeView(RequestClassificationTreeID, node, provider, new List<ITreeNode>());
                    }
                    else
                    {
                        List<ITreeNode> list = provider.GetProductParents(this.RequestProductID);
                        this.BindDictionaryTreeView(Guid.Empty, null, provider, list);
                        this.BindGeneralProperty(provider, list);
                    }
                }                
            }
        }

        protected Repeater GeneralColumnRepeater;
        public void BindGeneralProperty(ContentDomain provider, List<ITreeNode> parents)
        {
            //List<Property> source = provider.Properties.ToList();
            List<Property> source = provider.GetAvailableProperties(this.Roles);
            List<Property> fields = provider.GetUserProperties(this.User.ID, this.RequestClassificationTreeID, this.FieldPlaceHolderID);
            this.PreviouseProperties = fields.Select(f => f.ID).ToList();

            var q = from s in source.Where(f => f.ClassificationID.HasValue)
                    join l in parents on s.ClassificationID.Value equals l.ID
                    select new Property()
                    {
                        ID = s.ID,
                        Name = string.Format("{0} ({1})", s.Name, l.Name)
                    };
            source = q.ToList();

            foreach (Property item in fields)
            {                
                Property entity = source.Find(f => f.ID == item.ID);
                if (entity != null) entity.Selected = true;
            }
            GeneralColumnRepeater.DataSource = source;
            GeneralColumnRepeater.DataBind();
        }

        protected TreeView DictionaryTreeView;
        public void BindDictionaryTreeView(Guid parentID, TreeNode parentNode, ContentDomain domain, List<ITreeNode> parents)
        {
            using (DictionaryProvider provider = new DictionaryProvider())
            {
                //bind child dictionaries
                List<ITreeNode> list = new List<ITreeNode>();
                if (parentID.Equals(Guid.Empty))
                {
                    List<DictionaryTree> list1 = provider.GetDictionaryTreeList(parentID, this.Roles);
                    var q = from l in list1.Where(f => f.ClassificationID.HasValue)
                            join m in parents on l.ClassificationID.Value equals m.ID
                            select new DictionaryTree
                            {
                                ID = l.ID,
                                ParentID = l.ParentID,
                                Name = string.Format("{0} ({1})", l.Name, m.Name)
                            } as ITreeNode;
                    //l as ITreeNode;
                    list = q.ToList();
                }
                else
                {
                    list = provider.GetList(parentID, User.ID, Roles);
                }
                foreach (ITreeNode item in list)
                {
                    TreeNode node = new TreeNode(item.Name, item.ID.ToString());
                    node.SelectAction = TreeNodeSelectAction.Expand;
                    if (parentNode != null) parentNode.ChildNodes.Add(node);
                    else DictionaryTreeView.Nodes.Add(node);

                    this.BindDictionaryTreeView(item.ID, node, domain, parents);
                }
                //bind fields
                if (parentNode != null && !parentID.Equals(Guid.Empty))
                {
                    DictionaryTree entity = provider.DictionaryTrees.SingleOrDefault(d => d.ID == parentID);
                    List<IUserField> fields = domain.GetUserFields(this.User.ID, RequestClassificationTreeID, this.FieldPlaceHolderID).Where(uf => uf.DictionaryTreeID == parentID).ToList();

                    List<Guid> previous = this.PreviouseDictionaries;
                    previous.AddRange(fields.Select(f => f.DictionaryProperty.ID));
                    this.PreviouseDictionaries = previous;

                    //List<DictionaryProperty> source = entity.Dictionary.DictionaryProperties.ToList();
                    List<DictionaryProperty> source = provider.GetAvailableDictionaryProperties(this.Roles, entity.DictionaryID);

                    foreach (DictionaryProperty prop in source)
                    {
                        TreeNode node = new TreeNode(prop.Name, prop.ID.ToString());
                        node.SelectAction = TreeNodeSelectAction.None;
                        IUserField field = fields.Find(f => f.DictionaryProperty.ID == prop.ID);
                        if (field != null)
                        {
                            node.Checked = true;
                            node.Value = string.Format("{0},{1}", node.Value, field.Sequence);
                        }
                        else node.Checked = false;
                        parentNode.ChildNodes.AddAt(0, node);
                    }
                }
                //--
            }
        }
        private int orderCounter = 700;
        protected void Save_Click(object sender, EventArgs e)
        {
            using (ContentDomain provider = Aspect.Model.Common.GetContentDomain(ClassifiacationTypeView))
            {            
                if (ClassifiacationTypeView != Aspect.Model.ClassifiacationTypeView.Dictionary)
                {
                    List<Guid> oldProperties = this.PreviouseProperties;
                    List<Guid> helperList = new List<Guid>();
                    foreach (RepeaterItem item in GeneralColumnRepeater.Items)
                    {
                        if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                        {
                            if ((item.FindControl("Seleteced") as CheckBox).Checked)
                            {
                                Guid id = new Guid((item.FindControl("HiddenID") as HiddenField).Value);

                                if (!oldProperties.Contains(id))
                                {
                                    orderCounter++;
                                    provider.AddUserProperty(User.ID, this.RequestClassificationTreeID, FieldPlaceHolderID, id, orderCounter);
                                }
                                else
                                {
                                    helperList.Add(id);
                                }
                            }
                        }
                    }
                    helperList = oldProperties.Where(p => !helperList.Contains(p)).ToList();
                    provider.DeleteUserProperties(User.ID, this.RequestClassificationTreeID, FieldPlaceHolderID, helperList);
                }
                //---
                /*provider.DeleteUserProperties(User.ID, this.RequestClassificationTreeID, FieldPlaceHolderID, new List<Guid>());
                foreach (RepeaterItem item in GeneralColumnRepeater.Items)
                {
                    if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                    {
                        if ((item.FindControl("Seleteced") as CheckBox).Checked)
                        {
                            Guid id = new Guid((item.FindControl("HiddenID") as HiddenField).Value);
                            orderCounter++;
                            provider.AddUserProperty(User.ID, this.RequestClassificationTreeID, FieldPlaceHolderID, id, orderCounter);
                        }
                    }
                }*/
                //provider.DeleteUserFields(this.User.ID, this.RequestClassificationTreeID, this.FieldPlaceHolderID);
                this.SaveDictionaryFields(DictionaryTreeView.Nodes, provider);
                helperDictList = this.PreviouseDictionaries.Where(p => !helperDictList.Contains(p)).ToList();
                provider.DeleteUserFields(this.User.ID, this.RequestClassificationTreeID, this.FieldPlaceHolderID, helperDictList);
            }
            string script = @"
<script language=JavaScript>
    self.parent.tb_remove();
    self.parent.refresh();
</script>
            ";
            script = string.Format(script, Server.UrlDecode(this.Request["url"]));
            this.ClientScript.RegisterStartupScript(this.GetType(), "mainview", script);
        }

        private List<UserField> userFields = new List<UserField>();
        List<Guid> helperDictList = new List<Guid>();
        private void SaveDictionaryFields(TreeNodeCollection nodes, ContentDomain provider)
        {
            List<Guid> oldDictionaries = this.PreviouseDictionaries;            

            foreach (TreeNode item in nodes)
            {
                if (item.Checked && item.Parent != null)
                {                    
                    string[] vals = item.Value.Split(',');
                    Guid id = new Guid(vals[0]);
                    int order = orderCounter;
                    if (vals.Length > 1 ) order = Convert.ToInt32(vals[1]);
                    if (!oldDictionaries.Contains(id))
                    {
                        orderCounter++;
                        provider.AddUserField(this.User.ID, RequestClassificationTreeID, id, new Guid(item.Parent.Value), FieldPlaceHolderID, /*orderCounter*/order);
                    }
                    else
                    {
                        helperDictList.Add(id);
                    }                    
                }                
                this.SaveDictionaryFields(item.ChildNodes, provider);
            }            
        }
    }
}
