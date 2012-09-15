using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspect.Domain;

namespace Aspect.Model.ProductDomain.Query
{
    internal class CustomClassificationQuery : Query
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
distinct
Product.ID as ID
{0}
FROM Product
LEFT join CustomClassificationNodeProduct ON Product.ID = CustomClassificationNodeProduct.ProductID
LEFT join ClassificationTreeProduct ON Product.ID = ClassificationTreeProduct.ProductID
{1}
{2}
{3}
";
            }
        }
        #endregion

        public CustomClassificationQuery(ProductProvider provider)
            : base(provider)
        {

        }
        public override string BuildListQuery(Guid treeNodeID)
        {
            StringBuilder sql = new StringBuilder();

            //--
            List<GetCustomClassificationTreeChildsResult> nodes = this.Provider.GetCustomClassificationTreeChilds(treeNodeID).ToList();

            WhereClause classificationWhere = WhereClause.Join(
                new List<WhereClause>() 
                { 
                    new CustomClassificationWhereClauseBuilder(nodes.Select(n => n.NodeID).Distinct().ToList()), 
                    new ClassificationWhereClauseBuilder(nodes.Select(n => n.ClassificationTreeID).Distinct().ToList(), Provider)
                }, WhereClause.Operator.OR);
            //--

            List<WhereClause> whereList = SearchSource.Convert(this.SearchExpression);
            //whereList.Insert(0, classificationWhere);
            //whereList.Insert(0, customClassificationWhere);
            whereList.Insert(0, classificationWhere);
            string where = WhereClause.BuildExpression(/*classificationWhere, this.SearchExpression*/whereList);
            if (where.Length == 0) where = WhereClause.NULLResultWhereClause;

            //-collect from and join values-
            Aspect.Model.Query.FieldsSQLBuilder propertyQuery = new PropertyFieldsSQLBuilder(Columns);
            Aspect.Model.Query.FieldsSQLBuilder dictionaryQuery = new DictionaryFieldsSQLBuilder(UserFields, Provider);

            StringBuilder join = new StringBuilder();
            join.Append(propertyQuery.Join);
            join.Append(dictionaryQuery.Join);

            StringBuilder from = new StringBuilder();
            from.Append(propertyQuery.From);
            from.Append(dictionaryQuery.From);

            sql.AppendFormat(SqlTemplate,
                from.ToString(),
                join.ToString(),
                where,
                OrderExpression.OrderClause);
            return sql.ToString();
        }
    }
}
