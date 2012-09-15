using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Aspect.UI.Web
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SignOut : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            System.Web.Security.FormsAuthentication.SignOut();
            context.Response.Redirect(System.Web.Security.FormsAuthentication.LoginUrl);
            //System.Web.Security.FormsAuthentication.RedirectToLoginPage();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
