using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Controls
{
    public class TextBoxProductGridField : EditableProductGridField
    {
        public TextBoxProductGridField()
            : base()
        {
        }



        public TextBoxProductGridField(Aspect.Domain.EditableGridColumn gridColumn)
            : base(gridColumn)
        {
        }
        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            //base.InitializeDataCell(cell, rowState);
            TextBox textBox = new TextBox();
            textBox.ID = this.ControlID;
            textBox.Width = new Unit(GridColumn.Width, UnitType.Pixel);
            if (GridColumn.Size != 0) textBox.MaxLength = GridColumn.Size;
            textBox.DataBinding += new EventHandler(this.textBox_DataBinding);
            base.InitializeDataCell(cell, rowState);
            cell.Controls.Add(textBox);
            CompareValidator vld = new CompareValidator();
            vld.ControlToValidate = textBox.ID;
            vld.ID = textBox.ID + "vld";
            vld.Operator = ValidationCompareOperator.DataTypeCheck;
            vld.ErrorMessage = "не верный формат (2)";
            vld.Text = "! (2)";
            vld.Display = ValidatorDisplay.Dynamic;
            if (GridColumn.Type == typeof(int)) vld.Type = ValidationDataType.Integer;
            if (GridColumn.Type == typeof(decimal)) vld.Type = ValidationDataType.Double;
            if (GridColumn.Type == typeof(string)) vld.Type = ValidationDataType.String;
            cell.Controls.Add(vld);
            if (!GridColumn.AllowNULL)
            {
                RequiredFieldValidator reqvld = new RequiredFieldValidator();
                reqvld.ControlToValidate = textBox.ID;
                reqvld.ID = textBox.ID + "reqvld";
                reqvld.ErrorMessage = "поле не может быть пустым (1)";
                reqvld.Text = "! (1)";
                reqvld.Display = ValidatorDisplay.Dynamic;
                cell.Controls.Add(reqvld);
            }
        }

        private void textBox_DataBinding(Object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            GridViewRow row = (GridViewRow)t.NamingContainer;
            System.Data.DataRowView data = row.DataItem as System.Data.DataRowView;
            t.Text = data[this.DataField].ToString();
        }

        internal override object GetValue(Control control)
        {
            TextBox textBox = control as TextBox;
            if (textBox != null && !String.IsNullOrEmpty(textBox.Text))
            {
                try
                {
                    if (GridColumn.Type == typeof(int)) return Convert.ToInt32(textBox.Text);
                    else if (GridColumn.Type == typeof(decimal)) return Convert.ToDecimal(textBox.Text);
                    else if (GridColumn.Size != 0 && textBox.Text.Length > GridColumn.Size) return textBox.Text.Substring(0, this.GridColumn.Size);
                    else return textBox.Text;
                }
                catch (Exception ex)
                {
                    //CompareValidator vld = control.Parent.FindControl("") as CompareValidator;
                    //vld.IsValid = false;
                    throw ex;
                }
            }
            return null;
        }
    }
}
