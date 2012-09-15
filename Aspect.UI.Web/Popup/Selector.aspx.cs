using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Aspect.Domain;
using Aspect.UI.Web.Basic;
using Aspect.Model.DictionaryDomain;
using Aspect.UI.Web.Controls;

namespace Aspect.UI.Web.Popup
{
    public partial class Selector : ContentPageBase
    {
        public string HiddenControlID
        {
            get
            {
                return Request["ctrlID"];
            }
        }
        public string TextControlID
        {
            get
            {
                return Request["textCtrlID"];
            }
        }
        public bool MultiMode
        {
            get
            {
                return (Request["mode"] == "multi");
            }
        }

        public Guid DictionaryTreeID
        {
            get
            {
                return new Guid(Request["treeID"]);
            }
        }
        public Guid SelectedValueID
        {
            get
            {
                return new Guid(Request["ID"]);
            }
        }

        protected string ValueField { get; set; }

        protected string TextField { get; set; }

        protected HiddenField SearchExp;
        protected HiddenField MultiSelection;
        protected HyperLink MultiInsert;
        protected GridView SelectorGrid;
        protected Pager GridPager;
        protected System.Web.UI.HtmlControls.HtmlAnchor ProductSearch;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindGridColumns();
                GridPager_CurrentPageChanged(sender, new PagerEventArgs(0, SelectorGrid.PageSize, 0));
                //this.PopupIframeInitializationString(ProductSearch, "Поиск", string.Format("ProductSearch.aspx{0}", this.Request.Url.Query), 700, 500);

                if (!MultiMode)
                {
                    MultiInsert.Visible = false;
                }
                else
                {
                    MultiInsert.Attributes.Add("onclick", "self.parent.setSelectedMultiValue($('#ctl00_ContentPlaceHolder1_MultiSelection').val());self.parent.tb_remove();return false;");
                }
            }
            this.PopupIframeInitializationString(ProductSearch, "Поиск", string.Format("ProductSearch2.aspx?{0}={1}&srh={2}", RequestKeyClassificationTreeID, this.DictionaryTreeID, SearchExp.Value), 700, 500);
        }

        private DictionaryTree tree = null;
        protected void SelectorGrid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                {
                    if (e.Row.RowState == DataControlRowState.Alternate) e.Row.CssClass += " row2";

                    //HyperLink link = e.Row.Cells[0].FindControl("СhooseButton") as HyperLink;
                    HyperLink link = e.Row.Cells[0].Controls[0] as HyperLink;
                    link.NavigateUrl = "javascript:void(0);";
                    using (DictionaryProvider provider = new DictionaryProvider())
                    {
                        if (tree == null) tree = provider.GetDictionaryTreeNode(this.DictionaryTreeID);
                        Guid id = new Guid(DataBinder.Eval(e.Row.DataItem, /*this.ValueField*/"ID").ToString());
                        string text = provider.GetProductDictionaryText(id, tree).ToString();
                        string function;
                        if (MultiMode)
                        {
                            function = String.Format("fnSelectRow('{0}','{1}');return false;", id, text);
                            e.Row.CssClass += " " + id.ToString();

                            var pair = MultiSelection.Value.Split(';').Where(i => !String.IsNullOrEmpty(i));
;
                            var idsList = pair.Select(i => new Guid (i.Split(':').First()));

                            if (idsList.Contains(id)) e.Row.CssClass += " RowSelected";
                        }
                        else 
                        {
                            function = String.Format("self.parent.setSelectedValue('{0}','{1}','{2}','{3}');self.parent.tb_remove();return false;", this.HiddenControlID, id, this.TextControlID, text);
                        }
                        
                        //string function = String.Format("self.parent.setSelectedValue('{0}','{1}','{2}','{3}');self.parent.tb_remove();return false;", this.HiddenControlID, DataBinder.Eval(e.Row.DataItem, /*this.ValueField*/"ID"), this.TextControlID, DataBinder.Eval(e.Row.DataItem, this.TextField));
                        link.Attributes.Add("onclick", function);
                    }                    

                    e.Row.Attributes["onmouseover"] = "highLightRow(this)";
                    e.Row.Attributes["onmouseout"] = "unHighLightRow(this)";
                }
            }
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            this.OrderExpression = new OrderExpression();
            this.BindGridColumns();
            GridPager_CurrentPageChanged(sender, new PagerEventArgs(0, SelectorGrid.PageSize, 0));
        }
        protected override List<SearchExpression> SearchConditions
        {
            get
            {
                List<Aspect.Domain.SearchExpression> searchConditions = new List<Aspect.Domain.SearchExpression>();
                if (!String.IsNullOrEmpty(SearchExp.Value))
                {
                    string[] conds = SearchExp.Value.Split(',');
                    foreach (string item in conds)
                    {
                        string[] st = item.Split('=');
                        searchConditions.Add(new SearchExpression()
                        {
                            FieldID = new Guid(st[0]),
                            FieldValue = Server.HtmlDecode(st[1])
                        });
                    }
                }
                return searchConditions;
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

        protected void SelectorGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (OrderExpression.Expression == e.SortExpression)
            {
                if (OrderExpression.SortDirection == Aspect.Domain.SortDirection.asc) orderExpression.SortDirection = Aspect.Domain.SortDirection.desc;
                else OrderExpression.SortDirection = Aspect.Domain.SortDirection.asc;
            }
            OrderExpression.Expression = e.SortExpression;
            GridPager.CurrentPage = 0;
            GridPager_CurrentPageChanged(sender, new PagerEventArgs(0, GridPager.PageSize, 0));
        }

        protected void GridPager_CurrentPageChanged(object sender, PagerEventArgs e)
        {
            using (DictionaryProvider provider = new DictionaryProvider())
            {
                DictionaryTree entity = provider.DictionaryTrees.Single(d => d.ID == DictionaryTreeID);
                List<GridColumn> columns = provider.GetGridColumns(this.User.ID, DictionaryTreeID, FieldPlaceHolder.Grid);
                System.Data.DataSet source = provider.GetList(DictionaryTreeID, this.User.ID, this.OrderExpression, 
                    this.SearchConditions, new PagingInfo(false));
                //provider.GetDictionaryDataSource(entity, SearchExp);
                ValueField = entity.PK;
                TextField = entity.Dictionary.IdentifierField;
                if (source != null && source.Tables.Count > 0)
                {
                    GridPager.Visible = SelectorGrid.Visible = true;
                    GridPager.CurrentPage = e.CurrentPage;
                    GridPager.TotalRecords = source.Tables[0].DefaultView.Count;

                    SelectorGrid.DataSource = source;
                    SelectorGrid.PageIndex = e.CurrentPage;
                    SelectorGrid.DataBind();
                    this.AppendGridHeader(columns);
                }
            }
        }

        private void BindGridColumns()
        {
            SelectorGrid.Columns.Clear();
            HyperLinkField selector = new HyperLinkField();
            selector.Text = "Выбрать";
            selector.ItemStyle.Width = new Unit(30, UnitType.Pixel);
            SelectorGrid.Columns.Add(selector);
            using (DictionaryProvider provider = new DictionaryProvider())
            {
                List<GridColumn> columns = provider.GetGridColumns(this.User.ID, DictionaryTreeID, FieldPlaceHolder.Grid);
                foreach (GridColumn item in columns)
                {
                    ProductGridField field = new ProductGridField(item.Name, item.DataItem);
                    field.SortExpression = item.DataItem;
                    SelectorGrid.Columns.Add(field);
                    SearchExpression expr = this.SearchConditions.FirstOrDefault(s => s.FieldID == item.ID);
                    field.HeaderText = expr == null ? field.HeaderText : string.Format("{0} ({1})", field.HeaderText, expr.FieldValue);
                }
            }
        }

        private void AppendGridHeader(List<GridColumn> columns)
        {
            GridViewRow row = new GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);
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
            if (SelectorGrid.Controls.Count > 0)
            {
                Table tbl = SelectorGrid.Controls[0] as Table;
                if (tbl != null)
                {
                    tbl.Rows.AddAt(0, row);
                }
            }
        }

    }
}
