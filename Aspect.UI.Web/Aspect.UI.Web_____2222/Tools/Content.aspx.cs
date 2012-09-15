using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web
{
    public partial class Content : System.Web.UI.Page
    {
        protected Literal SelectedObjectName;
        protected PlaceHolder SecondHolder;
        protected PlaceHolder FirstHolder;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ShowSecond_Click(object sender, EventArgs e)
        {
            FirstHolder.Visible = false;
            SecondHolder.Visible = true;
        }
        protected void ShowFirst_Click(object sender, EventArgs e)
        {
            FirstHolder.Visible = true;
            SecondHolder.Visible = false;
        }
    }
}
