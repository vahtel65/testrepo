using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using Aspect.Domain;

namespace Aspect.UI.Web.Administrator
{
    public partial class Users : Basic.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (CommonDomain domain = new CommonDomain())
            {                
                var usersInfo =  
                    from u in domain.Users
                    select new
                    {
                        userID = u.ID,
                        userName = u.Name,
                        userRoles = string.Join(", ",
                            (from ur in domain.UserRoles
                             join r in domain.Roles on ur.RoleID equals r.ID
                             where ur.UserID == u.ID
                             select r.Name).ToArray()
                            )
                    };

                UsersStore.DataSource = usersInfo;
                UsersStore.DataBind();
            }
        }
    }
}
