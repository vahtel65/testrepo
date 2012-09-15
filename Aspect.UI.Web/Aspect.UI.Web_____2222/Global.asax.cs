using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Security.Permissions;
using System.Security.Principal;

namespace Aspect.UI.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authenticationCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authenticationCookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authenticationCookie.Value);
                FormsIdentity identity = new FormsIdentity(ticket);
                HttpContext.Current.User = new GenericPrincipal(identity, ticket.UserData.Split(','));
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}