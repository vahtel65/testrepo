using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{
    public enum SortDirection
    {
        desc,
        asc
    }

    [Serializable]
    public class OrderExpression
    {
        public OrderExpression()
        {
            Expression = String.Empty;//ProductProvider.ProductName;
            this.SortDirection = SortDirection.asc;
            ColumnType = TypeEnum.Default;
        }

        private string template = "ORDER BY {0} {1}";

        public string Expression { get; set; }

        //public bool UseBrackets { get; set; }

        public SortDirection SortDirection { get; set; }

        public TypeEnum ColumnType { get; set; }

        public string OrderClause
        {
            get
            {
                if (Expression.Length > 0)
                {
                    string expr = Expression;
                    switch (ColumnType)
                    {
                        case TypeEnum.Boolean:
                            expr = String.Format("CAST({0} as bit)", this.Expression);
                            break;
                        case TypeEnum.Datetime:
                            expr = String.Format("CAST({0} as datetime)", this.Expression);
                            break;
                        case TypeEnum.Integer:
                            expr = String.Format("CAST({0} as integer)", this.Expression);
                            break;
                        case TypeEnum.Decimal:
                            expr = String.Format("CAST({0} as decimal(12,6))", this.Expression);
                            break;
                        case TypeEnum.Default:
                        default:
                            expr = string.Format("{0}",this.Expression);
                            break;
                            //return string.Format(template, Expression, SortDirection.ToString());
                    }
                    return string.Format(template, /*Expression*/expr, SortDirection.ToString());
                }
                else return string.Empty;
            }
        }
    }
}
