using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Controls
{
    public class TreeActionProductGridField : ProductGridField
    {
        public TreeActionProductGridField() : base()
        {
        }
        public TreeActionProductGridField(string colname, string dataname)
            : base(colname, dataname)
        {
        }

        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            //base.InitializeDataCell(cell, rowState);
            Panel panel = new Panel();
            panel.DataBinding += new EventHandler(panel_DataBinding);
            HyperLink link = new HyperLink();
            link.DataBinding += new EventHandler(this.link_DataBinding);
            link.CssClass = "thickbox";
            panel.Controls.Add(link);
            cell.Controls.Add(panel);
            //cell.Controls.Add(link);
        }

        private void panel_DataBinding(object sender, EventArgs e)
        {
            Panel l = (Panel)sender;
            GridViewRow row = (GridViewRow)l.NamingContainer;
            System.Data.DataRowView data = row.DataItem as System.Data.DataRowView;

            int padding = Convert.ToInt32(data["Level"]) * 15;
            l.Style[HtmlTextWriterStyle.PaddingLeft] = String.Format("{0}px", padding);
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
            l.NavigateUrl = String.Format("~/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}", data[Aspect.Model.Common.IDColumnTitle].ToString(), Guid.Empty);
        }

        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            base.InitializeCell(cell, cellType, rowState, rowIndex);
        }
       
    }
}
