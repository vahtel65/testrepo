using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspect.Domain;

namespace Aspect.Model.SpecificationDomain
{
    public class SpecificationProvider : CommonDomain
    {
        /*public Specification_1 GetKmhCard(Guid productID, Guid orderArticleID)
        {
            // получаем продукт
            Product product = Products.Where(prod => prod.ID == productID).Single();

            // получаем все КМХ карты для данного изделия
            List<Specification_1> spec_list = (from s in Specification_1s
                                              where s._Product_ID == product._dictNomenID
                                              select s).ToList();
            Specification_1 actual_spec;
            if (spec_list.Where(s => s.OrderArticleID == orderArticleID).Count() > 0)
            {
                // если есть приказная - берём её
                actual_spec = spec_list.Where(s => s.OrderArticleID == orderArticleID).Single();
            }
            else
            {
                // если приказной нет - берём актуальную на момент создания изделия
                actual_spec = (from s in spec_list
                               where s.StartDT < product.CreatedDate 
                               && s.FinishDT > product.CreatedDate
                               select s).Single();
            }
            return actual_spec;

        }*/

        #region Template
        /// <summary>
        /// 0 - CurrentDT (всегда текущая дата)
        /// </summary>
        protected string sqlListOfWares
        {
            get
            {
                return @"SELECT 
                            Product.ID as ProductID,
                            _dictNomen.pn1 as pn1,
                            _dictNomen.superpole as superpole,
                            Product.CreatedDate as created,
                            OrderArticle.ID as order_id,
                            OrderArticle.cco as cco,
                            OrderArticle.year as year,
                            OrderArticle.LastVersCreatedDate as LastVersCreatedDate,
                            spec_stand.* ,spec_order.*,
                            TechnReadiness.him_date as him_date,
                            TechnReadiness.svar_date as svar_date,
                            TechnReadiness.techn_date as techn_date
                        FROM OrderArticle
                        INNER JOIN _dictNomen ON _dictNomen.ID = OrderArticle._dictNomenID
                        INNER JOIN Product ON Product.ID = OrderArticle.LastVersProductID
                        LEFT OUTER JOIN Specification_1 spec_stand ON spec_stand._Product_ID = _dictNomen.ID AND spec_stand.StartDT < Product.CreatedDate AND spec_stand.FinishDT > Product.CreatedDate AND spec_stand.OrderArticleID IS NULL
                        LEFT OUTER JOIN Specification_1 spec_order ON spec_order._Product_ID = _dictNomen.ID AND spec_order.StartDT < '{0}' AND spec_order.FinishDT > '{0}' AND spec_order.OrderArticleID = OrderArticle.ID

                        LEFT OUTER JOIN TechnReadiness ON TechnReadiness.OrderArticleID = OrderArticle.ID AND TechnReadiness._dictNomenID = _dictNomen.ID
                        
                        WHERE OrderArticle.LastVersProductID IS NOT NULL AND OrderArticle.cco IS NOT NULL AND OrderArticle.cco IS NOT NULL";
            }
        }
        #endregion        
         
        /// <param name="CurrentDT">Текущая дата</param>
        /// <returns></returns>
        public System.Data.DataSet GetListOfWares(DateTime CurrentDT)
        {
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                return provider.ExecuteCommand(String.Format(sqlListOfWares, CurrentDT.ToString("yyyy-MM-dd HH:mm:ss")));
            }
        }

        #region Template
        /// <summary>
        /// 0 - CurrentDT
        /// 1 - Specification Table ( =1 || =2)
        /// 2 - MaterialID
        /// </summary
        protected string sqlMaterialApplicability
        {
            get
            {
                return @"SELECT 
                            MainProduct.ID as productID,
                            _dictNomen.pn1 as pn1,
                            _dictNomen.pn2 as pn2,
                            Specification_{1}.no as no,    
                            _dictUM.umn1 as um,
                            _dictS.sn1 as s,
                            norder.Value as norder,
                            yorder.Value as yorder
                        FROM Specification_{1}
                        JOIN _dictNomen ON _dictNomen.ID = Specification_{1}._Product_ID
                        JOIN Product MainProduct ON MainProduct._dictNomenID = Specification_{1}._Product_ID 
	                        AND Specification_{1}.OrderArticleID IS NULL
	                        AND (
		                        (SELECT mv.Value FROM ProductProperty mv WHERE mv.ProductID = MainProduct.ID AND mv.PropertyID = 'BBE170B0-28E4-4738-B365-1038B03F4552') = 1
		                        OR
		                        (SELECT norder.Value FROM ProductProperty norder WHERE norder.ProductID = MainProduct.ID AND norder.PropertyID = '9A38E338-DD60-4636-BFE3-6A98BAF8AE87') <> ''
	                        )

                        LEFT JOIN _dictUM ON _dictUM.ID = Specification_{1}._dictUMID
                        LEFT JOIN _dictS ON _dictS.ID = Specification_{1}._dictSID

                        LEFT JOIN ProductProperty norder ON norder.ProductID = MainProduct.ID AND norder.PropertyID = '9A38E338-DD60-4636-BFE3-6A98BAF8AE87'
                        LEFT JOIN ProductProperty yorder ON yorder.ProductID = MainProduct.ID AND yorder.PropertyID = '2CCD4FF3-6D43-4A35-9784-969FAB46B5CC'
                        --LEFT JOIN ProductProperty mv ON mv.ProductID = MainProduct.ID AND mv.PropertyID = 'BBE170B0-28E4-4738-B365-1038B03F4552'
                        WHERE _Material_ID = '{2}'
                        AND StartDT < '{0}' AND FinishDT > '{0}'";
            }
        }
        #endregion

        public System.Data.DataSet GetApplicabilityList(DateTime CurrentDT, int SpecificationTable, Guid MaterialId)
        {
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                return provider.ExecuteCommand(String.Format(sqlMaterialApplicability, CurrentDT.ToString("yyyy-MM-dd HH:mm:ss"), SpecificationTable, MaterialId));
            }
        }
    }
}
