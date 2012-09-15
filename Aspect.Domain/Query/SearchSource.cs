using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Domain
{    
    public class SearchSource : WhereClause
    {
        public SearchSource()
        {
            FeildType = TypeEnum.Default;
        }
        public string Alias { get; set; }

        public string ColumnName { get; set; }

        public string FieldValue { get; set; }

        public TypeEnum FeildType { get; set; }

        public Condition FieldCond { get; set; }

        private string searchClauseTemplate = @"{2}.[{0}] LIKE '{1}%'";

        private string templateInt = @"CONVERT(int,{0}.[{1}]) {2} '{3}'";
        private string templateDecimal = @"CONVERT(decimal,REPLACE({0}.[{1}], ',', '.')) {2} REPLACE('{3}', ',', '.')";

        private string searchClauseDateTimeTemplate = @"convert(nvarchar(50), {2}.[{0}], 104) LIKE '%{1}%'";

        private string searchClauseBeingTemplate = @"{1}.[{0}] IS NOT NULL";
        //

        public override StringBuilder Expression
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                if (!String.IsNullOrEmpty(ColumnName) && !String.IsNullOrEmpty(FieldValue))
                {
                    if (this.FieldCond == Condition.Inset)
                    {
                        string list = FieldValue.Split(',').Select(p => string.Format("'{0}'", p)).Aggregate((a, b) => a + "," + b);
                        builder.AppendFormat("{0} IN ({1})", ColumnName, list);                            
                    } else
                    if (this.FieldCond == Condition.Beable)
                    {
                        builder.AppendFormat(searchClauseBeingTemplate, ColumnName, Alias);
                    } else
                    switch (this.FeildType)
                    {
                        case TypeEnum.Datetime:
                            //invent here something with FieldValue
                            builder.AppendFormat(searchClauseDateTimeTemplate, ColumnName, FieldValue, Alias);
                            break;
                        case TypeEnum.Decimal:
                            builder.AppendFormat(templateDecimal, Alias, ColumnName, FieldCond.toSqlString(), FieldValue);
                            break;
                        case TypeEnum.Integer:
                            builder.AppendFormat(templateInt, Alias, ColumnName, FieldCond.toSqlString(), FieldValue);
                            break;
                        case TypeEnum.Boolean:
                        case TypeEnum.Default:
                        default:
                            builder.AppendFormat(searchClauseTemplate, ColumnName, FieldValue, Alias);
                            break;
                    }
                    //builder.AppendFormat(searchClauseTemplate, ColumnName, FieldValue, Alias);
                }
                return builder;
            }
        }

        public static List<WhereClause> Convert(List<SearchSource> list)
        {
            return list.ConvertAll(delegate(SearchSource n)
            {
                return (WhereClause)n;
            });
        }
    }
}
