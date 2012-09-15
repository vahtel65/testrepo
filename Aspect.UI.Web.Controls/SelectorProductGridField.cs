using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Controls
{
    //public delegate void SelectAllEventHandler(object sender, EventArgs e);
    public class SelectorProductGridField : ProductGridField
    {
        public SelectorProductGridField()
            : base()
        {
        }
        public SelectorProductGridField(string colname, string dataname, bool showCheckBox)
            : base(colname, dataname)
        {
            this.ShowCheckBox = showCheckBox;
        }
        public SelectorProductGridField(string colname, string dataname, bool showCheckBox, string selectorContainerID)
            : base(colname, dataname)
        {
            this.ShowCheckBox = showCheckBox;
            this.SelectorContainerID = selectorContainerID;
        }
        protected bool ShowCheckBox
        {
            get
            {
                if (ViewState["ShowCheckBox"] == null) return false;
                return Convert.ToBoolean(ViewState["ShowCheckBox"]);
            }
            set
            {
                ViewState["ShowCheckBox"] = value;
            }
        }
        protected string SelectorContainerID
        {
            get
            {
                if (ViewState["SelectorContainerID"] == null) return string.Empty;
                return ViewState["SelectorContainerID"].ToString();
            }
            set
            {
                ViewState["SelectorContainerID"] = value;
            }
        }
        /*public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            if (cellType == DataControlCellType.Header)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.ID = "select_all";
                checkBox.Attributes.Add("onclick", "SelectAll(this)");
                //checkBox.AutoPostBack = true;
                //checkBox.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                cell.Controls.Add(checkBox);
            }
            else if (cellType == DataControlCellType.DataCell)
            {
                HiddenField idField = new HiddenField();
                idField.DataBinding += new EventHandler(idField_DataBinding);
                cell.Controls.Add(idField);
                //base.InitializeDataCell(cell, rowState);
                CheckBox checkBox = new CheckBox();
                checkBox.ID = "SelectCheckBox";
                checkBox.Visible = ShowCheckBox;
                checkBox.DataBinding += new EventHandler(this.checkBox_DataBinding);
                cell.Controls.Add(checkBox);
            }
        }

        protected void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }*/

        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            base.InitializeCell(cell, cellType, rowState, rowIndex);
            if (cellType == DataControlCellType.Header && !this.SelectorContainerID.Equals(string.Empty))
            {
                //field.HeaderText = String.Format("<input type=\"checkbox\" onclick=\"clearSelection('{0}');\" name=\"clearselectionsname\" id=\"clearselectionsid\">", SelectedProductsHidden.ClientID);
                CheckBox checkBox = new CheckBox();
                checkBox.ID = "ClearSelection";
                checkBox.Attributes.Add("name", "clear");
                checkBox.Attributes.Add("onclick", String.Format("clearSelection('{0}');", SelectorContainerID));
                cell.Controls.Add(checkBox);
            }
            
        }

        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            HiddenField idField = new HiddenField();
            idField.DataBinding += new EventHandler(idField_DataBinding);
            cell.Controls.Add(idField);
            //base.InitializeDataCell(cell, rowState);
            CheckBox checkBox = new CheckBox();
            checkBox.ID = "SelectCheckBox";
            checkBox.Visible = ShowCheckBox;
            checkBox.Attributes.Add("name", "selector");
            checkBox.DataBinding += new EventHandler(this.checkBox_DataBinding);
            cell.Controls.Add(checkBox);
        }

        void idField_DataBinding(object sender, EventArgs e)
        {
            HiddenField t = (HiddenField)sender;
            GridViewRow row = (GridViewRow)t.NamingContainer;
            System.Data.DataRowView data = row.DataItem as System.Data.DataRowView;
            if (data[this.DataField] != null) t.Value = data[this.DataField].ToString();
        }

        private void checkBox_DataBinding(Object sender, EventArgs e)
        {
            CheckBox t = (CheckBox)sender;
            GridViewRow row = (GridViewRow)t.NamingContainer;
            System.Data.DataRowView data = row.DataItem as System.Data.DataRowView;
            
            try
            {
                if (data[this.DataField] == null || data[this.DataField] == DBNull.Value || Guid.Empty.Equals(new Guid(data[this.DataField].ToString()))) t.Enabled = false;
            }
            catch (Exception)
            {
                t.Enabled = false;
            }
            /*GridViewRow row = (GridViewRow)t.NamingContainer;
            System.Data.DataRowView data = row.DataItem as System.Data.DataRowView;
            if(data[this.DataField] != null) t.Checked = Convert.ToBoolean(data[this.DataField]);*/
            //t.Text = data[this.DataField].ToString();
        }
    }
}
