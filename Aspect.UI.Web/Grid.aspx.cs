using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Data.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Aspect.Model.Classification;
using Aspect.Model.ProductDomain;
using Aspect.Domain;
using Aspect.Model;
using Aspect.UI.Web.Controls;
using Aspect.Model.SettingDomain;

using AODL.Document.TextDocuments;
using AODL.Document.Content.Tables;
using AODL.Document.Content.Text;

using System.Web.Script.Serialization;
using System.Globalization;

namespace Aspect.UI.Web
{
    public partial class Grid : Basic.ContentPageBase
    {    
        //private ClassifiacationTypeView classifiacationTypeView = ClassifiacationTypeView.NotDefined;
        /*protected override ClassifiacationTypeView ClassifiacationTypeView
        {
            get
            {
                if (classifiacationTypeView == ClassifiacationTypeView.NotDefined)
                {
                    if (ClassificationView.SelectedNode != null) classifiacationTypeView = Common.GetClassifiacationTypeView(new Guid(ClassificationView.SelectedNode.Value));
                    else classifiacationTypeView = Common.GetClassifiacationTypeView(RequestClassificationTreeID);
                }
                return classifiacationTypeView;
            }
        }

        protected Guid ClassificationTopNodeID
        {
            get
            {
                try
                {
                    return new Guid(this.Request["pid"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }*/

        //protected TreeView ClassificationView;
        protected System.Web.UI.HtmlControls.HtmlAnchor ChooseColumns;
        protected System.Web.UI.HtmlControls.HtmlAnchor ChooseColumnsOrder;
        protected System.Web.UI.HtmlControls.HtmlAnchor ShowBuffer;
        protected System.Web.UI.HtmlControls.HtmlAnchor ChooseCardFields;
        protected System.Web.UI.HtmlControls.HtmlAnchor ProductSearch;

        protected System.Web.UI.HtmlControls.HtmlAnchor ShowSummaryWeight;

        /*protected System.Web.UI.HtmlControls.HtmlAnchor ConfEdit;
        protected System.Web.UI.HtmlControls.HtmlAnchor ConfUsage;
        protected System.Web.UI.HtmlControls.HtmlAnchor ConfView;
        protected System.Web.UI.HtmlControls.HtmlAnchor ConfTree;*/

        protected LinkButton AddToFavorites;
        protected LinkButton PrintSelected;
        protected Literal UserName;
        protected Literal DateTimeLabel;
        protected PlaceHolder CopyToBufferPlaceHolder;
        protected LinkButton InsertFromBuffer;
        protected LinkButton DeleteFromClass;

        protected Ext.Net.ComboBox FilterView;
        protected Ext.Net.Store Store1;
        protected Ext.Net.GridPanel GridPanel1;
        protected Ext.Net.TreePanel ClassView;
        protected Ext.Net.Window SearchWindow;
        protected Ext.Net.Hidden hiddenSearch; /* параметры поиска */
        protected Ext.Net.Hidden hiddenSelectedProducts; /* список выделенных продуктов */

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Ext.Net.X.IsAjaxRequest)
            {
                DateTimeLabel.Text = String.Format("{0} <b>{1}</b>", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
                InsertFromBuffer.Visible = CopyToBufferPlaceHolder.Visible = !(ClassifiacationTypeView == ClassifiacationTypeView.Dictionary);
                DeleteFromClass.Visible = (ClassifiacationTypeView == ClassifiacationTypeView.Custom);

                #region classification_tree
                using (ITreeProvider provider = new ClassificationProvider())
                {
                    this.BindClassView(provider, Guid.Empty, null, RequestClassificationTreeID);
                }

                using (ITreeProvider provider = new CustomClassificationProvider())
                {
                    this.BindClassView(provider, Guid.Empty, null, RequestClassificationTreeID);
                }
                using (ITreeProvider provider = new Aspect.Model.DictionaryDomain.DictionaryProvider())
                {

                    this.BindClassView(provider, Guid.Empty, null, RequestClassificationTreeID);
                }
                #endregion

                this.BindGrid2Columns();
                InitialTopPageBar();

                UserName.Text = this.User.Name;
                this.Title += this.selectedNodeText;
            }
        }

        public void SaveAreaSizes(string value)
        {
            SettingProvider provider = new SettingProvider();
            provider.SaveSetting(this.User.ID, SettingEnum.MainGridAreaSize, value);
        }
        public string GetAreaSizes()
        {
            SettingProvider provider = new SettingProvider();
            return provider.GetSetting(this.User.ID, SettingEnum.MainGridAreaSize);
        }

        protected void InitialTopPageBar()
        {
            this.PopupIframeInitializationString(ShowBuffer, "Буфер", "Popup/Buffer.aspx", 700, 500);
            this.PopupIframeInitializationString(ShowSummaryWeight, "Суммарный вес разузлованного состава", 
                string.Format("Popup/SummaryWeight.aspx?pid='+selectedProductID+'", 
                hiddenSelectedProducts.Value == null ? "" : hiddenSelectedProducts.Value.ToString()), 700, 500);
            if (String.IsNullOrEmpty(this.Request.Url.Query))
            {
                this.PopupIframeInitializationString(ProductSearch, "Поиск", string.Format("Popup/ProductSearch.aspx?{0}={1}&pid={2}", RequestKeyClassificationTreeID, RequestClassificationTreeID, Guid.Empty), 700, 500);
            }
            else
            {
                this.PopupIframeInitializationString(ProductSearch, "Поиск", string.Format("Popup/ProductSearch.aspx{0}", this.Request.Url.Query), 700, 500);
            }
            this.PopupIframeInitializationStringWithProduct(ChooseColumns, "Колонки", string.Format("Popup/UserColumns.aspx?{0}={1}&productid&{2}={3}&url={4}", RequestKeyClassificationTreeID, RequestClassificationTreeID, RequestKeyFieldPlaceHolder, FieldPlaceHolderEnum.Grid, Server.UrlEncode(this.Request.Url.ToString())), 700, 500);
            this.PopupIframeInitializationStringWithProduct(ChooseColumnsOrder, "Колонки", string.Format("Popup/UserColumnsOrder.aspx?{0}={1}&productid&{2}={3}&url={4}", RequestKeyClassificationTreeID, RequestClassificationTreeID, RequestKeyFieldPlaceHolder, FieldPlaceHolderEnum.Grid, Server.UrlEncode(this.Request.Url.ToString())), 700, 500);
            this.PopupIframeInitializationStringWithProduct(ChooseCardFields, "Колонки", string.Format("Popup/UserColumns.aspx?{0}={1}&productid&{2}={3}&url={4}", RequestKeyClassificationTreeID, RequestClassificationTreeID, RequestKeyFieldPlaceHolder, FieldPlaceHolderEnum.GridCard, Server.UrlEncode(this.Request.Url.ToString())), 700, 500);

        }

        /*public void BindClassificationView(ITreeProvider provider, Guid parentID, TreeNode parentNode, Guid selectedID)
        {
            List<ITreeNode> list = new List<ITreeNode>();
            if (!parentID.Equals(Guid.Empty) && parentNode == null)
            {
                ITreeNode entity = provider.GetTreeNode(parentID);
                if (entity == null) return;
                list.Add(entity);
            }
            else
            {
                list = provider.GetList(parentID, User.ID, Roles);
                //List<ITreeNode> list = provider.GetList(parentID);
            }
            foreach (ITreeNode item in list)
            {
                Basic.ColoredTreeNode node;
                switch (item.Section)
                {
                    case TreeNodeSection.Custom:
                        node = new Basic.ColoredTreeNode(item.Name, String.Format("cust-{0}", item.ID));
                        node.Color = 
                    case TreeNodeSection.Default:
                        new Basic.ColoredTreeNode(item.Name, item.ID.ToString());
                }
                = new Basic.ColoredTreeNode(item.Name, item.ID.ToString());
                node.Color = item.Color;
                
                if (item.ID == selectedID) node.Select();
                node.NavigateUrl = string.Format("Grid.aspx?cid={0}&pid={1}", node.Value, this.ClassificationTopNodeID);
                if (parentNode != null) parentNode.ChildNodes.Add(node);
                else ClassificationView.Nodes.Add(node);
                this.BindClassificationView(provider, item.ID, node, selectedID);
            }
        }*/

        public void BindClassView(ITreeProvider provider, Guid parentID, Ext.Net.TreeNode parentNode, Guid selectedID)
        {
            List<ITreeNode> list = new List<ITreeNode>();
            if (!parentID.Equals(Guid.Empty) && parentNode == null)
            {
                ITreeNode entity = provider.GetTreeNode(parentID);
                if (entity == null) return;
                list.Add(entity);
            }
            else
            {
                list = provider.GetList(parentID, User.ID, Roles);
                //List<ITreeNode> list = provider.GetList(parentID);
            }
            foreach (ITreeNode item in list)
            {                
                Ext.Net.TreeNode treeNode;

                if (item.ID.Equals(new Guid("00497ABC-7ADB-11E0-AD88-63F04724019B")))
                {
                    // hack for "Wares"
                    treeNode = new Ext.Net.TreeNode(String.Format("ware-{0}", item.ID), item.Name, Ext.Net.Icon.Folder);
                    treeNode.Cls = "TreeNode-Wares";
                    treeNode.Href = String.Format("/Technology/ListWares.aspx");
                }
                else
                {
                    switch (item.Section)
                    {
                        case TreeNodeSection.Custom:
                            treeNode = new Ext.Net.TreeNode(String.Format("cust-{0}", item.ID), item.Name, Ext.Net.Icon.Folder);
                            treeNode.Cls = "TreeNode-Custom";
                            break;
                        case TreeNodeSection.Dictionary:
                            treeNode = new Ext.Net.TreeNode(String.Format("dict-{0}", item.ID), item.Name, Ext.Net.Icon.Folder);
                            treeNode.Cls = "TreeNode-Dictionary";
                            break;
                        default:
                            treeNode = new Ext.Net.TreeNode(String.Format("dflt-{0}", item.ID), item.Name, Ext.Net.Icon.Folder);
                            treeNode.Cls = "TreeNode-Default";
                            break;
                    }
                    treeNode.Href = String.Format("/Grid.aspx?cid={0}", item.ID);                
                }                
                if (parentNode != null) parentNode.Nodes.Add(treeNode);
                else (ClassView.Root[0] as Ext.Net.TreeNode).Nodes.Add(treeNode);
                
                if (item.ID == this.RequestClassificationTreeID)
                {
                    this.selectedNodeID = this.RequestClassificationTreeID;
                    this.selectedNodeText = item.Name;
                    ClassView.SelectNode(treeNode.NodeID);
                    Ext.Net.TreeNodeBase tmpParent = treeNode.ParentNode;
                    while (tmpParent != null)
                    {
                        tmpParent.Expanded = true;
                        tmpParent = tmpParent.ParentNode;
                    }

                    //ClassView.SelectNode(treeNode.NodeID);
                    //ClassView.ExpandChildNodes(treeNode.NodeID);
                }
                this.BindClassView(provider, item.ID, treeNode, selectedID);
            }
        }
        
        protected void AddSearchPanel(Ext.Net.Window parent, string caption, Guid identifier, TypeEnum type)
        {
            List<SearchExpression> searchList = this.SearchConditions;

            /* строка поиска */
            Ext.Net.Toolbar searchBar = new Ext.Net.Toolbar();
            searchBar.CtCls = "searchBar";
            searchBar.Layout = "hbox";            

            /* text label */
            Ext.Net.Label searchLabel = new Ext.Net.Label(caption);
            searchLabel.Margins = "4 2 2 2";
            searchLabel.Flex = 1;
            searchBar.Add(searchLabel);

            /* combo box */
            Ext.Net.ComboBox searchCombo = new Ext.Net.ComboBox();
            searchCombo.CtCls = "condition";
            searchCombo.Editable = false;
            searchCombo.Items.Add(new Ext.Net.ListItem("~"));
            if (type == TypeEnum.Integer || type == TypeEnum.Decimal)
            {
                searchCombo.Items.Add(new Ext.Net.ListItem(">"));
                searchCombo.Items.Add(new Ext.Net.ListItem("<"));
                searchCombo.Items.Add(new Ext.Net.ListItem("!~"));
            }           
            searchCombo.SelectedIndex = 0;
            searchCombo.HiddenName = "";
            searchCombo.Margins = "2 5 0 5";
            searchCombo.Width = 80;
            searchCombo.SubmitValue = false;
            searchBar.Add(searchCombo);

            /* editor field */
            Ext.Net.TextField searchField = new Ext.Net.TextField();            
            searchField.CtCls = "value";            
            searchField.Flex = 1;
            searchField.Margins = "2 2 0 0";
            searchField.SubmitValue = false;
            if (type == TypeEnum.Integer)
            {
                searchField.Regex = @"^\d+$";
                searchField.RegexText = "Введите целое число";
            }
            if (type == TypeEnum.Decimal)
            {
                searchField.Regex = @"^\d+(,\d+)?$";
                searchField.RegexText = "Введи вещественное число с запятой";
            }
            if (type == TypeEnum.Boolean)
            {
                searchField.Regex = @"^0|1$";
                searchField.RegexText = "Введи либо 0 (да) либо 1 (нет)";
            }
            searchBar.Add(searchField);            

            /* hidden value */
            Ext.Net.Hidden searchIdent = new Ext.Net.Hidden();
            searchIdent.CtCls = "identifier";
            searchIdent.Value = identifier.ToString();
            searchIdent.HideLabel = true;
            searchIdent.SubmitValue = false;
            searchBar.Add(searchIdent);

            parent.Items.Add(searchBar);

            /* Restore previous state */
            try
            {
                SearchExpression searchExpr = searchList.First(p => p.FieldID == identifier);
                searchField.Value = searchExpr.FieldValue;
                searchCombo.Value = searchExpr.FieldCond.toString();
            }
            catch {}
                
        }

        private void BindGrid2Columns()
        {
            /*
             * Создание окна поиска 
             */
            Ext.Net.Toolbar bar = new Ext.Net.Toolbar();
            bar.Layout = "hbox";
            
            using (ContentDomain provider = Aspect.Model.Common.GetContentDomain(ClassifiacationTypeView))
            {
                //List<SearchExpression> source = new List<SearchExpression>();
                //List<IUserField> dictColumns = provider.GetUserFields(this.User.ID, RequestClassificationTreeID, FieldPlaceHolder.Grid);
                //List<UserProperty> columns = provider.GetUserPropertyColumns(this.User.ID, RequestClassificationTreeID, FieldPlaceHolder.Grid);

                // получаем пользовательские колонки для данного класса
                List<GridColumn> columns = provider.GetGridColumns(this.User.ID, RequestClassificationTreeID, FieldPlaceHolder.Grid);

                // извлекаем ширины колонок для данного класса
                List<ColumnWidth> columnWidths = provider.ColumnWidths.Where(p => p.ClassificationTreeID == RequestClassificationTreeID && p.UserID == this.User.ID).ToList();

                List<GridColumn> gridColumns = provider.GetGridColumns(this.User.ID, RequestClassificationTreeID, FieldPlaceHolder.Grid);
                foreach (var column in gridColumns)
                {
                    string fieldName = column.IsDictionary ? String.Format("{1} - {0}", column.Name, column.Group) : column.Name;
                    /*IEnumerable<SearchExpression> list = this.SearchConditions.Where(s => s.FieldID == column.ID);
                    source.Add(new SearchExpression()
                    {
                        FieldValue = list.Count() > 0 ? list.First().FieldValue : string.Empty,
                        FieldName = fieldName,
                        FieldID = column.ID,
                        Order = column.Order
                    });*/
                    AddSearchPanel(SearchWindow, fieldName, column.IsDictionary ? column.ID : column.SourceID, column.GridColumnType);
                }                

                /*
                 * Создание списка колонок в гриде продуктов
                 */
                    
                foreach (GridColumn column in columns)
                {
                    Ext.Net.ColumnBase newColumn;
                    switch (column.GridColumnType) {
                        case TypeEnum.Boolean:
                            newColumn = new Ext.Net.CheckColumn(); break;
                        default:
                            newColumn = new Ext.Net.Column(); break;
                    }

                    newColumn.ColumnID = column.ID.ToString();
                    foreach (int width in columnWidths.Where(p => p.ColumnID == column.ID).Select(p => p.Width))
                    {
                        newColumn.Width = width;
                    }
                    newColumn.Header = column.Alias;
                    newColumn.Tooltip = column.IsDictionary ? String.Format("{0} :: {1}", column.Group, column.Name) : column.Name;
                    newColumn.DataIndex = String.Format("{0}|{1}", column.OrderExpression, column.GridColumnType.ToString());
                    newColumn.Hideable = false;
                    this.GridPanel1.ColumnModel.Columns.Add(newColumn);                    
                    Store1.AddField(new Ext.Net.RecordField(newColumn.DataIndex));                        
                }
                // adding another field
                Store1.AddField(new Ext.Net.RecordField("ID"));
                Store1.AddField(new Ext.Net.RecordField("CID"));
            }
        }

        /*private void BindGridColumns()
        {
            TreeNode node = ClassificationView.SelectedNode;
            if (node != null)
            {
                ProductGrid.Columns.Clear();
                SelectorProductGridField selector = new SelectorProductGridField(string.Empty, "ID", true, SelectedProductsHidden.ClientID);
                //field.HeaderText = String.Format("<input type=\"checkbox\" onclick=\"clearSelection('{0}');\" name=\"clearselectionsname\" id=\"clearselectionsid\">", SelectedProductsHidden.ClientID);
                selector.ItemStyle.Width = new Unit(15, UnitType.Pixel);
                ProductGrid.Columns.Add(selector);
                List<GridColumn> columns = new List<GridColumn>();
                ContentDomain provider = null;
                try
                {
                    provider = Common.GetContentDomain(ClassifiacationTypeView);
                    if (provider != null) columns = provider.GetGridColumns(this.User.ID, new Guid(node.Value), FieldPlaceHolder.Grid);
                }
                finally
                {
                    if (provider != null) provider.Dispose();
                }
                bool actionColumnAdded = false;
                //bool actionColumnAdded = (ClassifiacationTypeView == ClassifiacationTypeView.Dictionary);
                foreach (GridColumn item in columns)
                {
                    //ProductGridField field;
                    BoundField field;
                    if (!actionColumnAdded)
                    {
                        if (ClassifiacationTypeView == ClassifiacationTypeView.Dictionary)
                        {
                            field = new ActionProductGridField(item.Name, item.DataItem, this.RequestClassificationTreeID);
                            
                        }
                        else
                        {
                            field = new ActionProductGridField(item.Name, item.DataItem);
                        }
                        //field.SortExpression = item.DataItem;
                        field.SortExpression = string.Format("{0}|{1}", item.OrderExpression, TypeEnum.Default);
                        ProductGrid.Columns.Add(field);
                        actionColumnAdded = true;
                    }
                    else
                    {
                        if (item.GridColumnType == TypeEnum.Boolean)
                        {
                            field = new BooleanGridField();
                            field.DataField = item.DataItem;
                            field.HeaderText = item.Name;
                            field.ReadOnly = true;
                        }
                        else
                        {
                            field = new ProductGridField(item.Name, item.DataItem);   
                        }
                        //field.SortExpression = item.DataItem;
                        field.SortExpression = string.Format("{0}|{1}", item.OrderExpression, item.GridColumnType.ToString());
                        ProductGrid.Columns.Add(field);

                    }

                    SearchExpression expr = this.SearchConditions.FirstOrDefault(s => s.FieldID == item.ID);
                    field.HeaderText = expr == null ? field.HeaderText : string.Format("{0} ({1})", field.HeaderText, expr.FieldValue);
                }
            }
        }*/

        /*protected void GridPager_CurrentPageChanged(object sender, PagerEventArgs e)
        {
            TreeNode node = ClassificationView.SelectedNode;
            if (node != null)
            {
                DataSet data = null;
                ContentDomain provider = null;
                List<GridColumn> columns = new List<GridColumn>();
                try
                {
                    provider = Common.GetContentDomain(ClassifiacationTypeView);
                    if (provider != null)
                    {
                        //Response.Write(provider.getQuery(new Guid(node.Value), this.User.ID, this.OrderExpression, this.SearchConditions));
                        //return;

                        if (provider is ProductProvider)
                        {
                            data = (provider as ProductProvider).GetListEx(new Guid(node.Value), this.User.ID, this.OrderExpression, this.SearchConditions, ViewFilter.SelectedItem.Value);
                        }
                        else
                        {
                            data = provider.GetList(new Guid(node.Value), this.User.ID, this.OrderExpression, this.SearchConditions);
                        }
                        if (ShowSelected && !String.IsNullOrEmpty(SelectedProductsHidden.Value))
                        {
                            string[] ids = SelectedProductsHidden.Value.Split(',').Select(s => string.Format("CONVERT('{0}','System.Guid')", s)).ToArray();
                            data.Tables[0].DefaultView.RowFilter = string.Format("ID in ({0})", string.Join(",", ids));
                        }
                        columns = provider.GetGridColumns(this.User.ID, new Guid(node.Value), FieldPlaceHolder.Grid);

                    }

                }
                finally
                {
                    if (provider != null) provider.Dispose();
                }
                if (data != null && data.Tables.Count > 0)
                {  
                    GridPager.Visible = ProductGrid.Visible = true;
                    GridPager.CurrentPage = e.CurrentPage;
                    GridPager.PageSize = 10;
                    GridPager.TotalRecords = data.Tables[0].DefaultView.Count;//data.Tables[0].Rows.Count;
                    ProductGrid.DataSource = data.Tables[0].DefaultView;//data;

                    ProductGrid.PageIndex = e.CurrentPage;
                    ProductGrid.PageSize = 10;
                    ProductGrid.DataBind();
                    if (data.Tables[0].Rows.Count > 0)
                    {
                        string pid = data.Tables[0].Rows[0][Common.IDColumnTitle].ToString();
                        string cid = this.ClassificationView.SelectedNode.Value;
                        string function = string.Format("<script language=JavaScript>onGridViewRowSelectedCallback('{0}','{1}', document.getElementById('{2}'),'{3}');</script>", pid, cid, ProductGrid.Rows[0].ClientID, ProductGrid.Controls[0].ClientID);

                        this.ClientScript.RegisterStartupScript(this.GetType(), "mainview", function);
                        //ProductGrid_OnRowSelected(0);
                    }
                    //---
                    this.AppendGridHeader(columns);
                }
                else
                {
                    GridPager.Visible = ProductGrid.Visible = false;
                }
            }
        }*/

        [Ext.Net.DirectMethod]
        public void OnColumnResize(Guid columnId, int newSize)
        {
            ContentDomain provider = Common.GetContentDomain(ClassifiacationTypeView);
            List<ColumnWidth> columnWidths = provider.ColumnWidths.Where(p => p.ClassificationTreeID == this.RequestClassificationTreeID).ToList();
            
            if (columnWidths.Count(p => p.ColumnID == columnId) > 0)
            {
                ColumnWidth width = columnWidths.Single(p => p.ColumnID == columnId);
                width.Width = newSize;
                provider.SubmitChanges();
            }
            else
            {
                ColumnWidth width = new ColumnWidth();
                width.ID = Guid.NewGuid();
                width.ClassificationTreeID = this.RequestClassificationTreeID;
                width.ColumnID = columnId;
                width.UserID = User.ID;
                width.Width = newSize;
                provider.ColumnWidths.InsertOnSubmit(width);
                provider.SubmitChanges();
            }
        }

        /*
         * Метод перелистывания страниц в гриде
         */
        protected void Store1_RefreshData(object sender, Ext.Net.StoreRefreshDataEventArgs e)
        {
            DataSet data = null;            
            List<GridColumn> columns = new List<GridColumn>();
            
            using (ContentDomain provider = Common.GetContentDomain(ClassifiacationTypeView))
            {
                Domain.OrderExpression OrderExpression = new OrderExpression();
                if (e.Sort.Length > 0)
                {
                    string expr = e.Sort.Split('|')[0];
                    string tp = e.Sort.Split('|')[1];
                    OrderExpression.Expression = expr;
                    OrderExpression.ColumnType = (TypeEnum)Enum.Parse(typeof(TypeEnum), tp);
                }

                if (e.Dir == Ext.Net.SortDirection.DESC)
                    OrderExpression.SortDirection = Aspect.Domain.SortDirection.desc;
                            
                List<SearchExpression> listSearch = this.SearchConditions.ToList();
                // показывать только выбранные
                if (this.ShowSelected && !String.IsNullOrEmpty(hiddenSelectedProducts.Value.ToString()))
                {
                    SearchExpression selected = new SearchExpression();
                    selected.FieldCond = Condition.Inset;                
                    selected.FieldValue = hiddenSelectedProducts.Value.ToString();
                    selected.FieldName = "ID";
                    listSearch.Add(selected);
                }
                // показывать основные версии
                if (FilterView.Value.ToString().Contains("mainVers"))
                {
                    SearchExpression mainVers = new SearchExpression();
                    mainVers.FieldID = new Guid("BBE170B0-28E4-4738-B365-1038B03F4552"); //основная версия
                    mainVers.FieldValue = "1";
                    listSearch.Add(mainVers);
                }
                // показывать приказные версии
                if (FilterView.Value.ToString().Contains("prikazVers"))
                {
                    SearchExpression mainVers = new SearchExpression();
                    mainVers.FieldCond = Condition.Beable;
                    mainVers.FieldValue = "9A38E338-DD60-4636-BFE3-6A98BAF8AE87"; // номер приказала
                    listSearch.Add(mainVers);
                }

                // получить страницу из списка продуктов, попадающих под все заданные условия 
                data = provider.GetList(RequestClassificationTreeID, this.User.ID, OrderExpression, listSearch, new PagingInfo(e.Start, e.Limit));
                columns = provider.GetGridColumns(this.User.ID, RequestClassificationTreeID, FieldPlaceHolder.Grid);
            }               
            if (data != null && data.Tables.Count > 0)
            {
                e.Total = data.Tables[1].Rows[0].Field<int>(0);
                List<object> dataSource = new List<object>();

                foreach (DataRow row in data.Tables[2].Rows)
                {
                    List<object> dataRow = new List<object>();
                    foreach (GridColumn column in columns)
                    {
                        if (column.GridColumnType == TypeEnum.Boolean)
                        {
                            dataRow.Add(row[column.DataItem].ToString() == "1");
                        } 
                        else if (column.GridColumnType == TypeEnum.Datetime)
                        {
                            try
                            {
                                dataRow.Add(row.Field<DateTime>(column.DataItem).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            }
                            catch
                            {
                                dataRow.Add(row[column.DataItem].ToString());
                            }
                        }
                        else
                        {
                            dataRow.Add(row[column.DataItem].ToString());
                        }
                    }
                    // adding another field
                    dataRow.Add(row["ID"].ToString());
                    dataRow.Add(ClassifiacationTypeView == ClassifiacationTypeView.Dictionary ? this.RequestClassificationTreeID : Guid.Empty);
                    dataSource.Add(dataRow.ToArray());
                }
                this.Store1.DataSource = dataSource;
                this.Store1.DataBind();
            }                     
        }

        /*private void AppendGridHeader(List<GridColumn> columns)
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
            if (ProductGrid.Controls.Count > 0)
            {
                System.Web.UI.WebControls.Table tbl = ProductGrid.Controls[0] as System.Web.UI.WebControls.Table;
                if (tbl != null)
                {
                    tbl.Rows.AddAt(0, row);
                }
            }
        }*/

        /*protected void ProductGrid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                {                    
                    e.Row.ID = string.Format("{0}_Row{1}", ProductGrid.ClientID, e.Row.RowIndex);
                    string pid = DataBinder.Eval(e.Row.DataItem, Common.IDColumnTitle).ToString();
                    string cid = this.ClassificationView.SelectedNode.Value;
                    string function = string.Format("onGridViewRowSelectedCallback('{0}','{1}', this, '{2}');", pid, cid, ProductGrid.Controls[0].ClientID);
                    e.Row.Attributes.Add("onclick", function);
                    if (e.Row.RowState == DataControlRowState.Alternate) e.Row.CssClass = "row2";
                    else e.Row.CssClass = string.Empty;

                    e.Row.Attributes["onmouseover"] = "highLightRow(this)";
                    e.Row.Attributes["onmouseout"] = "unHighLightRow(this)";
                    CheckBox chk = e.Row.Cells[0].FindControl("SelectCheckBox") as CheckBox;
                    chk.Attributes.Add("onclick", String.Format("selectProduct(event,this,'{0}','{1}');", pid, SelectedProductsHidden.ClientID));
                    chk.Checked = hiddenSelectedProducts.Value.ToString().Contains(pid);

                    // Хак для перемещения курсора на строку, при клике на ссылку
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        foreach(Control control in cell.Controls)
                        {
                            if (control is HyperLink)
                            {
                                function = string.Format("onGridViewRowSelectedCallback('{0}','{1}', this.parentNode.parentNode, '{2}');", pid, cid, ProductGrid.Controls[0].ClientID);                                
                                (control as HyperLink).Attributes.Add("onclick", function);
                            }
                        }
                    }
                }
            }
        }*/

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
            string[] args = e.SortExpression.Split('|');
            string expr = args[0];
            string tp = args[1];
            
            if (OrderExpression.Expression == expr/*e.SortExpression*/)
            {
                if (OrderExpression.SortDirection == Aspect.Domain.SortDirection.asc) orderExpression.SortDirection = Aspect.Domain.SortDirection.desc;
                else OrderExpression.SortDirection = Aspect.Domain.SortDirection.asc;
            }
            OrderExpression.Expression = expr;//e.SortExpression;
            OrderExpression.ColumnType = (TypeEnum)Enum.Parse(typeof(TypeEnum), tp);
            this.BindGrid2Columns();
            //GridPager_CurrentPageChanged(sender, new PagerEventArgs(0, GridPager.PageSize, 0));            
        }
       
        protected class SearchCondition
        {
            public Guid id;
            public string cond;
            public string value;
        }

        protected class SearchConditionList
        {
            public string mode;
            public List<SearchCondition> items;
        }
        
        protected override List<Aspect.Domain.SearchExpression> SearchConditions
        {
            get
            {
               List<SearchExpression> searchConditions = new List<Aspect.Domain.SearchExpression>();

               if (hiddenSearch != null && hiddenSearch.Value != null)
               {
                   try
                   {
                       JavaScriptSerializer js = new JavaScriptSerializer();
                       SearchConditionList listConditions = (SearchConditionList)js.Deserialize<SearchConditionList>(hiddenSearch.Value.ToString());
                       foreach (SearchCondition field in listConditions.items)
                       {
                           Condition cond = new Condition();
                           switch (field.cond)
                           {
                               case "!~": cond = Condition.Nequal; break;
                               case "~": cond = Condition.Equal; break;
                               case ">": cond = Condition.More; break;
                               case "<": cond = Condition.Less; break;
                           }
                           searchConditions.Add(new Aspect.Domain.SearchExpression()
                           {
                               FieldID = field.id,
                               FieldValue = field.value.Trim(),
                               FieldCond = cond
                           });
                       }
                   }
                   catch { }
               }
               return searchConditions;
            }
        }

        
        public bool ShowSelected
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
            if (String.IsNullOrEmpty(hiddenSelectedProducts.Value.ToString()))
            {
                ShowSelected = false;
            }

            //BindGrid2Columns();
            //GridPager_CurrentPageChanged(sender, new PagerEventArgs(0, GridPager.PageSize, 0));
        }
        
        /*protected void RefreshButton_Click(object sender, EventArgs e)
        {
            this.OrderExpression = new OrderExpression();
            //this.BindGridColumns();
            this.BindGrid2Columns();
            //GridPager_CurrentPageChanged(sender, new PagerEventArgs(GridPager.CurrentPage, GridPager.PageSize, GridPager.TotalPages));
        }*/

        public Guid selectedNodeID;
        public string selectedNodeText;
        public override void AddVisitHistory()
        {
            using (Aspect.Model.UserDomain.UserProvider provider = new Aspect.Model.UserDomain.UserProvider())
            {
                provider.AddToLastViewed(selectedNodeID, this.User.ID, selectedNodeText, string.Format("/Grid.aspx?cid={0}&pid={0}", selectedNodeID));
            }
        }

        protected void AddToFavorites_Click(object sender, EventArgs e)
        {
            using (Aspect.Model.UserDomain.UserProvider provider = new Aspect.Model.UserDomain.UserProvider())
            {
                provider.AddToFavorites(selectedNodeID, this.User.ID, selectedNodeText, string.Format("/Grid.aspx?cid={0}&pid={0}", selectedNodeID));
            }
        }

        protected void InsertFromBuffer_Click(object sender, EventArgs e)
        {
            ContentDomain provider = null;
            try
            {
                provider = Common.GetContentDomain(ClassifiacationTypeView);
                if (provider != null)
                {
                    if ((this.MultiBuffer.Count > 0))
                    {
                        provider.AddProducts(this.User.ID, this.RequestClassificationTreeID, this.MultiBuffer.Keys.ToList());
                    }
                }

            }
            finally
            {
                if (provider != null) provider.Dispose();
            }


            /*using (CustomClassificationProvider provider = new CustomClassificationProvider())
            {
                if ((this.ProductsBuffer.Count > 0) && provider.GetTreeNode(id) != null)
                {
                    provider.AddProducts(id, this.ProductsBuffer);
                }
            }*/

            GridPanel1.Reload();
            //RefreshButton_Click(sender, e);
        }

        /// <summary>
        /// Для выделенных в гриде продуктов устанавливает поле "Основная версия" в "1",
        /// при условии что в данный момент времени это поле установлено в "0"
        /// </summary>
        protected void SetMainVersion_Click(object sender, EventArgs e)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                // составляем список выделенных продуктов
                List<Guid> selectedRows;
                if (hiddenSelectedProducts.Value.ToString() != "")
                    selectedRows = hiddenSelectedProducts.Value.ToString().Split(',').Select(s => new Guid(s)).ToList();
                else
                    return ;

                provider.SetMainVersion(this.User.ID, selectedRows);

                // insert here
                // RefreshButton_Click(sender, e);
                // GridPanel1.Reload();
            }
        }

        protected void PrintSelected_Click(object sender, EventArgs e)
        {
            string fileName = string.Format("products-{0:yyyy-MM-dd_hh-mm-ss}.odt", DateTime.Now);
                        
            TextDocument document = new TextDocument();
            AODL.Document.Content.Tables.Table table;
            document.New();            

            using (ContentDomain provider = Common.GetContentDomain(ClassifiacationTypeView))
            {
                //TreeNode node = ClassificationView.SelectedNode;
                List<GridColumn> columns = provider.GetGridColumns(this.User.ID, this.RequestClassificationTreeID, FieldPlaceHolder.Grid);
                DataSet data = provider.GetList(this.RequestClassificationTreeID, this.User.ID, this.OrderExpression, 
                    this.SearchConditions, new PagingInfo(false));

                // составляем список выделенных продуктов
                List<Guid> selectedRows;
                if (hiddenSelectedProducts.Value.ToString() != "")
                    selectedRows = hiddenSelectedProducts.Value.ToString().Split(',').Select(s => new Guid(s)).ToList();
                else
                    selectedRows = new List<Guid>();
                
                //Create a table for a text document using the TableBuilder
                table = TableBuilder.CreateTextDocumentTable(
                    document,
                    "table1",
                    "table1",
                    selectedRows.Count + 1,
                    columns.Count,
                    16.99,
                    false,
                    true);

                //Fill the cells                                
                int columnIndex = 0, rowIndex = 0;
                foreach (GridColumn column in columns)
                {
                    Cell cell = table.RowCollection[0].CellCollection[columnIndex];
                    Paragraph paragraph = ParagraphBuilder.CreateStandardTextParagraph(document);                                        
                    paragraph.TextContent.Add(new SimpleText(document, column.Name));
                    cell.Content.Add(paragraph);
                    columnIndex++;
                }
                rowIndex++;
                                
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    Guid rodId = row.Field<Guid>("ID");
                    if (!selectedRows.Contains(rodId)) continue;

                    columnIndex = 0;
                    foreach (GridColumn column in columns)
                    {
                        if (!row.IsNull(column.DataItem))
                        {
                            try
                            {
                                Cell cell = table.RowCollection[rowIndex].CellCollection[columnIndex];
                                Paragraph paragraph = ParagraphBuilder.CreateStandardTextParagraph(document);
                                string value = row.Field<string>(column.DataItem);
                                paragraph.TextContent.Add(new SimpleText(document, value));
                                cell.Content.Add(paragraph);
                            }
                            catch (System.Exception)
                            {
                                try
                                {
                                    Cell cell = table.RowCollection[rowIndex].CellCollection[columnIndex];
                                    Paragraph paragraph = ParagraphBuilder.CreateStandardTextParagraph(document);
                                    int value = row.Field<int>(column.DataItem);
                                    paragraph.TextContent.Add(new SimpleText(document, value.ToString()));
                                    cell.Content.Add(paragraph);
                                }
                                catch (System.Exception)
                                {
                                    try
                                    {
                                        Cell cell = table.RowCollection[rowIndex].CellCollection[columnIndex];
                                        Paragraph paragraph = ParagraphBuilder.CreateStandardTextParagraph(document);
                                        decimal value = row.Field<decimal>(column.DataItem);
                                        paragraph.TextContent.Add(new SimpleText(document, value.ToString()));
                                        cell.Content.Add(paragraph);
                                    }
                                    catch (System.Exception)
                                    {

                                    }  
                                }  
                            }
                        }
                        columnIndex++;
                    }                    
                    rowIndex++;
                }

            }
            
            //Add table to the document
            document.Content.Add(table);
            //Save the document
            string tmpCurrentDitectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Server.MapPath("~/tmp").ToString();
            String fs_guid = Server.MapPath(String.Format("~/tmp/{0}.odt", Guid.NewGuid().ToString()));
            document.SaveTo(fs_guid);
            document.Dispose();
            Environment.CurrentDirectory = tmpCurrentDitectory;

            Response.Clear();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/octet-stream";            

            // Copy file to Response stream
            FileStream fs = new FileStream(fs_guid, FileMode.Open);
            Response.AddHeader("Content-Length", fs.Length.ToString());
            
            int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            while (true)
            {
                int read = fs.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    break;
                }
                Response.OutputStream.Write(buffer, 0, read);
            }

            fs.Close(); //закрываем writer
            File.Delete(fs_guid);
            Response.End(); //заканчиваем ответ сервера, иначе после этого вставится весь контент страницы
            return;
        }

        [Ext.Net.DirectMethod]
        public void RefreshState()
        {
            this.OrderExpression = new OrderExpression();            
            this.BindGrid2Columns();
            this.GridPanel1.Render();
            this.SearchWindow.Render();
        }

        protected void DeleteFromClass_Click(object sender, EventArgs e)
        {
            using (CustomClassificationProvider provider = new CustomClassificationProvider())
            {
                if ((this.MultiBuffer.Count > 0) && provider.GetTreeNode(this.RequestClassificationTreeID) != null)
                {
                    provider.DeleteProducts(this.User.ID, this.RequestClassificationTreeID, hiddenSelectedProducts.Value.ToString().Split(',').Select(s => new Guid(s)).ToList());
                    //provider.DeleteProducts(id, this.ProductsBuffer);
                }
            }
            //insert here
            //RefreshButton_Click(sender, e);
            GridPanel1.Reload();
        }

        /*protected void OnSelectFilter(object sender, EventArgs e)
        {
            ViewFilterState = ViewFilter.SelectedValue;
            //this.BindGridColumns();
            this.BindGrid2Columns();
            //GridPager_CurrentPageChanged(sender, new PagerEventArgs(GridPager.CurrentPage, GridPager.PageSize, GridPager.TotalPages));
        }*/

        /*protected string ViewFilterState
        {
            get
            {
                if (ViewState["ViewFilterState"] == null)
                {
                    ViewState["ViewFilterState"] = "";
                }
                return Convert.ToString(ViewState["ViewFilterState"]);
            }
            set
            {
                ViewState["ViewFilterState"] = value;
            }
        }*/

        protected void ClearBuffer_Click(object sender, EventArgs e)
        {
            ClearMultiBuffer();
        }
    }
}
