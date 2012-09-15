using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Web.Services;
using Aspect.Model.ConfigurationDomain;
using Aspect.Domain;
using System.Data;
using Aspect.Model.ProductDomain;
using System.Web.Script.Serialization;

namespace Aspect.UI.Web.Technology
{
    [ScriptService]
    public partial class TechnDates : System.Web.UI.Page
    {
        
        private static Guid UserID
        {
            get
            {
                return (Guid)HttpContext.Current.Session["userID"]; ;
            }
        }

        /*
         * Exceptions:
         *   404 - product not found (be given product_id)
         */
        [WebMethod]
        public static string GenerateForOrder(Guid order_id, Guid product_id, DateTime gen_date)
        {
            using (ConfigurationTreeProvider provider = new ConfigurationTreeProvider())
            {
                // получаем разузлованный состав для данной главной детали
                DataSet tree = provider.GetList(FormGridView.ConfigurationTree, product_id, UserID, new List<SearchExpression>());

                // удаляем все существующие даты для данного изделия и данного приказа
                var subprod_dictNomenIDs = new List<Guid>();
                foreach (DataRow row in tree.Tables[0].Rows)
                {
                    subprod_dictNomenIDs.Add((Guid)row["_dictNomenID"]);
                }
                using (ProductProvider pprovider = new ProductProvider())
                {
                    subprod_dictNomenIDs.Add(pprovider.GetProduct(product_id)._dictNomenID.Value);
                }

                var datesForDelete = provider.TechnDates.Where(
                    it => subprod_dictNomenIDs.Contains(it._dictNomenID)
                    && it.OrderArticleID == order_id);

                provider.TechnDates.DeleteAllOnSubmit(datesForDelete);
                provider.SubmitChanges();

                // создаём новые записи для данного приказа
                foreach (Guid subprod_dictNomenID in subprod_dictNomenIDs.Distinct())
                {
                    provider.TechnDates.InsertOnSubmit(new TechnDate()
                    {
                        OrderArticleID = order_id,
                        _dictNomenID = subprod_dictNomenID,
                        gen_date = gen_date
                    });                                
                }
                
                provider.SubmitChanges();
            }
            return new PostResult("ok", 0).ToString();
        }

        /// <summary>
        /// Возвращает даты готовностей для детали в рамках приказа.
        /// Для стандартных составов - order_id = "00000000-0000-0000-0000-000000000000"
        /// </summary>        
        [WebMethod]
        public static string GetForProduct(Guid order_id, Guid product_id)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                var product = provider.GetProduct(product_id);
                if (product == null)
                {
                    return new PostResult("Certain product not found.", 404).ToString();
                }

                var date = provider.TechnReadinesses.SingleOrDefault(it =>
                    it.OrderArticleID == order_id
                    && it._dictNomenID == product._dictNomenID);

                if (date == null)
                {
                    return new PostResult("Not found certain row in table [dbo].[TechnDates].", 401).ToString();
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(new transfer_techn_dates()
                {
                    order_id = order_id,
                    dictnomen_id = product._dictNomenID.Value,
                    
                    svar_date = date.svar_date,
                    him_date = date.him_date,
                    techn_date = date.techn_date
                });
            }                      
        }

        /// <summary>
        /// Устанавливает даты готовностей для детали в рамках приказа.
        /// Для стандартных составов - order_id = "00000000-0000-0000-0000-000000000000"
        /// </summary>
        [WebMethod]
        public static string SaveForProduct(Guid product_id, Guid order_id, transfer_techn_dates dates)
        {
            try
            {
                using (ProductProvider provider = new ProductProvider())
                {
                    var product = provider.GetProduct(product_id);
                    if (product == null)
                    {
                        return new PostResult("Certain product not found.", 404).ToString();
                    }
                                        
                    // select row with dates from DB
                    var unit_dates = provider.TechnReadinesses.SingleOrDefault(it 
                        => it._dictNomenID == product._dictNomenID 
                        && it.OrderArticleID == order_id);

                    // if row doesn't exists create new row
                    if (unit_dates == null)
                    {
                        unit_dates = new TechnReadiness()
                        {
                            _dictNomenID = product._dictNomenID.Value,
                            OrderArticleID = order_id
                        };
                        provider.TechnReadinesses.InsertOnSubmit(unit_dates);
                    }

                    // clear dates if needed and save dates to row
                    unit_dates.techn_date = dates.techn_date;
                    unit_dates.svar_date = dates.svar_date;
                    unit_dates.him_date = dates.him_date;                    

                    // if row contain all nulled fields delete it
                    if (unit_dates.techn_date == null &&
                        unit_dates.him_date == null &&
                        unit_dates.svar_date == null)
                    {
                        provider.TechnReadinesses.DeleteOnSubmit(unit_dates);
                    }

                    // save dates to DB
                    provider.SubmitChanges();
                }
                return new PostResult("Ok", 0).ToString();
            }
            catch (Exception e)
            {
                return new PostResult("Unknown exception: " + e.Message, -1).ToString();
            }
        }        

        [WebMethod]
        public static string GetOrdersWithKmh(Guid _productNomenId)
        {
            List<transfer_order_article> orders = new List<transfer_order_article>();

            using (ProductProvider provider = new ProductProvider())
            {
                // select all order's ids what contain certain kmh
                var dates = from date in provider.TechnDates
                            where date._dictNomenID == _productNomenId
                            select date.OrderArticleID;
                
                // select all order by list of it ids
                var raw_orders = from order in provider.OrderArticles
                             where dates.Contains(order.ID)
                             select order;

                foreach (var raw_order in raw_orders)
                {
                    orders.Add(new transfer_order_article()
                    {
                        order_id = raw_order.ID,
                        year = raw_order.year,
                        cco = raw_order.cco,
                        created = raw_order.LastVersCreatedDate.Value,
                        exists_orderkmh = provider.Specification_2s.Count( it 
                            => it._Product_ID == _productNomenId
                            && it.OrderArticleID == raw_order.ID) > 0
                    });
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(orders);
        }
        

        [WebMethod]
        public static string ExchangeKmhInOrders(List<Guid> orderArticles, DateTime timeStamp, Guid prodNomenId, TechnDatesSpeciality speciality)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                var dates = from date in provider.TechnDates
                            where orderArticles.Contains(date.OrderArticleID)
                            && date._dictNomenID == prodNomenId
                            select date;
                foreach (var date in dates)
                {
                    switch (speciality)
                    {
                        case TechnDatesSpeciality.Main: 
                            date.gen_date = timeStamp.ToLocalTime();
                            break;
                        case TechnDatesSpeciality.Svar:
                            date.svar_date = timeStamp.ToLocalTime();
                            break;
                        case TechnDatesSpeciality.Him:
                            date.him_date = timeStamp.ToLocalTime();
                            break;
                        case TechnDatesSpeciality.Techn:
                            date.techn_date = timeStamp.ToLocalTime();
                            break;
                    }                    
                }

                provider.SubmitChanges();
            }

            return new PostResult("Ok", 0).ToString();
        }

        protected void Page_Load(object sender, EventArgs e){}
    }
}