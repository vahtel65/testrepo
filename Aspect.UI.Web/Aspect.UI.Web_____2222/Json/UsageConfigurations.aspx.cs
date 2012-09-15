using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

using Aspect.Domain;
using Aspect.Model.ProductDomain;

namespace Aspect.UI.Web.Json
{
    class UsageRow
    {
        public Guid uid;
        public string superpole;
        public string version;
        public string actual;
        public string orderyear;
        public string ordernumber;
        public UsageRow(Guid uid, string superpole, string version, string actual, string orderyear, string ordernumber)
        {
            this.uid = uid;
            this.superpole = superpole;
            this.version = version;
            this.actual = actual;
            this.orderyear = orderyear;
            this.ordernumber = ordernumber;            
        }
    }

    class UsageResponse
    {
        public List<UsageRow> rows = new List<UsageRow>();
        public int totalCount = 0;
    }
    
    public partial class UsageConfigurations : System.Web.UI.Page
    {
        public string jsonResponse;

        protected void Page_Load(object sender, EventArgs e)
        {            
            using (ProductProvider provider = new ProductProvider())
            {
                Guid uid = new Guid(Request["uid"]);

                List<Aspect.Domain.Configuration> confs = provider.Configurations.Where(c => c.ProductID == uid).ToList();
                List<Guid> ownerProdsId = confs.DistinctBy(c => c.ProductOwnerID).Select(c => c.ProductOwnerID).ToList();

                List<Product> ownerProds = provider.Products.Where(p => ownerProdsId.Contains(p.ID)).ToList();

                UsageResponse response = new UsageResponse();
                foreach (Product prod in ownerProds)
                {
                    if (provider.IsMainVersion(prod.ID) || provider.isPrikazVersion(prod.ID))
                    {
                        UsageRow row = new UsageRow(prod.ID, prod._dictNomen.superpole, prod.Version,
                            prod.MainVersion,
                            prod.OrderYear, prod.OrderNumber);
                        response.rows.Add(row);
                    };
                }

                response.totalCount = response.rows.Count;

                JavaScriptSerializer js = new JavaScriptSerializer();
                jsonResponse = js.Serialize(response);
            }
        }
    }
}
