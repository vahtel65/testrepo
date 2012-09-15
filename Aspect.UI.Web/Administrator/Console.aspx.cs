using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Administrator
{
    public partial class Console : Basic.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Консоль администратора";   
        }
    }
}
