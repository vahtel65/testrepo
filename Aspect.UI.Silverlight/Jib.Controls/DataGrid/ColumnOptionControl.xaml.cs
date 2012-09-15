using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Jib.Controls.DataGrid
{
    /// <summary>
    /// This control is added before the HeaderContent in the ColumHeader.  It provides the ability to group and pin the columns
    /// </summary>
    public partial class ColumnOptionControl : UserControl, INotifyPropertyChanged
    {
        ColumnOptionItem _addPin = new ColumnOptionItem(Enums.ColumnOption.PinColumn, "Закрепить колонку", "/Jib.Controls;component/Images/PinUp.png");
        ColumnOptionItem _addGroup = new ColumnOptionItem(Enums.ColumnOption.AddGrouping, "Сгруппировать", "/Jib.Controls;component/Images/GroupBy.png");
        ColumnOptionItem _removePin = new ColumnOptionItem(Enums.ColumnOption.UnpinColumn, "Открепить колонку", "/Jib.Controls;component/Images/pinDown.png");
        ColumnOptionItem _removeGroup = new ColumnOptionItem(Enums.ColumnOption.RemoveGrouping, "Разгруппировать", "/Jib.Controls;component/Images/RemoveGroupBy.png");

        private bool _CanGroup = true;
        public bool CanGroup
        {
            get { return _CanGroup; }
            set
            {
                if (value != _CanGroup)
                {
                    _CanGroup = value;
                    OnPropertyChanged("CanGroup");
                    SetOptions();
                }
            }
        }

        private bool _CanPin = true;
        public bool CanPin
        {
            get { return _CanPin; }
            set
            {
                if (value != _CanPin)
                {
                    _CanPin = value;
                    OnPropertyChanged("CanPin");
                    SetOptions();
                }
            }
        }

        private bool _IsGrouped;
        public bool IsGrouped
        {
            get { return _IsGrouped; }
            set
            {
                if (value != _IsGrouped)
                {
                    _IsGrouped = value;
                    OnPropertyChanged("IsGrouped");
                    SetOptions();
                }
            }
        }

        private bool _IsPinned;
        public bool IsPinned
        {
            get { return _IsPinned; }
            set
            {
                if (value != _IsPinned)
                {
                    _IsPinned = value;
                    OnPropertyChanged("IsPinned");
                    SetOptions();
                }
            }
        }

        private void SetOptions()
        {
            ColumnOptions.Clear();
            if (CanPin)
            {
                if (IsPinned)
                    ColumnOptions.Add(_removePin);
                else
                    ColumnOptions.Add(_addPin);
            }
            if (CanGroup)
            {
                if (IsGrouped)
                    ColumnOptions.Add(_removeGroup);
                else
                    ColumnOptions.Add(_addGroup);
            }
            if (!CanGroup && !CanPin)
                this.Visibility = System.Windows.Visibility.Collapsed;
        }

        private ColumnOptionItem _SelectedColumnOptionItem;
        public ColumnOptionItem SelectedColumnOptionItem
        {
            get { return _SelectedColumnOptionItem; }
            set
            {
                if (_SelectedColumnOptionItem != value)
                {
                    _SelectedColumnOptionItem = value;
                    OnPropertyChanged("SelectedColumnOptionItem");
                    cbOptions.IsDropDownOpen = false;

                }
            }
        }

        public ObservableCollection<ColumnOptionItem> ColumnOptions { get; private set; }

        private FilterColumnInfo _FilterColumnInfo;
        public FilterColumnInfo FilterColumnInfo
        {
            get { return _FilterColumnInfo; }
            set
            {
                if (value != _FilterColumnInfo)
                {
                    _FilterColumnInfo = value;
                }
            }
        }

        private DataGridColumn _Column;
        public DataGridColumn Column
        {
            get { return _Column; }
            set
            {
                if (value != _Column)
                {
                    _Column = value;
                }
            }
        }

        public ColumnOptionControl()
        {
            ColumnOptions = new ObservableCollection<ColumnOptionItem>();
            InitializeComponent();
            ColumnOptions.Add(_addPin);
            ColumnOptions.Add(_addGroup);
            this.DataContext = this;
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// If a filterColumnInfo is set, the control is visible.  As each ColumnOptionControl is loaded in the grid, the relating column
        /// is used to build FilterColumnInfo for the control.
        /// </summary>
        /// <param name="filterColumnInfo">Binding information about the column</param>
        internal void ResetOptionValues(FilterColumnInfo filterColumnInfo)
        {
            FilterColumnInfo = filterColumnInfo;
            if (!CanGroup && !CanPin)
                this.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                if (!string.IsNullOrWhiteSpace(filterColumnInfo.PropertyPath))
                    this.Visibility = System.Windows.Visibility.Visible;
                else
                    this.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #region IPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
        #endregion
    }
}
