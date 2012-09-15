using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Aspect.Model.UserDomain;
using Aspect.UI.Web.Basic;

namespace Aspect.UI.Web
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected Repeater PreviouseRepeater;
        protected Repeater FavoriteRepeater;
        protected Repeater PreDefinedRepeater;
        protected PlaceHolder menuAdministrator;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Ext.Net.X.IsAjaxRequest)
            {
                using (UserProvider provider = new UserProvider())
                {
                    if (!provider.IsAdministrator((this.Page as PageBase).Roles))
                    {

                        menuAdministrator.Visible = false;
                    }

                    PreviouseRepeater.DataSource = provider.GetLastViewed((this.Page as PageBase).User.ID);
                    PreviouseRepeater.DataBind();

                    PreDefinedRepeater.DataSource = provider.GetPreDefined((this.Page as PageBase).User.ID);
                    PreDefinedRepeater.DataBind();
                }
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            if (!Ext.Net.X.IsAjaxRequest)
            {
                using (UserProvider provider = new UserProvider())
                {
                    FavoriteRepeater.DataSource = provider.GetFavorites((this.Page as PageBase).User.ID);
                    FavoriteRepeater.DataBind();
                }
                base.OnPreRender(e);
            }
        }

        public string ServerUrl
        {
            get
            {
                string absoluteUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                if (string.IsNullOrEmpty(absoluteUrl))
                    return string.Empty;

                int pathStart = absoluteUrl.IndexOf(System.Web.HttpContext.Current.Request.Url.LocalPath);
                return absoluteUrl.Remove(pathStart);
            }
        }
    }
}
