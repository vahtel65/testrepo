using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Model.ProductDomain.Query
{
    internal class DictionaryFieldsSQLBuilder : Aspect.Model.Query.DictionaryFieldsSQLBuilderBase
    {
        public DictionaryFieldsSQLBuilder(List<Aspect.Domain.IUserField> userFields, Aspect.Domain.CommonDomain provider)
            : base(userFields)
        {
            IEnumerable<Aspect.Domain.TreeViewResult> tableList = this.GetDistinctDictionaryTablesTree(userFields, provider);
            foreach (Aspect.Domain.TreeViewResult node in tableList)
            {
                Aspect.Domain.DictionaryTree item = provider.DictionaryTrees.Single(d => d.ID == node.ID);

                if (item.ParentID != null)
                {
                    Join.AppendFormat(propertyJoinTemplate, item.Dictionary.TableName, item.Alias, item.DictionaryTree1.Alias, item.FK, item.PK);
                }
                else
                {
                    Join.AppendFormat(propertyJoinTemplate, item.Dictionary.TableName, item.Alias, Aspect.Model.ProductDomain.ProductProvider.ProductTable, item.FK, item.PK);
                }
            }
        }
    }
}
