using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Aspect.Model.Classification;
using Aspect.Model.ProductDomain;
using Aspect.Domain;
using Aspect.Model;
using Aspect.UI.Web.Controls;

namespace Aspect.UI.Web.Callback
{
    public partial class ProductCard : Basic.ContentPageBase
    {
        protected Guid ProductID
        {
            get
            {
                return new Guid(Request[RequestKeyProductID]);
            }
        }

        protected Repeater ProductCard1;
        protected Repeater ProductCard2;

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Pair<string, object>> source = new List<Pair<string, object>>();
            DataRow dr = null;
            ContentDomain provider = null;
            List<GridColumn> columns = new List<GridColumn>();
            List<ITreeNode> list = new List<ITreeNode>();
            try
            {
                provider = Common.GetContentDomain(ClassifiacationTypeView);
                if (provider != null)
                {
                    dr = provider.GetEntity(ProductID, this.User.ID, this.RequestClassificationTreeID);
                    columns = provider.GetGridColumns(this.User.ID, this.RequestClassificationTreeID, FieldPlaceHolder.GridCard);
                    list = provider.GetProductParents(this.ProductID);
                }


                if (dr != null && provider != null)
                {
                    foreach (GridColumn clm in columns)
                    {
                        if (clm.Group != clm.Name) clm.Name = string.Format("{1} - {0}", clm.Name, clm.Group);
                        if (clm.ClassificationID.HasValue)
                        {
                            ITreeNode node = list.SingleOrDefault(l => l.ID == clm.ClassificationID);
                            if (node != null) clm.Name = string.Format("{0} ({1})", clm.Name, node.Name);
                        }
                        
                        source.Add(new Pair<string, object>(clm.Name, dr[clm.DataItem]));
                    }
                    source = source.OrderBy(s => s.First).ToList();
                    if (source.Count > 1)
                    {
                        ProductCard1.DataSource = source.Take(source.Count / 2);
                        ProductCard1.DataBind();
                        ProductCard2.DataSource = source.Skip(source.Count / 2);
                        ProductCard2.DataBind();
                    }
                    else
                    {
                        ProductCard1.DataSource = source;
                        ProductCard1.DataBind();
                        ProductCard2.Visible = false;
                    }
                }
            }
            finally
            {
                if (provider != null) provider.Dispose();
            }
        }
    }
}
