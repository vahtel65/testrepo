using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Callback
{
    public partial class ClearBuffer : Basic.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ClearMultiBuffer();
        }
    }
}
