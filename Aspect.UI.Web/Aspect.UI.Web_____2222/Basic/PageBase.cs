using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using System.Web.Security;
using System.Security.Permissions;
using System.Security.Principal;

using Aspect.Domain;

namespace Aspect.UI.Web.Basic
{
    public abstract class PageBase : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ErrorPage = "error.htm";
        }

        public virtual void AddVisitHistory()
        {
        }

        protected void RedirectToErrorPage()
        {
            Response.Redirect("~/error.htm", true);
        }
        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            if (!Ext.Net.X.IsAjaxRequest)
            {
                this.AddVisitHistory();
            }
        }                     

        // словарь [id продукта] = id конфигурации с этим продуктом 
        protected Dictionary<Guid, Guid> MultiBuffer
        {
            get
            {
                if (this.Session["MultiBuffer"] == null)
                {
                    this.Session["MultiBuffer"] = new Dictionary<Guid, Guid>();
                }
                return this.Session["MultiBuffer"] as Dictionary<Guid, Guid>;
            }
            private set
            {
                this.Session["MultiBuffer"] = value;
            }
        }

        // добавление продуктов в мультибуфер ( configuration = Guid.Empty )
        public void AddProductToMultiBuffer(IEnumerable<Guid> products)
        {
            foreach (Guid product in products)
            {
                MultiBuffer[product] = Guid.Empty;
            }
        }

        // добавление конфигураций в мультибуфер
        public void AddConfigurationsToMultiBuffer(IEnumerable<Guid> confids)
        {
            using (CommonDomain domain = new CommonDomain())
            {
                foreach (Guid confid in confids)
                {
                    var conf = domain.Configurations.Where(c => c.ID == confid).Single();
                    MultiBuffer[conf.ProductID] = conf.ID;
                }
            }              
        }
        public void RemoveProductFromMultiBuffer(Guid productid)
        {
            MultiBuffer.Remove(productid);
        }

        // очистка мультибуфера от всех продуктов и их конфигураций
        public void ClearMultiBuffer()
        {
            MultiBuffer.Clear();
        }

        private Aspect.Domain.User user = null;
        public virtual new Aspect.Domain.User User
        {
            get
            {
                if (user == null)
                {
                    using (Aspect.Model.Authentication.Provider provider = new Aspect.Model.Authentication.Provider())
                    {
                        user = provider.GetUser(new Guid(base.User.Identity.Name));
                    }
                }
                return user;
            }
        }
        private List<Guid> roles = null;
        public List<Guid> Roles
        {
            get
            {
                if (roles == null)
                {
                    using (Aspect.Model.Authentication.Provider provider = new Aspect.Model.Authentication.Provider())
                    {
                        roles = provider.GetUserRoles(this.User.ID);
                    }
                }
                return roles;
            }
        }

        public void Authenticate(Guid userID /*, string[] roles*/)
        {
            Session["userID"] = userID;

            string[] roles = new string[] { };
            string roleString = String.Join(",", roles);
            bool isPersistent = true;
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userID.ToString(), DateTime.Now, DateTime.Now.AddHours(12), isPersistent, roleString, FormsAuthentication.FormsCookiePath);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            Response.Cookies.Add(cookie);
            FormsIdentity identity = new FormsIdentity(ticket);
            HttpContext.Current.User = new GenericPrincipal(identity, roles);

            if (!String.IsNullOrEmpty(Request["ReturnUrl"]))
            {
                Response.Redirect(Request["ReturnUrl"]);
            }
            else
            {
                using (Aspect.Model.UserDomain.UserProvider provider = new Aspect.Model.UserDomain.UserProvider())
                {
                    List<Aspect.Domain.UserMenuItem> list = provider.GetLastViewed(userID);
                    if (list.Count > 0) Response.Redirect(string.Format("~{0}", list[0].Url ));
                    else Response.Redirect("/Grid.aspx?cid=55c7b455-0638-4acb-ac2e-5b4992e48462&pid=55c7b455-0638-4acb-ac2e-5b4992e48462");
                }
                //Response.Redirect(FormsAuthentication.DefaultUrl);
                
            }
        }

        #region RequestKeys
        protected const string RequestKeyClassificationTreeID = "cid";
        protected const string RequestKeyProductID = "pid";
        protected const string RequestKeyFieldPlaceHolder = "fph";
        #endregion

        #region Thickbox Helper
        protected void PopupIframeInitializationString(System.Web.UI.Control control, string title, string url, int width, int height)
        {
            string urlstring = this.buildPopupUrl(url, width, height);
            urlstring = string.Format("javascript:tb_show('{0}','{1}','');", title, urlstring);
            ((IAttributeAccessor)control).SetAttribute("onclick", urlstring);
        }

        protected void PopupIframeInitializationStringWithProduct(System.Web.UI.Control control, string title, string url, int width, int height)
        {
            string urlstring = this.buildPopupUrl(url, width, height);
            urlstring = string.Format("javascript:tb_show1('{0}','{1}','');", title, urlstring);
            ((IAttributeAccessor)control).SetAttribute("onclick", urlstring);
        }

        public string buildPopupUrl(string HelpURL, int width, int height)
        {
            Uri uri = new Uri(HelpURL, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri(Context.Request.Url, uri);
            }

            UriBuilder builder = new UriBuilder(uri);
            string query = builder.Query;
            if (query.StartsWith("?"))
            {
                query = query.Substring(1);
            }

            if (query != string.Empty)
            {
                query += "&";
            }

            query += "caller=" + HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString()) +
                             string.Format("&KeepThis=true&TB_iframe=true&width={0}&height={1}&modal=true", width, height);

            builder.Query = query;

            string urlComplete = builder.ToString();
            return urlComplete;
        }
        #endregion
    }
}
