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
using System.Runtime.InteropServices;
using JeffWilcox.Utilities.Silverlight;
using System.Threading;
using ListWares;

namespace TreeWithKmh
{
    public partial class MainPage : UserControl    
    {                
        public class MetaColumn
        {
            public string Header { set; get; }
            public bool Visible { set; get; }
            public string Binding;
            public bool Booled;
            public int Width;
            public IValueConverter Converter = null;
        }

        public MainPage()
        {
            InitializeComponent();
            HtmlPage.RegisterScriptableObject("myApp", this);
            MainGridBusy.IsBusy = true;  
            Loaded += ColumnsBind;
            Loaded += RequestDicts;

            ProductInfo.Text = (string) HtmlPage.Document.GetProperty("title");            
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

            MainGridBusy.BusyContent = "Загрузка разузлованного состава...";
            DataBind(this, new RoutedEventArgs());
        }
        #endregion

        [ScriptableMember()]
        public void ShowGlobalMask()
        {
            LayoutRoot.IsBusy = true;
        }

        [ScriptableMember()]
        public void HideGlobalMask()
        {
            LayoutRoot.IsBusy = false;
        }

        List<MetaColumn> columns = new List<MetaColumn>();

        protected void ColumnsBind(object sender, RoutedEventArgs e)
        {
            //list.Add(new MetaColumn() { Header = "Уровень вхождения", Binding = "level" });

            columns.Add(new MetaColumn() { Header = "Обозначение узла", Binding = "unit_pn1" });
            columns.Add(new MetaColumn() { Header = "Наименование узла", Binding = "unit_pn2" });

            columns.Add(new MetaColumn() { Header = "Обозначение детали", Binding = "prod_pn1" });
            columns.Add(new MetaColumn() { Header = "Наименование детали", Binding = "prod_pn2" });

            columns.Add(new MetaColumn() { Header = "Позиция", Binding = "level" });
            columns.Add(new MetaColumn() { Header = "Кол-во деталей в узле по спецификации", Binding = "count" });
            //list.Add(new MetaColumn() { Header = "Единица измерения количества", Binding = "config.measure" });
            columns.Add(new MetaColumn() { Header = "Группа замены", Binding = "group_exchange" });
            columns.Add(new MetaColumn() { Header = "Главная замена", Binding = "number_exchange" });

            columns.Add(new MetaColumn() { Header = "Материал", Binding = "material" });
            columns.Add(new MetaColumn() { Header = "Вид поставки", Binding = "pvd" });
            columns.Add(new MetaColumn() { Header = "Маршрут", Binding = "route" });
            columns.Add(new MetaColumn() { Header = "Маршрут по применяемости", Binding = "route_changed", Booled = true });
            //list.Add(new MetaColumn() { Header = "Материал", Binding = "kmh.material" });
            columns.Add(new MetaColumn() { Header = "Форма заготовки", Binding = "sf" });
            columns.Add(new MetaColumn() { Header = "Размер заготовки", Binding = "ss" });
            columns.Add(new MetaColumn() { Header = "Кол-во деталей из заготовки", Binding = "sd" });
            columns.Add(new MetaColumn() { Header = "Масса штамповки", Binding = "stw" });
            columns.Add(new MetaColumn() { Header = "Масса заготовки", Binding = "sw" });
            columns.Add(new MetaColumn() { Header = "Размер поковки", Binding = "sp" });
            columns.Add(new MetaColumn() { Header = "Норма расхода", Binding = "no" });
            columns.Add(new MetaColumn() { Header = "Единица измерения нормы расхода", Binding = "um" });
            columns.Add(new MetaColumn() { Header = "Примечание", Binding = "cmt_ogt" });
            //list.Add(new MetaColumn() { Header = "Автор последнего изменения", Binding = "last_user" });
            columns.Add(new MetaColumn() { Header = "КМХ утв гл. технологом", Binding = "gotov_kmh", Booled = true });
            columns.Add(new MetaColumn() { Header = "КМХ утв гл. технологом (Дата)", Binding = "gotov_date" });
            //list.Add(new MetaColumn() { Header = "Дата последнего изменения", Binding = "date_update" });

            columns.Add(new MetaColumn() { Header = "Готовность технолога", Binding = "gotov_tech", Booled = true });
            columns.Add(new MetaColumn() { Header = "Готовность сварщика", Binding = "gotov_svar", Booled = true });
            columns.Add(new MetaColumn() { Header = "Готовность химика", Binding = "gotov_him", Booled = true });
            columns.Add(new MetaColumn() { Header = "Дата добавления", Binding = "added_date", Converter = new DateTimeConverter()});
            columns.Add(new MetaColumn() { Header = "Актуальность", Binding = "actual", Booled = true });
            columns.Add(new MetaColumn() { Header = "По приказу", Binding = "isprikaz", Booled = true });

            columns.Add(new MetaColumn() { Header = "Последние изменения (пользователь)", Binding = "last_change_user" });
            columns.Add(new MetaColumn() { Header = "Последние изменения (дата)", Binding = "last_change_date", Converter = new DateTimeConverter() });

            foreach (var metaColumn in columns)
            {
                metaColumn.Visible = true;

                if (metaColumn.Booled)
                {
                    grid.Columns.Add(new DataGridCheckBoxColumn()
                    {
                        Header = metaColumn.Header,
                        Binding = new Binding(metaColumn.Binding),                        
                        IsReadOnly = true
                    });
                }
                else
                {                    
                    grid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = metaColumn.Header,
                        Binding = new Binding(metaColumn.Binding)
                        {
                            Converter = metaColumn.Converter
                        },
                        IsReadOnly = true
                    });                 
                }                
            }
            
            // get setting from server DB
            PostRequest<List<transfer_column>> post = new PostRequest<List<transfer_column>>(this.Dispatcher, "/Technology/Service.aspx/GetColumns");
            post.ProcessResponse += delegate(List<transfer_column> list2)
            {
                foreach (var metaColumn in this.columns)
                {
                    var dbColumn = list2.Single(it => it.uid == new Guid(MD5CryptoServiceProvider.GetMd5String(metaColumn.Header)));
                    var gridColumn = grid.Columns.Single(clm => String.Equals(clm.Header, metaColumn.Header));
                    metaColumn.Visible = !dbColumn.hidden;
                    metaColumn.Width = dbColumn.width;
                    gridColumn.Width = new DataGridLength(dbColumn.width);
                    gridColumn.DisplayIndex = dbColumn.position;
                }
                SyncMetaColumnsToUserUI(false);
                //getting_columns = true;
                
                bw.DoWork += grid_ColumnWidthWorker;
                bw.RunWorkerAsync();

                grid.ColumnReordered += this.grid_ColumnDisplayIndexChanged;
                grid.LayoutUpdated += this.grid_LayoutUpdated;               
            };
            post.Perform(new SET_COLUMNS_PROTO() { ClassificationTreeId = new Guid("11110000-0000-0000-0000-000011110101") });
        }

        public class SET_COLUMNS_PROTO
        {
            public List<transfer_column> columns { set; get; }
            public Guid ClassificationTreeId;
        }

        public class GET_COLUMNS_PROTO
        {            
            public Guid ClassificationTreeId;
        }

        protected void SyncMetaColumnsToUserUI(bool ordered)
        {
            foreach (var metaColumn in this.columns)
            {
                var gridColumn = grid.Columns.Single(clm => String.Equals(clm.Header, metaColumn.Header));
                gridColumn.Visibility = metaColumn.Visible ? Visibility.Visible : Visibility.Collapsed;
                if (ordered) gridColumn.DisplayIndex = this.columns.IndexOf(metaColumn) + 2;
            }
        }

        protected void SyncColumnsToServerDB()
        {
            List<transfer_column> saving_columns = new List<transfer_column>();
            foreach (var metaColumn in this.columns)
            {
                var gridColumn = grid.Columns.Single(clm => String.Equals(clm.Header, metaColumn.Header));
                saving_columns.Add(new transfer_column()
                {
                    uid = new Guid(MD5CryptoServiceProvider.GetMd5String(metaColumn.Header)),
                    hidden = gridColumn.Visibility == Visibility.Visible ? false : true,
                    position = gridColumn.DisplayIndex,
                    width = (int)gridColumn.Width.DisplayValue
                });
            }

            PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/Service.aspx/SetColumns");
            post.Perform(new SET_COLUMNS_PROTO() { columns = saving_columns, ClassificationTreeId = new Guid("11110000-0000-0000-0000-000011110101") });
        }

        private BackgroundWorker bw = new BackgroundWorker();
        DateTime colWidthLastChange = DateTime.MinValue;

        private void grid_ColumnWidthWorker(object sender, DoWorkEventArgs e)
        {            
            while (!e.Cancel)
            {
                lock (bw)
                {
                    this.Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        if (colWidthLastChange != DateTime.MinValue)
                        {
                            if ((DateTime.Now - colWidthLastChange).TotalSeconds > 5)
                            {
                                SyncColumnsToServerDB();
                                colWidthLastChange = DateTime.MinValue;                                
                            }
                        }
                    }));
                }
                
                System.Threading.Thread.Sleep(2000);                
            }
        }

        private AutoResetEvent _resetEvent = new AutoResetEvent(false);

        private void grid_LayoutUpdated(object sender, EventArgs e)
        {
            lock (bw)
            {
                colWidthLastChange = DateTime.Now;                
            }                
                // tracking columns width change
            /*if (!getting_columns) return;

            bool need_sync = false;
            foreach (var metaColumn in this.columns)
            {
                var gridColumn = grid.Columns.Single(clm => String.Equals(clm.Header, metaColumn.Header));
                if (metaColumn.Width != gridColumn.Width.DisplayValue)
                {
                    metaColumn.Width = (int) gridColumn.Width.DisplayValue;
                    need_sync = true;
                }
            }

            if (need_sync) SyncColumnsToServerDB();*/
        }

        // show windows with columns selector
        protected void ShowColumnsSelector(object sender, RoutedEventArgs e)
        {
            this.columns = this.columns.OrderBy(col => grid.Columns.Single(it => (string) it.Header == col.Header).DisplayIndex).ToList();
            SelectorColumns window = new SelectorColumns(this.columns);            
            window.Closed += new EventHandler(delegate(object sender2, EventArgs e2)
            {
                if (window.ApplyColumns)
                {
                    // show changes on UserUI
                    SyncMetaColumnsToUserUI(true);

                    // send changes to server DB
                    SyncColumnsToServerDB();
                }
            });
            window.Show();            
        }

        /*public class PostAnswerData
        {
            public string name;
        }

        public class PostResponse<T>
        {
            public T d;
        }

        public void DoPost(object sender, RoutedEventArgs e)
        {
            //PostRequest post = new PostRequest(this.Dispatcher, new Uri(HtmlPage.Document.DocumentUri, "/Technology/TreeWithKmh.aspx/ReturnAnswer"));
            //post.Perform<PostResponse<PostAnswerData>>("Dmitriy");


        }*/

        private List<transfer> transferList = new List<transfer>();

        private void DataBind(object sender, RoutedEventArgs e)
        {
            PostRequest<List<transfer>> post = new PostRequest<List<transfer>>(this.Dispatcher, "/Technology/Service.aspx/GetTechConsist");
            post.ProcessResponse += this.ProcessListKmh;
                        
            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            post.Perform(string.Format("{{ 'prod_id': '{0}', 'order_id': '{1}' }}", urlparams["prodid"], urlparams.Keys.Contains("orderid") ? urlparams["orderid"] : Guid.Empty.ToString()));
        }

        public void ProcessListKmh(List<transfer> list)
        {            
            ObservableCollection<transfer> transferList = new ObservableCollection<transfer>(list);
            grid.FilteredItemsSource = transferList; 
            
            MainGridBusy.IsBusy = false;            
        }

        /*private void UpdateState(object sender, RoutedEventArgs e)
        {
            MainGridBusy.IsBusy = true;
            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            string baseAddress = Application.Current.Host.Source.ToString();
            baseAddress = baseAddress.Substring(0, baseAddress.LastIndexOf('/')); // removing /App.xap
            Uri dataAddress = new Uri(baseAddress + String.Format("/GetPartOfTree.aspx?prodid={0}", urlparams["prodid"]));

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += DataBind;
            wc.DownloadStringAsync(dataAddress);  
        }

       // private PagedCollectionView _pagedProductsView;
        private void DataBind(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<transfer>));
                ObservableCollection<transfer> list = new ObservableCollection<transfer>((IEnumerable<transfer>)serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(e.Result))));

                ObservableCollection<transfer> transferList = new ObservableCollection<transfer>(list);
                grid.FilteredItemsSource = transferList;                
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
            MainGridBusy.IsBusy = false;
        }  */      

        protected void openProductMenu(object sender, RoutedEventArgs e)
        {
            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;

            ShowGlobalMask();

            Guid prodid = (Guid)((Button)e.OriginalSource).Tag;
            HtmlPage.Window.Invoke("tb_show", new string[] {
                "Меню действий",
                String.Format("/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}{2}", 
                        prodid.ToString(), 
                        Guid.Empty, 
                        urlparams.Keys.Contains("orderid") ? "&order_id=" + urlparams["orderid"] : ""),
                "/img/close.gif"
            });
        }

        private void createNewKmh(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string editurl;
            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            if (urlparams.Keys.Contains("orderid"))
            {
                editurl = string.Format("/Technology/EditorKmh.aspx?prodid={0}&orderid={1}", button.Tag.ToString(), urlparams["orderid"]);
            }
            else
            {
                editurl = string.Format("/Technology/EditorKmh.aspx?prodid={0}", button.Tag.ToString());
            }            
            
            HtmlPage.Window.Navigate(new Uri(editurl, UriKind.Relative), "_blank");
        }

        private void HideColumn(object sender, RoutedEventArgs e)
        {
            grid.Columns.Where(c => c.Header.ToString() == "Title").Single().Visibility = System.Windows.Visibility.Collapsed;            
        }

        private void RouteByApplicability(object sender, RoutedEventArgs e)
        {
            if (grid.SelectedItems.Count != 1) return;

            this._edited_route = grid.SelectedItem as transfer;

            Routes routesWin = new Routes(this._edited_route.prod_id, this._edited_route.prod_pn2);
            routesWin.Show();
            //routesWin.Closed += routesWin_Closed;
        }

        /*void routesWin_Closed(object sender, EventArgs e)
        {
            try
            {
                Routes routesWin = sender as Routes;

                var route = (from r in routesWin.routesList
                             where r.unitNomenID == this._edited_route.unit_id
                             && r.prodNomenID == this._edited_route.prod_id
                             select r).Single();

                if (route.route != this._edited_route.route)
                {
                    this._edited_route.route = route.route;
                    this._edited_route.route_changed = true;
                }
            }
            catch (Exception ee) {
                MessageBox.Show(ee.Message);
            }
        }*/

        private transfer _edited_route;
        
        /// <summary>
        /// Открыть окно "Маршрут по применяемости"
        /// </summary>
        private void ShowSelectorWindow(object sender, RoutedEventArgs e)
        {
            if (grid.SelectedItems.Count != 1) return;

            this._edited_route = grid.SelectedItem as transfer;

            List<DictItem> newRoute = new List<DictItem>();
            SelectionRoute selectionRoute = new SelectionRoute(_dicts.Ss.Select(m => m.Name).ToList(), this._edited_route.route_changed ? this._edited_route.route : "");
            selectionRoute.Show();
            selectionRoute.Closed += new EventHandler(selectionRoute_Closed);
        }

        public class SaveRoute_PROTO
        {
            public transfer saved_route { set; get; }
        }

        void selectionRoute_Closed(object sender, EventArgs e)
        {
            SelectionRoute selectionRoute = sender as SelectionRoute;
            if ((bool)selectionRoute.DialogResult)
            {
                this._edited_route.route = selectionRoute.ResultRoute;
                this._edited_route.route_changed = true;

                PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/Service.aspx/SaveRoute2");
                post.ProcessResponse += ProcessSavingRoute;
                post.Perform(new SaveRoute_PROTO() { saved_route = this._edited_route });
            }
        }

        public void ProcessSavingRoute(PostResult result)
        {
        }

        private void grid_ColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
        {            
            SyncColumnsToServerDB();
        }

        public class GerenateDates_PROTO
        {
            public Guid order_id { set; get; }
            public Guid product_id { set; get; }
            public DateTime gen_date { set; get; }
        }

        private void GenerateDatesForOrder(object sender, RoutedEventArgs e)
        {
            PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/TechnDates.aspx/GenerateForOrder");
            post.ProcessResponse += new PostRequest<PostResult>.ProcessResponseEvent(delegate (PostResult result)
                {

                });
            post.Perform(new GerenateDates_PROTO() { 
                order_id = new Guid(dbg_order_id.Text),
                product_id = new Guid(dbg_product_id.Text),
                gen_date = DateTime.Now
            });
        }

        private void ShowReadinessForm(object sender, RoutedEventArgs e)
        {
            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            if (grid.SelectedItems.Count != 1) return;
            if (!urlparams.Keys.Contains("orderid"))
            {
                MessageBox.Show("Возможность проставить готовность есть только для приказных составов");
                return;
            }

            var current_row = grid.SelectedItem as transfer;            

            var modalForm = new ReadinessForm()
                {
                    product_pn1 = current_row.prod_pn1,
                    product_id = current_row.prod_id,
                    order_id =  new Guid(urlparams["orderid"]),
                    current_date = DateTime.Now,
                };
            modalForm.Show();
        }

        private void btnShowDevButtons(object sender, RoutedEventArgs e)
        {
            if (DevButtons.Visibility == Visibility.Collapsed)
            {
                DevButtons.Visibility = Visibility.Visible;
            }
            else
            {
                DevButtons.Visibility = Visibility.Collapsed;
            }

        }       
    }
    
    
}
