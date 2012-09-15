using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspect.UI.Web.Basic;

using Aspect.Model.ProductDomain;
using Aspect.Model.Query;
using Aspect.Domain;

namespace Aspect.UI.Web.Popup
{
    public partial class ProductSearch : ContentPageBase
    {
        /*protected Guid ProductID
        {
            get
            {
                return new Guid(Request[RequestKeyProductID]);
            }
        }*/
        protected Repeater SearchRepeater;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                ContentDomain provider = null;
                try
                {
                    provider = Aspect.Model.Common.GetContentDomain(ClassifiacationTypeView);
                    if (provider != null)
                    {
                        List<SearchExpression> source = new List<SearchExpression>();
                        List<IUserField> dictColumns = provider.GetUserFields(this.User.ID, RequestClassificationTreeID, FieldPlaceHolder.Grid);
                        List<UserProperty> columns = provider.GetUserPropertyColumns(this.User.ID, RequestClassificationTreeID, FieldPlaceHolder.Grid);

                        foreach (IUserField item in dictColumns)
                        {
                            IEnumerable<SearchExpression> list = this.SearchConditions.Where(s => s.FieldID == item.ID);
                            
                            source.Add(new SearchExpression()
                            {
                                FieldValue = list.Count() > 0 ? list.First().FieldValue : string.Empty,
                                FieldName = String.Format("{1} - {0}", item.DictionaryProperty.Name, item.DictionaryTree.Name),
                                FieldID = item.ID,
                                Order = item.Sequence
                            });
                        }
                        foreach (UserProperty item in columns)
                        {
                            IEnumerable<SearchExpression> list = this.SearchConditions.Where(s => s.FieldID == item.ID);
                            source.Add(new SearchExpression()
                            {
                                FieldValue = list.Count() > 0 ? list.First().FieldValue : string.Empty,
                                FieldName = item.Property.Name,
                                FieldID = item.Property.ID,
                                Order = item.Sequence
                            });
                        }
                        SearchRepeater.DataSource = source.OrderBy(r => r.Order);
                        SearchRepeater.DataBind();
                    }
                }
                finally
                {
                    if (provider != null) provider.Dispose();
                }
            }
        }

        protected void Clear_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in SearchRepeater.Items)
            {
                ITextControl expression = item.FindControl("SearchExpression") as ITextControl;
                expression.Text = "";
            }
        }

        protected void Search_Click(object sender, EventArgs e)
        {
            string template = "&{0}={1}";
            StringBuilder query = new StringBuilder();
            foreach (RepeaterItem item in SearchRepeater.Items)
            {
                ITextControl expression = item.FindControl("SearchExpression") as ITextControl;
                HiddenField column = item.FindControl("ColumnID") as HiddenField;
                if (!String.IsNullOrEmpty(expression.Text))
                {
                    query.AppendFormat(template, column.Value, Server.UrlEncode(expression.Text.Trim()));
                }

            }
            
            string script = @"
<script language=JavaScript>
    window.parent.location ='/Grid.aspx?{0}={1}&pid={3}{2}'
</script>
            ";
            script = string.Format(script, RequestKeyClassificationTreeID, RequestClassificationTreeID.ToString(), query.ToString(), this.Request["pid"]);
            this.ClientScript.RegisterStartupScript(this.GetType(), "mainview", script);
        }
    }
}
