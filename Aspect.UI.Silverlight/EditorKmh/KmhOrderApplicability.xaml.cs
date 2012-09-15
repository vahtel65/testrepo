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
using System.Collections.ObjectModel;
using System.Windows.Data;
using Aspect;
using System.Globalization;

namespace EditorKmh
{
    public partial class KmhOrderApplicability : ChildWindow
    {
        public KmhOrderApplicability()
        {
            InitializeComponent();
            this.Loaded += LoadOrders;
        }

        public Guid _dictNomenID;
        public Guid orderID;
        public DateTime timeStamp;
        public TechnDatesSpeciality speciality = TechnDatesSpeciality.Main;
        
        ObservableCollection<transfer_order_article> orderList;        

        public class GetOrdersWithKmh_PROTO
        {
            public Guid _productNomenId;
        }

        private void LoadOrders(object sender, RoutedEventArgs e)
        {
            PostRequest<List<transfer_order_article>> post = new PostRequest<List<transfer_order_article>>(this.Dispatcher, "/Technology/TechnDates.aspx/GetOrdersWithKmh");
            post.ProcessResponse += new PostRequest<List<transfer_order_article>>.ProcessResponseEvent(delegate(List<transfer_order_article> list)
                {
                    // select current order
                    if (orderID != Guid.Empty)
                    {
                        var cur_order = list.SingleOrDefault(or => or.order_id == orderID);
                        if (cur_order != null)
                        {
                            cur_order.ischecked = true;
                        }
                    }                    

                    orderList = new ObservableCollection<transfer_order_article>(list);
                    grid.FilteredItemsSource = orderList;

                    
                    //MainGridBusy.IsBusy = false;            
                });

            post.Perform(new GetOrdersWithKmh_PROTO()
            {
                _productNomenId = this._dictNomenID                
            });
        }

        public class ExchangeKmhInOrders_PROTO
        {
            public List<Guid> orderArticles { set; get; }
            public DateTime timeStamp { set; get; }
            public Guid prodNomenId { set; get; }
            public TechnDatesSpeciality speciality { set; get; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/TechnDates.aspx/ExchangeKmhInOrders");
            post.ProcessResponse += new PostRequest<PostResult>.ProcessResponseEvent(delegate (PostResult result)
                {
                });
            post.ProcessError += new PostRequest<PostResult>.ProcessErrorEvent(delegate()
                {
                    MessageBox.Show("Возникла не предвиденная ошибка");
                });
            post.Perform(new ExchangeKmhInOrders_PROTO()
            {
                orderArticles = this.orderList.ToList().Where(it => it.ischecked).Select(it => it.order_id).ToList(),
                prodNomenId = this._dictNomenID,
                timeStamp = this.timeStamp,
                speciality = this.speciality
            });

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }

    public class BoolToVisibilityConverter : BoolToValueConverter<bool> { }

    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }

    public class ImportanceColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush s_lowBrush = new SolidColorBrush(SystemColors.WindowColor);
        private static readonly SolidColorBrush s_mediumBrush = new SolidColorBrush(Color.FromArgb(255, 249, 235, 82));        

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((bool)value)
            {
                case true:
                    return s_mediumBrush;
                case false:
                    return s_lowBrush;                
            }

            throw new NotSupportedException("Can't create importance colour.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}

