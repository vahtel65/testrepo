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
using System.Windows.Data;

namespace Jib.Controls.DataGrid
{
    /// <summary>
    /// The FilterColumnInfo class has the information about the column binding information.  It is used to configure
    /// the ColumnFilterHEader and the ColumnOptionControl
    /// </summary>
    public class FilterColumnInfo
    {
        public string PropertyPath { get; set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }
        public System.Globalization.CultureInfo ConverterCultureInfo { get; set; }
        public Type PropertyType { get; set; }

    }
}
