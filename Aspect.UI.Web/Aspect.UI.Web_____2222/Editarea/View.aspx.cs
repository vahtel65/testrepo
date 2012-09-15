using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.Web.UI.WebControls;
using Aspect.UI.Web.Basic;

using Aspect.Model.DictionaryDomain;
using Aspect.Model.ProductDomain;
using Aspect.Domain;


namespace Aspect.UI.Web.Editarea
{
    public partial class View : Basic.PageBase
    {
        protected Guid newProductID = Guid.Empty;
        protected Guid RequestProductID
        {
            get
            {
                if (newProductID != Guid.Empty && this.IsNew)
                {
                    return newProductID;
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
            List<ITreeNode> parents = new List<ITreeNode>();
            using (CommonDomain domain = new CommonDomain())
            {
                parents = domain.GetProductParents(this.RequestProductID);

                Aspect.Domain.Product prod = domain.GetProduct(this.RequestProductID);

                if (prod == null) return;
                this.Title = HeaderLiteral.Text = string.Format("Редактирование {0} Версия {1}", prod.PublicName, prod.Version == null ? string.Empty : prod.Version.ToString());
                if (this.IsNew) this.Title = HeaderLiteral.Text = string.Format("Добавление по аналогу {0} Версия {1}", prod.PublicName, prod.Version == null ? string.Empty : prod.Version.ToString());
                HeaderDateLiteral.Text = string.Format(HeaderDateLiteral.Text, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());

                BindGeneralPropertyList(parents);
                BindDictionaryList(parents);
            }
        }

        private void BindGeneralPropertyList(List<ITreeNode> parents)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                //--- general property-------
                List<Property> props = provider.GetAvailablePropertiesToModify(this.Roles);

                var q = from s in props.Where(f => f.ClassificationID.HasValue)
                        join l in parents on s.ClassificationID.Value equals l.ID
                        select new Property()
                        {
                            ID = s.ID,
                            Type = s.Type,
                            Name = string.Format("{0} ({1})", s.Name, l.Name)
                        };
                props = q.ToList();

                List<ProductProperty> values = provider.ProductProperties.Where(pp => pp.ProductID == this.RequestProductID).ToList();
                List<Pair<Property, ProductProperty>> source = new List<Pair<Property, ProductProperty>>();
                foreach (Property item in props)
                {
                    ProductProperty val = values.SingleOrDefault(pp => pp.PropertyID == item.ID);
                    Pair<Property, ProductProperty> entity = new Pair<Property, ProductProperty>(item, val);
                    source.Add(entity);
                }

                GeneralPropertyRepeater.DataSource = source;//props;
                GeneralPropertyRepeater.DataBind();


            }
        }

        private void BindDictionaryList(List<ITreeNode> parents)
        {
            BindDictionaryList(parents, false);
        }
        private void BindDictionaryList(List<ITreeNode> parents, bool isEmpty)
        {
            using (DictionaryProvider provider = new DictionaryProvider())
            {
                //--- dictionary property -----
                Product product = provider.Products.SingleOrDefault(p => p.ID == this.RequestProductID);
                if (product == null) this.RedirectToErrorPage();

                List<DictionaryTree> list = provider.GetDictionaryTreeList(Guid.Empty, this.Roles);
                var q = from l in list.Where(f => f.ClassificationID.HasValue)
                        join m in parents on l.ClassificationID.Value equals m.ID
                        select new Pair<DictionaryTree, string>(l, m.Name);

                List<DictionaryDDLSource> source = new List<DictionaryDDLSource>();
                foreach (Pair<DictionaryTree, string> item in q.ToList())
                {
                    DictionaryDDLSource entity = new DictionaryDDLSource();
                    entity.DictionaryTreeID = item.First.ID;
                    //set dictionary property title
                    entity.Title = string.Format("{0} ({1})", item.First.Name, item.Second);
                    //get product dicitonary value
                    object value = provider.GetProductDicitonaryValue(this.RequestProductID, item.First.FK);

                    if (value != null && !string.IsNullOrEmpty(value.ToString()) && !isEmpty)
                    {
                        entity.ID = new Guid(value.ToString());//entity.SelectedValue = value.ToString();
                        string valueText = provider.GetProductDictionaryText(entity.ID, item.First).ToString();
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
