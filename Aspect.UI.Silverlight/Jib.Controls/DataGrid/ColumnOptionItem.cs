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
    /// This class is used to wrap a single column option used by the ColumnOptionControl
    /// </summary>
    public class ColumnOptionItem
    {
        public Enums.ColumnOption ColumnOption { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public ColumnOptionItem(Enums.ColumnOption operation, string description, string imagePath)
        {
            ColumnOption = operation;
            Description = description;
            ImagePath = imagePath;
        }
    }
}
