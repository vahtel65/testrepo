using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Model.ProductDomain.Query
{
    public class CustomClassificationWhereClauseBuilder : Aspect.Domain.WhereClause
    {
        #region Templates
        private string customClassificationWhereClauseTemplate = @"CustomClassificationNodeProduct.CustomClassificationNodeID IN ({0})";
        #endregion

        /*private StringBuilder expression;
        /// <summary>
        /// Classification WHERE Clause
        /// </summary>
        public override StringBuilder Expression
        {
            get
            {
                if (expression == null) expression = new StringBuilder();
                return expression;
            }
        }*/

        public CustomClassificationWhereClauseBuilder(List<Guid?> nodes)
        {
            if (nodes.Count > 0)
            {
                List<string> list = new List<string>();
                foreach (Guid? item in nodes)
                {
                    if (item != null) list.Add(string.Format("'{0}'", ((Guid)item).ToString()));
                }
                string[] ids = list.ToArray();
                if (ids.Count() > 0)
                {
                    Expression.AppendFormat(customClassificationWhereClauseTemplate, string.Join(",", ids));
                }
            }
        }


    }
}
