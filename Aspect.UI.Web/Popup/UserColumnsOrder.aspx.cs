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
    public partial class UserColumnsOrder : ContentPageBase
    {
        protected FieldPlaceHolderEnum FieldPlaceHolder
        {
            get
            {
                return FieldPlaceHolderEnum.Grid;
                //return (FieldPlaceHolderEnum)Enum.Parse(typeof(FieldPlaceHolderEnum), Request[RequestKeyFieldPlaceHolder]);
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

        public class Source
        {
            public Guid ID { get; set; }
            public string Text { get; set; }
        }

        protected ListBox OrderList;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ContentDomain provider = null;
                try
                {
                    provider = Aspect.Model.Common.GetContentDomain(ClassifiacationTypeView);
                    if (provider != null)
                    {
                        List<GridColumn> list = provider.GetGridColumns(this.User.ID, this.RequestClassificationTreeID, this.FieldPlaceHolderID);
                        //--
                        List<Source> source = list.ConvertAll(
                            delegate(GridColumn n)
                            {
                                if (!n.IsDictionary) return new Source() { ID = n.ID, Text = n.Name };
                                else return new Source() { ID = n.ID, Text = string.Format("{1} - {0}", n.Name, n.Group) };
                            });
                        //--
                        OrderList.DataSource = source.Where(s => !s.ID.Equals(Guid.Empty)); //list;
                        OrderList.DataBind();
                    }
                }
                finally
                {
                    if (provider != null) provider.Dispose();
                }
            }
        }
        protected void Save_Click(object sender, EventArgs e)
        {
            ContentDomain provider = null;
            try
            {
                provider = Aspect.Model.Common.GetContentDomain(ClassifiacationTypeView);
                if (provider != null)
                {
                    int index = 0;
                    foreach (ListItem item in OrderList.Items)
                    {
                        Guid id = new Guid(item.Value);
                        index++;
                        provider.SetGridColumnOrder(id, index);
                    }
                }
            }
            finally
            {
                if (provider != null) provider.Dispose();
            }
            string script = @"
<script language=JavaScript>
    self.parent.tb_remove();
    self.parent.refresh();
</script>
            ";
            //script = string.Format(script, Server.UrlDecode(this.Request["url"]));
            this.ClientScript.RegisterStartupScript(this.GetType(), "mainview", script);
        }

        protected void Up_Click(object sender, EventArgs e)
        {
            ListItem item = OrderList.SelectedItem;
            if (item != null)
            {
                int index = OrderList.SelectedIndex - 1;
                if (index >= 0)
                {
                    OrderList.Items.Remove(item);
                    OrderList.Items.Insert(index, item);
                }
            }
        }

        protected void Down_Click(object sender, EventArgs e)
        {
            ListItem item = OrderList.SelectedItem;
            if (item != null)
            {
                int index = OrderList.SelectedIndex + 1;
                if (index < OrderList.Items.Count)
                {
                    OrderList.Items.Remove(item);
                    OrderList.Items.Insert(index, item);
                }
            }
        }
    }
}
