using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Aspect.Model.ProductDomain;
using Aspect.Domain;

namespace Aspect.UI.Web.Technology
{
    public partial class Applicability : Basic.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                Product product = provider.GetProduct(new Guid(Request.Params["prodid"]));
                Page.Title = String.Format("Применяемость по изделиям для {0} версии {1}",
                        product.Name,
                        product.Version);
            }
        }
    }
}
