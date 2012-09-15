using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspect.Model.ProductDomain;

namespace Aspect.UI.Web.Editarea
{
    public partial class NewVersion : Basic.PageBase
    {
        protected Guid RequestProductID
        {
            get
            {
                try
                {
                    return new Guid(this.Request["ID"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                Guid id = provider.CreateNewVersionOfProduct(RequestProductID, User.ID);
                Response.Redirect(String.Format("~/Configuration/Edit.aspx?ID={0}", id));
            }
        }
    }
}
