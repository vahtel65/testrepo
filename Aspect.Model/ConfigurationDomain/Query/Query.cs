using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspect.Domain;

namespace Aspect.Model.ConfigurationDomain.Query
{
    internal class Query : Aspect.Model.ProductDomain.Query.Query
    {
        #region Templates
        /// <summary>
        /// 0 - from
        /// 1 - join
        /// 2 - where
        /// 3 - order
        /// </summary>
        protected override string SqlTemplate
        {
            get
            {
                return @"

WITH ChildProducts(ProductID, ProductOwnerID, QNumber) AS 
(
	SELECT ProductID, ProductOwnerID, 
		CASE QuantityInclusive
			WHEN 0 THEN 0
			ELSE CAST(Quantity/QuantityInclusive AS decimal(18,5))
		END AS QNumber
	FROM Configuration
	WHERE ProductOwnerID = '{2}' 
	AND (Configuration.GroupNumber IS NULL or Configuration.GroupNumber = 0)
	UNION ALL
	SELECT d.ProductID, d.ProductOwnerID, 
		CASE d.QuantityInclusive
			WHEN 0 THEN 0
			ELSE CAST( (d.Quantity/d.QuantityInclusive)*cd.QNumber AS decimal(18,5))
		END AS QNumber
	FROM Configuration d
		INNER JOIN ChildProducts cd
		ON d.ProductOwnerID = cd.ProductID
	WHERE (d.GroupNumber IS NULL or d.GroupNumber = 0)
)

SELECT 
Product.ID AS ID
{0}
,_dictUM.umn1 AS EdIzmName
,sum(ChildProducts.QNumber) AS Quantity
,max(Configuration.GroupToChange) as GroupToChange
FROM Product
INNER JOIN ChildProducts ON ChildProducts.ProductID = Product.ID
INNER JOIN Configuration ON ChildProducts.ProductID = Configuration.ProductID AND ChildProducts.ProductOwnerID = Configuration.ProductOwnerID
INNER JOIN _dictUM ON Configuration._dictUMID = _dictUM.ID
{1}

GROUP BY {3}
";
            }
        }
        //--WHERE NOT product.ID IN (SELECT c.ProductOwnerID FROM Configuration c WHERE c.ProductOwnerID = product.ID)
        #endregion

        public Query(CommonDomain provider)
            : base(provider)
        {

        }

        public override string BuildEntityQuery(Guid ID)
        {
            return base.BuildEntityQuery(ID);
        }

        protected const string ProductIDColumn = "Configuration.ProductOwnerID";

        private const string groupbytemplateDict = "[{0}].[{1}]";
        private const string groupbytemplateProp = "[{0}].Value";
        public override string BuildListQuery(Guid productID)
        {
            StringBuilder sql = new StringBuilder();
            //--
            //string where = WhereClause.BuildExpression(ProductIDColumn, productID.ToString());

            //-collect from and join values-
            Aspect.Model.Query.FieldsSQLBuilder propertyQuery = new Aspect.Model.ProductDomain.Query.PropertyFieldsSQLBuilder(Columns);
            Aspect.Model.Query.FieldsSQLBuilder dictionaryQuery = new Aspect.Model.ProductDomain.Query.DictionaryFieldsSQLBuilder(UserFields, Provider);

            //-collect group by 
            List<string> groupby = new List<string>() { "product.id", "_dictUM.umn1" };
            groupby.AddRange(Columns.Select(c => string.Format(groupbytemplateProp, c.Alias)).ToList());
            groupby.AddRange(UserFields.Select(us => string.Format(groupbytemplateDict, us.DictionaryTree.Alias, us.DictionaryProperty.ColumnName)).ToList());
            /*Columns.ConvertAll(
                delegate(Property p)
                {
                    return String.Format(groupbytemplateProp,p.Alias
                }
                );*/
            //--

            StringBuilder join = new StringBuilder();
            join.Append(propertyQuery.Join);
            join.Append(dictionaryQuery.Join);

            StringBuilder from = new StringBuilder();
            from.Append(propertyQuery.From);
            from.Append(dictionaryQuery.From);

            sql.AppendFormat(SqlTemplate,
                from.ToString(),
                join.ToString(),
                productID.ToString(),
                string.Join(",", groupby.ToArray())
                //,where
                //,OrderExpression.OrderClause
                );
            return sql.ToString();
        }
    }
}
