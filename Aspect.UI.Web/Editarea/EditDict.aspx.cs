using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.Web.UI.WebControls;
using Aspect.UI.Web.Basic;

using Aspect.Model.DictionaryDomain;
using Aspect.UI.Web.Controls;
using Aspect.Domain;
using Aspect.Model.ProductDomain;
using System.Globalization;

namespace Aspect.UI.Web.Editarea
{
    public partial class EditDict : Basic.PageBase
    {
        //protected Guid newDictionaryEntityID = Guid.Empty;
        protected Guid RequestDictionaryEntityID
        {
            get
            {
                return new Guid(ViewState["RequestDictionaryEntityID"].ToString());
                /*if (newDictionaryEntityID != Guid.Empty && this.IsNew)
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
                }*/
            }
            set
            {
                ViewState["RequestDictionaryEntityID"] = value;
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
        protected bool IsNewAdded
        {
            get
            {
                return Convert.ToBoolean(ViewState["IsNewAdded"]);
            }
            set
            {
                ViewState["IsNewAdded"] = value;
            }
        }

        protected HyperLink SelectorButton;
        protected HiddenField userCID;
        protected Repeater GeneralPropertyRepeater;
        protected Repeater DictionaryPropertyRepeater;
        protected CheckBox CopyMainVersion;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {                                
                IsNewAdded = false;
                try
                {
                    RequestDictionaryEntityID = new Guid(this.Request["ID"]);
                }
                catch
                {
                    this.RedirectToErrorPage();
                }

                // для "Номенклатура" и "Номен. материала"
                if (IsNew && DictionaryTreeID == new Guid("316c6bc7-d883-44c8-aae0-602f49c73595"))
                {
                    using (DictionaryProvider provider = new DictionaryProvider())
                    {
                        if (!provider.IsSimpleDetal(this.RequestDictionaryEntityID))
                        {
                            CopyMainVersion.Visible = true;
                        }
                    }
                }
                // только для новых пунктов в Номенклатуре
                if (!IsNew || !DictionaryTreeID.Equals(new Guid("316c6bc7-d883-44c8-aae0-602f49c73595")))
                {
                    SelectorButton.Visible = false;
                }
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
                foreach (DictionaryProperty item in props.OrderBy(p => p.Position))
                {
                    object obj = provider.GetDictionaryItemValue(this.DictionaryTreeID, this.RequestDictionaryEntityID, item.ColumnName);
                    Pair<DictionaryProperty, object> entity = new Pair<DictionaryProperty, object>(item, obj);
                    if (this.IsNew && item.ColumnName == "superpole_ua")
                    {
                        entity.Second = "";
                    }
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
                foreach (DictionaryTree item in list.OrderBy(d => d.Position))
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

                HyperLink deleteLink = e.Item.FindControl("DeleteLink") as HyperLink;
                deleteLink.Attributes.Add("onclick", string.Format("setSelectedValue('{0}','{1}','{2}','{3}');return false;", valueCtrlID, Guid.Empty.ToString(), control.ClientID, "Выбрать"));
                //string.Format("document.getElementById('{0}').value = '';document.getElementById('{1}').innerHTML = 'Выбрать';", valueCtrlID,control.ClientID);
                ;
            }
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in GeneralPropertyRepeater.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    //(item.FindControl("PropertyValue") as TextBox).Text = string.Empty;
                    (item.FindControl("PropertyValueAdv") as EditControl).Clear();
                }
            }
            /*using (CommonDomain domain = new CommonDomain())
            {
                List<ITreeNode> parents = domain.GetProductParents(this.RequestProductID);
                BindDictionaryList(parents, true);
            }*/
        }
        protected CustomValidator UniqueValueValidator;
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (this.DictionaryTreeID == new Guid("316C6BC7-D883-44C8-AAE0-602F49C73595"))
            {
                Guid _dictP1STID = Guid.Empty;
                Guid _dictGOSTID = Guid.Empty;
                string PN1 = string.Empty;
                string PN2_2 = string.Empty;
                EditControl superfieldControl = null;
                EditControl superfieldControl_ua = null;
                foreach (RepeaterItem item in GeneralPropertyRepeater.Items)
                {
                    string col_name = (item.FindControl("HiddenID") as HiddenField).Value;
                    if (col_name.ToUpper() == "PN1")
                    {
                        EditControl editControl = item.FindControl("PropertyValueAdv") as EditControl;
                        if (!editControl.Validate()) return;
                        PN1 = editControl.Value.ToString().Trim();
                    }
                    if (col_name.ToUpper() == "PN2_2")
                    {
                        EditControl editControl = item.FindControl("PropertyValueAdv") as EditControl;
                        if (!editControl.Validate()) return;
                        PN2_2 = editControl.Value.ToString().Trim();
                    }
                    if (col_name.ToLower() == "superpole")
                    {
                        superfieldControl = item.FindControl("PropertyValueAdv") as EditControl;
                    }
                    if (col_name.ToLower() == "superpole_ua")
                    {
                        superfieldControl_ua = item.FindControl("PropertyValueAdv") as EditControl;
                    }
                }

                foreach (RepeaterItem item in DictionaryPropertyRepeater.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        Guid dictId = new Guid((item.FindControl("HiddenTreeID") as HiddenField).Value);
                        if (dictId == new Guid("98C60832-A66C-4A84-BC90-EB83EE43C4C9"))
                        {
                            _dictP1STID = new Guid((item.FindControl("HiddenID") as HiddenField).Value);
                        }
                        if (dictId == new Guid("BB4BCA91-C56D-4746-B5C2-C7CF29596F3D"))
                        {
                            _dictGOSTID = new Guid((item.FindControl("HiddenID") as HiddenField).Value);
                        }
                    }
                }
               
               using (DictionaryProvider provider = new DictionaryProvider())
               {
                   string superfield = provider.GetNomenSuperFieldValue(_dictP1STID, _dictGOSTID, PN1, PN2_2);
                   string superfield_ua = provider.GetNomenSuperFieldValueUa(_dictP1STID, _dictGOSTID, PN1, PN2_2);
                   
                   if (superfieldControl != null) superfieldControl.Value = superfield;
                   if (superfieldControl_ua != null )
                   {
                       if (String.IsNullOrEmpty(superfieldControl_ua.Value.ToString().Trim()))
                       {
                           // если поле с "укр. суперполем" пусто, устанавливаем расчитанное значение
                           superfieldControl_ua.Value = superfield_ua;
                       }
                   }

                   #region Check [superpole] && [pn1] for uniqueness
                   // Проверка поля [superpole] на уникальность
                   List<_dictNomen> list = provider._dictNomens.Where(d => d.superpole.Trim() == superfield).ToList();
                   if (this.IsNew && !this.IsNewAdded && list.Count > 0)
                   {

                       UniqueValueValidator.ErrorMessage = string.Format("Значение Общ. наименование должно быть уникальным ( {0} )", superfield);
                       UniqueValueValidator.IsValid = false;
                       return;
                   }
                   else if (list.Count > 0 && list.Where(l => l.ID == this.RequestDictionaryEntityID).Count() == 0)
                   {
                       UniqueValueValidator.ErrorMessage = string.Format("Значение Общ. наименование должно быть уникальным ( {0} )", superfield);
                       UniqueValueValidator.IsValid = false;
                       return;
                   }

                   // Проверка поля [pn1] на уникальность
                   List<_dictNomen> listPn1 = provider._dictNomens.Where(d => d.pn1.Trim() == PN1).ToList();
                   if (this.IsNew && !this.IsNewAdded && listPn1.Count > 0)
                   {
                       UniqueValueValidator.Text = string.Format("! Значение поля [Обозначение] должно быть уникальным ( {0} )", PN1);
                       UniqueValueValidator.IsValid = false;
                       return;
                   }
                   else if (listPn1.Count > 0 && listPn1.Where(l => l.ID == this.RequestDictionaryEntityID).Count() == 0)
                   {
                       UniqueValueValidator.Text = string.Format("! Значение поля [Обозначение] должно быть уникальным ( {0} )", PN1);
                       UniqueValueValidator.IsValid = false;
                       return;
                   }                   

                   #endregion
               }                
            }

            Guid oldDictionaryEntityID = this.RequestDictionaryEntityID;
            if (this.IsNew && !this.IsNewAdded)
            {
                using (DictionaryProvider provider = new DictionaryProvider())
                {
                    Guid newID = provider.AddNewDictionaryEntity(this.DictionaryTreeID, this.RequestDictionaryEntityID);
                    this.RequestDictionaryEntityID = newID;
                    IsNewAdded = true;
                    //newDictionaryEntityID = newID;
                    Aspect.Utility.TraceHelper.Log(User.ID, "Справочник treeID: {0}. Новая запись: {1}", this.DictionaryTreeID, newID);
                }
            }
            using (DictionaryProvider provider = new DictionaryProvider())
            {
                //update properties
                foreach (RepeaterItem item in GeneralPropertyRepeater.Items)
                {
                    string col_name = (item.FindControl("HiddenID") as HiddenField).Value;
                    //string value = (item.FindControl("PropertyValue") as TextBox).Text.Trim();
                    EditControl editControl = item.FindControl("PropertyValueAdv") as EditControl;
                    if (!editControl.Validate())
                    {
                        continue;
                    }
                    string value = editControl.Value.ToString().Trim();
                    /*if (col_name == "superpole" && this.DictionaryTreeID == new Guid("316C6BC7-D883-44C8-AAE0-602F49C73595"))
                    {
                    }*/
                    try
                    {
                        if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        {
                            object obj = provider.GetDictionaryItemValue(this.DictionaryTreeID, this.RequestDictionaryEntityID, col_name);
                            if (obj == null || obj == DBNull.Value || !value.Equals(obj.ToString().Trim()))
                            {
                                Aspect.Utility.TraceHelper.Log(User.ID, "Справочник treeID: {0}.  Запись: {4}. Свойство изменино: {1}. Старое значение {2}. Новое значение {3}", this.DictionaryTreeID, col_name, ((obj == null || obj == DBNull.Value) ? "NULL" : obj.ToString()), value, RequestDictionaryEntityID);
                                if (editControl.ControlType == TypeEnum.Datetime)
                                {
                                    DateTime dt = DateTime.ParseExact(value, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    if (dt == DateTime.MinValue) provider.DeleteDictionaryItemValue(this.DictionaryTreeID, this.RequestDictionaryEntityID, col_name);
                                    else provider.SetDictionaryItemValue(this.DictionaryTreeID, this.RequestDictionaryEntityID, col_name, dt.ToString("MM/dd/yyyy"), User.ID);
                                }
                                else
                                {
                                    if (IsNew && col_name == "raznes") value = "0";
                                    /*
                                     * Переносим вес в первую версию продукта основную версию при условии
                                     */
                                    #region moving_weight
                                    if (IsNew && col_name == "pw")
                                    {                                                                               
                                        using (ProductProvider productProvider = new ProductProvider())
                                        {
                                            try
                                            {
                                                productProvider.SetProductWeight(this.RequestDictionaryEntityID, Convert.ToDecimal(value.Replace(",", "."), CultureInfo.InvariantCulture));
                                            }
                                            catch { }
                                        }                                        
                                    }
                                    #endregion
                                    provider.SetDictionaryItemValue(this.DictionaryTreeID, this.RequestDictionaryEntityID, col_name, value, User.ID);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        this.NotSavedProperties.Add(new Pair<string, string>(col_name, value));
                        //(item.FindControl("PropertyValidator") as CustomValidator).IsValid = false;    
                    }
                }

                foreach (RepeaterItem item in DictionaryPropertyRepeater.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        Guid dictId = new Guid((item.FindControl("HiddenTreeID") as HiddenField).Value);
                        string valueString = (item.FindControl("HiddenID") as HiddenField).Value;
                        DictionaryTree dict = provider.DictionaryTrees.Single(d => d.ID == dictId);

                        object obj = provider.GetDictionaryItemValue(this.DictionaryTreeID, this.RequestDictionaryEntityID, dict.FK);

                        if (!string.IsNullOrEmpty(valueString) && !(new Guid(valueString).Equals(Guid.Empty)))
                        {
                            if (obj == null || obj == DBNull.Value || obj.ToString() != valueString)
                            {
                                Aspect.Utility.TraceHelper.Log(User.ID, "Справочник treeID: {0}. Запись: {4}. Свойство изменино: {1}. Старое значение {2}. Новое значение {3}", this.DictionaryTreeID, dict.FK, ((obj == null || obj == DBNull.Value) ? "NULL" : obj.ToString()), valueString, this.RequestDictionaryEntityID);
                                provider.SetDictionaryItemValue(this.DictionaryTreeID, this.RequestDictionaryEntityID, dict.FK, valueString.Trim(), User.ID);
                            }
                        }
                        else
                        {
                            if (obj != null && obj != DBNull.Value)
                            {
                                Aspect.Utility.TraceHelper.Log(User.ID, "Справочник treeID: {0}. Запись: {3}. Свойство удалено: {1}. Старое значение {2}. Новое значение NULL", this.DictionaryTreeID, dict.FK, obj.ToString(), this.RequestDictionaryEntityID);
                                provider.DeleteDictionaryItemValue(this.DictionaryTreeID, this.RequestDictionaryEntityID, dict.FK);
                            }
                        }
                    }
                }
                //provider.SubmitChanges();
            }
            //this.BindDictionaryList();
            /*if (this.IsNew && this.IsValid)
            {
                Response.Redirect(string.Format("~/Editarea/EditDict.aspx?ID={0}&new=false&DictID={1}", this.RequestDictionaryEntityID, Request["DictID"]));
            }*/

            // для "Номенклатура" и "Номен. материала"
            if (this.IsNew && DictionaryTreeID == new Guid("316c6bc7-d883-44c8-aae0-602f49c73595"))
            {
                // если "Копировать состав из основной версии"
                if (this.CopyMainVersion.Checked)
                {
                    using (ProductProvider provider = new ProductProvider())
                    {
                        Guid newMainProductID = (from prod in provider.Products
                                                 where prod._dictNomenID == this.RequestDictionaryEntityID
                                                 select prod.ID).FirstOrDefault();
                        Guid oldMainProductID = (from prod in provider.Products
                                              join vers in provider.ProductProperties
                                                on prod.ID equals vers.ProductID
                                              where vers.Value == "1" && vers.PropertyID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552") // основная версия
                                              && prod._dictNomenID == oldDictionaryEntityID
                                              select prod.ID).FirstOrDefault();                                                
                        
                        if (!Guid.Empty.Equals(oldMainProductID))
                        {
                            //newMainProductID = provider.CopyProduct(oldMainProductID, 2, RequestDictionaryEntityID);
                            provider.CopyConfiguration(oldMainProductID, newMainProductID, User.ID);
                        }
                    }                                        
                }
                // если класс не "МатРесурсы"
                if (new Guid(userCID.Value) != new Guid("55c7b455-0638-4acb-ac2e-5b4992e48462"))
                {
                    using (ProductProvider provider = new ProductProvider())
                    {
                        provider.ChangeProductClassification(this.RequestDictionaryEntityID, new Guid(userCID.Value));
                    }
                }                
            }
            if (IsNew) Response.Redirect(string.Format("/Editarea/EditDict.aspx?ID={0}&new=false&DictID={1}", RequestDictionaryEntityID, DictionaryTreeID));
            else if(Page.IsValid) this.DataLoad();
        }
    }
}
