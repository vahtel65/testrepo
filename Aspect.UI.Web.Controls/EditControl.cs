using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using Aspect.Domain;

namespace Aspect.UI.Web.Controls
{
    public class EditControl : CompositeControl, INamingContainer
    {
        public EditControl()
        {
        }

        public TypeEnum ControlType
        {
            get
            {
                return (TypeEnum)Enum.Parse(typeof(TypeEnum), ViewState["ControlType"].ToString());
            }
            set
            {
                ViewState["ControlType"] = value.ToString();
            }
        }

        public string Title
        {
            get
            {
                if (ViewState["Title"] == null) ViewState["Title"] = string.Empty;
                return ViewState["Title"].ToString();
            }
            set
            {
                ViewState["Title"] = value;
            }
        }

        public TextBox StringValueControl;

        public CheckBox CheckBoxValueControl;

        //public Calendar CalendarValueControl;
        //public AjaxControlToolkit.CalendarExtender CalendarValueControlAdv;
        //public TextBox CalendarValueControlAdvTextBox;
        public CalendarControl CalendareValueControl2;

        public CustomValidator ValueValidator;

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            ValueValidator = new CustomValidator();
            ValueValidator.ID = "valueValidator";
            ValueValidator.Text = "!";
            ValueValidator.ErrorMessage = String.Format("Неверный формат поля: {0}", this.Title);
            //ValueValidator.ValidationGroup = "EditValidationGroup";
            //ValueValidator.Display = ValidatorDisplay.Dynamic;

            Control valueControl = null;
            switch (ControlType)
            {
                case TypeEnum.Boolean:
                    CheckBoxValueControl = new CheckBox();
                    valueControl = CheckBoxValueControl;
                    break;
                case TypeEnum.Datetime:
                    CalendareValueControl2 = new CalendarControl();
                    valueControl = CalendareValueControl2;
                    //ValueValidator.ControlToValidate = CalendareValueControl2.TextBoxID;
                    break;
                case TypeEnum.Decimal:
                case TypeEnum.Integer:
                case TypeEnum.Default:
                default:
                    StringValueControl = new TextBox();
                    valueControl = StringValueControl;
                    valueControl.ID = "editControlValueField";
                    ValueValidator.ControlToValidate = valueControl.ID;
                    break;
            }
            this.Controls.Add(valueControl);
            this.Controls.Add(ValueValidator);
        }
        public bool IsEmpty
        {
            get{
            switch (ControlType)
                {
                    case TypeEnum.Boolean:
                        return false;
                    case TypeEnum.Datetime:
                        return CalendareValueControl2.SelectedDate == DateTime.MinValue;//.ToString("dd.MM.yyyy");
                    case TypeEnum.Integer:
                    case TypeEnum.Decimal:
                    case TypeEnum.Default:
                    default:
                        return StringValueControl.Text.Length == 0;
                }
            }
        }
        public object Value
        {
            get
            {
                switch (ControlType)
                {
                    case TypeEnum.Boolean:
                        return CheckBoxValueControl.Checked ? "1" : "0";
                    case TypeEnum.Datetime:
                        
                        return CalendareValueControl2.SelectedDate.ToString("dd.MM.yyyy");
                        
                        //return CalendarValueControl.SelectedDate.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    case TypeEnum.Integer:
                        return Int32.Parse(StringValueControl.Text);
                        //return Convert.ToInt32(StringValueControl.Text);
                    case TypeEnum.Decimal:
                        return Decimal.Parse(StringValueControl.Text);
                        //return Convert.ToDecimal(StringValueControl.Text);
                    case TypeEnum.Default:
                    default:
                        return StringValueControl.Text;
                }
            }
            set
            {
                this.EnsureChildControls();
                if (value == null || value == DBNull.Value) return;
                switch (ControlType)
                {
                    case TypeEnum.Boolean:

                        CheckBoxValueControl.Checked = Convert.ToBoolean(Convert.ToInt32(value));
                        break;
                    case TypeEnum.Datetime:
                        //CalendareValueControl2.SelectedDate = DateTime.ParseExact(value.ToString(), "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        if(!string.IsNullOrEmpty(value.ToString())) CalendareValueControl2.SelectedDate = Convert.ToDateTime(value, System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case TypeEnum.Decimal:
                        if (!string.IsNullOrEmpty(value.ToString())) StringValueControl.Text = Decimal.Parse(value.ToString()).ToString();
                        break;
                    case TypeEnum.Integer:
                    case TypeEnum.Default:
                    default:
                        StringValueControl.Text = value.ToString();
                        break;
                }
            }
        }

        public void Clear()
        {
            switch (ControlType)
            {
                case TypeEnum.Boolean:
                    CheckBoxValueControl.Checked = false;
                    break;
                case TypeEnum.Datetime:
                    CalendareValueControl2.Clear();
                    break;
                case TypeEnum.Integer:
                case TypeEnum.Decimal:
                case TypeEnum.Default:
                default:
                    StringValueControl.Text = string.Empty;
                    break;
            }
        }

        public bool Validate()
        {
            try
            {
                object m = this.Value;
                /*switch (ControlType)
                {
                    case TypeEnum.Boolean:
                        Convert.ToBoolean(this.Value);
                        break;
                    case TypeEnum.Datetime:
                        Convert.ToDateTime(this.Value);
                        break;
                    case TypeEnum.Integer:
                        Convert.ToInt32(this.Value);
                        break;
                    case TypeEnum.Decimal:
                        Convert.ToDecimal(this.Value);
                        break;
                    case TypeEnum.Default:
                    default:
                        break;
                }*/
            }
            catch (Exception)
            {
                ValueValidator.IsValid = false;
                return false;
            }
            ValueValidator.IsValid = true;
            return true;
        }

        public bool IsValid
        {
            get
            {
                //this.EnsureChildControls();
                return ValueValidator.IsValid;
            }
        }
    }
}

