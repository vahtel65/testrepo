using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Aspect.Model.ConfigurationDomain;
using Aspect.Domain;

namespace Aspect.UI.Web.Configuration
{
    public partial class Usage : Basic.PageBase
    {
        protected Guid ProductID
        {
            get
            {
                try
                {
                    return new Guid(Request["ID"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }

        protected Guid CurrentProductID
        {
            get
            {
                if (ViewState["CurrentProductID"] == null) return Guid.Empty;
                return new Guid(this.ViewState["CurrentProductID"].ToString());
            }
            set
            {
                ViewState["CurrentProductID"] = value;
            }
        }

        protected Repeater ProductUsage;
        protected Repeater ProductUsageChange;
        protected Repeater ProductSpec;
        protected Repeater ProductSpecChange;
        /*protected Literal ProductName1;
        protected Literal ProductName2;
        protected Literal ProductName3;
        protected Literal ProductName4;*/
        protected Literal ProductName5;
        protected Label HeaderDateLiteral;

        protected Repeater ProductVersions;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                this.CurrentProductID = this.ProductID;
                using (UsageConfigurationProvider provider = new UsageConfigurationProvider())
                {
                    ProductVersions.DataSource = provider.GetProductVersions(this.ProductID);
                    ProductVersions.DataBind();
                }
                this.LoadContent();
            }
        }

        private void LoadContent()
        {
            using (UsageConfigurationProvider provider = new UsageConfigurationProvider())
            {
                Product product = provider.GetProduct(this.CurrentProductID);
                ProductName5.Text = /*ProductName4.Text = ProductName3.Text = ProductName2.Text = ProductName1.Text = */product.PublicName;
                this.Title = String.Format("Применяемость {0}", ProductName5.Text);
                HeaderDateLiteral.Text = string.Format(HeaderDateLiteral.Text, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
                ProductUsage.DataSource = provider.GetProductUsage(this.CurrentProductID);
                ProductUsage.DataBind();
                ProductUsageChange.DataSource = provider.GetProductUsageChange(this.CurrentProductID);
                ProductUsageChange.DataBind();
                ProductSpec.DataSource = provider.GetProductSpecification(this.CurrentProductID);
                ProductSpec.DataBind();
                ProductSpecChange.DataSource = provider.GetProductSpecificationChange(this.CurrentProductID);
                ProductSpecChange.DataBind();
            }
        }

        protected void SelectProduct_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in ProductVersions.Items)
            {
                (item.FindControl("RDButton") as RadioButton).Checked = false;
            }
            RepeaterItem selecteditem = (sender as Control).Parent as RepeaterItem;
            (selecteditem.FindControl("RDButton") as RadioButton).Checked = true;
            Guid id = new Guid((selecteditem.FindControl("HiddentID") as HiddenField).Value);
            this.CurrentProductID = id;
            this.LoadContent();
        }
    }
}
