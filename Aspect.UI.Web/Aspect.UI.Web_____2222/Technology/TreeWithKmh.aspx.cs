using System;
using System.Linq;
using Aspect.Domain;
using System.Web.Services;
using System.Web.Script.Services;
using Aspect.Model.ProductDomain;

namespace Aspect.UI.Web.Technology
{
    public partial class TreeWithKmh : Basic.PageBase
    {
        protected const string PROD_ID = "prodid";

        protected void Page_Load(object sender, EventArgs e)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                // ищем продукт в приказных изделиях
                Product product = provider.GetProduct(new Guid(Request[PROD_ID]));
                if (!String.IsNullOrEmpty(product.OrderNumber) && !String.IsNullOrEmpty(product.OrderYear))
                {
                    // приказное изделия
                    var orders = from order in provider.OrderArticles
                                 where order.year == product.OrderYear
                                 && order.cco == product.OrderNumber
                                 select order;
                    if (orders.Count() > 0)
                    {
                        // есть приказ соотвествующий номеру и году
                        Page.Title = string.Format("Разузлованный технологический состав для {0} версии {1}. Номер приказа: {2}. Год приказа: {3}",
                            product._dictNomen.pn1,
                            product.Version,
                            product.OrderNumber,
                            product.OrderYear);
                    }
                    else
                    {
                        // приказа нет, поэтому будем считать его стандартным
                        Page.Title = string.Format("Разузлованный технологический состав для {0} стандартной версии {1}.",
                            product._dictNomen.pn1,
                            product.Version);
                    }
                }
                else
                {
                    // не приказное изделие
                    Page.Title = string.Format("Разузлованный технологический состав для {0} стандартной версии {1}.",
                        product._dictNomen.pn1,
                        product.Version);
                }                
            }                        
        }        
    }
}
