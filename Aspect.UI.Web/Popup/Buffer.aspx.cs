using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Aspect.Domain;

namespace Aspect.UI.Web.Popup
{
    public partial class Buffer : Basic.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindList();
            }
        }

        public HyperLink CompareButton;

        protected Repeater BufferRepeater;
        private void BindList()
        {
            using (Aspect.Model.ConfigurationDomain.EditConfigurationProvider provider = new Aspect.Model.ConfigurationDomain.EditConfigurationProvider())
            {
                List<Aspect.Domain.Product> list  = base.MultiBuffer.Keys.ToList().ConvertAll(delegate(Guid id)
                {
                    return provider.GetProduct(id);
                });
                list = list.Where(l => l != null).ToList();

                BufferRepeater.DataSource = list;
                BufferRepeater.DataBind();

                /* управление кнопкой "Сравнить спецификации" */
                if (CountAll() == 2)
                {
                    CompareButton.NavigateUrl = String.Format("~/Configuration/Compare.aspx?PID1={0}&PID2={1}", getFirstItem(), getSecondItem());
                    CompareButton.Visible = true;
                }
                else
                {
                    CompareButton.Visible = false;
                }
            }
        }

        protected Guid getFirstItem()
        {
            return MultiBuffer.Keys.ToList()[0];
        }

        protected Guid getSecondItem()
        {
            return MultiBuffer.Keys.ToList()[1];
        }

        protected int CountAll()
        {
            return MultiBuffer.Count();
        }

        protected void DeleteAll(object sender, CommandEventArgs e)
        {
            this.ClearMultiBuffer();
            this.BindList();
        }

        protected void Delete(object sender, CommandEventArgs e)
        {
            Guid id = new Guid(e.CommandArgument.ToString());
            this.RemoveProductFromMultiBuffer(id);
            this.BindList();       
        }
    }
}
