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
using System.ComponentModel;

namespace Jib.Controls.DataGrid
{
    /// <summary>
    /// Each ColumnFilterHeader control contains a distinct list of values.  The user can select the values they 
    /// wish to filter on.  This class supports the selection of these distinct values.
    /// </summary>
    public class CheckboxComboItem : INotifyPropertyChanged
    {
        private bool _IsChecked;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    OnPropertChanged("IsChecked");
                }
            }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertChanged("Description");
                }
            }
        }

        private object _Tag;
        public object Tag
        {
            get { return _Tag; }
            set
            {
                if (_Tag != value)
                {
                    _Tag = value;
                    OnPropertChanged("Tag");
                }
            }
        }
        public override string ToString()
        {
            return Description;
        }
        private void OnPropertChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
