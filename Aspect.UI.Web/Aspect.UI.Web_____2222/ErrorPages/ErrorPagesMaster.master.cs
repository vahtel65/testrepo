using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Aspect.UI.Web.ErrorPages
{
    public partial class ErrorPagesMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlLink link = new HtmlLink();
            link.Attributes.Add("href", "/css/login.css");
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("rel", "stylesheet");

            Page.Header.Controls.AddAt(0, link); 
        }
    }
}
