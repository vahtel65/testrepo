using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Jib.Controls.DataGrid
{
    public class Enums
    {
        public enum FilterOperation
        {
            Unknown,
            Contains,
            Equals,
            StartsWith,
            EndsWith,
            GreaterThanEqual,
            LessThanEqual,
            GreaterThan,
            LessThan
        }
        public enum ColumnOption
        {
            Unknown = 0,
            AddGrouping,
            RemoveGrouping,
            PinColumn,
            UnpinColumn
        }
    }
}
