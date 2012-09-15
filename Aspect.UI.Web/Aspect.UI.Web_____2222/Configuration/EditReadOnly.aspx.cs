using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Aspect.Model.ConfigurationDomain;
using Aspect.Domain;
using Aspect.Model;
using Aspect.UI.Web.Controls;

using AODL.Document.TextDocuments;
using AODL.Document.Content.Tables;
using AODL.Document.Content.Text;

namespace Aspect.UI.Web.Configuration
{
    public partial class EditReadOnly : Basic.PageBase
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

        protected Guid RequestClassificationTreeID
        {
            get
            {
                return Aspect.Domain.FormGridView.ConfigurationView;
            }
        }

        protected System.Web.UI.HtmlControls.HtmlAnchor ShowBuffer;
        protected System.Web.UI.HtmlControls.HtmlAnchor SummaryWeight;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.PopupIframeInitializationString(ShowBuffer, "Буфер", "../Popup/Buffer.aspx", 700, 500);
                this.PopupIframeInitializationString(SummaryWeight, "Суммарный вес разузлованного состава", string.Format("../Popup/SummaryWeight.aspx?pid='+selectedProductID+'", SelectedProductsHidden.Value), 700, 500);
                this.BindGridColumns();
                this.BindData(new Dictionary<Guid, Guid>());
            }
            this.Title = HeaderLiteral.Text;
        }
        protected ITextControl HeaderLiteral;
        protected ITextControl HeaderDateLiteral;
        protected GridView EditConfigurationGrid;

        private void BindGridColumns()
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
                            field = new DropDownProductGridField(editableColumn/*, editableColumn.DataSource.DataSource*/);
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
        }

        protected void BindData(Dictionary<Guid, Guid> multiBuffer)
        {
            using (EditConfigurationProvider provider = new EditConfigurationProvider())
            {
                System.Data.DataTable dt = provider.GetList(ProductID).Tables[0];
                if (multiBuffer.Count > 0)
                {
                    System.Data.DataTable bfDt = provider.GetAppendList(multiBuffer).Tables[0];
                    dt.Merge(bfDt);
                }
                EditConfigurationGrid.DataSource = dt;
                EditConfigurationGrid.DataBind();
            }
        }

        protected void PrintSelected_Click(object sender, EventArgs e)
        {
            string fileName = string.Format("configurations-{0:yyyy-MM-dd_hh-mm-ss}.odt", DateTime.Now);

            Response.Clear();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/octet-stream";

            TextDocument document = new TextDocument();
            AODL.Document.Content.Tables.Table table;
            document.New();

            using (EditConfigurationProvider provider = new EditConfigurationProvider())
            {
                List<GridColumn> columns = provider.GetGridColumns();
                DataTable data = provider.GetList(ProductID).Tables[0];

                List<Guid> selectedRows;
                if (SelectedProductsHidden.Value != "")
                    selectedRows = SelectedProductsHidden.Value.Split(',').Select(s => new Guid(s)).ToList();
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
            String fs_guid = Server.MapPath(String.Format("/tmp/{0}.odt", Guid.NewGuid().ToString()));
            document.SaveTo(fs_guid);
            document.Dispose();

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
        
        protected LinkButton SelectAllRecords;

        /// <summary>
        /// Выделение всех записей в списке продуктов,
        /// входящих в спецификацию
        /// </summary>
        protected void SelectAll_Click(object sender, EventArgs e)
        {
            SelectedProductsHidden.Value = string.Join(",", AllRecords.Select(a => a.ToString()).ToArray());
            this.BindData(new Dictionary<Guid, Guid>());
        }

        /// <summary>
        /// Отмена выделения всех записей в списке продуктов,
        /// входяших в спецификацию
        /// </summary>
        protected void UnselectAll_Click(object sender, EventArgs e)
        {
            SelectedProductsHidden.Value = string.Empty;
            this.BindData(new Dictionary<Guid, Guid>());
        }

        protected HiddenField SelectedProductsHidden;
        protected void EditConfigurationGrid_RowCreated(object sender, GridViewRowEventArgs e)
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
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            this.BindData(new Dictionary<Guid, Guid>());
        }
    }
}
