using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web.Controls
{
    public class ProductGridField : BoundField //System.Web.UI.ITemplate, IStateManager
    {
        public ProductGridField() : base()
        {
        }
        public ProductGridField(string colname, string dataname)
        {
            this.DataField = dataname;
            this.HeaderText = colname;
        }
    }
}
