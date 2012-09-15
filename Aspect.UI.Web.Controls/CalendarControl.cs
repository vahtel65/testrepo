using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Controls
{
    public class CalendarControl : CompositeControl
    {
        public AjaxControlToolkit.CalendarExtender CalendarValueControlAdv;
        public TextBox CalendarValueControlAdvTextBox;

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            CalendarValueControlAdv = new AjaxControlToolkit.CalendarExtender();
            CalendarValueControlAdvTextBox = new TextBox();
            CalendarValueControlAdv.Format = "dd.MM.yyyy";
            //CalendarValueControlAdv.Format = System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
            
            CalendarValueControlAdvTextBox.ID = "CalendarValueControlAdvTextBox";
            CalendarValueControlAdv.TargetControlID = CalendarValueControlAdvTextBox.ID;
            this.Controls.Add(CalendarValueControlAdvTextBox);
            this.Controls.Add(CalendarValueControlAdv);
        }
        public string TextBoxID
        {
            get
            {
                return CalendarValueControlAdvTextBox.ID;
            }
        }
        public void Clear()
        {
            CalendarValueControlAdv.SelectedDate = null;
            CalendarValueControlAdvTextBox.Text = string.Empty;
        }
        public DateTime SelectedDate
        {
            get
            {
                this.EnsureChildControls();
                if (string.IsNullOrEmpty(CalendarValueControlAdvTextBox.Text)) return DateTime.MinValue;
                return DateTime.ParseExact(CalendarValueControlAdvTextBox.Text, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //return Convert.ToDateTime(CalendarValueControlAdvTextBox.Text, System.Globalization.CultureInfo.CurrentUICulture);
                //return CalendarValueControlAdv.SelectedDate.HasValue ? CalendarValueControlAdv.SelectedDate.Value : DateTime.Now;
            }
            set
            {
                this.EnsureChildControls();
                if (value != DateTime.MinValue)
                {
                    CalendarValueControlAdv.SelectedDate = value;
                }
            }
        }
    }
}
