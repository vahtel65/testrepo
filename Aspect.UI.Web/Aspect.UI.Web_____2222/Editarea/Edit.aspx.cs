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
using Aspect.UI.Web.Controls;
using System.Globalization;

namespace Aspect.UI.Web.Editarea
{
    public partial class Edit : Basic.PageBase
    {
        protected Guid newProductID = Guid.Empty;
        protected ITextControl HeaderLiteral;
        protected ITextControl HeaderDateLiteral;
        protected Repeater GeneralPropertyRepeater;
        protected Repeater DictionaryPropertyRepeater;

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

        protected bool IsWithConfs
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this.Request["withconfs"]);
                }
                catch
                {
                    return false;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.DataLoad();
            }
        }
        

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
                    // для добавления по аналогу сбрасываем галочку "Основная версия"
                    if (this.IsNew && item.ID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552"))
                    {
                        val.Value = "0";
                    }
                    // для "Не новых" продуктов нельзя менять основание изменений
                    // if (!this.IsNew && item.ID == new Guid("c266b994-9740-41f6-94dd-07ea5b5fa34a")) continue;
                    
                    // для "Новых" продуктов наращиваем номер версии
                    if (this.IsNew && item.ID == new Guid("0789DB1A-9BAA-4574-B405-AE570C746C03"))
                    {
                        int maxVersion = provider.getMaxVersionByProductID(this.RequestProductID) + 1;
                        val.Value = maxVersion.ToString();
                    }

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
                        if(!string.IsNullOrEmpty(valueText) && valueText.Trim().Length > 0) entity.ValueText = valueText;
                    }

                    source.Add(entity);
                }
                DictionaryPropertyRepeater.DataSource = source;
                DictionaryPropertyRepeater.DataBind();
            }
        }

        protected void GeneralPropertyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HiddenField hiddenID = e.Item.FindControl("HiddenID") as HiddenField;                

                // если это поле Версия (МатРесурсы) запрещаем его редактирование
                if (new Guid(hiddenID.Value) == new Guid("0789db1a-9baa-4574-b405-ae570c746c03"))
                {
                    EditControl control = e.Item.FindControl("PropertyValueAdv") as EditControl;
                    control.Enabled = false;
                }

                // Вес по приказу
                if (new Guid(hiddenID.Value) == new Guid("ac37f816-e4c1-4751-99ed-6180d7cca142"))
                {
                    EditControl control = e.Item.FindControl("PropertyValueAdv") as EditControl;
                    control.CssClass = "valueForWeight " + control.CssClass;
                }                
            }
        }

        protected void DictionaryPropertyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HiddenField valueField = e.Item.FindControl("HiddenID") as HiddenField;
                string value = valueField.Value;
                string valueCtrlID = valueField.ClientID;
                string treeID = (e.Item.FindControl("HiddenTreeID") as HiddenField).Value;                

                HyperLink control = e.Item.FindControl("PropertyValueSelector") as HyperLink;
                this.PopupIframeInitializationStringWithProduct(control, "", string.Format("../Popup/Selector.aspx?ID={0}&ctrlID={1}&treeID={2}&textCtrlID={3}", value, valueCtrlID, treeID, control.ClientID), 800, 500);

                // Hack for handle to change _nomedID
                if (/*this.IsNew &&*/ new Guid(treeID) == new Guid ("316C6BC7-D883-44C8-AAE0-602F49C73595"))// номенклатура
                {
                    control.CssClass = "selectorForNomen " + control.CssClass;
                }                                

                HyperLink deleteLink = e.Item.FindControl("DeleteLink") as HyperLink;
                deleteLink.Attributes.Add("onclick", string.Format("setSelectedValue('{0}','{1}','{2}','{3}');return false;", valueCtrlID, Guid.Empty.ToString(), control.ClientID, "Выбрать"));
                //string.Format("document.getElementById('{0}').value = '';document.getElementById('{1}').innerHTML = 'Выбрать';", valueCtrlID,control.ClientID);
            }
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in GeneralPropertyRepeater.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    (item.FindControl("PropertyValueAdv") as EditControl).Clear();
                }
            }
            /*using (CommonDomain domain = new CommonDomain())
            {
                List<ITreeNode> parents = domain.GetProductParents(this.RequestProductID);
                BindDictionaryList(parents, true);
            }*/
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            //if (this.IsNew)
            //{
                string version = null;
                string nomenValue = null;
                CustomValidator validator1 = null;
                CustomValidator validator2 = null;
                #region Get the @version
                foreach (RepeaterItem item in GeneralPropertyRepeater.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        Guid id = new Guid((item.FindControl("HiddenID") as HiddenField).Value);
                        if (id == new Guid("0789DB1A-9BAA-4574-B405-AE570C746C03"))
                        {
                            EditControl editControl = item.FindControl("PropertyValueAdv") as EditControl;
                            if (!editControl.Validate()) return;
                            version = editControl.Value.ToString().Trim();
                            //version = (item.FindControl("PropertyValue") as TextBox).Text.Trim();
                            validator1 = item.FindControl("UniqueValueValidator") as CustomValidator;
                        }
                    }
                }
                #endregion
                #region Get the @nomenValue
                foreach (RepeaterItem item in DictionaryPropertyRepeater.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        Guid dictId = new Guid((item.FindControl("HiddenTreeID") as HiddenField).Value);
                        if (dictId == new Guid("316C6BC7-D883-44C8-AAE0-602F49C73595"))
                        {
                            nomenValue = (item.FindControl("HiddenID") as HiddenField).Value;
                            validator2 = item.FindControl("UniqueValueValidator") as CustomValidator;
                        }
                    }
                }
                #endregion
                #region Check the unique @version && @nomenValue
                if (version != null && nomenValue != null && validator1 != null && validator2 != null)
                {
                    using (ProductProvider provider = new ProductProvider())
                    {
                        var q = from p in provider.Products
                                join pp in provider.ProductProperties on p.ID equals pp.ProductID
                                where pp.PropertyID == new Guid("0789DB1A-9BAA-4574-B405-AE570C746C03") //&& p.ID == this.RequestProductID
                                && pp.Value == version && p._dictNomenID == new Guid(nomenValue)
                                select p;
                        List<Product> list = q.ToList();
                        if (list.Count > 0 && this.IsNew)
                        {
                            validator1.IsValid = false;
                            validator2.IsValid = false;
                            return;
                        }
                        else if (list.Count > 0 && list.Where(p => p.ID == this.RequestProductID).Count() == 0)
                        {
                            validator1.IsValid = false;
                            validator2.IsValid = false;
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
                #endregion
                #region Check the main version canceling...
                string mainVersionValue = null;
                CustomValidator mainVersionValidator = null;
                foreach (RepeaterItem item in GeneralPropertyRepeater.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        Guid id = new Guid((item.FindControl("HiddenID") as HiddenField).Value);
                        if (id == new Guid("bbe170b0-28e4-4738-b365-1038b03f4552"))
                        {
                            EditControl editControl = item.FindControl("PropertyValueAdv") as EditControl;
                            if (!editControl.Validate()) return;
                            mainVersionValue = editControl.Value.ToString().Trim();
                            mainVersionValidator = item.FindControl("UniqueValueValidator") as CustomValidator;
                        }
                    }
                }
                if (mainVersionValue != null && mainVersionValidator != null && !this.IsNew)
                {
                    using (ProductProvider provider = new ProductProvider())
                    {
                        var q = from p in provider.ProductProperties
                                where p.PropertyID == new Guid("bbe170b0-28e4-4738-b365-1038b03f4552")
                                && p.ProductID == this.RequestProductID
                                select p;
                        List<ProductProperty> list = q.ToList();
                        if (list.Count == 1)
                        {
                            // попытка изменить основную версию
                            if (list.First().Value == "1")
                            {
                                mainVersionValidator.IsValid = false;
                                return;
                            }
                        }
                    }
                }
                #endregion
                //}
            if (this.IsNew)
            {
                using (ProductProvider provider = new ProductProvider())
                {
                    Guid newID = provider.AddNewProduct(this.RequestProductID, User.ID);
                    if (this.IsWithConfs)
                    {
                        // копирование по аналогу вместе с составом                        
                        provider.CopyConfiguration(this.RequestProductID, newID, this.User.ID);
                    }
                    newProductID = newID;                    
                }
            }

            using (ProductProvider provider = new ProductProvider())
            {
                Product editedProduct = provider.GetProduct(RequestProductID);
                if (editedProduct.userID != User.ID)
                {
                    editedProduct.userID = User.ID;
                    provider.SubmitChanges();
                }
            }

            #region update dictionary values
            using (DictionaryProvider provider = new DictionaryProvider())
            {
                foreach (RepeaterItem item in DictionaryPropertyRepeater.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        Guid dictId = new Guid((item.FindControl("HiddenTreeID") as HiddenField).Value);
                        string valueString = (item.FindControl("HiddenID") as HiddenField).Value;
                        DictionaryTree dict = provider.DictionaryTrees.Single(d => d.ID == dictId);
                        if (!string.IsNullOrEmpty(valueString) && !(new Guid(valueString).Equals(Guid.Empty)))
                        {
                            provider.SetProductDictioanryValue(this.RequestProductID, dict.FK, valueString.Trim(), User.ID);
                        }
                        else
                        {
                            provider.DeleteProductDictionaryValue(this.RequestProductID, dict.FK, User.ID);
                        }
                    }
                }
                provider.SubmitChanges();
            }
            #endregion

            #region update properties values
            using (ProductProvider provider = new ProductProvider())
            {
                bool settingMainVersion = false;

                foreach (RepeaterItem item in GeneralPropertyRepeater.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        Guid id = new Guid((item.FindControl("HiddenID") as HiddenField).Value);
                        //string value = (item.FindControl("PropertyValue") as TextBox).Text.Trim();
                        EditControl editControl = item.FindControl("PropertyValueAdv") as EditControl;
                        if (!editControl.IsEmpty && !editControl.Validate())
                        {
                            continue;
                        }
                        
                        // Если поле "Вес по приказу" не заполнено, то заполняем его из номенклатуры
                        if (id == new Guid("AC37F816-E4C1-4751-99ED-6180D7CCA142") && editControl.IsEmpty)
                        {
                            Product prod = provider.GetProduct(this.RequestProductID);
                            if (prod._dictNomen.pw.HasValue)
                            {
                                editControl.Value = prod._dictNomen.pw.Value.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
                            }

                        }

                        if (editControl.IsEmpty)
                        {
                            ProductProperty prop = provider.ProductProperties.SingleOrDefault(pp => pp.PropertyID == id && pp.ProductID == this.RequestProductID);
                            if (prop != null)
                            {
                                provider.ProductProperties.DeleteOnSubmit(prop);
                                provider.SubmitChanges();
                            }
                            continue;
                        }
                        
                        string value = editControl.Value.ToString().Trim();
                        if (editControl.ControlType == TypeEnum.Datetime)
                        {
                            DateTime dt = DateTime.ParseExact(value, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            if (dt == DateTime.MinValue) value = string.Empty;
                            else value = dt.ToString("yyyy-MM-dd hh:mm:ss.fff");
                        }
                        
                        // установка признака "Основная версия"
                        if (id == new Guid ("BBE170B0-28E4-4738-B365-1038B03F4552") && value == "1")
                        {
                            settingMainVersion = true;
                        }                        

                        ProductProperty property = provider.ProductProperties.SingleOrDefault(pp => pp.PropertyID == id && pp.ProductID == this.RequestProductID);
                        if (property != null)
                        {
                            if (property.Value != value)
                            {
                                Aspect.Utility.TraceHelper.Log(User.ID, "Продукт: {0}. Свойство изменино: {1}. Старое значение {2}. Новое значение {3}", this.RequestProductID, property.Property.Name, property.Value, value);
                            }
                            property.Value = value;
                        }
                        else
                        {
                            property = new ProductProperty()
                            {
                                ID = Guid.NewGuid(),
                                ProductID = this.RequestProductID,
                                PropertyID = id,
                                Value = value
                            };
                            provider.ProductProperties.InsertOnSubmit(property);
                            Property prop = provider.Properties.Single(p => p.ID == id);
                            Aspect.Utility.TraceHelper.Log(User.ID, "Продукт: {0}. Свойство изменино: {1}. Старое значение NULL. Новое значение {2}", this.RequestProductID, prop.Name, value);
                        }
                        provider.SubmitChanges();
                    }
                }

                // Переносим вес из продуктов в _dictNomen, если там он отсутствует (0 или null)            
                if (settingMainVersion)
                {
                    try
                    {
                        // Пытаемся получить свойство с весом если оно есть
                        string raw_pw = (from p in provider.Products
                                      join pp in provider.ProductProperties on p.ID equals pp.ProductID
                                      where p.ID == this.RequestProductID && pp.PropertyID == new Guid("AC37F816-E4C1-4751-99ED-6180D7CCA142")
                                      select pp.Value).Single();
                        decimal prod_pw = Convert.ToDecimal(raw_pw.Replace(',', '.'), CultureInfo.InvariantCulture);

                        // Если свойство есть переносим его
                        if (prod_pw != 0)
                        {
                            _dictNomen dict = (from p in provider.Products
                                               join d in provider._dictNomens on p._dictNomenID equals d.ID
                                               where p.ID == this.RequestProductID
                                               select d).Single();
                            dict.pw = prod_pw;
                            provider.SubmitChanges();
                        }
                    }
                    catch
                    {
                        // перехватываем исключение, так как веса у продукта может вовсе и не быть 
                    }
                }
            }
            #endregion

            if (this.IsNew)
            {
                // добавить свойство "пустой состав"
                if (!this.IsWithConfs)
                {
                    using (CommonDomain provider = new CommonDomain())
                    {
                        var properties = from props in provider.ProductProperties
                                         where props.PropertyID == new Guid("00ACC1C7-6857-4317-8713-8B8D9479C5CC") // Свойство "Наличие состава"
                                         && props.ProductID == RequestProductID
                                         select props;

                        if (properties.Count() > 1)
                        {
                            // если несколько одинаковых свойств - генерируем исключение
                            throw new Exception("У продукта не может быть больше одного свойства \"Наличие состава\"!");
                        }
                        else if (properties.Count() == 1)
                        {
                            // если только одно свойство - редактируем его, и сохраняемся
                            properties.First().Value = "-";                            
                        } else 
                        {
                            // если нет ниодного свойства, создаём его с нужным нам значеним
                            provider.ProductProperties.InsertOnSubmit(new ProductProperty()
                            {
                                ID = Guid.NewGuid(),
                                ProductID = RequestProductID,
                                PropertyID = new Guid("00ACC1C7-6857-4317-8713-8B8D9479C5CC"),
                                Value = "-"
                            });                            
                            
                        }
                        provider.SubmitChanges();
                    }
                }

                // перенаправить на редактирование
                Response.Redirect(string.Format("Edit.aspx?ID={0}", this.RequestProductID));
                return;
            }
            if(Page.IsValid) this.DataLoad();
        }
    }

    public class DictionaryDDLSource
    {
        public DictionaryDDLSource()
        {
            ID = Guid.Empty;
            ValueText = "Выбрать";
        }
        public Guid DictionaryTreeID { get; set; }
        public string Title { get; set; }
        public Guid ID { get; set; }
        public string ValueText { get; set; }
    }
}
