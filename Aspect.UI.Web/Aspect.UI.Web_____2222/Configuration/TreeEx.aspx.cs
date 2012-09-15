using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Aspect.Model.ConfigurationDomain;
using Aspect.Domain;
using Aspect.Model;
using Aspect.UI.Web.Controls;

namespace Aspect.UI.Web.Configuration
{
    public partial class TreeEx : Basic.ContentPageBase
    {
        protected override Guid RequestClassificationTreeID
        {
            get
            {
                return Aspect.Domain.FormGridView.ConfigurationTree;
            }
        }
        protected Guid ProductID
        {
            get
            {
                try
                {
                    return new Guid(Request["ID"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }

        protected System.Web.UI.HtmlControls.HtmlAnchor ChooseColumns;
        protected System.Web.UI.HtmlControls.HtmlAnchor ChooseColumnsOrder;
        protected System.Web.UI.HtmlControls.HtmlAnchor ChooseCardFields;
        protected System.Web.UI.HtmlControls.HtmlAnchor ShowBuffer;

        protected Ext.Net.Hidden selectedProducts;
        protected Ext.Net.TreeGrid ProductTree;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.PopupIframeInitializationString(ShowBuffer, "Буфер", "/Popup/Buffer.aspx", 700, 500);
                this.BindGridColumns();
                this.BindGrid();
                this.PopupIframeInitializationStringWithProduct(ChooseColumns, "Колонки", string.Format("../Popup/UserColumns.aspx?{0}={1}&productid&{2}={3}&url={4}", RequestKeyClassificationTreeID, this.RequestClassificationTreeID, RequestKeyFieldPlaceHolder, FieldPlaceHolderEnum.Grid, Server.UrlEncode(this.Request.Url.ToString())), 700, 500);
                this.PopupIframeInitializationStringWithProduct(ChooseColumnsOrder, "Колонки", string.Format("../Popup/UserColumnsOrder.aspx?{0}={1}&productid&{2}={3}&url={4}", RequestKeyClassificationTreeID, this.RequestClassificationTreeID, RequestKeyFieldPlaceHolder, FieldPlaceHolderEnum.Grid, Server.UrlEncode(this.Request.Url.ToString())), 700, 500);
                this.PopupIframeInitializationStringWithProduct(ChooseCardFields, "Колонки", string.Format("../Popup/UserColumns.aspx?{0}={1}&productid&{2}={3}&url={4}", RequestKeyClassificationTreeID, this.RequestClassificationTreeID, RequestKeyFieldPlaceHolder, FieldPlaceHolderEnum.GridCard, Server.UrlEncode(this.Request.Url.ToString())), 700, 500);
            }

        }
        protected ITextControl HeaderLiteral;
        protected ITextControl HeaderDateLiteral;

        protected Ext.Net.ResourceManager ResourceManager1;

        private void BindGrid()
        {
            DataSet data = null;
            ConfigurationTreeProvider provider = null;
            List<GridColumn> columns = new List<GridColumn>();
            try
            {
                provider = new ConfigurationTreeProvider();
                if (provider != null)
                {
                    Aspect.Domain.Product prod = provider.GetProduct(this.ProductID);
                    if (prod == null) return;
                    this.Title = HeaderLiteral.Text = string.Format(HeaderLiteral.Text, prod.PublicName, prod.Version == null ? string.Empty : prod.Version.ToString());
                    HeaderDateLiteral.Text = string.Format(HeaderDateLiteral.Text, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
                    //Response.Write(provider.getQuery(new Guid(node.Value), this.User.ID, this.OrderExpression, this.SearchConditions));
                    //return;
                    data = provider.GetList(this.RequestClassificationTreeID, this.ProductID, this.User.ID, this.SearchConditions);
                    if (ShowSelected && !String.IsNullOrEmpty(SelectedProductsHidden.Value))
                    {
                        string[] ids = SelectedProductsHidden.Value.Split(',').Select(s => string.Format("CONVERT('{0}','System.Guid')", s)).ToArray();
                        data.Tables[0].DefaultView.RowFilter = string.Format("ID in ({0})", string.Join(",", ids));
                    }
                    columns = provider.GetGridColumns(this.User.ID, this.RequestClassificationTreeID, FieldPlaceHolder.Grid);
                }

            }
            finally
            {
                if (provider != null) provider.Dispose();
            }
            if (data != null && data.Tables.Count > 0)
            {
                //ProductGrid.DataSource = data.Tables[0].DefaultView;
                //ProductGrid.DataBind();

                Stack<Ext.Net.TreeNodeCollection> stackRoots = new Stack<Ext.Net.TreeNodeCollection>();
                Ext.Net.TreeNodeCollection currentRoot = (ProductTree.Root.First() as Ext.Net.TreeNode).Nodes;
                int? prevLevel = null, curLevel = null;

                foreach (DataRow row in data.Tables[0].Rows)
                {
                    curLevel = Convert.ToInt32(row["Level"]);
                    if (!prevLevel.HasValue) prevLevel = curLevel;
                    
                    Ext.Net.TreeNode node = new Ext.Net.TreeNode();
                    node.Checked = Ext.Net.ThreeStateBool.False;
                    foreach(GridColumn column in columns)
                    {
                        node.CustomAttributes.Add(new Ext.Net.ConfigItem(column.DataItem.Replace('.', '_'), row[column.DataItem].ToString(), Ext.Net.ParameterMode.Value));
                    }
                    node.CustomAttributes.Add(new Ext.Net.ConfigItem("ProductID", row["ID"].ToString(), Ext.Net.ParameterMode.Value));
                    if (curLevel > prevLevel)
                    {
                        stackRoots.Push(currentRoot);                        
                        currentRoot = (currentRoot.Last() as Ext.Net.TreeNode).Nodes;
                    }
                    if (curLevel < prevLevel)
                    {
                        currentRoot = stackRoots.Pop();
                    }
                    currentRoot.Add(node);
                    prevLevel = curLevel;
                }
                
                if (data.Tables[0].Rows.Count > 0)
                {
                    string function = String.Format("Ext.namespace('App'); App.cid = '{0}'; ", this.RequestClassificationTreeID);                    
                    ResourceManager1.AddAfterClientInitScript(function);
                }

                this.AppendGridHeader(columns);
            }
        }

        private void AppendGridHeader(List<GridColumn> columns)
        {
           /* GridViewRow row = new GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);
            row.CssClass = "table-header";
            //begin create cells
            TableCell thf = new TableHeaderCell();
            thf.HorizontalAlign = HorizontalAlign.Center;
            thf.ColumnSpan = 1;
            thf.Text = String.Empty;
            row.Cells.Add(thf);
            if (columns.Count > 0)
            {
                int counter = 0;
                string group = columns[0].Group;
                foreach (GridColumn item in columns)
                {
                    if (group.Equals(item.Group))
                    {
                        counter++;
                    }
                    else
                    {
                        //--
                        TableCell th = new TableHeaderCell();
                        th.HorizontalAlign = HorizontalAlign.Center;
                        th.ColumnSpan = counter;
                        th.Text = group;
                        row.Cells.Add(th);
                        //--
                        group = item.Group;
                        counter = 1;
                    }
                }
                TableCell thl = new TableHeaderCell();
                thl.HorizontalAlign = HorizontalAlign.Center;
                thl.ColumnSpan = counter;
                thl.Text = group;
                row.Cells.Add(thl);
            }
            //------
            if (ProductGrid.Controls.Count > 0)
            {
                Table tbl = ProductGrid.Controls[0] as Table;
                if (tbl != null)
                {
                    tbl.Rows.AddAt(0, row);
                    //(ProductGrid.Controls[0] as Table).Rows.AddAt(0, row);
                }
            }*/
        }

        private void BindGridColumns()
        {

            //ProductGrid.Columns.Clear();
            /*for (int i = 1; i < ProductGrid.Columns.Count; i++)
            {
                ProductGrid.Columns.RemoveAt(i);
            }*/
            //SelectorProductGridField selector = new SelectorProductGridField(string.Empty, Common.IDColumnTitle);
            //ProductGrid.Columns.Add(selector);
            //SelectorProductGridField selector = new SelectorProductGridField(string.Empty, "ID", true, SelectedProductsHidden.ClientID);
            //selector.ItemStyle.Width = new Unit(15, UnitType.Pixel);
            //ProductGrid.Columns.Add(selector);

            List<GridColumn> columns = new List<GridColumn>();
            ContentDomain provider = null;
            try
            {
                provider = Common.GetContentDomain(ClassifiacationTypeView);
                if (provider != null) columns = provider.GetGridColumns(this.User.ID, RequestClassificationTreeID, FieldPlaceHolder.Grid);
            }
            finally
            {
                if (provider != null) provider.Dispose();
            }

            foreach (GridColumn item in columns)
            {
                Ext.Net.TreeGridColumn column = new Ext.Net.TreeGridColumn();
                column.Header = item.Name;
                column.DataIndex = item.DataItem.Replace('.','_');
                column.Width = 100;
                ProductTree.Columns.Add(column);
            }
        }

        private Aspect.Domain.OrderExpression orderExpression = null;
        protected Aspect.Domain.OrderExpression OrderExpression
        {
            get
            {
                if (ViewState["OrderExpression"] == null) ViewState["OrderExpression"] = new Aspect.Domain.OrderExpression();
                if (orderExpression == null) orderExpression = ViewState["OrderExpression"] as Aspect.Domain.OrderExpression;
                return orderExpression;
            }
            set
            {
                ViewState["OrderExpression"] = value;
                orderExpression = value;
            }
        }

        protected void ProductGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (OrderExpression.Expression == e.SortExpression)
            {
                if (OrderExpression.SortDirection == Aspect.Domain.SortDirection.asc) orderExpression.SortDirection = Aspect.Domain.SortDirection.desc;
                else OrderExpression.SortDirection = Aspect.Domain.SortDirection.asc;
            }
            OrderExpression.Expression = e.SortExpression;
            BindGrid();
        }
        protected HiddenField SelectedProductsHidden;
        protected void ProductGrid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            /*if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                {
                    e.Row.ID = string.Format("{0}_Row{1}", ProductGrid.ClientID, e.Row.RowIndex);
                    string pid = DataBinder.Eval(e.Row.DataItem, Common.IDColumnTitle).ToString();
                    string cid = this.RequestClassificationTreeID.ToString();
                    string function = string.Format("onGridViewRowSelectedCallback('{0}','{1}', this, '{2}');", pid, cid, ProductGrid.Controls[0].ClientID);
                    e.Row.Attributes.Add("onclick", function);
                    if (e.Row.RowState == DataControlRowState.Alternate) e.Row.CssClass = "row2";
                    else e.Row.CssClass = string.Empty;

                    e.Row.Attributes["onmouseover"] = "highLightRow(this)";
                    e.Row.Attributes["onmouseout"] = "unHighLightRow(this)";
                    CheckBox chk = e.Row.Cells[0].FindControl("SelectCheckBox") as CheckBox;
                    chk.Attributes.Add("onclick", String.Format("selectProduct(event,this,'{0}','{1}');", pid, SelectedProductsHidden.ClientID));
                    chk.Checked = SelectedProductsHidden.Value.Contains(pid);
                }
            }*/
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            this.BindGridColumns();
            this.BindGrid();
        }

        protected bool ShowSelected
        {
            get
            {
                if (ViewState["ShowSelected"] == null)
                {
                    ViewState["ShowSelected"] = false;
                }
                return Convert.ToBoolean(ViewState["ShowSelected"]);
            }
            set
            {
                ViewState["ShowSelected"] = value;
            }
        }

        protected void ShowSelectedProducts_Click(object sender, EventArgs e)
        {
            ShowSelected = !ShowSelected;
            if (String.IsNullOrEmpty(SelectedProductsHidden.Value))
            {
                ShowSelected = false;
            }
            this.BindGrid();
        }
    }
}
