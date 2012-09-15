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
using System.Windows.Data;
using Aspect;
using System.Runtime.Serialization.Json;
using System.Collections.ObjectModel;
using TreeWithKmh;

namespace ListWares
{
    public partial class MainPage : UserControl
    {
        public class MetaColumn
        {
            public string Header;
            public string Binding;
            public bool Booled;
            public bool DateConverter;
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

        public MainPage()
        {
            InitializeComponent();
            HtmlPage.RegisterScriptableObject("myApp", this);
            MainGridBusy.IsBusy = true;
            Loaded += ColumnsBind;
            Loaded += DataBind;
        }

        protected void openProductMenu(object sender, RoutedEventArgs e)
        {
            ShowGlobalMask();

            Guid prodid = (Guid)((Button)e.OriginalSource).Tag;
            HtmlPage.Window.Invoke("tb_show", new string[] {
                "Меню действий",
                String.Format("/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}", prodid.ToString(), Guid.Empty),
                "/img/close.gif"
            });
        }

        protected void ColumnsBind(object sender, RoutedEventArgs e)
        {
            List<MetaColumn> list = new List<MetaColumn>();

            list.Add(new MetaColumn() { Header = "Обозначение изделия", Binding = "ware_pn1" });
            list.Add(new MetaColumn() { Header = "Наименование изделия", Binding = "ware_pn2" });

            list.Add(new MetaColumn() { Header = "Дата создания", Binding = "created", DateConverter = true });
            list.Add(new MetaColumn() { Header = "Дата формирования приказа", Binding = "LastVersCreatedDate", DateConverter = true });            
            list.Add(new MetaColumn() { Header = "Номер приказа", Binding = "order_number" });
            list.Add(new MetaColumn() { Header = "Год приказа", Binding = "order_year" });
            list.Add(new MetaColumn() { Header = "Примечание", Binding = "note" });
            list.Add(new MetaColumn() { Header = "Автор изменения", Binding = "author" });
            list.Add(new MetaColumn() { Header = "Дата изменения", Binding = "date" });

            //list.Add(new MetaColumn() { Header = "Технолога готовность", Binding = "gotov_tech", Booled = true });
            //list.Add(new MetaColumn() { Header = "Сварщика готовность", Binding = "gotov_svar", Booled = true });
            //list.Add(new MetaColumn() { Header = "Химика готовность", Binding = "gotov_him", Booled = true });

            list.Add(new MetaColumn() { Header = "Технолога готовность", Binding = "gotov_techn_date", DateConverter = true });
            list.Add(new MetaColumn() { Header = "Сварщика готовность", Binding = "gotov_svar_date", DateConverter = true });
            list.Add(new MetaColumn() { Header = "Химика готовность", Binding = "gotov_him_date", DateConverter = true });

            foreach (var metaColumn in list)
            {
                Binding binding = new Binding(metaColumn.Binding);
                binding.Mode = BindingMode.TwoWay;

                if (metaColumn.DateConverter)
                {
                    binding.Converter = new DateTimeConverter();
                }

                if (metaColumn.Booled)
                {
                    grid.Columns.Add(new DataGridCheckBoxColumn()
                    {
                        Header = metaColumn.Header,
                        Binding = binding,
                        IsReadOnly = true
                    });
                }
                else
                {
                    grid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = metaColumn.Header,
                        Binding = binding,
                        IsReadOnly = true
                    });
                }
            }
        }

        protected void DataBind(object sender, RoutedEventArgs e)
        {            
            PostRequest<List<transfer_ware>> post = new PostRequest<List<transfer_ware>>(this.Dispatcher, "/Technology/Service.aspx/RequestListWares");
            post.ProcessResponse += this.ProcessListWares;            
            post.Perform("{ }");
        }

        public void ProcessListWares(List<transfer_ware> list)
        {
            grid.FilteredItemsSource = list;
            MainGridBusy.IsBusy = false;            
        }

        private void ShowReadinessForm(object sender, RoutedEventArgs e)
        {
            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            if (grid.SelectedItems.Count != 1) return;            

            var current_row = grid.SelectedItem as transfer_ware;

            var modalForm = new ReadinessForm()
            {
                product_pn1 = current_row.ware_pn1,
                product_id = current_row.ware_id,
                order_id = current_row.order_id,
                current_date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
            };
            modalForm.Closed += new EventHandler(delegate(Object sender2, EventArgs e2)
                {
                    current_row.gotov_him_date = modalForm.techn_dates.him_date;
                    current_row.gotov_techn_date = modalForm.techn_dates.techn_date;
                    current_row.gotov_svar_date = modalForm.techn_dates.svar_date;
                });
            modalForm.Show();            
        }        
    }
}
