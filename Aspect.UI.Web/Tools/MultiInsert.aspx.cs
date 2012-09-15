using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using Aspect.UI.Web.Json;

namespace Aspect.UI.Web.Tools
{
 /*   public class SearchField
    {
        public string concat; // {and, or}
        public string op; // {=>, <=, >, <, =, !=, ==, !==}
        public string id;
    }

    public class JsonField
    {
        public string Alias;
        public string Caption;
        public string Type;
        public Guid Uid;

        public static string ListToJson()
        {
            List<JsonField> listFields = new List<JsonField>();
            SearchBuilder searchBuilder = SearchBuilder.InitInstance();

            foreach (BaseField field in searchBuilder.m_listFields)
            {
                JsonField jfield = new JsonField();
                jfield.Alias = field.Alias;
                jfield.Caption = field.Name;
                jfield.Uid = field.Id;
                jfield.Type = field.getJsonType();
                listFields.Add(jfield);
            }

            JavaScriptSerializer js = new JavaScriptSerializer();            
            return js.Serialize(listFields); 
        }
    }
*/
    public partial class MultiInsert : Basic.PageBase
    {
        public string jsonFields;
        protected string umID;
        protected int umSelectedIndex;
        protected void RegisterClientCss(string url)
        {
            HtmlLink link = new HtmlLink();
            link.Attributes.Add("href", url);
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("rel", "stylesheet");
            this.Header.Controls.AddAt(0, link); 
        }
        protected void DropDownLoaded(object sender, EventArgs e)
        {
            ((DropDownList)sender).SelectedValue = "68CD2019-85F6-4E52-AEFE-09CA5C2B64F3"; //.SelectedIndex=2;
         }
        protected void Page_Load(object sender, EventArgs e)
        {
            
           
            this.Title = "Инструмент :: Множественная вставка";            
            Page.ClientScript.RegisterClientScriptInclude("ext-base", "/scripts/extjs/adapter/ext/ext-base.js");
            Page.ClientScript.RegisterClientScriptInclude("ext-all", "/scripts/extjs/ext-all.js");            
            Page.ClientScript.RegisterClientScriptInclude("ext-CheckColumn", "/scripts/extjs/ux/CheckColumn.js");
            Page.ClientScript.RegisterClientScriptInclude("ext-Application.ComboTree", "/scripts/extjs/Application.ComboTree.js");
            Page.ClientScript.RegisterClientScriptInclude("ext-Application.SearchForm", "/scripts/extjs/Application.SearchForm.js");
            RegisterClientCss("/css/xtheme-gray.css");
            RegisterClientCss("/css/ext-all.css");

            jsonFields = JsonField.ListToJson();
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
 
        }

        protected void DropDownList1_TextChanged(object sender, EventArgs e)
        {
             
        }
    }
}
