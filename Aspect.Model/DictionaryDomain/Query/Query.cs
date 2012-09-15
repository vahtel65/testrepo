using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspect.Domain;

namespace Aspect.Model.DictionaryDomain.Query
{
    internal class Query : Aspect.Model.Query.QueryBuilder
    {
        #region Templates
        /// <summary>
        /// 0 - from
        /// 1 - join
        /// 2 - where
        /// 3 - order
        /// 4 - tablename
        /// 5 - primarykey
        /// 6 - prim alias
        /// </summary>
        protected override string SqlTemplate
        {
            get 
            {
                return @"
SELECT 
distinct
[{6}].[{5}] as ID
{0}
FROM {4} as [{6}]
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

        public string BuildEntityQuery(Guid ID, Guid treeNodeID)
        {
            StringBuilder sql = new StringBuilder();
            //-collect from and join values-
            Aspect.Model.Query.FieldsSQLBuilder dictionaryQuery = new DictionaryFieldsSQLBuilder(UserFields, Provider, treeNodeID);
            

            StringBuilder join = new StringBuilder();
            join.Append(dictionaryQuery.Join);

            StringBuilder from = new StringBuilder();
            from.Append(dictionaryQuery.From);
            //
            Aspect.Domain.DictionaryTree entity = Provider.DictionaryTrees.Single(d => d.ID == treeNodeID);
            //item.Dictionary.TableName

            string where = WhereClause.BuildExpression(string.Format("[{0}].[{1}]", entity.Alias, entity.PK), ID.ToString());
            //
            sql.AppendFormat(SqlTemplate,
                from.ToString(),
                join.ToString(),
                where,
                string.Empty,
                entity.Dictionary.TableName,
                entity.PK,
                entity.Alias);
            return sql.ToString();
        }

        /// <summary>
        /// Фильтрует материалы при выдаче (_dictNomen.cod > 1000000)
        /// </summary>
        public string BuildListQueryEx(Guid treeNodeID)
        {
            StringBuilder sql = new StringBuilder();
            //--
            string where = WhereClause.BuildExpression(SearchSource.Convert(this.SearchExpression));
            
            //-collect from and join values-
            Aspect.Model.Query.FieldsSQLBuilder dictionaryQuery = new DictionaryFieldsSQLBuilder(UserFields, Provider, treeNodeID);
            
            StringBuilder join = new StringBuilder();
            join.Append(dictionaryQuery.Join);

            StringBuilder from = new StringBuilder();
            from.Append(dictionaryQuery.From);

            Aspect.Domain.DictionaryTree entity = Provider.DictionaryTrees.Single(d => d.ID == treeNodeID);

            sql.AppendFormat(SqlTemplate,
                from.ToString(),
                join.ToString(),
                where,
                OrderExpression.OrderClause, 
                entity.Dictionary.TableName,
                entity.PK,
                entity.Alias);
            return sql.ToString();
        }
        
        public string BuildListQuery(Guid treeNodeID)
        {
            StringBuilder sql = new StringBuilder();
            //--
            string where = WhereClause.BuildExpression(SearchSource.Convert(this.SearchExpression));
            
            //-collect from and join values-
            Aspect.Model.Query.FieldsSQLBuilder dictionaryQuery = new DictionaryFieldsSQLBuilder(UserFields, Provider, treeNodeID);
            
            StringBuilder join = new StringBuilder();
            join.Append(dictionaryQuery.Join);

            StringBuilder from = new StringBuilder();
            from.Append(dictionaryQuery.From);

            Aspect.Domain.DictionaryTree entity = Provider.DictionaryTrees.Single(d => d.ID == treeNodeID);

            sql.AppendFormat(SqlTemplate,
                from.ToString(),
                join.ToString(),
                where,
                OrderExpression.OrderClause, 
                entity.Dictionary.TableName,
                entity.PK,
                entity.Alias);
            return sql.ToString();
        }
    }
}
