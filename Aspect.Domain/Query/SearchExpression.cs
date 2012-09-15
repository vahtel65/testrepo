using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Aspect.Domain
{
    public enum Condition
    {
        [DescriptionAttribute("~")]     Equal,
        [DescriptionAttribute("!~")]    Nequal,
        [DescriptionAttribute(">")]     More,
        [DescriptionAttribute("<")]     Less,
        [DescriptionAttribute("in")]    Inset,
        [DescriptionAttribute("be")]    Beable
    }

    public static class ConditionUtil
    {
        public static string toString(this Condition e)
        {
            var s = e.ToString();
            var fi = e.GetType().GetField(s);
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
                s = attributes[0].Description;
            return s;
        }

        public static string toSqlString(this Condition e)
        {
            switch (e)
            {
                case Condition.Equal: return "=";
                case Condition.Nequal: return "<>";
                case Condition.More: return ">";
                case Condition.Less: return "<";
                default: return "";
            }
        }
    }

    public class SearchExpression
    {
        public Guid FieldID { get; set; }

        public string FieldName { get; set; }

        public string FieldValue { get; set; }

        public int Order { get; set; }

        public Condition FieldCond { get; set; }
    }
}
