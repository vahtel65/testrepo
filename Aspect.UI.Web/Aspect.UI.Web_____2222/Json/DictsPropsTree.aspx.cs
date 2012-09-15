using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

using Aspect.Model.DictionaryDomain;
using Aspect.Domain;

namespace Aspect.UI.Web.Json
{
    public class JsonResponse
    {
        public bool error = false;
        public string errorMessage;
        public Guid userID;

        public List<JsonTreeNode> children = new List<JsonTreeNode>();

        public string getJsonString()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(this.children);
        }
    }

    public class JsonTreeNode
    {        
        public string text;
        public string uid;
        public bool expanded = true;
        public bool leaf = false;
        public List<JsonTreeNode> children = new List<JsonTreeNode>();
        
        public JsonTreeNode(string text)
        {
            this.text = text;
        }
    }

    public partial class DictsPropsTree : Basic.PageBase
    {
        public JsonResponse jsonReponse = new JsonResponse();

        protected void BindDictionaryBranch(JsonTreeNode branch, Guid parentDict)
        {
            using (DictionaryProvider provider = new DictionaryProvider())
            {
                List<DictionaryTree> dicts = provider.GetDictionaryTreeList(parentDict, this.Roles);

                foreach (DictionaryTree dict in dicts)
                {
                    JsonTreeNode tempNode = new JsonTreeNode(dict.Name);
                    branch.children.Add(tempNode);
                    BindDictionaryBranch(tempNode, dict.ID);
                }
                if (!parentDict.Equals(Guid.Empty))
                {
                    DictionaryTree entity = provider.DictionaryTrees.SingleOrDefault(d => d.ID == parentDict);
                    List<DictionaryProperty> source = provider.GetAvailableDictionaryProperties(this.Roles, entity.DictionaryID);

                    foreach (DictionaryProperty prop in source)
                    {
                        JsonTreeNode tempNode = new JsonTreeNode(prop.Name);
                        tempNode.uid = String.Format("{0}_{1}", prop.Dictionary.TableName, prop.ColumnName);
                        tempNode.leaf = true;
                        branch.children.Add(tempNode);
                    }
                }
            }
        }
        
        protected void BindDictionaryTree()
        {
            JsonTreeNode rootDicts = new JsonTreeNode("Словари");
            BindDictionaryBranch(rootDicts, Guid.Empty);
            jsonReponse.children.Add(rootDicts);
        }

        protected void BindPropertyTree()
        {
            JsonTreeNode rootProps = new JsonTreeNode("Свойства");
            using (DictionaryProvider provider = new DictionaryProvider())
            {
                List<Property> source = provider.GetAvailableProperties(this.Roles);
                foreach (Property prop in source)
                {
                    JsonTreeNode tempNode = new JsonTreeNode(prop.Name);
                    tempNode.leaf = true;
                    tempNode.uid = prop.Alias;
                    rootProps.children.Add(tempNode);
                }
            }
            jsonReponse.children.Add(rootProps);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                BindDictionaryTree();
                BindPropertyTree();
            }
            catch (System.Exception except)
            {
                jsonReponse.error = true;
                jsonReponse.errorMessage = except.Message;
            }
        }
    }
}
