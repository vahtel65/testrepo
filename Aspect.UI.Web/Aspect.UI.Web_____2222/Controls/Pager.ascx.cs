using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Aspect.UI.Web.Controls
{
    public partial class Pager : UserControl
    {
        
        protected LinkButton BtnPrevious;
        protected LinkButton BtnNext;
        protected LinkButton BtnEnd;
        protected LinkButton BtnBegin;
        protected Repeater PagesRepeater;
        protected PlaceHolder PreviousDots;
        protected PlaceHolder NextDots;
        protected Label TotalRecordsLabel;
        public delegate void PagerEventHandler(object sender, PagerEventArgs ea);
        public event PagerEventHandler CurrentPageChanged;

        [Browsable(true)]
        public int PageSize
        {
            get
            {
                if (ViewState["PageSize"] != null)
                {
                    return (int)ViewState["PageSize"];
                }
                return 20;
            }
            set
            {
                ViewState["PageSize"] = value;

                SetupUI();
            }
        }

        public void JumpToItem(int itemIndex)
        {
            if (itemIndex < 0)
            {
                CurrentPage = 0;
            }
            else
            {
                if (TotalRecords > 0) CurrentPage = ((int)Math.Ceiling((itemIndex + 1) * 1.0f / PageSize) - 1);
            }
        }

        public int TotalPages
        {
            get
            {
                if (PageSize == 0) return 0;
                return (int)Math.Ceiling(TotalRecords * 1.0f / PageSize);
            }
        }

        [Browsable(true)]
        public int TotalRecords
        {
            get
            {
                if (ViewState["TotalRecords"] != null)
                {
                    return (int)ViewState["TotalRecords"];
                }
                return 0;
            }
            set
            {
                
                ViewState["TotalRecords"] = value;
                //if (CurrentPage >= TotalPages) CurrentPage = TotalPages - 1;
                SetupUI();
            }
        }

        public int Range
        {
            get
            {
                if(ViewState["Range"] != null)
                {
                    return (int)ViewState["Range"];
                }
                return 8;
            }
            set
            {
                ViewState["Range"] = value;
            }
        }

        [Browsable(true)]
        public int CurrentPage
        {
            get
            {
                if (ViewState["CurrentPage"] != null)
                {
                    return (int)ViewState["CurrentPage"];
                }
                return 0;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Must be greater or equal to zero");
                }

                if (value >= TotalPages && value>0)
                {
                    throw new ArgumentOutOfRangeException("value", "Must be less than " + TotalPages);
                }

                ViewState["CurrentPage"] = value;

                SetupUI();

                /*if (CurrentPageChanged != null)
                {
                    CurrentPageChanged(this, new PagerEventArgs(CurrentPage, PageSize, TotalPages));
                }*/
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetupUI();
        }
        //private int range = 8;
        private void SetupUI()
        {
            BtnBegin.Visible = BtnPrevious.Visible = CurrentPage != 0;
            BtnEnd.Visible = BtnNext.Visible = CurrentPage < (TotalPages - 1);

            TotalRecordsLabel.Text = this.TotalRecords.ToString();
            //LblPagerInfo.Text = string.Format("Page {0} of {1}", CurrentPage + 1, TotalPages);

            List<int> listPages = new List<int>();

            //--

            if (TotalPages > 1)
            {
                int start = this.CurrentPage - Range;
                if (start < 0) start = 0;
                int end = this.CurrentPage + Range;
                if (end > TotalPages - 1) end = TotalPages - 1;
                for (int i = start; i <= end; i++)
                {
                    listPages.Add(i);
                }
                PreviousDots.Visible = (start > 0);
                NextDots.Visible = (end < (TotalPages - 1));
                /*for (int pageIndex = 0; pageIndex < TotalPages; pageIndex++)
                {
                    listPages.Add(pageIndex);
                }*/
            }

            PagesRepeater.DataSource = listPages;
            PagesRepeater.DataBind();
        }

        protected void Fire()
        {
            if (CurrentPageChanged != null)
            {
                CurrentPageChanged(this, new PagerEventArgs(CurrentPage, PageSize, TotalPages));
            }
        }

        protected void BtnPage_Click(object sender, EventArgs e)
        {
            LinkButton btnPage = sender as LinkButton;
            HiddenField pageIndexField = btnPage.NamingContainer.FindControl("PageIndex") as HiddenField;

            int pageIndex;
            if(Int32.TryParse(pageIndexField.Value,out pageIndex))
            {
                CurrentPage = pageIndex;
                Fire();
            }
        }

        protected void BtnPrevious_Click(object sender, EventArgs e)
        {
            CurrentPage -= 1;
            Fire();
        }

        protected void BtnNext_Click(object sender, EventArgs e)
        {
            CurrentPage += 1;
            Fire();
        }

        protected void BtnEnd_Click(object sender, EventArgs e)
        {
            CurrentPage = TotalPages - 1;
            Fire();
        }

        protected void BtnBegin_Click(object sender, EventArgs e)
        {
            CurrentPage = 0;
            Fire();
        }

        protected void PagesRepeater_ItemDataBound(object source, RepeaterItemEventArgs e)
        {
            /*if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int pageIndex = (int)e.Item.DataItem;
                if (pageIndex == CurrentPage)
                {
                    LinkButton pageButton = e.Item.FindControl("BtnPage") as LinkButton;
                    Label pageLabel = e.Item.FindControl("BtnActivePage") as Label;
                    pageButton.Visible = false;
                    pageLabel.Visible = true;
                }
            }*/
        }

        public void Reset()
        {
            ViewState["TotalRecords"] = 0;
            ViewState["CurrentPage"] = 0;
            SetupUI();
        }
    }

    public class PagerEventArgs : EventArgs
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public new static PagerEventArgs Empty { get { return new PagerEventArgs(); } }

        public PagerEventArgs() { }

        public PagerEventArgs(int currentPage, int pageSize, int totalPages)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
        }
    }
}