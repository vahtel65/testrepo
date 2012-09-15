using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Aspect.Domain;
using Aspect.Model.ConfigurationDomain;
using Aspect.Model.ProductDomain;
using Aspect.UI.Web.Controls;
using Aspect.Model;

using AODL.Document.TextDocuments;
using AODL.Document.Content.Tables;
using AODL.Document.Content.Text;
using System.Text;

using System.Web.Script.Serialization;

namespace Aspect.UI.Web.Configuration
{
    public partial class Edit : Basic.PageBase
    {
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

        protected bool IsViewMode
        {
            get
            {
                try
                {
                    return ("view" == Request["mode"].ToString());
                }
                catch
                {
                    return false;
                }
            }
        }

        protected Guid RequestClassificationTreeID
        {
            get
            {
                return Aspect.Domain.FormGridView.ConfigurationView;
            }
        }

        protected LinkButton AddFromBuffer;
        protected LinkButton Save;
        protected LinkButton Cancel;
        protected Label ModeLabel;
        protected Label LabelErrorMessage;
        protected CheckBox MadeBasicVersion;
        protected TextBox ReasonChanges;
        protected Label ReasonChangesLavel;

        protected System.Web.UI.HtmlControls.HtmlAnchor ShowBuffer;
        protected System.Web.UI.HtmlControls.HtmlAnchor SummaryWeight;
        protected Ext.Net.Hidden hiddenStoreData;
        protected Ext.Net.Hidden hiddenSelectedProducts;
        protected Ext.Net.Store Store1;
        protected Ext.Net.GridPanel GridPanel;

        protected void Page_Load(object sender, EventArgs e)
        {         
            if (IsViewMode)
            {
                ModeLabel.Text = "Просмотр";
                AddFromBuffer.Visible = false;
                Save.Visible = false;
                Cancel.Visible = false;
                MadeBasicVersion.Visible = false;
                ReasonChanges.Visible = false;
                ReasonChangesLavel.Visible = false;
            }
            else
            {
                ModeLabel.Text = "Редактирование";
            }

            if (!Page.IsPostBack)
            {
                // только при открытии спецификации на редактирование
                if (!IsViewMode)
                {
                    using (ProductProvider provider = new ProductProvider())
                    {
                        // для основных версий продукта создаём новую версию и 
                        // перенаправляем туда пользователя
                        if (provider.IsMainVersion(ProductID))
                        {
                            Guid newProductID = provider.CreateNewVersionOfProduct(ProductID, User.ID);
                            Response.Redirect(String.Format("/Configuration/Edit.aspx?ID={0}", newProductID));
                        }
                    }
                }
                
                this.PopupIframeInitializationString(ShowBuffer, "Буфер", "../Popup/Buffer.aspx", 700, 500);
                this.PopupIframeInitializationString(SummaryWeight, "Суммарный вес разузлованного состава", 
                    string.Format("../Popup/SummaryWeight.aspx?pid='+selectedProductID+'",
                    hiddenSelectedProducts.Value == null ? "" : hiddenSelectedProducts.Value.ToString()), 700, 500);
                //this.BindGridColumns();
                this.BindGridColumns2();
                this.BindData(new Dictionary<Guid, Guid>());
            }
            this.Title = HeaderLiteral.Text;
        }
        protected ITextControl HeaderLiteral;
        protected ITextControl HeaderDateLiteral;
        protected GridView EditConfigurationGrid;

        /*private void BindGridColumns()
        {
            using (EditConfigurationProvider provider = new EditConfigurationProvider())
            {
                Aspect.Domain.Product prod = provider.GetProduct(this.ProductID);
                if (prod == null) return;
                this.Title = HeaderLiteral.Text = string.Format(HeaderLiteral.Text, prod.PublicName, prod.Version == null ? string.Empty : prod.Version.ToString());
                HeaderDateLiteral.Text = string.Format(HeaderDateLiteral.Text, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());

                //---
                SelectorProductGridField selector = new SelectorProductGridField(string.Empty, "ConfigurationID", true);
                selector.ItemStyle.Width = new Unit(15, UnitType.Pixel);
                EditConfigurationGrid.Columns.Add(selector);
                //---

                bool actionColumnAdded = false;
                List<GridColumn> list = provider.GetGridColumns();
                foreach (GridColumn item in list)
                {
                    BoundField field = null;
                    EditableGridColumn editableColumn = item as EditableGridColumn;                    
                    if (editableColumn == null)
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
                            if (!actionColumnAdded)
                            {
                                field = new ActionProductGridField(item.Name, item.DataItem);
                                actionColumnAdded = true;
                            }
                            else
                            {
                                field = new ProductGridField(item.Name, item.DataItem);
                            }
                            field.ItemStyle.Font.Size = new FontUnit(FontSize.Smaller);
                        }
                        //field = new ActionProductGridField(item.Name, item.DataItem);
                    }
                    else
                    {

                        if (editableColumn.View == EditableGridColumn.GridColumnView.TextBox)
                        {
                            field = new TextBoxProductGridField(editableColumn);
                        }
                        else if (editableColumn.View == EditableGridColumn.GridColumnView.DropDown)
                        {
                            field = new DropDownProductGridField(editableColumn);
                        }
                        else if (editableColumn.View == EditableGridColumn.GridColumnView.CheckBox)
                        {
                            field = new CheckBoxProductGridField(editableColumn);
                        }
                    }

                    if (field != null)
                    {
                        EditConfigurationGrid.Columns.Add(field);
                        //EditAddedConfigurationGrid.Columns.Add(field);
                    }
                }
            }
        }*/

        private void BindGridColumns2()
        {            
            using (EditConfigurationProvider provider = new EditConfigurationProvider())
            {
                Aspect.Domain.Product prod = provider.GetProduct(this.ProductID);
                if (prod == null) return;

                this.Title = HeaderLiteral.Text = string.Format(HeaderLiteral.Text, prod.PublicName, prod.Version == null ? string.Empty : prod.Version.ToString());
                HeaderDateLiteral.Text = string.Format(HeaderDateLiteral.Text, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
                
                // извлекаем ширины колонок для данного класса
                List<ColumnWidth> columnWidths = provider.ColumnWidths.Where(p => p.ClassificationTreeID == new Guid("11110000-0000-0000-0000-000011110001")
                    && p.UserID == this.User.ID).ToList();
                                    
                List<GridColumn> list = provider.GetGridColumns();
                
                // sort columns by user's order
                list = list.OrderBy(clm => columnWidths.Exists(w => w.ColumnID == clm.ID) ? columnWidths.Single(w => w.ColumnID == clm.ID).Index : Int32.MaxValue).ToList();                
                
                foreach (GridColumn column in list)
                {
                    Ext.Net.ColumnBase extjsColumn = null;
                    EditableGridColumn editableColumn = column as EditableGridColumn;
                    if (editableColumn == null)
                    {
                        if (column.GridColumnType == TypeEnum.Boolean)
                        {
                            extjsColumn = new Ext.Net.CheckColumn();
                            extjsColumn.DataIndex = column.DataItem;
                            extjsColumn.Header = column.Name;
                        }
                        else
                        {
                            extjsColumn = new Ext.Net.Column();
                            extjsColumn.DataIndex = column.DataItem;
                            extjsColumn.Header = column.Name;
                        }
                    }
                    else
                    {

                        if (editableColumn.View == EditableGridColumn.GridColumnView.TextBox)
                        {
                            extjsColumn = new Ext.Net.Column();
                            if (editableColumn.Type == typeof(int) || editableColumn.Type == typeof(decimal))
                            {
                                Ext.Net.TextField field = new Ext.Net.TextField();
                                field.Regex = @"^\d+(,\d+)?$";
                                field.RegexText = "0";
                                extjsColumn.Editor.Add(field);
                            }
                            else
                            {                                
                                extjsColumn.Editor.Add(new Ext.Net.TextField());
                            }
                            extjsColumn.DataIndex = column.DataItem;
                            extjsColumn.Header = column.Name;
                            extjsColumn.Editable = true;
                        }
                        else if (editableColumn.View == EditableGridColumn.GridColumnView.DropDown)
                        {
                            extjsColumn = new Ext.Net.Column();
                            extjsColumn.DataIndex = column.DataItem;
                            extjsColumn.Header = column.Name;
                            extjsColumn.Editable = true;

                            StringBuilder renderFunction = new StringBuilder();
                            renderFunction.AppendLine("function(value){");                                                        
                            Ext.Net.ComboBox field = new Ext.Net.ComboBox();
                            field.Editable = false;
                            foreach (Pair<Guid, string> itemDropList in (editableColumn.DataSource.DataSource as List<Pair<Guid, string>>))
                            {
                                field.Items.Add(new Ext.Net.ListItem(itemDropList.Second, itemDropList.First.ToString()));
                                renderFunction.AppendFormat("if (value=='{0}') return '{1}';\n", itemDropList.First.ToString(), itemDropList.Second);
                            }
                            renderFunction.AppendLine("return 'error'; }");

                            extjsColumn.Editor.Add(field);
                            extjsColumn.Renderer.Handler = renderFunction.ToString();
                        }
                        else if (editableColumn.View == EditableGridColumn.GridColumnView.CheckBox)
                        {
                            extjsColumn = new Ext.Net.CheckColumn();
                            extjsColumn.DataIndex = column.DataItem;
                            extjsColumn.Header = column.Name;
                            extjsColumn.Editable = true;
                            extjsColumn.Editor.Add(new Ext.Net.Checkbox());
                        }
                    }
                    
                    if (extjsColumn != null)
                    {
                        // setting visibility column
                        extjsColumn.Hidden = columnWidths.Exists(w => w.ColumnID == column.ID) ? columnWidths.Single(w => w.ColumnID == column.ID).Hidden : false;

                        extjsColumn.ColumnID = column.ID.ToString();
                        foreach (int width in columnWidths.Where(p => p.ColumnID == column.ID).Select(p => p.Width))
                        {
                            extjsColumn.Width = width;
                        }
                        Store1.AddField( new Ext.Net.RecordField(extjsColumn.DataIndex));
                        GridPanel.ColumnModel.Columns.Add(extjsColumn);
                    }
                }
                // дополнительные поля
                Store1.AddField(new Ext.Net.RecordField("ID"));
                Store1.AddField(new Ext.Net.RecordField("CID"));
                Store1.AddField(new Ext.Net.RecordField("ConfID"));
            }
        }

        protected void BindData(Dictionary<Guid, Guid> productsConfigurations)
        {
            using (EditConfigurationProvider provider = new EditConfigurationProvider())
            {
                System.Data.DataTable dt = provider.GetList(ProductID).Tables[0];
                if (productsConfigurations.Count > 0)
                {
                    System.Data.DataTable bfDt = provider.GetAppendList(productsConfigurations).Tables[0];
                    dt.Merge(bfDt);
                }

                // FOR NEW 
                using (EditConfigurationProvider prov = new EditConfigurationProvider())
                {
                    List<object> lst = new List<object>();

                    List<ColumnWidth> columnWidths = provider.ColumnWidths.Where(p => p.ClassificationTreeID == new Guid("11110000-0000-0000-0000-000011110001") && p.UserID == this.User.ID).ToList();
                    List<GridColumn> list = prov.GetGridColumns();
                    list = list.OrderBy(clm => columnWidths.Exists(w => w.ColumnID == clm.ID) ? columnWidths.Single(w => w.ColumnID == clm.ID).Index : Int32.MaxValue).ToList();


                    foreach (DataRow row in dt.Rows)
                    {
                        List<object> lst_sub = new List<object>();
                        foreach (GridColumn column in list)
                        {
                            if (column.GridColumnType == TypeEnum.Boolean)
                            {
                                if (row[column.DataItem].ToString() == "0") lst_sub.Add(false);
                                else lst_sub.Add(true);
                            }
                            else
                            {
                                lst_sub.Add(row[column.DataItem].ToString());
                            }
                        }
                        // дополнительные поля
                        lst_sub.Add(row["ID"].ToString());
                        lst_sub.Add(this.RequestClassificationTreeID.ToString());
                        lst_sub.Add(row["ConfigurationID"].ToString());
                        lst.Add(lst_sub.ToArray());
                    }

                    Store1.DataSource = lst;
                    Store1.DataBind();
                }

                //EditConfigurationGrid.DataSource = dt;
                //EditConfigurationGrid.DataBind();

                // загружаем основание изменений               
                ProductProperty reasonProperty = provider.ProductProperties.SingleOrDefault(
                    pr => pr.PropertyID == new Guid("C266B994-9740-41F6-94DD-07EA5B5FA34A")
                    && pr.ProductID == this.ProductID);

                if (reasonProperty != null)
                {
                    ReasonChanges.Text = reasonProperty.Value;
                }
            }            
        }

        protected void PrintSelected_Click(object sender, EventArgs e)
        {
            string fileName = string.Format("configurations-{0:yyyy-MM-dd_hh-mm-ss}.odt", DateTime.Now);
            
            Response.Clear();
            Response.AppendHeader("Content-Disposition", "attachment; filename="+fileName);
            Response.ContentType = "application/octet-stream";

            TextDocument document = new TextDocument();
            AODL.Document.Content.Tables.Table table;
            document.New();            

            using (EditConfigurationProvider provider = new EditConfigurationProvider())
            {
                List<GridColumn> columns = provider.GetGridColumns();
                DataTable data = provider.GetList(ProductID).Tables[0];

                List<Guid> selectedRows;
                if (!string.IsNullOrEmpty(hiddenSelectedProducts.Value.ToString()))
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
                                
                foreach (DataRow row in data.Rows)
                {
                    Guid rodId = row.Field<Guid>("ConfigurationID");
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
            
            //Merge some cells. Notice this is only available in text documents!
            // table.RowCollection[1].MergeCells(document, 1, 2, true);
            //Add table to the document
            document.Content.Add(table);
            //Save the document            
            string tmpCurrentDitectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Server.MapPath("~/tmp").ToString();
            String fs_guid = Server.MapPath(String.Format("~/tmp/{0}.odt", Guid.NewGuid().ToString()));
            document.SaveTo(fs_guid);
            document.Dispose();
            Environment.CurrentDirectory = tmpCurrentDitectory;

            // Copy file to Response stream
            FileStream fs = new FileStream(fs_guid, FileMode.Open);                        
            
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

        protected List<Guid> AllRecords
        {
            get
            {
                if (this.ViewState["AllRecords"] == null)
                {
                    this.ViewState["AllRecords"] = new List<Guid>();
                }
                return this.ViewState["AllRecords"] as List<Guid>;
            }
            private set
            {
                this.ViewState["AllRecords"] = value;
            }
        }

       /* protected void EditConfigurationGrid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                {
                    //e.Row.ID = string.Format("{0}_Row{1}", EditConfigurationGrid.ClientID, e.Row.RowIndex);

                    string pid = DataBinder.Eval(e.Row.DataItem, "ConfigurationID").ToString();
                    if (!Guid.Empty.Equals(new Guid(pid)))
                    {
                        AllRecords.Add(new Guid(pid));
                    }

                    string prodid = DataBinder.Eval(e.Row.DataItem, Common.IDColumnTitle).ToString();
                    string cid = this.RequestClassificationTreeID.ToString();
                    string function = string.Format("onGridViewRowSelectedCallback('{0}','{1}', this, '{2}');", prodid, cid, EditConfigurationGrid.Controls[0].ClientID);
                    e.Row.Attributes.Add("onclick", function);
                    //string function = string.Format("onGridViewRowSelected(this, '{0}');", EditConfigurationGrid.Controls[0].ClientID);
                    //e.Row.Attributes.Add("onclick", function);

                    if (e.Row.RowState == DataControlRowState.Alternate) e.Row.CssClass = "row2";
                    else e.Row.CssClass = string.Empty;

                    e.Row.Attributes["onmouseover"] = "highLightRow(this)";
                    e.Row.Attributes["onmouseout"] = "unHighLightRow(this)";
                    CheckBox chk = e.Row.Cells[1].FindControl("SelectCheckBox") as CheckBox;
                    chk.Attributes.Add("onclick", String.Format("selectProduct(event,this,'{0}','{1}');", pid, SelectedProductsHidden.ClientID));
                    chk.Checked = SelectedProductsHidden.Value.Contains(pid);

                    // Хак для перемещения курсора на строку, при клике на ссылку
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        foreach (Control control in cell.Controls)
                        {
                            if (control is HyperLink)
                            {
                                function = string.Format("onGridViewRowSelectedCallback('{0}','{1}', this.parentNode.parentNode, '{2}');", prodid, cid, EditConfigurationGrid.Controls[0].ClientID);
                                (control as HyperLink).Attributes.Add("onclick", function);
                            }
                        }
                    }
                }
            }
        }*/

        protected void AddFromBuffer_Click(object sender, EventArgs e)
        {
            Save_Click(sender, e);
            if (LabelErrorMessage.Text.Length > 0)
            {
                LabelErrorMessage.Text += " Вставка из буфера отменена.";
                return;
            }
            if (this.MultiBuffer.Count > 0)
            {
                this.BindData(this.MultiBuffer);
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            LabelErrorMessage.Text = "";
            BindGridColumns2();
            this.BindData(new Dictionary<Guid, Guid>());
        }

        protected void Delete_Command(object sender, CommandEventArgs e)
        {
            EditConfigurationGrid.DeleteRow(Convert.ToInt32(e.CommandArgument));
        }

        protected bool fnCheckContains(Guid dictNomenID, Guid whereProduct)
        {
            using (CommonDomain domain = new CommonDomain())
            {                
                List<Aspect.Domain.Configuration> confs;
                confs = (from conf in domain.Configurations
                        where conf.ProductOwnerID == whereProduct
                        select conf).ToList();
                foreach (Aspect.Domain.Configuration conf in confs)
                {
                    if (conf.Product._dictNomenID == dictNomenID) return true;
                    if (fnCheckContains(dictNomenID, conf.ProductID)) return true;
                }
                return false;
            }
            
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            this.Validate();
            if (this.IsValid)
            {
                // Проверяем, является ли продукт основной версией
                ProductProvider productProvider = new ProductProvider();
                if (productProvider.IsMainVersion(this.ProductID))
                {
                    LabelErrorMessage.Text = "[ ! ] Содержащий данную спецификацию продукт является основной версией. Сохранение отменено.";
                    return;
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<Dictionary<string, string>> rows = js.Deserialize<List<Dictionary<string, string>>>(hiddenStoreData.Value.ToString());
                EditConfigurationProvider provider = new EditConfigurationProvider();
                List<GridColumn> gridColumns = provider.GetGridColumns();

                // Сохраняем данные, полученные от пользователя в списке конфигураций
                List<Aspect.Domain.Configuration> result = new List<Aspect.Domain.Configuration>();                
                #region convert Request to list of Configuration
                foreach (Dictionary<string, string> row in rows)
                {
                    Guid productID = new Guid(row["ID"]);
                    Aspect.Domain.Configuration conf = new Aspect.Domain.Configuration();
                    conf.ID = new Guid(row["ConfID"]);
                    conf.ProductID = productID;
                    conf.ProductOwnerID = this.ProductID;
                    conf.UserID = this.User.ID;

                    foreach (GridColumn column in gridColumns)
                    {
                        if (column is EditableGridColumn)
                        {
                            System.Reflection.PropertyInfo prop = typeof(Aspect.Domain.Configuration).GetProperty(column.DataItem);
                            if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(Nullable<decimal>))
                            {
                                prop.SetValue(conf, Convert.ToDecimal(row[column.DataItem]), null);
                            }
                            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(Nullable<int>))
                            {
                                prop.SetValue(conf, Convert.ToInt32(row[column.DataItem]), null);
                            }
                            else if (prop.PropertyType == typeof(Guid) || prop.PropertyType == typeof(Nullable<Guid>))
                            {
                                prop.SetValue(conf, new Guid(row[column.DataItem]), null);
                            }
                            else if (prop.PropertyType == typeof(Boolean) || prop.PropertyType == typeof(Nullable<Boolean>))
                            {                                
                                prop.SetValue(conf, Convert.ToBoolean(row[column.DataItem]), null);
                            }
                            else
                            {
                                prop.SetValue(conf, row[column.DataItem], null);
                            }
                        }
                    }

                    result.Add(conf);
                }
                #endregion

                using (CommonDomain domain = new CommonDomain())
                {
                    // Проверка на включение материалов
                    #region check_including_material                    
                    foreach (Aspect.Domain.Configuration conf in result)
                    {
                        Product prod = domain.Products.Single(p => p.ID == conf.ProductID);
                        if (prod._dictNomen.cod >= 1000000)
                        {                        
                            LabelErrorMessage.Text = "[ ! ] Обнаружены материалы в спецификации. Сохранение отменино.";
                            return;
                        }
                    }
                    #endregion

                    // Проверка на циклы
                    #region check_for_cycles                    
                    Guid dictNomenID = (Guid) (from p in domain.Products
                                               where p.ID == this.ProductID
                                               select p).Single()._dictNomenID;
                    foreach (Aspect.Domain.Configuration conf in result)
                    {
                        Product prod = domain.Products.Where(p => p.ID == conf.ProductID).Single();
                        if (dictNomenID == prod._dictNomenID)
                        {
                            LabelErrorMessage.Text = "[ ! ] Обнаружены циклические включения продуктов в спецификацию. Сохранение отменино.";
                            return;
                        }
                        if (fnCheckContains(dictNomenID, prod.ID))
                        {
                            LabelErrorMessage.Text = "[ ! ] Обнаружены циклические включения продуктов в спецификацию. Сохранение отменино.";
                            return;
                        }
                    }
                    #endregion
                }                             
                
                provider.SaveProductConfiguration(this.ProductID, result, this.User.ID);

                // установка признака "Основная версия"
                if (MadeBasicVersion.Checked)
                {
                    productProvider.SetMainVersion(this.User.ID, new List<Guid>{ProductID});                    
                }

                // устанавливаем основание изменений
                if (!String.IsNullOrEmpty(ReasonChanges.Text))
                {
                    ProductProperty reasonProperty = productProvider.ProductProperties.SingleOrDefault(
                        pr => pr.PropertyID == new Guid("C266B994-9740-41F6-94DD-07EA5B5FA34A")
                        && pr.ProductID == this.ProductID);

                    if (reasonProperty == null)
                    {
                        reasonProperty = new ProductProperty()
                        {
                            ID = Guid.NewGuid(),
                            PropertyID = new Guid("C266B994-9740-41F6-94DD-07EA5B5FA34A"),
                            ProductID = this.ProductID,
                            Value = ReasonChanges.Text
                        };
                        productProvider.ProductProperties.InsertOnSubmit(reasonProperty);
                    }
                    else
                    {
                        reasonProperty.Value = ReasonChanges.Text;
                    }
                    productProvider.SubmitChanges();
                }            
    
                // Очищаем сообщение об ошибке и обновляем данные о спецификации
                LabelErrorMessage.Text = "";
                this.BindGridColumns2();
                this.BindData(new Dictionary<Guid, Guid>());
            }
        }

        [Ext.Net.DirectMethod]
        public void OnColumnResize(Guid columnId, int newSize)
        {
            using (ContentDomain provider = Common.GetContentDomain(ClassifiacationTypeView.Standard))
            {
                List<ColumnWidth> columnWidths = provider.ColumnWidths.Where(p => p.ClassificationTreeID == new Guid("11110000-0000-0000-0000-000011110001") && p.UserID == this.User.ID).ToList();

                if (columnWidths.Count(p => p.ColumnID == columnId) > 0)
                {
                    ColumnWidth width = columnWidths.Single(p => p.ColumnID == columnId);
                    width.Width = newSize;                    
                }
                else
                {
                    provider.ColumnWidths.InsertOnSubmit(new ColumnWidth()
                    {
                        ID = Guid.NewGuid(),
                        ClassificationTreeID = new Guid("11110000-0000-0000-0000-000011110001"),
                        ColumnID = columnId,
                        UserID = User.ID,
                        Width = newSize,
                        Hidden = false,
                        Index = Int32.MaxValue
                    });
                }

                provider.SubmitChanges();
            }
        }

        [Ext.Net.DirectMethod]
        public void OnColumnHiddenChange(Guid columnId, bool hiddenStatus)
        {
            using (ContentDomain provider = Common.GetContentDomain(ClassifiacationTypeView.Standard))
            {
                List<ColumnWidth> columnWidths = provider.ColumnWidths.Where(p => p.ClassificationTreeID == new Guid("11110000-0000-0000-0000-000011110001") && p.UserID == this.User.ID).ToList();

                if (columnWidths.Exists(clm => clm.ColumnID == columnId))
                {
                    ColumnWidth width = columnWidths.Single(clm => clm.ColumnID == columnId);
                    width.Hidden = hiddenStatus;                    
                } else {
                    provider.ColumnWidths.InsertOnSubmit(new ColumnWidth()
                    {
                        ID = Guid.NewGuid(),
                        ClassificationTreeID = new Guid("11110000-0000-0000-0000-000011110001"),
                        ColumnID = columnId,
                        UserID = User.ID,
                        Width = 50,
                        Hidden = false,
                        Index = Int32.MaxValue
                    });
                    
                }
                provider.SubmitChanges();
            }
        }

        [Ext.Net.DirectMethod]
        public void OnColumnMoved(string columns)
        {
            using (ContentDomain provider = Common.GetContentDomain(ClassifiacationTypeView.Standard))
            {
                List<Guid> listColumns = columns.Split(',').Select(i => new Guid(i)).ToList();                
                
                List<ColumnWidth> columnWidths = provider.ColumnWidths.Where(p => p.ClassificationTreeID == new Guid("11110000-0000-0000-0000-000011110001") && p.UserID == this.User.ID).ToList();
                foreach (var columnId in listColumns)
                {
                    if (columnWidths.Exists(clm => clm.ColumnID == columnId))
                    {
                        // update column index
                        ColumnWidth width = columnWidths.Single(clm => clm.ColumnID == columnId);
                        width.Index = listColumns.IndexOf(columnId) + 3;                        
                    } else {
                        // create column width record
                        provider.ColumnWidths.InsertOnSubmit(new ColumnWidth()
                        {
                            ID = Guid.NewGuid(),
                            ClassificationTreeID = new Guid("11110000-0000-0000-0000-000011110001"),
                            ColumnID = columnId,
                            UserID = User.ID,
                            Width = 50,
                            Hidden = false,
                            Index = listColumns.IndexOf(columnId) + 3
                        });
                        
                    }
                }
                provider.SubmitChanges();
            }
        }

        /*protected void EditConfigurationGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            EditConfigurationGrid.Rows[e.RowIndex].Visible = false;
        }*/
    }
}
