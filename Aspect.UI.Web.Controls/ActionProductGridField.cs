using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Controls
{
    public class ActionProductGridField : ProductGridField
    {
        public ActionProductGridField() : base()
        {
        }
        public ActionProductGridField(string colname, string dataname)
            : base(colname, dataname)
        {
        }
        public ActionProductGridField(string colname, string dataname, Guid classificationID)
            : this(colname, dataname)
        {
            this.ClassificationID = classificationID;
        }

        protected Guid ClassificationID
        {
            get
            {
                if (ViewState["ClassificationID"] == null) return Guid.Empty;
                return new Guid(ViewState["ClassificationID"].ToString());
            }
            set
            {
                ViewState["ClassificationID"] = value;
            }
        }

        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            //base.InitializeDataCell(cell, rowState);
            HyperLink link = new HyperLink();
            link.DataBinding += new EventHandler(this.link_DataBinding);
            link.CssClass = "thickbox";
            cell.Controls.Add(link);
        }

        private void link_DataBinding(Object sender, EventArgs e)
        {
            HyperLink l = (HyperLink)sender;
            GridViewRow row = (GridViewRow)l.NamingContainer;
            System.Data.DataRowView data = row.DataItem as System.Data.DataRowView;
            l.Text = data[this.DataField].ToString();
            if (data.DataView.Table.Columns.Contains("GroupToChange") && data["GroupToChange"] != DBNull.Value && Convert.ToBoolean(data["GroupToChange"]))
            {
                l.Font.Bold = true;
            }
            l.NavigateUrl = String.Format("~/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}", data[Aspect.Model.Common.IDColumnTitle].ToString(), this.ClassificationID);
        }

        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            base.InitializeCell(cell, cellType, rowState, rowIndex);
        }
       
    }
}
