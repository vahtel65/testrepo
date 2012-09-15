using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Controls
{
    public class BooleanGridField : CheckBoxField
    {
        public BooleanGridField()
            : base()
        {
        }

        /*protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            //base.InitializeDataCell(cell, rowState);
            CheckBox checkBox = new CheckBox();
            base.InitializeDataCell(cell, rowState);
            checkBox.EnableViewState = true;
            checkBox.DataBinding += new EventHandler(this.checkBox_DataBinding);
            checkBox.Enabled = false;
            cell.Controls.Add(checkBox);
        }*/
        protected override void OnDataBindField(object sender, EventArgs e)
        {
            CheckBox t = (CheckBox)sender;
            GridViewRow row = (GridViewRow)t.NamingContainer;
            System.Data.DataRowView data = row.DataItem as System.Data.DataRowView;
            if (data[this.DataField] != null) t.Checked = Convert.ToBoolean(Convert.ToInt32(data[this.DataField]));
            //base.OnDataBindField(sender, e);
        }

        /*private void checkBox_DataBinding(Object sender, EventArgs e)
        {
            CheckBox t = (CheckBox)sender;
            GridViewRow row = (GridViewRow)t.NamingContainer;
            System.Data.DataRowView data = row.DataItem as System.Data.DataRowView;
            if (data[this.DataField] != null) t.Checked = Convert.ToBoolean(data[this.DataField]);
        }*/
    }
}
