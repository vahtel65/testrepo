using System;
using System.Collections;
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
using System.Windows.Data;
using System.Windows.Browser;
using System.Globalization;
using System.Reflection;

namespace UniGrid
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            HtmlPage.RegisterScriptableObject("myApp", this);
            ConnectToDataSource();
        }

        [ScriptableMember()]
        public void ShowGlobalMask(String message)
        {
            LayoutRoot.BusyContent = message;
            LayoutRoot.IsBusy = true;
        }

        [ScriptableMember()]
        public void HideGlobalMask()
        {
            LayoutRoot.IsBusy = false;
        }

        protected void openProductMenu(object sender, RoutedEventArgs e)
        {
            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;

            ShowGlobalMask("Загружается меню действий ...");

            PropertyInfo propInfo = grid.SelectedItem.GetType().GetProperty("productID");
            string productID = propInfo.GetValue(grid.SelectedItem,null).ToString();
            
            HtmlPage.Window.Invoke("tb_show", new string[] {
                "Меню действий",
                String.Format("/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}{2}", 
                        productID, 
                        Guid.Empty,
                        /*urlparams.Keys.Contains("orderid") ? "&order_id=" + urlparams["orderid"] :*/ ""),
                "/img/close.gif"
            });
        }

        private void BindColumns(List<UniColumn> columns)
        {
            foreach (var column in columns)
            {
                switch (column.uniType)
                {
                    case UniColumn.UniType.String: {
                        grid.Columns.Add(new DataGridTextColumn()
                        {
                            Header = column.header,
                            Binding = new Binding(column.dataBind),
                            IsReadOnly = true
                        });
                        break;
                    };
                    case UniColumn.UniType.Boolean: {
                        grid.Columns.Add(new DataGridCheckBoxColumn()
                        {
                            Header = column.header,
                            Binding = new Binding(column.dataBind),
                            IsReadOnly = true
                        });
                        break;
                    }
                    case UniColumn.UniType.Decimal: {
                        grid.Columns.Add(new DataGridTextColumn()
                        {
                            Header = column.header,
                            Binding = new Binding(column.dataBind),
                            IsReadOnly = true
                        });
                        break;
                    };
                    case UniColumn.UniType.ProductMenu: {
                        DataGridTemplateColumn template = new DataGridTemplateColumn();
                        template.CellTemplate = (DataTemplate)this.Resources["ProductMenuCellTemplate"];
                        grid.Columns.Add(template);
                        break;
                    };
                }
            }                    
        }

        private void ConnectToDataSource()
        {
            IDictionary<string, string> urlparams = HtmlPage.Document.QueryString;
            PostRequest<UniTransfer> post = new PostRequest<UniTransfer>(this.Dispatcher, urlparams["data"].Substring(0, urlparams["data"].IndexOf('?')));
            post.ProcessResponse += new PostRequest<UniTransfer>.ProcessResponseEvent(delegate(UniTransfer result)
                {
                    ShowGlobalMask("Распаковка данных...");

                    BindColumns(result.columns);
                    
                    IList<IDictionary> source = new List<IDictionary>();
                    foreach (var row in result.rows)
                    {
                        var cells = new Dictionary<string, object>();

                        foreach (var column in result.columns)
                        {
                            if (column.uniType == UniColumn.UniType.Decimal)
                            {
                                cells[column.dataBind] = Convert.ToDecimal(row[result.columns.IndexOf(column)], CultureInfo.InvariantCulture);
                            }
                            else 
                            {
                                cells[column.dataBind] = row[result.columns.IndexOf(column)];
                            }
                        }

                        (source as List<IDictionary>).Add(cells);
                    }

                    if (source.Count() > 0)
                    {
                        grid.FilteredItemsSource = source.ToArray().ToDataSource();
                    }

                    HideGlobalMask();
                });
            post.ProcessError += new PostRequest<UniTransfer>.ProcessErrorEvent(delegate()
                {                    
                });

            string dataUrl = urlparams["data"];            
            string dataParams = dataUrl.Substring(dataUrl.IndexOf('?')+1);
            string innerParams = "";

            foreach (var param in dataParams.Split('&').Where(i => !String.IsNullOrEmpty(i)))
            {
                innerParams += string.Format("'{0}': '{1}',", param.Split('=').First(), param.Split('=').Last());
            }

            post.Perform(string.Format("{{ {0} }}", innerParams.Substring(0, innerParams.Length - 1)));
            ShowGlobalMask("Загрузка данных...");
        }

        private void CloseAction(object sender, RoutedEventArgs e)
        {
            HtmlPage.Window.Invoke("parent_tb_remove_ext");
        }
    }
}
