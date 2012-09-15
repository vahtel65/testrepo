using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aspect.Domain;

namespace Aspect.Model.Query
{
    internal abstract class QueryBuilder
    {
        public QueryBuilder()
        {
        }
        public QueryBuilder(CommonDomain provider)
        {
            this.Provider = provider;
        }

        public List<IUserField> UserFields { get; set; }

        public OrderExpression OrderExpression { get; set; }

        public List<SearchSource> SearchExpression { get; set; }

        public CommonDomain Provider { get; protected set; }

        protected abstract string SqlTemplate { get; }
    }
}
