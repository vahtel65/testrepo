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
using System.Windows.Browser;
using Aspect;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Applicability
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {            
            InitializeComponent();
            HtmlPage.RegisterScriptableObject("myApp", this);
            Loaded += ColumnsBind;
            Loaded += GetApplicability;

            ProductInfo.Text = (string)HtmlPage.Document.GetProperty("title");
        }

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

        #region Columns binding
        public class MetaColumn
        {
            public string Header;
            public string Binding;
            public bool Booled;
            public IValueConverter Converter = null;
        }

        protected void ColumnsBind(object sender, RoutedEventArgs e)
        {
            List<MetaColumn> list = new List<MetaColumn>();

            list.Add(new MetaColumn() { Header = "Обозначение изделия", Binding = "pn1" });
            list.Add(new MetaColumn() { Header = "Наименование изделия", Binding = "pn2" });
            list.Add(new MetaColumn() { Header = "Количество", Binding = "count" });
            list.Add(new MetaColumn() { Header = "Версия", Binding = "version" });
            list.Add(new MetaColumn() { Header = "Актуальность", Binding = "actual", Booled = true });
            list.Add(new MetaColumn() { Header = "Номер приказа", Binding = "order_number" });
            list.Add(new MetaColumn() { Header = "Год приказа", Binding = "order_year" });
            list.Add(new MetaColumn() { Header = "Верхний уровень", Binding = "top_ware", Booled = true });            

            foreach (var metaColumn in list)
            {
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
                        Binding = new Binding(metaColumn.Binding),
                        IsReadOnly = true
                    });
                }
            }
        }
        #endregion

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

        #region GetApplicability
        private void GetApplicability(object sender, RoutedEventArgs e)
        {
            PostRequest<List<transfer_appl>> post = new PostRequest<List<transfer_appl>>(this.Dispatcher, "/Technology/Service.aspx/GetApplicability");
            post.ProcessResponse += this.ProcessApplicability;

            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            post.Perform(String.Format("{{ 'prodid': '{0}' }}", urlparams["prodid"]));
        }

        public Dicts _dicts;

        public void ProcessApplicability(List<transfer_appl> appl_list)
        {
            ObservableCollection<transfer_appl> transferList = new ObservableCollection<transfer_appl>(appl_list);
            grid.FilteredItemsSource = transferList;

            MainGridBusy.IsBusy = false;
        }
        #endregion
    }
}
