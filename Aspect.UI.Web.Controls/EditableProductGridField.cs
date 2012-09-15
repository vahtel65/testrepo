using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Controls
{
    public abstract class EditableProductGridField : ProductGridField
    {
        public EditableProductGridField() : base()
        {
        }
        public EditableProductGridField(Aspect.Domain.EditableGridColumn gridColumn)
            : base(gridColumn.Name, gridColumn.DataItem)
        {
            this.GridColumn = gridColumn;
        }
        protected string ControlID
        {
            get
            {
                return string.Format("{0}EditableControlID", this.DataField);
            }
        }
        protected string EditableAttribute
        {
            get
            {
                return "colname";
            }
        }

        protected Aspect.Domain.EditableGridColumn GridColumn
        {
            get
            {
                return ViewState["GridColumn"] as Aspect.Domain.EditableGridColumn;
            }
            set
            {
                ViewState["GridColumn"] = value;
            }
        }


        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            cell.Attributes.Add(this.EditableAttribute, this.DataField);
        }
        public object GetValue(GridViewRow row)
        {
            foreach (TableCell cell in row.Cells)
            {
                if (cell.Attributes["colname"] != null && cell.Attributes["colname"] == this.DataField)
                {
                    return this.GetValue(cell.FindControl(this.ControlID));
                }
            }
            return null;
        }
        internal new abstract object GetValue(System.Web.UI.Control control);
    }
}
