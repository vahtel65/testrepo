using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspect.Domain;

namespace Aspect.Model.ConfigurationDomain.Query
{
    internal class TreeQuery : Query
    {
        #region Templates
        /// <summary>
        /// 0 - from
        /// 1 - join
        /// 2 - product id
        /// </summary>
        protected override string SqlTemplate
        {
            get
            {
                return @"
WITH ChildProducts(ProductID, ProductOwnerID, [Level], ProductOrder) AS 
(
	SELECT ProductID, ProductOwnerID, 0 AS [Level], CAST( (Row_NUmber() OVER(ORDER BY ID DESC)) AS nvarchar(200)) AS ProductOrder
	FROM Configuration
	WHERE ProductOwnerID = '{2}' --child DictionaryTreeID
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
Product.ID as ID,
Product._dictNomenID as _dictNomenID
{0}
,Configuration.Quantity AS Quantity
,_dictUM.umn1 AS EdIzmName
,Configuration.Position as Position
,Configuration.Comment as Comment
,ChildProducts.[Level] AS [Level]
,ChildProducts.ProductOrder 
,Configuration.GroupToChange as GroupToChange
,Configuration.QuantityInclusive as QuantityInclusive
FROM Product
INNER JOIN ChildProducts ON ChildProducts.ProductID = Product.ID
INNER JOIN Configuration ON ChildProducts.ProductID = Configuration.ProductID AND ChildProducts.ProductOwnerID = Configuration.ProductOwnerID
INNER JOIN _dictUM ON Configuration._dictUMID = _dictUM.ID

{1}
ORDER BY ChildProducts.ProductOrder
";
            }
        }
        #endregion

        public TreeQuery(CommonDomain provider)
            : base(provider)
        {

        }

        public override string BuildListQuery(Guid productID)
        {
            StringBuilder sql = new StringBuilder();
            //--
            //string where = WhereClause.BuildExpression(ProductIDColumn, productID.ToString());

            //-collect from and join values-
            Aspect.Model.Query.FieldsSQLBuilder propertyQuery = new Aspect.Model.ProductDomain.Query.PropertyFieldsSQLBuilder(Columns);
            Aspect.Model.Query.FieldsSQLBuilder dictionaryQuery = new Aspect.Model.ProductDomain.Query.DictionaryFieldsSQLBuilder(UserFields, Provider);

            StringBuilder join = new StringBuilder();
            join.Append(propertyQuery.Join);
            join.Append(dictionaryQuery.Join);

            StringBuilder from = new StringBuilder();
            from.Append(propertyQuery.From);
            from.Append(dictionaryQuery.From);

            sql.AppendFormat(SqlTemplate,
                from.ToString(),
                join.ToString(),
                productID.ToString());
            return sql.ToString();
        }
    }
}
