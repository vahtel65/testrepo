using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Aspect.Domain;
using Aspect.Model.Query;

namespace Aspect.Model.ConfigurationDomain
{
    public class ConfigurationTreeProvider : ConfigurationProvider
    {
        public override List<GridColumn> GetGridColumns(Guid userID, Guid formGridViewID, Guid fieldPlaceHolderID)
        {
            List<GridColumn> result =  base.GetGridColumns(userID, formGridViewID, fieldPlaceHolderID);
            if (fieldPlaceHolderID.Equals(FieldPlaceHolder.Grid))
            {
                result.Add(GridColumn.SetOrder(EditableGridColumn.QuantityInclusiveColumn,1002));
                result.Add(GridColumn.SetOrder(EditableGridColumn.PositionColumn, 1003));
                result.Add(GridColumn.SetOrder(EditableGridColumn.CommentColumn, 1004));
            }
            return result;
        }

        public override System.Data.DataSet GetList(Guid formGridViewID, Guid productID, Guid userID, /*OrderExpression order , */List<SearchExpression> searchExpression)
        {
            //
            //List<SearchExpression> searchExpression = new List<SearchExpression>();
            OrderExpression order = new OrderExpression();
            //
            Query.TreeQuery query = new Query.TreeQuery(this);
            //query.ClassificationTreeID = treeNodeID;
            query.Columns = this.GetUserProperties(userID, formGridViewID, FieldPlaceHolder.Grid);
            query.UserFields = this.GetUserFields(userID, formGridViewID, FieldPlaceHolder.Grid);
            query.OrderExpression = order;
            query.SearchExpression = this.ValidateSearchExpression(query.Columns, query.UserFields, searchExpression);
            string sql = query.BuildListQuery(productID);
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                return provider.ExecuteCommand(sql);
            }
        }               

        #region Templates

        /// <summary>
        /// 0 - from (UnitID)
        /// 1 - OrderArticleID or Guid.Empty
        /// 2 - ActualDT (для приказного изделия - дата его создания, для стандартного - текущая дата)
        /// 3 - CurrentDT (всегда текущая дата)
        /// </summary>
        protected string sqlWithKmh
        {
            get
            {
                #region OLD REQUEST
                /*return @"WITH ChildProducts(ProductID, ProductOwnerID, [Level], ProductOrder) AS 
                    (
	                    SELECT ProductID, ProductOwnerID, 0 AS [Level], CAST( (Row_NUmber() OVER(ORDER BY ID DESC)) AS nvarchar(200)) AS ProductOrder
	                    FROM Configuration
	                    WHERE ProductOwnerID = '{0}' --child DictionaryTreeID
                                AND (Configuration.GroupNumber IS NULL or Configuration.GroupNumber = 0)
	                    UNION ALL
	                    SELECT d.ProductID, d.ProductOwnerID, cd.Level + 1, CAST(cd.ProductOrder + '.' + CAST( (Row_NUmber() OVER(ORDER BY ID DESC)) AS nvarchar(100)) AS nvarchar(200)) AS ProductOrder
	                    FROM Configuration d
		                    INNER JOIN ChildProducts cd
		                    ON d.ProductOwnerID = cd.ProductID
                            WHERE (d.GroupNumber IS NULL or d.GroupNumber = 0)
                    )

                    SELECT 
                    distinct
                    
                    unit.ID as unitID,

                    unitNomen.ID as unitNomenID,
                    unitNomen.pn1 as unitPn1,
                    unitNomen.superpole as unitPn2,
                    --- unitVersion.Value as unitVersion,
                    --- unitOrderYear.Value as unitOrderYear,
                    --- unitOrderNumber.Value as unitOrderNumber,

                    prodNomen.pn1 as prodPn1,
                    prodNomen.superpole as prodPn2,
                    prodNomen.ID as prodNomenID,
                    --- prodVersion.Value as prodVersion,
                    --- prodOrderYear.Value as prodOrderYear,
                    --- prodOrderNumber.Value as prodOrderNumber,

                    Product.ID as prodID
                    ,Configuration.Quantity AS Quantity
                    ,_dictUM.umn1 AS EdIzmName
                    ,Configuration.Position as Position
                    ,Configuration.Comment as Comment
                    ,ChildProducts.[Level] AS [Level]
                    ,ChildProducts.ProductOrder 
                    ,Configuration.GroupToChange as GroupToChange
                    ,Configuration.GroupNumber as GroupNumber
                    ,Configuration.QuantityInclusive as QuantityInclusive
                    ,spec_stand.*
                    ,spec_order.*
                    ,spec_route.Route as RouteForChange
                    ,materialNomen_stand.superpole as MaterialSuperpole
                    ,materialNomen_order.superpole as MaterialSuperpole1


                    FROM Product
                    INNER JOIN ChildProducts ON ChildProducts.ProductID = Product.ID
                    INNER JOIN Configuration ON ChildProducts.ProductID = Configuration.ProductID AND ChildProducts.ProductOwnerID = Configuration.ProductOwnerID
                    INNER JOIN _dictUM ON Configuration._dictUMID = _dictUM.ID
                    INNER JOIN Product unit on unit.ID = ChildProducts.ProductOwnerID
                    INNER JOIN _dictNomen unitNomen ON unit._dictNomenID = unitNomen.ID
                    INNER JOIN _dictNomen prodNomen ON Product._dictNomenID = prodNomen.ID

					LEFT OUTER JOIN Specification spec_stand ON spec_stand._Product_ID = prodNomen.ID AND spec_stand.tn = 1 AND spec_stand.StartDT < '{2}' AND spec_stand.FinishDT > '{2}' AND spec_stand.OrderArticleID IS NULL
					LEFT OUTER JOIN Specification spec_order ON spec_order._Product_ID = prodNomen.ID AND spec_order.tn = 1 AND spec_order.StartDT < '{2}' AND spec_order.FinishDT > '{2}' AND spec_order.OrderArticleID = '{1}'
                    LEFT OUTER JOIN Specification spec_route ON spec_route._Product_ID = unit._dictNomenID AND spec_route._Material_ID = Product._dictNomenID AND spec_route.tn = 3

                    LEFT OUTER JOIN _dictNomen materialNomen_stand on materialNomen_stand.ID = spec_stand._Material_ID
                    LEFT OUTER JOIN _dictNomen materialNomen_order on materialNomen_order.ID = spec_order._Material_ID

                    --- LEFT JOIN ProductProperty unitVersion ON unitVersion.PropertyID = '0789DB1A-9BAA-4574-B405-AE570C746C03' AND unitVersion.ProductID = unit.ID
                    --- LEFT JOIN ProductProperty unitOrderYear ON unitOrderYear.PropertyID = '2CCD4FF3-6D43-4A35-9784-969FAB46B5CC' AND unitOrderYear.ProductID = unit.ID
                    --- LEFT JOIN ProductProperty unitOrderNumber ON unitOrderNumber.PropertyID = '9A38E338-DD60-4636-BFE3-6A98BAF8AE87' AND unitOrderNumber.ProductID = unit.ID

                    --- LEFT JOIN ProductProperty prodVersion ON prodVersion.PropertyID = '0789DB1A-9BAA-4574-B405-AE570C746C03' AND prodVersion.ProductID = Product.ID
                    --- LEFT JOIN ProductProperty prodOrderYear ON prodOrderYear.PropertyID = '2CCD4FF3-6D43-4A35-9784-969FAB46B5CC' AND prodOrderYear.ProductID = Product.ID
                    --- LEFT JOIN ProductProperty prodOrderNumber ON prodOrderNumber.PropertyID = '9A38E338-DD60-4636-BFE3-6A98BAF8AE87' AND prodOrderNumber.ProductID = Product.ID

                    ORDER BY ChildProducts.ProductOrder";*/
                #endregion
                return @"IF OBJECT_ID('tempdb..#withoutLMH') IS NOT NULL DROP TABLE #withoutLMH

                 WITH ChildProducts(ProductID, ProductOwnerID, [Level], ProductOrder) AS 
                (
                    SELECT ProductID, ProductOwnerID, 0 AS [Level], CAST( (Row_NUmber() OVER(ORDER BY ID DESC)) AS nvarchar(200)) AS ProductOrder
                    FROM Configuration
                    WHERE ProductOwnerID = '{0}' --child DictionaryTreeID
                            AND (Configuration.GroupNumber IS NULL or Configuration.GroupNumber = 0)
                    UNION ALL
                    SELECT d.ProductID, d.ProductOwnerID, cd.Level + 1, CAST(cd.ProductOrder + '.' + CAST( (Row_NUmber() OVER(ORDER BY ID DESC)) AS nvarchar(100)) AS nvarchar(200)) AS ProductOrder
                    FROM Configuration d
                        INNER JOIN ChildProducts cd
                        ON d.ProductOwnerID = cd.ProductID
                        WHERE (d.GroupNumber IS NULL or d.GroupNumber = 0)
                )

                SELECT 
                distinct

                unit.ID as unitID,

                unitNomen.ID as unitNomenID,
                unitNomen.pn1 as unitPn1,
                unitNomen.pn2 as unitPn2,
                --- unitVersion.Value as unitVersion,
                --- unitOrderYear.Value as unitOrderYear,
                --- unitOrderNumber.Value as unitOrderNumber,

                prodNomen.pn1 as prodPn1,
                prodNomen.pn2 as prodPn2,
                prodNomen.ID as prodNomenID,
                --- prodVersion.Value as prodVersion,
                --- prodOrderYear.Value as prodOrderYear,
                --- prodOrderNumber.Value as prodOrderNumber,

                Product.ID as prodID
                ,Configuration.Quantity AS Quantity
                ,_dictUM.umn1 AS EdIzmName
                ,Configuration.Position as Position
                ,Configuration.Comment as Comment
                ,ChildProducts.[Level] AS [Level]
                ,ChildProducts.ProductOrder 
                ,Configuration.GroupToChange as GroupToChange
                ,Configuration.GroupNumber as GroupNumber
                ,Configuration.QuantityInclusive as QuantityInclusive

                into #withoutLMH
                from Product 
                INNER JOIN ChildProducts ON ChildProducts.ProductID = Product.ID
                INNER JOIN Configuration ON ChildProducts.ProductID = Configuration.ProductID AND ChildProducts.ProductOwnerID = Configuration.ProductOwnerID
                INNER JOIN _dictUM ON Configuration._dictUMID = _dictUM.ID
                INNER JOIN Product unit on unit.ID = ChildProducts.ProductOwnerID
                INNER JOIN _dictNomen unitNomen ON unit._dictNomenID = unitNomen.ID
                INNER JOIN _dictNomen prodNomen ON Product._dictNomenID = prodNomen.ID                

                -- INSERT ROOT PRODUCT
	            INSERT INTO #withoutLMH (prodID, QuantityInclusive, unitID, unitNomenID, prodNomenID, 
		            Quantity, Position, Level, prodPn1, prodPn2, unitPn1, unitPn2) 
	            SELECT Product.ID
		            , 0 as QuantityInclusive
		            , '00000000-0000-0000-0000-000000000000' as unitID
		            , '00000000-0000-0000-0000-000000000000' as unitNomenID
		            , Product._dictNomenID as prodNomenID
		            , 0 as Quantity
		            , 0 as Position
		            , 0 as Level
		            , prodNomen.pn1 as prodPn1		
		            , prodNomen.pn2 as prodPn2
		            , '' as unitPn1
		            , '' as unitPn2
	            FROM Product
	            INNER JOIN _dictNomen prodNomen ON Product._dictNomenID = prodNomen.ID
	            WHERE Product.ID = '{0}'

                SELECT #withoutLMH.* ,spec_stand.* ,spec_order.*
                ,user_stand.*, user_order.*
                ,spec_route.Route as RouteForChange
                ,materialNomen_stand.superpole as MaterialSuperpole
                ,materialNomen_order.superpole as MaterialSuperpole1
                ,techn_readiness_stand.him_date
                ,techn_readiness_stand.svar_date
                ,techn_readiness_stand.techn_date                
                ,techn_readiness_order.him_date
                ,techn_readiness_order.svar_date
                ,techn_readiness_order.techn_date                
                --,techn_dates_stand.gen_date
                ,techn_dates_order.gen_date

                FROM #withoutLMH

                LEFT OUTER JOIN Specification_1 spec_stand ON spec_stand._Product_ID = prodNomenID AND spec_stand.StartDT <= {2} AND spec_stand.FinishDT > {2} AND spec_stand.OrderArticleID IS NULL
                LEFT OUTER JOIN Specification_1 spec_order ON spec_order._Product_ID = prodNomenID AND spec_order.StartDT <= '{3}' AND spec_order.FinishDT > '{3}' AND spec_order.OrderArticleID = '{1}'
                LEFT OUTER JOIN Specification_3 spec_route ON spec_route._Product_ID = unitNomenID AND spec_route.StartDT <= '{3}' AND spec_route.FinishDT > '{3}' AND spec_route._Material_ID = prodNomenID 

                LEFT OUTER JOIN [User] user_stand ON user_stand.ID = spec_stand.userID
                LEFT OUTER JOIN [User] user_order ON user_order.ID = spec_order.userID

                LEFT OUTER JOIN _dictNomen materialNomen_stand on materialNomen_stand.ID = spec_stand._Material_ID
                LEFT OUTER JOIN _dictNomen materialNomen_order on materialNomen_order.ID = spec_order._Material_ID
                
                --LEFT OUTER JOIN TechnDates techn_dates_stand ON techn_dates_stand._dictNomenID = prodNomenID AND techn_dates_stand.OrderArticleID = '00000000-0000-0000-0000-000000000000'                
                LEFT OUTER JOIN TechnDates techn_dates_order ON techn_dates_order._dictNomenID = prodNomenID AND techn_dates_order.OrderArticleID = '{1}'

                LEFT OUTER JOIN TechnReadiness techn_readiness_stand ON techn_readiness_stand._dictNomenID = prodNomenID AND techn_readiness_stand.OrderArticleID = '00000000-0000-0000-0000-000000000000'                
                LEFT OUTER JOIN TechnReadiness techn_readiness_order ON techn_readiness_order._dictNomenID = prodNomenID AND techn_readiness_order.OrderArticleID = '{1}'

                ORDER BY ProductOrder";
            }
        }

        #endregion

        /// <param name="ActualDT">Для приказного - дата создания, Для стандартного - текущая дата</param>
        /// <param name="CurrentDT">Текущая дата</param>
        /// <returns></returns>
        public System.Data.DataSet GetListWithKmh(Guid productID, Guid OrderArticleID, DateTime ActualDT, DateTime CurrentDT)
        {
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                var actualDate = String.Format("'{0}'", ActualDT.ToString("yyyy-MM-dd HH:mm:ss"));

                // для приказных составов, актульной датой стандартной детали является дата, записанная
                // в колонке [TechnDates].[gen_date]
                if (OrderArticleID != Guid.Empty)
                {
                    actualDate = String.Format("(SELECT gen_date FROM TechnDates WHERE OrderArticleID = '{0}' AND _dictNomenID = spec_stand._Product_ID)",
                        OrderArticleID);
                }

                return provider.ExecuteCommand(String.Format(sqlWithKmh, productID, OrderArticleID, actualDate, CurrentDT.ToString("yyyy-MM-dd HH:mm:ss")));
            }
        }
    }
}
