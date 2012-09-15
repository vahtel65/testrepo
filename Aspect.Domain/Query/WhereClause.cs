using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public class WhereClause
    {
        public enum Operator
        {
            AND,
            OR
        }
        private StringBuilder expression;
        /// <summary>
        /// Classification WHERE Clause
        /// </summary>
        public virtual StringBuilder Expression
        {
            get
            {
                if (expression == null) expression = new StringBuilder();
                return expression;
            }
        }

        private static string template = "WHERE {0}";
        private static string operatorAND = " AND ";

        public static string NULLResultWhereClause = "WHERE Product.ID is NULL";

        public static string BuildExpression(string column, string value)
        {
            return string.Format(template, string.Format(" {0} = '{1}' ", column, value));
        }

        public static WhereClause Join(List<WhereClause> list, Operator oper)
        {
            WhereClause entity = new WhereClause();
            string join = string.Format(" {0} ", oper.ToString());
            if (list.Count > 0)
            {
                List<string> expr = new List<string>();
                expr.AddRange(list.Where(l => l.Expression.Length > 0).Select(l => l.Expression.ToString()));
                string str = string.Join(join, expr.ToArray());
                if (str.Length > 0)
                {
                    entity.Expression.AppendFormat("( {0} )", str);
                }
            }
            return entity;
        }

        public static string BuildExpression(List<WhereClause> list)
        {
            if (list.Count > 0 && list.Where(l => l.Expression.Length > 0).Count() > 0)
            {
                //list.Where(l => l.Expression.Length > 0);
                List<string> expr = new List<string>();
                expr.AddRange(list.Where(l=>l.Expression.Length > 0).Select(l => l.Expression.ToString()));
                return string.Format(template, string.Join(operatorAND, expr.ToArray()));
            }
            return string.Empty;
        }
    }
}
