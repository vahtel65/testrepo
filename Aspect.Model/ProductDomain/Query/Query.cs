using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspect.Domain;

namespace Aspect.Model.ProductDomain.Query
{
    internal class Query : Aspect.Model.Query.QueryBuilder
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
SELECT 
--distinct
Product.ID as ID
{0}
FROM Product
LEFT join ClassificationTreeProduct ON Product.ID = ClassificationTreeProduct.ProductID
{1}
{2}
{3}
";
            }
        }
        #endregion

        public Query(CommonDomain provider)
            : base(provider)
        {

        }

        public virtual List<Aspect.Domain.Property> Columns { get; set; }

        public virtual string BuildEntityQuery(Guid ID)
        {
            StringBuilder sql = new StringBuilder();
            //-collect from and join values-
            Aspect.Model.Query.FieldsSQLBuilder propertyQuery = new PropertyFieldsSQLBuilder(Columns);
            Aspect.Model.Query.FieldsSQLBuilder dictionaryQuery = new DictionaryFieldsSQLBuilder(UserFields, Provider);

            StringBuilder join = new StringBuilder();
            join.Append(propertyQuery.Join);
            join.Append(dictionaryQuery.Join);

            StringBuilder from = new StringBuilder();
            from.Append(propertyQuery.From);
            from.Append(dictionaryQuery.From);
            //
            string where = WhereClause.BuildExpression(ProductProvider.ProductIDColumn, ID.ToString());
            sql.AppendFormat(this.SqlTemplate,
                from.ToString(),
                join.ToString(),
                where,
                string.Empty);
            return sql.ToString();
        }

        public virtual string BuildListQuery(Guid treeNodeID)
        {
            StringBuilder sql = new StringBuilder();
            WhereClause classificationWhere = new ClassificationWhereClauseBuilder(treeNodeID, Provider);
            //--
            List<WhereClause> whereList = SearchSource.Convert(this.SearchExpression);
            whereList.Insert(0, classificationWhere);
            string where = WhereClause.BuildExpression(/*classificationWhere, this.SearchExpression*/ whereList);
            
            //-collect from and join values-
            Aspect.Model.Query.FieldsSQLBuilder propertyQuery = new PropertyFieldsSQLBuilder(Columns);
            Aspect.Model.Query.FieldsSQLBuilder dictionaryQuery = new DictionaryFieldsSQLBuilder(UserFields, Provider);
            
            StringBuilder join = new StringBuilder();
            join.Append(propertyQuery.Join);
            join.Append(dictionaryQuery.Join);

            StringBuilder from = new StringBuilder();
            from.Append(propertyQuery.From);
            from.Append(dictionaryQuery.From);

            //prevent order by not selected record
            if (!from.ToString().Contains(OrderExpression.Expression)) OrderExpression = new OrderExpression();

            sql.AppendFormat(SqlTemplate,
                from.ToString(),
                join.ToString(),
                where,
                OrderExpression.OrderClause);
            return sql.ToString();
        }
    }
}
