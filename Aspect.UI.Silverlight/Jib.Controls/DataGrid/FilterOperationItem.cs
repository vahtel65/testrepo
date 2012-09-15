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
    /// <summary>
    /// This class defines a filter operation for the ColumnFilterHeader.  i.e.  Equals, Contains, StartsWith.... 
    /// </summary>
    public class FilterOperationItem
    {
        public Enums.FilterOperation FilterOption { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public FilterOperationItem(Enums.FilterOperation operation, string description, string imagePath)
        {
            FilterOption = operation;
            Description = description;
            ImagePath = imagePath;
        }
    }
}
