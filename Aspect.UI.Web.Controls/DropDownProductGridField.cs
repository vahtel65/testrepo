using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Controls
{
    public class DropDownProductGridField : EditableProductGridField
    {
        public DropDownProductGridField() : base()
        {
        }
        public DropDownProductGridField(Aspect.Domain.EditableGridColumn gridColumn/*, object dataSource*/)
            : base(gridColumn)
        {
        }

        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            DropDownList dropDown = new DropDownList();
            dropDown.DataValueField = GridColumn.DataSource.ValueField;
            dropDown.DataTextField = GridColumn.DataSource.TextField;
            dropDown.DataSource = GridColumn.DataSource.DataSource;
            dropDown.ID = this.ControlID;
            base.InitializeDataCell(cell, rowState);
            dropDown.DataBound += new EventHandler(dropDown_DataBound);
            cell.Controls.Add(dropDown);
        }

        private void dropDown_DataBound(object sender, EventArgs e)
        {
            DropDownList t = (DropDownList)sender;
            GridViewRow row = (GridViewRow)t.NamingContainer;
            System.Data.DataRowView data = row.DataItem as System.Data.DataRowView;
            t.SelectedValue = data[this.DataField].ToString();
        }

        internal override object GetValue(Control control)
        {
            DropDownList ddl = control as DropDownList;
            if (ddl != null && !string.IsNullOrEmpty(ddl.SelectedValue) )
            {
                return new Guid(ddl.SelectedValue);
            }
            return null;
        }
    }
}
