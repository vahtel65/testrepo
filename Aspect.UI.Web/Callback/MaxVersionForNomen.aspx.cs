using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspect.Model.ProductDomain;

namespace Aspect.UI.Web.Callback
{
    public partial class MaxVersionForNomen : System.Web.UI.Page
    {
        public class NidAjax
        {
            public string nid { get; set; }
        }
        
        public string maxVersion;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                if (!string.IsNullOrEmpty(Request["nid"]))
                {
                    try
                    {
                        Guid nid = new Guid(Request["nid"]);
                        int lmaxVersion = provider.getMaxVersionByNomenID(nid) + 1;
                        maxVersion = lmaxVersion.ToString();
                    }
                    catch
                    {
                        maxVersion = "";
                    }

                }
                else
                {
                    maxVersion = "";
                }
            }
        }
    }
}
