using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Web.Script.Services;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Serialization;

namespace Aspect.UI.Web.Technology
{
    [ScriptService] 
    public partial class Buffer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private static List<transfer_add> bufferAddMaterials 
        {
            get
            {
                if (HttpContext.Current.Session["bufferAddMaterials"] == null)
                {
                    HttpContext.Current.Session["bufferAddMaterials"] = new List<transfer_add>();
                };

                return (List<transfer_add>) HttpContext.Current.Session["bufferAddMaterials"];
            }            
        }

        [WebMethod]
        public static string Insert(List<transfer_add> inserted)
        {
            try
            {
                bufferAddMaterials.Clear();
                bufferAddMaterials.AddRange(inserted);
                return new PostResult("", 0).ToString();
            }
            catch
            {
                return new PostResult("Неизвестная ошибка.", -1).ToString();
            }            
        }

        [WebMethod]
        public static string Select()
        {
            List<transfer_add> selected = new List<transfer_add>(bufferAddMaterials);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(selected);
        }

    }
}
