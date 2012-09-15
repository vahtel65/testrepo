using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.Web.UI.WebControls;
using Aspect.UI.Web.Basic;

using Aspect.Model.DictionaryDomain;
//using Aspect.Model.ProductDomain;
using Aspect.Domain;


namespace Aspect.UI.Web.Editarea
{
    public partial class ViewDict : Basic.PageBase
    {
        protected Guid newDictionaryEntityID = Guid.Empty;
        protected Guid RequestDictionaryEntityID
        {
            get
            {
                if (newDictionaryEntityID != Guid.Empty && this.IsNew)
                {
                    return newDictionaryEntityID;
                }
                try
                {
                    return new Guid(this.Request["ID"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }
        protected Guid DictionaryTreeID
        {
            get
            {
                try
                {
                    return new Guid(this.Request["DictID"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }
        protected bool IsNew
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this.Request["new"]);
                }
                catch
                {
                    return false;
                }
            }
        }

        protected Repeater GeneralPropertyRepeater;
        protected Repeater DictionaryPropertyRepeater;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.DataLoad();
            }
        }

        protected ITextControl HeaderLiteral;
        protected ITextControl HeaderDateLiteral;
        private void DataLoad()
        {
            using (DictionaryProvider domain = new DictionaryProvider())
            {
                object val = domain.GetProductDictionaryText(this.RequestDictionaryEntityID, this.DictionaryTreeID);
                //Aspect.Domain.Product prod = domain.GetProduct(this.RequestDictionaryEntityID);

                //if (prod == null) return;
                this.Title = HeaderLiteral.Text = string.Format("Редактирование {0}", val.ToString());
                if (this.IsNew) this.Title = HeaderLiteral.Text = string.Format("Добавление по аналогу {0}", val.ToString());
                HeaderDateLiteral.Text = string.Format(HeaderDateLiteral.Text, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());

                BindGeneralPropertyList();
                BindDictionaryList();
            }
        }

        protected List<Pair<string, string>> NotSavedProperties
        {
            get
            {
                if (ViewState["NotSavedProperties"] == null)
                {
                    ViewState["NotSavedProperties"] = new List<Pair<string, string>>();
                }
                return (List<Pair<string, string>>)ViewState["NotSavedProperties"];
            }
            set
            {
                ViewState["NotSavedProperties"] = value;
            }
        }

        private void BindGeneralPropertyList()
        {
            using (DictionaryProvider provider = new DictionaryProvider())
            {
                //--- general property-------
                List<DictionaryProperty> props = provider.GetAvailableDictionaryPropertiesToModify(this.DictionaryTreeID, this.Roles);


                List<Pair<DictionaryProperty, object>> source = new List<Pair<DictionaryProperty, object>>();
                foreach (DictionaryProperty item in props)
                {
                    object obj = provider.GetDictionaryItemValue(this.DictionaryTreeID, this.RequestDictionaryEntityID, item.ColumnName);
                    Pair<DictionaryProperty, object> entity = new Pair<DictionaryProperty, object>(item, obj);
                    source.Add(entity);
                }
                GeneralPropertyRepeater.DataSource = source;//props;
                GeneralPropertyRepeater.DataBind();

                NotSavedProperties = new List<Pair<string, string>>();
            }
        }

        protected void GeneralPropertyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string col_name = (e.Item.DataItem as Pair<DictionaryProperty, object>).First.ColumnName;
                Pair<string, string> property = NotSavedProperties.SingleOrDefault(n => n.First == col_name);
                if (property != null)
                {
                    (e.Item.FindControl("PropertyValidator") as CustomValidator).IsValid = false;
                    (e.Item.FindControl("PropertyValue") as TextBox).Text = property.Second;
                }
            }
        }

        private void BindDictionaryList()
        {
            using (DictionaryProvider provider = new DictionaryProvider())
            {
                List<DictionaryTree> list = provider.GetDictionaryTreeList(this.DictionaryTreeID, this.Roles);

                List<DictionaryDDLSource> source = new List<DictionaryDDLSource>();
                foreach (DictionaryTree item in list)
                {
                    DictionaryDDLSource entity = new DictionaryDDLSource();
                    entity.DictionaryTreeID = item.ID;
                    //set dictionary property title
                    entity.Title = item.Name; //string.Format("{0} ({1})", item.First.Name, item.Second);
                    //get product dicitonary value
                    object value = provider.GetDictionaryItemValue(this.DictionaryTreeID, this.RequestDictionaryEntityID, item.FK);

                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        entity.ID = new Guid(value.ToString());//entity.SelectedValue = value.ToString();
                        string valueText = provider.GetProductDictionaryText(entity.ID, item).ToString();
                        if (!string.IsNullOrEmpty(valueText) && valueText.Trim().Length > 0) entity.ValueText = valueText;
                    }

                    source.Add(entity);
                }
                DictionaryPropertyRepeater.DataSource = source;
                DictionaryPropertyRepeater.DataBind();
            }
        }
    }
}
