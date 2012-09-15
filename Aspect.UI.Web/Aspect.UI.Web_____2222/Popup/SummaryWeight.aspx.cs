using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Aspect.Model.Classification;
using Aspect.Model.ProductDomain;
using Aspect.Domain;
using Aspect.Model;

namespace Aspect.UI.Web.Popup
{
    public partial class SummaryWeight : System.Web.UI.Page
    {
        protected Guid ProductID
        {
            get
            {
                try
                {
                    return new Guid(Request["pid"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }
        
        public string divSummaryWeight;
        public string divProductName;
        public string divIgnore;

        protected void Page_Load(object sender, EventArgs e)
        {
           BindData(ProductID);
        }

        public class Statistic
        {
            public int ingoreProducts = 0;
            public int recordedProducts = 0;
        }       

        /*
         * return weight of product multiply to it count -- if don't have child products 
         * return summary weight of child products -- if have child products
         */
        protected decimal weightOfProduct(ProductProvider provider, Guid productId, int productCount, ref Statistic stat)
        {
            var confs = from m in provider.Configurations
                        where m.ProductOwnerID == productId
                        select new { m.GroupNumber, m._dictUMID, m.ProductID, m.Quantity };
            if (confs.Count() == 0)
            {
                // don't have child products
                Product product = provider.GetProduct(productId);
                decimal? weight =  product._dictNomen.pw;

                if (weight.HasValue)
                {
                    return weight.Value;
                }
                else
                {
                    stat.ingoreProducts += 1;
                    return 0;
                }
                
                /*
                var prop = from m in domain.ProductProperties
                           where m.PropertyID == new Guid("AC37F816-E4C1-4751-99ED-6180D7CCA142") // вес по приказу
                           && m.ProductID == productId
                           select new { m.Value };
                if (prop.Count() == 1)
                {
                    return Convert.ToDecimal(prop.First().Value) * productCount;
                }
                else
                {
                    // ignory because product don't have weight property
                    stat.ingoreProducts += 1;
                    return 0;
                }*/
            }
            else
            {
                // have child products
                decimal summaryWeight = 0;
                foreach (var conf in confs)
                {
                    if (conf.GroupNumber == 0 && conf._dictUMID == new Guid("68CD2019-85F6-4E52-AEFE-09CA5C2B64F3")) // штуки
                    {
                        summaryWeight += weightOfProduct(provider, conf.ProductID, Convert.ToInt32(conf.Quantity), ref stat);
                    }
                    else
                    {
                        // ignory because child product is order version
                        // or it measure not in pieces
                        stat.ingoreProducts += 1;
                    }

                }
                return summaryWeight;
            }
        }

        protected string productName(Guid productId)
        {
            using (Aspect.Domain.CommonDomain domain = new Aspect.Domain.CommonDomain())
            {
                var products = from m in domain.Products
                          where m.ID == productId
                          select m;
                if (products.Count() != 1)
                {
                    // except:none product of id
                    return "";
                }
                else
                {
                    return products.First().Name;
                }
            }
        }

        protected void BindData(Guid productId)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                if (Guid.Empty.Equals(productId)) return;
                Statistic stat = new Statistic();

                divSummaryWeight = weightOfProduct(provider, productId, 1, ref stat).ToString();
                divProductName = productName(productId);
                divIgnore = stat.ingoreProducts.ToString();
            }
        }
    }
}
