using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspect.Domain;

namespace Aspect.Model.DictionaryDomain.Query
{
    internal class DictionaryFieldsSQLBuilder : Aspect.Model.Query.DictionaryFieldsSQLBuilderBase
    {
        public DictionaryFieldsSQLBuilder(List<IUserField> userFields, Aspect.Domain.CommonDomain provider, Guid topNode)
            : base(userFields)
        {
            IEnumerable<Aspect.Domain.TreeViewResult> tableList = this.GetDistinctDictionaryTablesTree(userFields, provider);

            List<Aspect.Domain.TreeViewResult> childs = provider.GetDictionaryTreeChilds(topNode).ToList();
     

            foreach (Aspect.Domain.TreeViewResult node in tableList)
            {
                Aspect.Domain.DictionaryTree item = provider.DictionaryTrees.Single(d => d.ID == node.ID);

                if (childs.Contains(node, Aspect.Model.Query.DictionaryTreeEqualityComparer.Instance))
                {
                    if (item.ParentID == null) throw new ArgumentOutOfRangeException(string.Format("DictionaryTreeID {0}", item.ID));
                    Join.AppendFormat(propertyJoinTemplate, item.Dictionary.TableName, item.Alias, item.DictionaryTree1.Alias, item.FK, item.PK);
                }
            }
        }
    }
}
