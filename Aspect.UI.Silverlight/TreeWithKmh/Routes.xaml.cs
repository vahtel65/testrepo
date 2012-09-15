using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Windows.Browser;
using System.ComponentModel;
using System.Linq;
using Aspect;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Globalization;
using EditorKmh;

namespace TreeWithKmh
{
    public partial class Routes : ChildWindow
    {
        private Guid _prod_id;
        private string _pn2;

        public Routes(Guid prod_id, string pn2)
        {
            this._prod_id = prod_id;
            this._pn2 = pn2;

            InitializeComponent();            
            this.Title += String.Format("\"{0}\"", _pn2);
            this.Loaded += RequestDicts;
        }

        #region Load dictionaries
        private void RequestDicts(object sender, RoutedEventArgs e)
        {
            PostRequest<Dicts> post = new PostRequest<Dicts>(this.Dispatcher, "/Technology/Service.aspx/RequestDicts");
            post.ProcessResponse += this.ProcessDicts;

            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            post.Perform("{ 'dicts': 's' }");
        }

        public Dicts _dicts;

        public void ProcessDicts(Dicts dicts)
        {
            _dicts = dicts;

            BusyIndicator.BusyContent = "Загрузка маршрутов...";
            DataBind(this, new RoutedEventArgs());
        }
        #endregion

        #region Load list of routes
        private void DataBind(object sender, RoutedEventArgs e)
        {
            PostRequest<List<transfer_route>> post = new PostRequest<List<transfer_route>>(this.Dispatcher, "/Technology/Service.aspx/RequestAllRoutes");
            post.ProcessResponse += this.ProcessListRoutes;
            
            post.Perform(string.Format("{{ 'prod_id': '{0}' }}", _prod_id));
        }

        public ObservableCollection<transfer_route> routesList;
        public void ProcessListRoutes(List<transfer_route> list)
        {
            routesList = new ObservableCollection<transfer_route>(list);
            grid.ItemsSource = routesList;

            BusyIndicator.IsBusy = false;
        }
        #endregion

        #region Editing route
        private transfer_route _edited_route;

        private void ShowSelectorWindow(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            this._edited_route = button.Tag as transfer_route;
            
            List<DictItem> newRoute = new List<DictItem>();
            SelectionRoute selectionRoute = new SelectionRoute(_dicts.Ss.Select(m => m.Name).ToList(), this._edited_route.route);
            selectionRoute.Show();
            selectionRoute.Closed += new EventHandler(selectionRoute_Closed);
        }

        public class SaveRoute_PROTO
        {
            public transfer_route saved_route { set; get; }            
        }

        void selectionRoute_Closed(object sender, EventArgs e)
        {
            SelectionRoute selectionRoute = sender as SelectionRoute;
            if ((bool)selectionRoute.DialogResult)
            {
                this._edited_route.route = selectionRoute.ResultRoute;

                PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/Service.aspx/SaveRoute");
                post.ProcessResponse += ProcessSavingRoute;
                post.Perform(new SaveRoute_PROTO() { saved_route = this._edited_route });
            }
        }
       

        public void ProcessSavingRoute(PostResult result)
        {            
        }
        #endregion

        private void ApplyCurrentRouteToAll(object sender, RoutedEventArgs e)
        {
            if (grid.SelectedItems.Count > 0)
            {
                string current_route = (grid.SelectedItem as transfer_route).route;

                foreach (transfer_route item in grid.ItemsSource)
                {
                    item.route = current_route;

                    PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/Service.aspx/SaveRoute");
                    post.ProcessResponse += ProcessSavingRoute;
                    post.Perform(new SaveRoute_PROTO() { saved_route = item });
                }
            }
        }

        private void ApplyCurrentRouteToEmpty(object sender, RoutedEventArgs e)
        {
            if (grid.SelectedItems.Count > 0)
            {
                string current_route = (grid.SelectedItem as transfer_route).route;

                foreach (transfer_route item in grid.ItemsSource)
                {
                    if (!string.IsNullOrEmpty(item.route)) continue;

                    item.route = current_route;

                    PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/Service.aspx/SaveRoute");
                    post.ProcessResponse += ProcessSavingRoute;
                    post.Perform(new SaveRoute_PROTO() { saved_route = item });
                }
            }
        }
    }
}
