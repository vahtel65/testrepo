using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspect.Model.ProductDomain;
using Aspect.Domain;

namespace Aspect.UI.Web.Technology
{
    public partial class EditorKmh : Basic.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                Product product = provider.GetProduct(new Guid(Request.QueryString["prodid"]));

                if (Request.QueryString["orderid"] == null)
                {
                    Title = String.Format("Редактирование %state% КМХ для {0}.", product.PublicName);
                }
                else
                {
                    OrderArticle order = (from or in provider.OrderArticles
                                         where or.ID == new Guid(Request.QueryString["orderid"])
                                         select or).Single();
                    Title = String.Format("Редактирование %state% КМХ для {0}. По приказу №{1} от {2} года.", product.PublicName, order.cco, order.year);
                }
            }

            this.PopupIframeInitializationStringWithProduct(linkSelectMaterial, "", 
                string.Format("../Popup/Selector.aspx?ID={0}&ctrlID={1}&treeID={2}&textCtrlID={3}", 
                "", "valueID", "316c6bc7-d883-44c8-aae0-602f49c73595", "clientID"), 800, 500);
        }
    }
}
