using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using Aspect.Model;

namespace Aspect.UI.Web.Basic
{
    public abstract class ContentPageBase : PageBase
    {
        protected virtual Guid RequestClassificationTreeID
        {
            get
            {
                try
                {
                    return new Guid(Request[RequestKeyClassificationTreeID]);
                }
                catch
                {
                    //this.RedirectToErrorPage();
                    return Guid.Empty;
                }
            }
        }

        private ClassifiacationTypeView classifiacationTypeView = ClassifiacationTypeView.NotDefined;
        protected virtual ClassifiacationTypeView ClassifiacationTypeView
        {
            get
            {
                if (classifiacationTypeView == ClassifiacationTypeView.NotDefined)
                {
                    classifiacationTypeView = Common.GetClassifiacationTypeView(RequestClassificationTreeID/*new Guid(ClassificationView.SelectedNode.Value)*/);
                }
                return classifiacationTypeView;
            }
        }

        private List<Aspect.Domain.SearchExpression> searchConditions = null;
        protected virtual List<Aspect.Domain.SearchExpression> SearchConditions
        {
            get
            {
                if (searchConditions == null)
                {
                    searchConditions = new List<Aspect.Domain.SearchExpression>();
                    foreach (string item in Request.QueryString.AllKeys)
                    {
                        try
                        {
                            Guid key = new Guid(item);
                            string value = Server.UrlDecode(Request.QueryString[item]);
                            if (!key.Equals(Guid.Empty))
                            {
                                searchConditions.Add(new Aspect.Domain.SearchExpression()
                                {
                                    FieldID = key,
                                    FieldValue = value
                                });
                            }
                        }
                        catch { }
                    }
                }
                return searchConditions;
            }
        }
    }
}
