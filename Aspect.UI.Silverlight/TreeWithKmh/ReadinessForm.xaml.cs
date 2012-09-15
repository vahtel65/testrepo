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

namespace TreeWithKmh
{
    public partial class ReadinessForm : ChildWindow
    {
        public string product_pn1;
        public DateTime current_date;
        public Guid order_id;
        public Guid product_id;
        public transfer_techn_dates techn_dates;

        public ReadinessForm()
        {
            InitializeComponent();
        }

        public class SaveDates_PROTO
        {
            public Guid order_id { set; get; }
            public Guid product_id { set; get; }
            public transfer_techn_dates dates { set; get; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            // save readiness
            LayoutRoot.Content = "Сохранение готовностей...";
            LayoutRoot.IsBusy = true;

            if (chkHimDate.IsChecked.Value && !this.techn_dates.him_date.HasValue) this.techn_dates.him_date = this.current_date;
            if (chkSvarDate.IsChecked.Value && !this.techn_dates.svar_date.HasValue) this.techn_dates.svar_date = this.current_date;
            if (chkTechnDate.IsChecked.Value && !this.techn_dates.techn_date.HasValue) this.techn_dates.techn_date = this.current_date;

            if (!chkHimDate.IsChecked.Value && this.techn_dates.him_date.HasValue) this.techn_dates.him_date = null;
            if (!chkSvarDate.IsChecked.Value && this.techn_dates.svar_date.HasValue) this.techn_dates.svar_date = null;
            if (!chkTechnDate.IsChecked.Value && this.techn_dates.techn_date.HasValue) this.techn_dates.techn_date = null;

            PostRequest<PostResult> post = new PostRequest<PostResult>(this.Dispatcher, "/Technology/TechnDates.aspx/SaveForProduct");
            post.ProcessResponse += new PostRequest<PostResult>.ProcessResponseEvent(delegate(PostResult result)
            {
                LayoutRoot.IsBusy = false;
            });
            post.Perform(new SaveDates_PROTO()
            {
                order_id = this.order_id,
                product_id = this.product_id,
                dates = this.techn_dates
            });

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public class GetDates_PROTO
        {
            public Guid order_id { set; get; }
            public Guid product_id { set; get; }
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {            
            // load readiness
            LayoutRoot.IsBusy = true;
            ProductPn1.Text = product_pn1;

            PostRequest<transfer_techn_dates> post = new PostRequest<transfer_techn_dates>(this.Dispatcher, "/Technology/TechnDates.aspx/GetForProduct");
            post.ProcessResponse += new PostRequest<transfer_techn_dates>.ProcessResponseEvent(delegate(transfer_techn_dates dates)
            {
                this.techn_dates = dates;

                this.chkHimDate.IsChecked = dates.him_date.HasValue;
                this.chkSvarDate.IsChecked = dates.svar_date.HasValue;
                this.chkTechnDate.IsChecked = dates.techn_date.HasValue;

                LayoutRoot.IsBusy = false;
            });
            post.Perform(new GetDates_PROTO()
            {
                order_id = this.order_id,
                product_id = this.product_id,
            });
        }
    }
}

