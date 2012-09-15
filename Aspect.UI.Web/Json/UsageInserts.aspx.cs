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
    /*
    class UsageRow
    {
        public Guid uid;
        public string superpole;
       public string qdu; //120420
      public string qdu_new; //120420

        public string version;
        public string actual;
        public string orderyear;
        public string ordernumber;
        public UsageRow(Guid uid, string superpole, string qdu, string qdu_new, string version, string actual, string orderyear, string ordernumber) //120420
        {
            this.uid = uid;
            this.superpole = superpole;
            this.qdu = qdu; //120420
            this.qdu_new = qdu_new; //120420

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
 */
  //  public partial class UsageInserts : System.Web.UI.Page
    public partial class UsageInserts : Basic.PageBase
    {
        public string jsonResponse;

        protected void Page_Load(object sender, EventArgs e)
        {
              using (ProductProvider provider = new ProductProvider())
            {

                List<Guid> lg = new List<Guid>();
                IOrderedEnumerable<Guid> lg_q;

                lg_q = null;
                  //     Dictionary<Guid, Guid> MultiBuffer= new Dictionary<Guid, Guid>();
                //    MultiBuffer=this.Session["MultiBuffer"];


                try
                {
                    if (provider != null)
                    {
                        if ((this.MultiBuffer.Count > 0))
                        {
                            //                            lg_q = this.MultiBuffer.Keys.ToList().OrderBy(x => x);
                            lg = this.MultiBuffer.Keys.ToList();
                        }
                        else return;
                    }

                }
                finally
                {
  //                  if (provider != null) provider.Dispose();
                }


                List<Product> lp = new List<Product>();
                IOrderedEnumerable<Product> lp_q;
                lp_q=null;
                UsageResponse response = new UsageResponse();
                foreach (Guid prodID in lg)
                {
                    if (provider.IsMainVersion(prodID))  // || provider.isPrikazVersion(prodID))
                    {
                        Product prod = provider.GetProduct(prodID);
                        lp.Add(prod);
                      };
                }
                if (lp.Count == 0)
                {

                    return;

                }
                lp_q = lp.OrderBy(x => x._dictNomen.superpole);

                foreach (Product prod in lp_q)
                {
                     UsageRow row = new UsageRow(prod.ID, prod._dictNomen.superpole, "1","1","1", //bvv120420
                        "1",
                        "", "","0");
                    response.rows.Add(row);
                }









                response.totalCount = response.rows.Count;
                //response.rows = response.rows.OrderBy(c => c.superpole);
                JavaScriptSerializer js = new JavaScriptSerializer();
                jsonResponse = js.Serialize(response);

                
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                  
                 /* 
                  
                  Guid uid = new Guid(Request["uid"]);

                //List<Aspect.Domain.Configuration> confs = provider.Configurations.Where(c => c.ProductID == uid).ToList(); //bvv120420

                /*
                                var s = from confs0 in provider.Configurations
                                        join pp in provider.ProductProperties on confs0.ProductOwnerID equals pp.ProductID
                                        where confs0.ProductID == uid && pp.PropertyID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552") && pp.Value == "1"
                                        select confs0;//bvv120420

                 * 
                 * 
                 * 
                 * 
                 * 
                 * List<Aspect.Domain.Configuration> confs = s.ToList<Aspect.Domain.Configuration>(); //bvv120420

                 */

/*
                var s = from confs0 in provider.Configurations
                        join pp in provider.ProductProperties on confs0.ProductOwnerID equals pp.ProductID
                        join p in provider.Products on confs0.ProductOwnerID equals p.ID
                        join n in provider._dictNomens on p._dictNomenID equals n.ID
                        where confs0.ProductID == uid && pp.PropertyID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552") && pp.Value == "1"
                        orderby n.superpole
                        select confs0;//bvv120420



                 List<Aspect.Domain.Configuration> confs = s.OrderBy(c=>c.Product1._dictNomen.superpole).ToList<Aspect.Domain.Configuration>(); //bvv120420




              

                
                
                
                
                
                
                
                
                
                


                List<Guid> ownerProdsId = confs.DistinctBy(c => c.ProductOwnerID).Select(c => c.ProductOwnerID).ToList();

                List<Product> ownerProds = provider.Products.Where(p => ownerProdsId.Contains(p.ID)).OrderBy(p=>p._dictNomen.superpole).ToList();//bvv120420

                UsageResponse response = new UsageResponse();
                foreach (Product prod in ownerProds)
                {
                    if (provider.IsMainVersion(prod.ID) || provider.isPrikazVersion(prod.ID))
                    {

                        var ff = from confs0 in provider.Configurations
                                where confs0.ProductID == uid && confs0.ProductOwnerID == prod.ID
                                select confs0.Quantity;//bvv120420



                        UsageRow row = new UsageRow(prod.ID, prod._dictNomen.superpole, ff.First().ToString(), ff.First().ToString(), prod.Version, //bvv120420
                            prod.MainVersion,
                            prod.OrderYear, prod.OrderNumber);
                        response.rows.Add(row);
                    };
                }

                response.totalCount = response.rows.Count;
                //response.rows = response.rows.OrderBy(c => c.superpole);
                JavaScriptSerializer js = new JavaScriptSerializer();
                jsonResponse = js.Serialize(response);
 */
              
              
              
              }
        }
    }
}
