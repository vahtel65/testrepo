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
using Aspect;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EditorKmh
{
    public partial class SelectionRoute : ChildWindow
    {
        private ObservableCollection<RouteEntity> original = new ObservableCollection<RouteEntity>();
        private ObservableCollection<RouteEntity> route = new ObservableCollection<RouteEntity>();

        public class RouteEntity : INotifyPropertyChanged
        {
            private string _route;

            public string Route
            {
                get
                {
                    return _route;
                }
                set
                {
                    _route = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Route"));
                    }
                }
            }
           
            public RouteEntity(String route)
            {
                this._route = route;
            }

            public RouteEntity(RouteEntity entity)
            {
                this._route = entity._route;
            }

            public override string ToString()
            {
                return _route;
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        public string ResultRoute
        {
            get
            {
                if (route.Count > 0)
                {
                    return route.Select(rt => rt.Route).ToList().Aggregate<string>((first, second) => first + " " + second);
                }
                else
                {
                    return "";
                }

                
            }

        }

        public SelectionRoute(List<string> original, string route)
        {            
            original.Where(it => !String.IsNullOrEmpty(it)).ToList().ForEach(it => this.original.Add(new RouteEntity(it)));
            if (!String.IsNullOrEmpty(route))            
            {
                route.Split(' ').ToList().ForEach(it => this.route.Add(new RouteEntity(it)));
            }            

            InitializeComponent();

            OriginalList.ItemsSource = this.original;
            RouteList.ItemsSource = this.route;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void addToRoute(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                if (OriginalList.SelectedItems.Count == 1)
                {
                    if (RouteList.SelectedItems.Count == 1)
                    {
                        route.Insert(RouteList.SelectedIndex+1, new RouteEntity((RouteEntity)OriginalList.SelectedItem));
                    }
                    else
                    {
                        route.Add(new RouteEntity((RouteEntity)OriginalList.SelectedItem));
                    }
                }
            }
        }

        private void deleteFromRoute(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                if (RouteList.SelectedItems.Count == 1)
                {
                    route.RemoveAt(RouteList.SelectedIndex);
                }
            }
        }

        private void moveItemUp(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                int index = RouteList.SelectedIndex;
                if (index > 0)
                {
                    string oldValue = route[index].Route;
                    route[index].Route = route[index - 1].Route;
                    route[index - 1].Route = oldValue;
                }
            }
        }

        private void moveItemDown(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                int index = RouteList.SelectedIndex;
                if (index < route.Count() - 1)
                {
                    string oldValue = route[index].Route;
                    route[index].Route = route[index + 1].Route;
                    route[index + 1].Route = oldValue;
                }
            }
        }
    }
}

