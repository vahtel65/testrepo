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
using System.Windows.Navigation;
using System.Collections.ObjectModel;

namespace TreeWithKmh
{
    public partial class SelectorColumns : ChildWindow
    {
        List<TreeWithKmh.MainPage.MetaColumn> originalColumns;
        List<TreeWithKmh.MainPage.MetaColumn> columns;

        public bool ApplyColumns = false;

        public SelectorColumns(List<TreeWithKmh.MainPage.MetaColumn> columns)
        {
            //this.columns = new ObservableCollection<TreeWithKmh.MainPage.MetaColumn>(columns);
            this.originalColumns = columns;
            this.columns = columns.Select(it => new TreeWithKmh.MainPage.MetaColumn()
            {
                Header = it.Header,
                Visible = it.Visible,
                Binding = it.Binding,
                Booled = it.Booled,
                Converter = it.Converter
            }).ToList();

 
            InitializeComponent();
            ColumnList.ItemsSource = this.columns;
        }

        // close current form without applying
        private void actionClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // apply user selection
        private void actionApply(object sender, RoutedEventArgs e)
        {
            ApplyColumns = true;

            this.originalColumns.Clear();
            foreach (var column in this.columns)
            {
                this.originalColumns.Add(new MainPage.MetaColumn()
                {
                    Header = column.Header,
                    Visible = column.Visible,
                    Binding = column.Binding,
                    Booled = column.Booled,
                    Converter = column.Converter
                });                
            }

            Close();
        }
        
        private void moveItemUp(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                int index = ColumnList.SelectedIndex;
                if (index > 0)
                {

                    var oldValue = columns.ElementAt(index - 1); 
                    columns.RemoveAt(index - 1);
                    columns.Insert(index, oldValue);

                    ColumnList.ItemsSource = null;
                    ColumnList.ItemsSource = this.columns;
                    ColumnList.SelectedIndex = index - 1;
                }                
            }
        }

        private void moveItemDown(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                int index = ColumnList.SelectedIndex;
                if (index < columns.Count() - 1)
                {
                    var oldValue = columns.ElementAt(index + 1);
                    columns.RemoveAt(index + 1);
                    columns.Insert(index, oldValue);

                    ColumnList.ItemsSource = null;
                    ColumnList.ItemsSource = this.columns;
                    ColumnList.SelectedIndex = index + 1;
                }                
            }
        }
    }
}
