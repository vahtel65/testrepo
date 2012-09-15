using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using Aspect.Domain;
using Aspect.Model.ConfigurationDomain;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Configuration
{
    public partial class Compare : Basic.PageBase
    {
        protected GridView LeftGridView;
        protected GridView RightGridView;
        protected ITextControl HeaderLiteralLeft;
        protected ITextControl HeaderLiteralRight;     

        protected Guid ProductID1
        {
            get
            {
                try
                {
                    return new Guid(Request["PID1"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }

        protected Guid ProductID2
        {
            get
            {
                try
                {
                    return new Guid(Request["PID2"]);
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

        protected void Page_Load(object sender, EventArgs e)
        {
            using (EditConfigurationProvider provider = new EditConfigurationProvider())
            {
                Aspect.Domain.Product prodLeft = provider.GetProduct(this.ProductID1);
                Aspect.Domain.Product prodRight = provider.GetProduct(this.ProductID2);                
                HeaderLiteralLeft.Text = string.Format(HeaderLiteralLeft.Text, prodLeft.PublicName, prodLeft.Version == null ? string.Empty : prodLeft.Version.ToString());
                HeaderLiteralRight.Text = string.Format(HeaderLiteralRight.Text, prodRight.PublicName, prodRight.Version == null ? string.Empty : prodRight.Version.ToString());
            }            
            this.Title = String.Format("Сравнение: {0} | {1}", HeaderLiteralLeft.Text, HeaderLiteralRight.Text);
            BindGridColumns();
            BindData();
        }

        private void BindGridColumns()
        {
        }

        class ConfListComparer
        {
            public List<Aspect.Domain.Configuration> listUnical1;
            public List<Aspect.Domain.Configuration> listUnical2;
            public List<Aspect.Domain.Configuration> listChanged1;
            public List<Aspect.Domain.Configuration> listChanged2;            
            public List<Aspect.Domain.Configuration> listIdential1;
            public List<Aspect.Domain.Configuration> listIdential2;
            public Dictionary<Guid, Aspect.Domain.Configuration> dictAlternate1;
            public Dictionary<Guid, Aspect.Domain.Configuration> dictAlternate2;

            /*
             * сравнение двух конфигураций
             */
            public static bool isIdentialConf(Aspect.Domain.Configuration conf1, Aspect.Domain.Configuration conf2)
            {
                //if (conf1.AutoUpdate != conf2.AutoUpdate) return false;
                if (conf1.Comment != conf2.Comment) return false;
                if (conf1.GroupNumber != conf2.GroupNumber) return false;
                if (conf1.GroupToChange != conf2.GroupToChange) return false;
                if (conf1.Position != conf2.Position) return false;
                if (conf1.Quantity != conf2.Quantity) return false;
                if (conf1.version != conf2.version) return false;
                if (conf1.QuantityInclusive != conf2.QuantityInclusive) return false;
                if (conf1._dictUMID != conf2._dictUMID) return false;
                if (conf1.Zone != conf2.Zone) return false;
                return true;
            }

            public ConfListComparer(List<Aspect.Domain.Configuration> list1, List<Aspect.Domain.Configuration> list2)
            {
                listUnical1 = new List<Aspect.Domain.Configuration>();
                listUnical2 = new List<Aspect.Domain.Configuration>();
                listChanged1 = new List<Aspect.Domain.Configuration>();
                listChanged2 = new List<Aspect.Domain.Configuration>();
                listIdential1 = new List<Aspect.Domain.Configuration>();
                listIdential2 = new List<Aspect.Domain.Configuration>();
                dictAlternate1 = new Dictionary<Guid, Aspect.Domain.Configuration>();
                dictAlternate2 = new Dictionary<Guid, Aspect.Domain.Configuration>();
                bool isPresent;

                // поиск новых конфигураций
                foreach (Aspect.Domain.Configuration confEtalon in list2)
                {
                    isPresent = false;
                    foreach (Aspect.Domain.Configuration confItem in list1)
                    {
                        if (confEtalon.ProductID == confItem.ProductID)
                        {
                            isPresent = true;
                            break;
                        }
                    }
                    if (!isPresent) listUnical1.Add(confEtalon);
                }

                // поиск удалённых конфигураций
                foreach (Aspect.Domain.Configuration confEtalon in list1)
                {
                    isPresent = false;
                    foreach (Aspect.Domain.Configuration confItem in list2)
                    {
                        if (confEtalon.ProductID == confItem.ProductID)
                        {
                            isPresent = true;
                            break;
                        }
                    }
                    if (!isPresent) listUnical2.Add(confEtalon);
                }

                // поиск изменившихся и одинаковых
                foreach (Aspect.Domain.Configuration confEtalon in list1)
                {
                    foreach (Aspect.Domain.Configuration confItem in list2)
                    {
                        if (confEtalon.ProductID == confItem.ProductID)
                        {
                            if (!isIdentialConf(confEtalon, confItem))
                            {
                                // разные поля конфигурации                                
                                listChanged1.Add(confEtalon);
                                listChanged2.Add(confItem);
                            }
                            else
                            {
                                // одинаковые поля конфигурации
                                listIdential1.Add(confEtalon);
                                listIdential2.Add(confItem);
                            }
                        }
                    }
                }
                // поиск альтернативных версий
                foreach (Aspect.Domain.Configuration confEtalon in list1)
                {
                    foreach (Aspect.Domain.Configuration confItem in list2)
                    {
                        if (confEtalon.Product._dictNomenID == confItem.Product._dictNomenID)
                        {
                            dictAlternate2[confEtalon.ID] = confItem;
                            dictAlternate1[confItem.ID] = confEtalon;
                        }
                    }
                }
            }

            public Aspect.Domain.Configuration getAlternate1(Guid conf)
            {
                if (dictAlternate1.Keys.Contains(conf))
                    return dictAlternate1[conf];
                else
                    return new Aspect.Domain.Configuration();
            }

            public Aspect.Domain.Configuration getAlternate2(Guid conf)
            {
                if (dictAlternate2.Keys.Contains(conf))
                    return dictAlternate2[conf];
                else
                    return new Aspect.Domain.Configuration();
            }
 
        }

        private void BindDataSetCollumns(ref DataSet dataSet)
        {
            dataSet.Tables.Add();
            dataSet.Tables[0].Columns.Add("hideConfigurationID");
            dataSet.Tables[0].Columns.Add("hideProductID");
            dataSet.Tables[0].Columns.Add("hideType");
            dataSet.Tables[0].Columns.Add("hideAlternateID");
            dataSet.Tables[0].Columns.Add("ImageCompare");
            dataSet.Tables[0].Columns.Add("Name");
            dataSet.Tables[0].Columns.Add("Version");
            dataSet.Tables[0].Columns.Add("Count");
            dataSet.Tables[0].Columns.Add("Comment");
            dataSet.Tables[0].Columns.Add("Zone");
            dataSet.Tables[0].Columns.Add("Ngroup");
            dataSet.Tables[0].Columns.Add("Ngrchange");
            dataSet.Tables[0].Columns.Add("Position");    
        }

        public enum ConfType
        {
            Normal,
            Nulled,
            Changed,
            Newed
        }

        public String SafeWhiteString(string value)
        {
            if (value == null) return "";
            return new String('\u00a0', value.Trim().Length);
        }

        private void BindDataSetField(ref DataSet dataSet, Aspect.Domain.Configuration conf, ConfType type, Aspect.Domain.Configuration alternate)
        {
            if (type == ConfType.Nulled) {
                DataRow row = dataSet.Tables[0].NewRow();
                row["hideConfigurationID"] = Guid.Empty;
                row["hideProductID"] = Guid.Empty;
                row["hideType"] = "nulled";                
                row["Name"] = SafeWhiteString(conf.Product._dictNomen.superpole);
                row["Version"] = SafeWhiteString(conf.Product.Version);
                row["Count"] = SafeWhiteString(conf.Quantity.ToString());
                row["Position"] = SafeWhiteString(conf.Position);
                row["Comment"] = SafeWhiteString(conf.Comment);
                row["Ngroup"] = SafeWhiteString(conf.GroupNumber.ToString());
                row["Ngrchange"] = SafeWhiteString(conf.GroupToChange.ToString());
                row["Zone"] = SafeWhiteString(conf.Zone);
                dataSet.Tables[0].Rows.Add(row);
            }
            else {
                DataRow row = dataSet.Tables[0].NewRow();
                row["hideConfigurationID"] = conf.ID;
                row["hideProductID"] = conf.ProductID;                
                switch (type)
                {
                    case ConfType.Normal: row["hideType"] = "normal"; break;
                    case ConfType.Changed: row["hideType"] = "changed"; break;
                    case ConfType.Newed: row["hideType"] = "newed"; break;                    
                }
                row["Name"] = conf.Product._dictNomen.superpole;
                if (!alternate.ID.Equals(Guid.Empty))
                {
                    row["hideAlternateID"] = alternate.ProductID;
                    row["ImageCompare"] = "~/img/compare.png";
                }
                row["Version"] = conf.Product.Version;
                row["Count"] = conf.Quantity;
                row["Position"] = conf.Position;
                row["Comment"] = conf.Comment;
                row["Ngroup"] = conf.GroupNumber;
                row["Ngrchange"] = conf.GroupToChange;
                row["Zone"] = conf.Zone;
                dataSet.Tables[0].Rows.Add(row);
            }
        }

        private void BindData()
        {
            using (Aspect.Domain.CommonDomain domain = new Aspect.Domain.CommonDomain())
            {

                List<Aspect.Domain.Configuration> RightConfigurations = domain.Configurations.Where(c => c.ProductOwnerID == ProductID1).ToList();
                List<Aspect.Domain.Configuration> LeftConfigurations = domain.Configurations.Where(c => c.ProductOwnerID == ProductID2).ToList();

                ConfListComparer comparer = new ConfListComparer(LeftConfigurations, RightConfigurations);

                DataSet dataSet1 = new DataSet();
                DataSet dataSet2 = new DataSet();

                BindDataSetCollumns(ref dataSet1);
                BindDataSetCollumns(ref dataSet2);

                /* Сортировка по HSO */
                var hso = from h in domain._dictHs
                          orderby h.hso ascending
                          select h.hso;

                foreach (int hsID in hso)
                {
                    /* добавление одинаковых записей */
                    foreach (Aspect.Domain.Configuration conf in (from item in comparer.listIdential1 where item.Product._dictNomen._dictH.hso == hsID select item))
                    {
                        BindDataSetField(ref dataSet1, conf, ConfType.Normal, new Aspect.Domain.Configuration());
                    }

                    foreach (Aspect.Domain.Configuration conf in (from item in comparer.listIdential2 where item.Product._dictNomen._dictH.hso == hsID select item))
                    {
                        BindDataSetField(ref dataSet2, conf, ConfType.Normal, new Aspect.Domain.Configuration());
                    }

                    /* добавление изменившихся записей */
                    foreach (Aspect.Domain.Configuration conf in (from item in comparer.listChanged2 where item.Product._dictNomen._dictH.hso == hsID select item))
                    {
                        BindDataSetField(ref dataSet1, conf, ConfType.Changed, new Aspect.Domain.Configuration());
                    }

                    foreach (Aspect.Domain.Configuration conf in (from item in comparer.listChanged1 where item.Product._dictNomen._dictH.hso == hsID select item))
                    {
                        BindDataSetField(ref dataSet2, conf, ConfType.Changed, new Aspect.Domain.Configuration());
                    }

                    /* добавление уникальных записей */
                    foreach (Aspect.Domain.Configuration conf in (from item in comparer.listUnical1 where item.Product._dictNomen._dictH.hso == hsID select item))
                    {
                        BindDataSetField(ref dataSet1, conf, ConfType.Newed, comparer.getAlternate1(conf.ID));
                        BindDataSetField(ref dataSet2, conf, ConfType.Nulled, new Aspect.Domain.Configuration());
                    }

                    foreach (Aspect.Domain.Configuration conf in (from item in comparer.listUnical2 where item.Product._dictNomen._dictH.hso == hsID select item))
                    {
                        BindDataSetField(ref dataSet2, conf, ConfType.Newed, comparer.getAlternate2(conf.ID));
                        BindDataSetField(ref dataSet1, conf, ConfType.Nulled, new Aspect.Domain.Configuration());
                    }
                }                                
                
                LeftGridView.DataSource = dataSet1;
                LeftGridView.DataBind();

                RightGridView.DataSource = dataSet2;
                RightGridView.DataBind();
            }
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

        protected void LeftGridView_RowDateBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                String confid = (String) DataBinder.Eval(e.Row.DataItem, "hideConfigurationID");
                if (Guid.Empty.ToString() != confid)
                {
                    String changed = (String)DataBinder.Eval(e.Row.DataItem, "hideType");
                    if (changed == "changed")
                    {
                        e.Row.CssClass = "changedrow";
                        //e.Row.BackColor = System.Drawing.Color.FromArgb(255, 230, 186);
                    }
                    if (changed == "newed")
                    {
                        e.Row.CssClass = "newedrow";
                        //e.Row.BackColor = System.Drawing.Color.FromArgb(200, 255, 186);
                    }
                }
            }
        }

        protected void LeftGridView_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                {
                    e.Row.ID = string.Format("{0}_Row{1}", LeftGridView.ClientID, e.Row.RowIndex);

                    string pid = DataBinder.Eval(e.Row.DataItem, "hideConfigurationID").ToString();
                    if (!Guid.Empty.Equals(new Guid(pid)))
                    {
                        AllRecords.Add(new Guid(pid));
                    }

                    string prodid = DataBinder.Eval(e.Row.DataItem, "hideProductID").ToString();
                    string cid = this.RequestClassificationTreeID.ToString();
                    string function = string.Format("onGridViewRowSelectedCallbackLeft('{0}','{1}', this, '{2}');",
                        prodid, cid, LeftGridView.Controls[0].ClientID);
                    e.Row.Attributes.Add("onclick", function);

                    if (e.Row.RowState == DataControlRowState.Alternate) e.Row.CssClass = "row2";
                    else e.Row.CssClass = string.Empty;

                    e.Row.Attributes["onmouseover"] = "highLightRow(this)";
                    e.Row.Attributes["onmouseout"] = "unHighLightRow(this)";
                    //CheckBox chk = e.Row.Cells[1].FindControl("SelectCheckBox") as CheckBox;
                    //chk.Attributes.Add("onclick", String.Format("selectProduct(event,this,'{0}','{1}');", pid, new Guid()/*SelectedProductsHidden.ClientID)*/));
                    //chk.Checked = SelectedProductsHidden.Value.Contains(pid);
                }
            }
        }

        protected void RightGridView_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                {
                    e.Row.ID = string.Format("{0}_Row{1}", RightGridView.ClientID, e.Row.RowIndex);

                    string pid = DataBinder.Eval(e.Row.DataItem, "hideConfigurationID").ToString();
                    if (!Guid.Empty.Equals(new Guid(pid)))
                    {
                        AllRecords.Add(new Guid(pid));
                    }

                    string prodid = DataBinder.Eval(e.Row.DataItem, "hideProductID").ToString();
                    string cid = this.RequestClassificationTreeID.ToString();
                    string function = string.Format("onGridViewRowSelectedCallbackRight('{0}','{1}', this, '{2}');",
                        prodid, cid, RightGridView.Controls[0].ClientID);
                    e.Row.Attributes.Add("onclick", function);
                    
                    if (e.Row.RowState == DataControlRowState.Alternate) e.Row.CssClass = "row2";
                    else e.Row.CssClass = string.Empty;

                    e.Row.Attributes["onmouseover"] = "highLightRow(this)";
                    e.Row.Attributes["onmouseout"] = "unHighLightRow(this)";
                    //CheckBox chk = e.Row.Cells[1].FindControl("SelectCheckBox") as CheckBox;
                    //chk.Attributes.Add("onclick", String.Format("selectProduct(event,this,'{0}','{1}');", pid, new Guid()/*SelectedProductsHidden.ClientID)*/));
                    //chk.Checked = SelectedProductsHidden.Value.Contains(pid);
                }
            }
        }

    }
}
