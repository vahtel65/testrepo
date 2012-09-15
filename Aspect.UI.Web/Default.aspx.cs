using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aspect.UI.Web
{
    public partial class _Default : Basic.PageBase
    {
        protected CustomValidator ValidatorAuthenticationFaild;

        protected TextBox UserNameTextBox;
        protected TextBox UserPasswordTextBox;
        protected Literal CustomerCompanyName;
        protected Image PerfectStoreLogo;
        protected Image CustomerLogo;
        protected DropDownList UsersDropDown;
        protected Literal LastLogin;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                List<Aspect.Domain.User> list = new List<Aspect.Domain.User>();
                using (Aspect.Model.UserDomain.UserProvider provider = new Aspect.Model.UserDomain.UserProvider())
                {
                    list = provider.GetUsers();
                    UsersDropDown.DataSource = list;
                }
                try
                {
                    HttpCookie cookie = this.Request.Cookies["LastLogin"];
                    if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                    {
                        if (list.Where(l => l.ID == new Guid(cookie.Value)).Count() > 0)
                        {
                            UsersDropDown.SelectedValue = cookie.Value;
                            //LastLogin.Text = UsersDropDown.SelectedItem.Text;
                        }
                        
                    }
                }
                catch (Exception)
                {
                }
                Page.DataBind();
            }
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            using (Aspect.Model.Authentication.Provider provider = new Aspect.Model.Authentication.Provider())
            {
                Aspect.Domain.User user = provider.SelectFromListAuthentication(new Guid(UsersDropDown.SelectedValue), UserPasswordTextBox.Text);
                if (user != null)
                {
                    HttpCookie cookie = new HttpCookie("LastLogin", /*user.Name*/user.ID.ToString());
                    cookie.Expires = DateTime.Now.AddYears(3);
                    this.Response.Cookies.Add(cookie);
                    this.Authenticate(user.ID);
                }
                else
                {
                    ValidatorAuthenticationFaild.IsValid = false;
                }
            }
        }
    }
}
