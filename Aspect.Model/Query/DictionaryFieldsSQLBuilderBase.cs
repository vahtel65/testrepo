using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspect.Domain;

namespace Aspect.Model.Query
{
    internal abstract class DictionaryFieldsSQLBuilderBase : Aspect.Model.Query.FieldsSQLBuilder
    {
        #region Templates
        //protected string propertyFromTemplate = ",[{0}].[{2}] AS [{1}]";
        protected string propertyFromTemplate = ",[{0}].[{1}] AS [{0}.{1}]";
        protected string propertyJoinTemplate = @"
LEFT JOIN {0} {1} ON {2}.{3} = {1}.{4}
";
        #endregion

        protected IEnumerable<TreeViewResult> GetDistinctDictionaryTablesTree(List<IUserField> userFields, Aspect.Domain.CommonDomain provider)
        {
            List<Guid> list = userFields.Select(s => s.DictionaryTreeID).Distinct().ToList();
            List<Aspect.Domain.GetDictionaryTreeParentsResult> tableList = new List<Aspect.Domain.GetDictionaryTreeParentsResult>();
            foreach (Guid item in list)
            {
                List<Aspect.Domain.GetDictionaryTreeParentsResult> lll = provider.GetDictionaryTreeParents(item).ToList();
                int max =lll.Max(l => l.Level).Value;
                lll.ForEach(delegate(GetDictionaryTreeParentsResult n)
                {
                    n.Level = (n.Level - max) * -1;
                });
                tableList.AddRange(lll);
            }
            
            //return tableList.Distinct(DictionaryTreeResultEqualityComparer.Instance).OrderByDescending(d => d.Level).ToList().ConvertAll(
            return tableList.Distinct(DictionaryTreeResultEqualityComparer.Instance).OrderBy(d => d.Level).ToList().ConvertAll(
                delegate(GetDictionaryTreeParentsResult n)
                {
                    return new TreeViewResult()
                    {
                        ID = n.ID,
                        Level = (int)n.Level
                    };
                }
                ).ToList();
        }

        public DictionaryFieldsSQLBuilderBase(List<IUserField> userFields)
        {
            From = new StringBuilder();
            Join = new StringBuilder();
            foreach (Aspect.Domain.IUserField item in userFields)
            {
                //From.AppendFormat(propertyFromTemplate, item.DictionaryTree.Alias, item.DictionaryProperty.Name, item.DictionaryProperty.ColumnName);
                From.AppendFormat(propertyFromTemplate, item.DictionaryTree.Alias, item.DictionaryProperty.ColumnName);
            }
        }
    }
}
