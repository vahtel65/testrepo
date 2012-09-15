using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Controls
{
    public class CheckBoxProductGridField : EditableProductGridField
    {
        public CheckBoxProductGridField()
            : base()
        {
        }
        public CheckBoxProductGridField(Aspect.Domain.EditableGridColumn gridColumn)
            : base(gridColumn)
        {
        }


        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            //base.InitializeDataCell(cell, rowState);
            CheckBox checkBox = new CheckBox();
            checkBox.ID = this.ControlID;
            base.InitializeDataCell(cell, rowState);
            checkBox.EnableViewState = true;
            checkBox.DataBinding += new EventHandler(this.checkBox_DataBinding);
            cell.Controls.Add(checkBox);
        }

        private void checkBox_DataBinding(Object sender, EventArgs e)
        {
            CheckBox t = (CheckBox)sender;
            GridViewRow row = (GridViewRow)t.NamingContainer;
            System.Data.DataRowView data = row.DataItem as System.Data.DataRowView;
            if (data[this.DataField] != null) t.Checked = Convert.ToBoolean(data[this.DataField]);
            //t.Text = data[this.DataField].ToString();
        }

        internal override object GetValue(Control control)
        {
            CheckBox chk = control as CheckBox;
            if (chk != null) return chk.Checked;
            else return null;
        }
    }
}
