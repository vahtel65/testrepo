using System;
using System.Web.Services;
using Aspect.Domain;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using Aspect.Model.ProductDomain;
using System.Collections.Generic;
using Aspect.Model.SpecificationDomain;
using Aspect.Model.ConfigurationDomain;
using System.Data;
using Aspect.Model.UserDomain;
using System.Web;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace Aspect.UI.Web.Technology
{
    public class PostResult
    {
        public string Message { set; get; }
        public int Opcode { set; get; }
        public DateTime TimeStamp { set; get; }

        public PostResult(string message, int opcode)
        {
            Message = message;
            Opcode = opcode;
        }

        public override string ToString()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(this);
        }
    }

    [ScriptService] 
    public partial class Service : System.Web.UI.Page
    {
        private static DateTime MinusInfinity = new DateTime(1990, 1, 1);
        private static DateTime PlusInfinity = new DateTime(2222, 1, 1);

        private Aspect.Domain.User _user = null;
        public virtual new Aspect.Domain.User User
        {
            get
            {
                if (_user == null)
                {
                    using (Aspect.Model.Authentication.Provider provider = new Aspect.Model.Authentication.Provider())
                    {
                        _user = provider.GetUser(new Guid(base.User.Identity.Name));
                    }
                }
                return _user;
            }
        }        

        /*protected void Page_Load(object sender, EventArgs e)
        {

        }*/

        /// <summary>
        /// tran -> kmhCard
        /// </summary>
        protected static void LoadKmh(Specification_1 kmhCard, transfer tran)
        {
            kmhCard._Product_ID = tran.prod_id;
            kmhCard._Material_ID = tran.material_id;

            kmhCard.sw = tran.sw;
            kmhCard.stw = tran.stw;
            
            kmhCard.ss = tran.ss;
            kmhCard.sp = tran.sp;
            kmhCard.sd = tran.sd; 

            kmhCard.h_got =  tran.gotov_him ? "ГОТОВ" : "НЕ ГОТОВ";
            kmhCard.t_got = tran.gotov_tech ? "ГОТОВ" : "НЕ ГОТОВ";
            kmhCard.s_got = tran.gotov_svar ? "ГОТОВ" : "НЕ ГОТОВ";

            kmhCard.gt_kmh = tran.gotov_kmh ? 1 : 0;            

            kmhCard.no = tran.no;
            kmhCard.cmt = tran.cmt_ogt;
            kmhCard.Route = tran.route;

            kmhCard._dictPVDID = tran.pvd_id == null ? new Guid("C23B8D6F-FFA7-45C2-8F3B-D6CA53566906") : tran.pvd_id.Value;
            kmhCard._dictUMID = tran.um_id;
            kmhCard._dictSFID = tran.sf_id;
                        
            
        }

        /// <summary>
        /// Row of kmhCard -> tran
        /// </summary>
        protected static void FillKmhByRow(CommonDomain provider, DataRow kmhRow, transfer tran, DateTime currentDT)
        {            
            string postfix = "";
            if (kmhRow.Field<Guid?>("id1").HasValue)
            {
                // карточка приказная
                postfix = "1";
                tran.isprikaz = true;
            }
            else
            {
                // карточка стандартная
                if (!kmhRow.Field<Guid?>("id").HasValue)
                {
                    // нет никакой карточки
                    return;
                }
            }

            tran.material_id = kmhRow.Field<Guid?>("_Material_ID" + postfix);
            if (tran.material_id != null)
            {
                tran.material = kmhRow.Field<string>("MaterialSuperpole" + postfix);
            }

            tran.sw = kmhRow.Field<decimal?>("sw" + postfix);
            tran.stw = kmhRow.Field<decimal?>("stw" + postfix);

            tran.ss = kmhRow.Field<string>("ss" + postfix);
            tran.sp = kmhRow.Field<string>("sp" + postfix);
            tran.sd = kmhRow.Field<string>("sd" + postfix);            
            
            /*tran.gotov_him = kmhRow.Field<string>("h_got" + postfix).Trim() == "ГОТОВ" ? true : false;
            tran.gotov_tech = kmhRow.Field<string>("t_got" + postfix).Trim() == "ГОТОВ" ? true : false;
            tran.gotov_svar = kmhRow.Field<string>("s_got" + postfix).Trim() == "ГОТОВ" ? true : false;*/

            tran.gotov_him = kmhRow.Field<DateTime?>("him_date" + postfix).HasValue;
            tran.gotov_tech = kmhRow.Field<DateTime?>("techn_date" + postfix).HasValue;
            tran.gotov_svar = kmhRow.Field<DateTime?>("svar_date" + postfix).HasValue;

            /*tran.gotov_him = kmhRow.Field<DateTime?>("him_date" + postfix).HasValue;
            tran.gotov_tech = kmhRow.Field<DateTime?>("techn_date" + postfix).HasValue;
            tran.gotov_svar = kmhRow.Field<DateTime?>("svar_date" + postfix).HasValue;*/            

            tran.no = kmhRow.Field<decimal?>("no" + postfix);
            tran.cmt_ogt = kmhRow.Field<string>("cmt" + postfix);
            tran.route = kmhRow.Field<string>("Route" + postfix) == null ? "" : kmhRow.Field<string>("Route" + postfix).Trim();

            tran.pvd_id = kmhRow.Field<Guid?>("_dictPVDID" + postfix);
            tran.um_id = kmhRow.Field<Guid?>("_dictUMID" + postfix);
            tran.sf_id = kmhRow.Field<Guid?>("_dictSFID" + postfix);
            tran.ste_id = kmhRow.Field<Guid?>("_dictS_TEID" + postfix);            
            
            /*tran.pvd_id = kmhCard._dictPVDID;
            tran.pvd = tran.pvd_id == null ? "" : kmhCard._dictPVD.pvdn;
            tran.um_id = kmhCard._dictUMID;
            tran.um = tran.um_id == null ? "" : kmhCard._dictUM.umn1;
            tran.sf_id = kmhCard._dictSFID;
            tran.sf = tran.sf_id == null ? "" : kmhCard._dictSF.sfn;

            tran.ste_id = kmhCard._dictS_TEID;
            tran.ste = tran.ste_id == null ? "" : kmhCard._dictS_te.type;*/

            tran.actual = (kmhRow.Field<DateTime>("StartDT" + postfix) < currentDT && kmhRow.Field<DateTime>("FinishDT" + postfix) > currentDT);

            if (!String.IsNullOrEmpty(kmhRow.Field<string>("RouteForChange")))
            {
                // заменяем маршрут из карточки на маршрут по применяемости
                tran.route = kmhRow.Field<string>("RouteForChange");
                tran.route_changed = true;
            }
            tran.last_change_date = kmhRow.Field<DateTime?>("dtle" + postfix);

            /*if (kmhRow.Field<DateTime?>("dtle" + postfix).HasValue)
            {
                tran.last_change_date = kmhRow.Field<DateTime?>("dtle" + postfix).Value;
            }
            else
            {
                tran.last_change_date = DateTime.MinValue;
            }*/
            tran.last_change_user = kmhRow.Field<string>("Name" + postfix);
            //tran.isprikaz = !(kmhCard.OrderArticleID == null);
        }

        /// <summary>
        /// kmhCard -> tran
        /// </summary>
        protected static void FillKmh(Specification_1 kmhCard, transfer tran, DateTime currentDT)
        {
            //tran.prod_id = kmhCard._Product_ID.Value;

            if (kmhCard._Material_ID != null)
            {
                tran.material = kmhCard.Material.superpole;
                tran.material_id = kmhCard._Material_ID;
            }

            tran.sw = kmhCard.sw;
            tran.stw = kmhCard.stw;

            tran.ss = kmhCard.ss;
            tran.sp = kmhCard.sp;
            tran.sd = kmhCard.sd;

            tran.gotov_him = kmhCard.h_got.Trim() == "ГОТОВ" ? true : false;
            tran.gotov_tech = kmhCard.t_got.Trim() == "ГОТОВ" ? true : false;
            tran.gotov_svar = kmhCard.s_got.Trim() == "ГОТОВ" ? true : false;

            tran.gotov_kmh = kmhCard.gt_kmh == 1 ? true : false;

            tran.no = kmhCard.no;
            tran.cmt_ogt = kmhCard.cmt;
            tran.route = kmhCard.Route == null ? "" : kmhCard.Route.Trim();

            
            tran.pvd_id = kmhCard._dictPVDID;
            tran.pvd = tran.pvd_id == null ? "" : kmhCard._dictPVD.pvdn;
            tran.um_id = kmhCard._dictUMID;
            tran.um = tran.um_id == null ? "" : kmhCard._dictUM.umn1;
            tran.sf_id = kmhCard._dictSFID;
            tran.sf = tran.sf_id == null ? "" : kmhCard._dictSF.sfn;

            tran.ste_id = kmhCard._dictS_TEID;
            tran.ste = tran.ste_id == null ? "" : kmhCard._dictS_te.type;            

            tran.actual = (kmhCard.StartDT < currentDT && kmhCard.FinishDT > currentDT);
            tran.isprikaz = !(kmhCard.OrderArticleID == null);

            try 
            {                
                using (UserProvider provider = new UserProvider())
                {
                    tran.last_change_date = kmhCard.dtle;
                    tran.last_change_user = (from u in provider.Users where u.ID == kmhCard.userID select u.Name).Single();
                }
            } catch {}
        }

        [WebMethod]
        public static string GetTechConsist(Guid prod_id, Guid order_id)
        {            
            using (ConfigurationTreeProvider provider = new ConfigurationTreeProvider())
            {
                // главный продукт (чья дата берётся как основная в случае по приказу)
                Product product = null;
                using (ProductProvider productProvider = new ProductProvider())
                {
                    product = productProvider.GetProduct(prod_id);
                }                                           
                                
                // для стандартных продуктов всегда берётся карта актуальная на текущий момент 
                // для приказного продукта
                // * если карточка приказная - на текущий момент 
                // * если карточка стандартная - на момент создания продукта
                //   !! в новой редакции, берётся не дата создания продукта, а дата из
                //   колонки [TechnDates].[gen_date]
                DateTime currentDT = DateTime.Now;
                DateTime actualDT = Guid.Empty.Equals(order_id) ? currentDT : product.CreatedDate;

                // Получаем разузлованный состав
                List<transfer> list = new List<transfer>();
                DataSet data = provider.GetListWithKmh(prod_id, order_id, actualDT, currentDT);

                // добавляем самую верхнюю деталь
                /*transfer tran = new transfer();
                tran.prod_id = product.ID;
                tran.prod_pn1 = product._dictNomen.pn1;
                tran.prod_pn2 = product._dictNomen.pn2;

                #region attachKMH
                // получаем приказную актуальную карту
                Specification_1 order_kmh = null;
                try 
                {
                    if (!Guid.Empty.Equals(order_id))
                    {
                        order_kmh = (from kmh in provider.Specification_1s
                                 where kmh.OrderArticleID == order_id
                                 && kmh._Product_ID == product._dictNomenID
                                 && kmh.StartDT <= currentDT
                                 && kmh.FinishDT > currentDT                                 
                                 select kmh).Single();
                    }
                }
                catch {}

                // получеам стандартную актуальную карту
                Specification_1 stand_kmh = null;
                try 
                {
                    stand_kmh = (from kmh in provider.Specification_1s
                                 where kmh.OrderArticleID == null
                                 && kmh._Product_ID == product._dictNomenID
                                 && kmh.StartDT <= actualDT
                                 && kmh.FinishDT > actualDT
                                 select kmh).Single();
                }
                catch {}

                if (!Guid.Empty.Equals(order_id) && order_kmh != null)
                {
                    FillKmh(order_kmh, tran, currentDT);
                }
                else if (stand_kmh != null)
                {
                    FillKmh(stand_kmh, tran, currentDT);                    
                }                
                #endregion*/

                // получаем маршрут по применяемости
                /*try
                {
                    var route = (from r in provider.Specification_3s
                                 where r._Material_ID == product._dictNomenID
                                 && r._Product_ID == Guid.Empty
                                 && r.StartDT <= currentDT
                                 && r.FinishDT > currentDT
                                 select r).Single();
                    tran.route = route.Route;
                    tran.route_changed = true;
                } catch { }

                list.Add(tran);*/

                try
                {
                    #region getch result of query
                    foreach (DataRow row in data.Tables[0].Rows)
                    {
                        transfer tran = new transfer();

                        tran.unit_id = row.Field<Guid>("unitID");
                        tran.unit_pn1 = row.Field<string>("unitPn1");
                        tran.unit_pn2 = row.Field<string>("unitPn2");

                        tran.prod_id = row.Field<Guid>("prodID");
                        tran.prod_pn1 = row.Field<string>("prodPn1");
                        tran.prod_pn2 = row.Field<string>("prodPn2");

                        tran.level = row.Field<int>("Level");
                        tran.count = row.Field<decimal>("Quantity");
                        tran.group_exchange = row.Field<int?>("GroupToChange");
                        tran.number_exchange = row.Field<int?>("GroupNumber");

                        // дата добавления в состав должна быть только у приказных составов
                        tran.added_date = row.Field<DateTime?>("gen_date");
            

                        /*#region attachKMH
                        // получаем приказную актуальную карту
                        order_kmh = null;
                        try 
                        {
                            if (!Guid.Empty.Equals(order_id))
                            {
                                order_kmh = (from kmh in provider.Specifications
                                         where kmh.tn == 1
                                         && kmh.OrderArticleID == order_id
                                         && kmh._Product_ID == row.Field<Guid>("prodNomenID")
                                         && kmh.StartDT < currentDT
                                         && kmh.FinishDT > currentDT
                                         select kmh).Single();
                            }
                        }
                        catch {}

                        // получеам стандартную актуальную карту
                        stand_kmh = null;
                        try 
                        {
                            stand_kmh = (from kmh in provider.Specifications
                                         where kmh.tn == 1
                                         && kmh.OrderArticleID == null
                                         && kmh._Product_ID == row.Field<Guid>("prodNomenID")
                                         && kmh.StartDT < actualDT
                                         && kmh.FinishDT > actualDT
                                         select kmh).Single();
                        }
                        catch {}

                        if (!Guid.Empty.Equals(order_id) && order_kmh != null)
                        {
                            FillKmh(order_kmh, tran, currentDT);                    
                        }
                        else if (stand_kmh != null)
                        {
                            FillKmh(stand_kmh, tran, currentDT);                    
                        }                
                        #endregion*/
                        FillKmhByRow(provider, row, tran, currentDT);

                        list.Add(tran);
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    string tmpMessage = e.Message;
                }

                Dictionary<Guid,string> PVDs = provider._dictPVDs.AsEnumerable().ToDictionary(i => i.ID, i => i.pvdn);
                Dictionary<Guid, string> UMs = provider._dictUMs.AsEnumerable().ToDictionary(i => i.ID, i => i.umn1);
                Dictionary<Guid, string> SFs = provider._dictSFs.AsEnumerable().ToDictionary(i => i.ID, i => i.sfn);

                foreach (var item in list)
                {
                    try
                    {

                        item.pvd = item.pvd_id.HasValue ? PVDs[item.pvd_id.Value] : "";
                        item.um = item.um_id.HasValue ? UMs[item.um_id.Value] : "";
                        item.sf = item.sf_id.HasValue ? SFs[item.sf_id.Value] : "";
                    }
                    catch { };

                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                // some fix, because setting in web.config not always working as good
                serializer.MaxJsonLength = 50000000;
                return serializer.Serialize(list);
            }
        }

        [WebMethod]
        public static string RequestKmhCard(Guid prod_id, Guid order_id)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                DateTime CurDateTime = DateTime.Now;
                Product prod = provider.GetProduct(prod_id);
                transfer tran = new transfer() { prod_id = prod._dictNomenID.Value };

                #region filling kmh card
                if (Guid.Empty.Equals(order_id))
                {                    
                    var actual_kmh = from kmh in provider.Specification_1s
                                     where kmh.OrderArticleID == null // стандарные (не приказные)
                                     && kmh.StartDT <= CurDateTime // на текущую дату
                                     && kmh.FinishDT > CurDateTime
                                     && kmh._Product_ID == prod._dictNomenID
                                     select kmh;                        

                    if (actual_kmh.Count() > 0)
                    {
                        FillKmh(actual_kmh.First(), tran, CurDateTime);
                        tran.prod_pn2 = actual_kmh.First().Product.superpole;
                    }
                    else
                    {
                        // создание новой КМХ для продукта
                        tran.prod_id = prod._dictNomenID.Value;
                        tran.prod_pn2 = prod._dictNomen.superpole;
                    }
                }
                else
                {                    
                    Product order_unit = provider.GetActualOrderProduct(order_id);
                    Specification_1 actual_kmh = null;

                    // пытаемся получить КМХ для данного приказа                    
                    try
                    {
                        actual_kmh = (from kmh in provider.Specification_1s
                                      where kmh.OrderArticleID == order_id // стандарные (не приказные)
                                      && kmh.StartDT <= CurDateTime // на текущую дату
                                      && kmh.FinishDT > CurDateTime
                                      && kmh._Product_ID == prod._dictNomenID
                                      select kmh).Single();
                    }
                    catch
                    {
                        // пытаемся получить КМХ на момент создания сборки
                        try
                        {
                            var actual_date = (from dates in provider.TechnDates
                                               where dates.OrderArticleID == order_id
                                               && dates._dictNomenID == prod._dictNomenID
                                               select dates.gen_date).SingleOrDefault();
                            if (actual_date == null) actual_date = order_unit.CreatedDate;
                            
                            actual_kmh = (from kmh in provider.Specification_1s
                                          where kmh.OrderArticleID == null // стандарные (не приказные)
                                          && kmh.StartDT <= actual_date
                                          && kmh.FinishDT > actual_date
                                          && kmh._Product_ID == prod._dictNomenID
                                          select kmh).Single();
                        }
                        catch
                        {
                        }
                    }

                    if (actual_kmh == null)
                    {
                        // создание новой КМХ для продукта
                        tran.prod_id = prod._dictNomenID.Value;
                        tran.prod_pn2 = prod._dictNomen.superpole;
                    }
                    else
                    {
                        // заполняем карточку
                        FillKmh(actual_kmh, tran, CurDateTime);
                        tran.prod_pn2 = actual_kmh.Product.superpole;
                    }
                }
                #endregion

                Guid userID = (Guid)HttpContext.Current.Session["userID"];
                List<Guid> userRoles = (from role in provider.UserRoles
                                        where role.UserID == userID
                                        select role.RoleID).ToList();

                var permisions = (from perm in provider.RoleViewPermissions
                                 where perm.PermissionEntityID == new Guid("11F1BC17-20FB-4E93-8389-A55BFA4CA251") // EditorKMH
                                 && userRoles.Contains(perm.RoleID)
                                 && perm.Read
                                 select perm.EntityID).Distinct();
                
                tran.enabled_fields.AddRange(permisions);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(tran);
            }            
        }

        [WebMethod]
        public static string RequestAllRoutes(Guid prod_id)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                DateTime currentDT = DateTime.Now;

                List<transfer_route> list_routes = new List<transfer_route>();
                Product product = provider.GetProduct(prod_id);

                var applicability = (from conf in provider.Configurations
                                    join prod in provider.Products on conf.ProductID equals prod.ID
                                    join ownprod in provider.Products on conf.ProductOwnerID equals ownprod.ID
                                    join ownnomen in provider._dictNomens on ownprod._dictNomenID equals ownnomen.ID
                                    where prod._dictNomenID == product._dictNomenID
                                    select ownnomen.ID).Distinct();

                var all_routes = from route in provider.Specification_3s
                                 where route._Material_ID == product._dictNomenID
                                 && route.StartDT <= currentDT
                                 && route.FinishDT > currentDT
                                 select route;

                // есть применяемость
                // нет маршрута
                var list1 = applicability.Where(appl => !all_routes.Any(rt => rt._Product_ID == appl));
                foreach (Guid ownnomen_id in list1)
                {
                    _dictNomen ownnomen = provider._dictNomens.SingleOrDefault(d => d.ID == ownnomen_id);

                    transfer_route adding_route = new transfer_route();
                    adding_route.prodNomenID = product._dictNomenID.Value;
                    adding_route.unitNomenID = ownnomen.ID;
                    adding_route.unit_pn1 = ownnomen.pn1;
                    adding_route.unit_pn2 = ownnomen.superpole;
                    list_routes.Add(adding_route);
                }

                // есть маршрут
                foreach (Specification_3 route in all_routes )
                {
                    try
                    {
                        _dictNomen unit_nomen = provider._dictNomens.SingleOrDefault(d => d.ID == route._Product_ID);

                        transfer_route adding_route = new transfer_route();
                        adding_route.ID = route.id;
                        adding_route.prodNomenID = product._dictNomenID.Value;
                        adding_route.unitNomenID = route._Product_ID;
                        adding_route.unit_pn1 = unit_nomen.pn1;
                        adding_route.unit_pn2 = unit_nomen.superpole;
                        adding_route.route = route.Route;
                        adding_route.comment = route.cmt;
                        adding_route.lastedit_date = route.dtle;

                        try 
                        {
                            adding_route.lastedit_author = (from u in provider.Users
                                                            where u.ID == route.userID
                                                            select u.Name).Single();
                        } catch {};

                        list_routes.Add(adding_route);
                    }
                    catch { }
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(list_routes);
            }
        }

        [WebMethod]
        public static string SetColumns(List<transfer_column> columns, Guid ClassificationTreeId)
        {
            Guid userID;
            try
            {
                userID = (Guid)HttpContext.Current.Session["userID"];
            }
            catch 
            {
                return new PostResult("Lose user session. Please reconnect.", 102).ToString();
            }

            using (ProductProvider provider = new ProductProvider())
            {
                // delete existing settings
                var columnIDs = columns.Select(it => it.uid);
                var forDelete = from col in provider.ColumnWidths
                                where col.ClassificationTreeID == ClassificationTreeId
                                && col.UserID == userID
                                && columnIDs.Contains(col.ColumnID)
                                select col;
                provider.ColumnWidths.DeleteAllOnSubmit(forDelete);
                provider.SubmitChanges();

                // saving new settings
                foreach (var column in columns)
                {
                    provider.ColumnWidths.InsertOnSubmit(new ColumnWidth()
                    {
                        ID = Guid.NewGuid(),
                        ClassificationTreeID = ClassificationTreeId,
                        UserID = userID,
                        ColumnID = column.uid,
                        Width = column.width,
                        Index = column.position,
                        Hidden = column.hidden
                    });
                }
                provider.SubmitChanges();
            }


            return new PostResult("ok", 0).ToString();
        }

        [WebMethod]
        public static string GetColumns(Guid ClassificationTreeId)
        {
            Guid userID;
            try
            {
                userID = (Guid)HttpContext.Current.Session["userID"];
            }
            catch
            {
                return new PostResult("Lose user session. Please reconnect.", 102).ToString();
            }

            List<transfer_column> list = new List<transfer_column>();
            using (ProductProvider provider = new ProductProvider())
            {
                // select existing settings                
                var columns = from col in provider.ColumnWidths
                                where col.ClassificationTreeID == ClassificationTreeId
                                && col.UserID == userID                                
                                select col;

                foreach (var column in columns)
                {
                    list.Add(new transfer_column()
                    {
                        hidden = column.Hidden,
                        position = column.Index,
                        uid = column.ColumnID,
                        width = (int) column.Width
                    });
                }                
            }
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(list);            
        }
        

        [WebMethod]
        public static string SaveRoute2(transfer saved_route)
        {
            using (ProductProvider provider = new ProductProvider())
            {
                Guid unitDictNomen = (saved_route.unit_id == Guid.Empty) ? Guid.Empty : provider.GetProduct(saved_route.unit_id)._dictNomenID.Value;
                Guid prodDictNomen = provider.GetProduct(saved_route.prod_id)._dictNomenID.Value;

                SaveRouteInternal(unitDictNomen, prodDictNomen, saved_route.route);
            };
           
            return new PostResult("ok", 0).ToString();
        }

        [WebMethod]
        public static string SaveRoute(transfer_route saved_route)
        {
            SaveRouteInternal(saved_route.unitNomenID, saved_route.prodNomenID, saved_route.route);                       
            return new PostResult("ok", 0).ToString();
        }

        public static void SaveRouteInternal(Guid _Product_ID, Guid _Material_ID, string Route)
        {
            DateTime currentDT = DateTime.Now;
            Route = Route.Trim();

            using (ProductProvider provider = new ProductProvider())
            {                
                // получаем действующие (по времени) маршруты по применяемости 
                // для данного продукта в данной сборке
                // ! в идеале такой маршрут должен быть один

                var routes = from rt in provider.Specification_3s
                             where rt._Product_ID == _Product_ID
                             && rt._Material_ID == _Material_ID
                             && rt.StartDT <= currentDT
                             && rt.FinishDT > currentDT
                             select rt;

                // завершаем маршруты текущей датой
                foreach (var route in routes)
                {
                    route.FinishDT = currentDT;
                }

                // создаём новый маршрут по применяемости, если он не пустой
                if (!String.IsNullOrEmpty(Route))
                {                    
                    Specification_3 newRoute = new Specification_3()
                    {
                        id = Guid.NewGuid(),
                        _Product_ID = _Product_ID,
                        _Material_ID = _Material_ID,
                        Route = Route,
                        dtle = currentDT,
                        StartDT = currentDT,
                        FinishDT = DateTime.MaxValue
                    };
                    try
                    {
                        newRoute.userID = (Guid)HttpContext.Current.Session["userID"];
                    }
                    catch { }
                    provider.Specification_3s.InsertOnSubmit(newRoute);
                }
                                              
                provider.SubmitChanges();
            }

        }

        [WebMethod]
        public static string RequestAddMaterials(Guid prod_id, Guid order_id)
        {
            DateTime CurDateTime = DateTime.Now;

            using (ProductProvider provider = new ProductProvider())
            {
                // пытаемся получить приказные доп. материалы для данного приказа
                Product prod = provider.GetProduct(prod_id);

                // пытаемся получить временные метки для химика, сварщика и технолога
                DateTime himDate = DateTime.Now, svarDate = DateTime.Now, technDate = DateTime.Now;
                var dateLabels = provider.TechnDates.SingleOrDefault(it 
                    => it._dictNomenID == prod._dictNomenID
                    && it.OrderArticleID == order_id);
                
                // временные метки учитываются только для приказных составов
                if (dateLabels != null && order_id != Guid.Empty)
                {
                    himDate = dateLabels.him_date.HasValue ? dateLabels.him_date.Value : dateLabels.gen_date.Value;
                    technDate = dateLabels.techn_date.HasValue ? dateLabels.techn_date.Value : dateLabels.gen_date.Value;
                    svarDate = dateLabels.svar_date.HasValue ? dateLabels.svar_date.Value : dateLabels.gen_date.Value;
                }


                // получаем отдельно приказные материалы для химика, сварщика и технолога                
                #region request him kmh
                var him_kmh = from kmh in provider.Specification_2s
                                    where (kmh.OrderArticleID == order_id)
                                        && kmh.StartDT <= CurDateTime
                                        && kmh.FinishDT > CurDateTime
                                        && kmh._Product_ID == prod._dictNomenID
                                        && kmh._dictS_TEID == new Guid("46A00C26-1768-4521-9A33-88336E65D50C")
                                    select kmh;
                if (him_kmh.Count() == 0)
                {
                    him_kmh = from kmh in provider.Specification_2s
                                    where kmh.OrderArticleID == null
                                        && kmh.StartDT <= himDate
                                        && kmh.FinishDT > himDate
                                        && kmh._Product_ID == prod._dictNomenID
                                    select kmh;
                }
                #endregion
                #region request svar kmh
                var svar_kmh = from kmh in provider.Specification_2s
                                    where (kmh.OrderArticleID == order_id)
                                        && kmh.StartDT <= CurDateTime
                                        && kmh.FinishDT > CurDateTime
                                        && kmh._Product_ID == prod._dictNomenID
                                        && kmh._dictS_TEID == new Guid("61931973-A5BD-40CD-92A6-FA802DE6CE6A")
                                    select kmh;
                if (svar_kmh.Count() == 0)
                {
                    svar_kmh = from kmh in provider.Specification_2s
                                    where kmh.OrderArticleID == null
                                        && kmh.StartDT <= svarDate
                                        && kmh.FinishDT > svarDate
                                        && kmh._Product_ID == prod._dictNomenID
                                    select kmh;
                }
                #endregion
                #region request techn kmh
                var techn_kmh = from kmh in provider.Specification_2s
                                    where (kmh.OrderArticleID == order_id)
                                        && kmh.StartDT <= CurDateTime
                                        && kmh.FinishDT > CurDateTime
                                        && kmh._Product_ID == prod._dictNomenID
                                    select kmh;
                if (techn_kmh.Count() == 0)
                {
                    techn_kmh = from kmh in provider.Specification_2s
                                    where kmh.OrderArticleID == null
                                        && kmh.StartDT <= technDate
                                        && kmh.FinishDT > technDate
                                        && kmh._Product_ID == prod._dictNomenID
                                        && kmh._dictS_TEID == new Guid("BCE12453-3AB9-4FCB-8FB3-4811A311B764")
                                    select kmh;
                }
                #endregion

                var actual_kmh = new List<Specification_2>();
                actual_kmh.AddRange(him_kmh);
                actual_kmh.AddRange(techn_kmh);
                actual_kmh.AddRange(svar_kmh);                

                List<transfer_add> list = new List<transfer_add>();
                foreach (Specification_2 iter_kmh in actual_kmh)
                {
                    // пропускаем специальный материал с Guid = {00000000-0000-0000-0000-000000000000}
                    // которые обозначают пустые списки
                    if (iter_kmh._Material_ID == Guid.Empty) continue;

                    transfer_add add_material = new transfer_add()
                    {
                        s_id = iter_kmh._dictSID,
                        material = !iter_kmh._Material_ID.HasValue ? "" : iter_kmh.Material.superpole,
                        um_id = iter_kmh._dictUMID,
                        ste_id = iter_kmh._dictS_TEID,
                        material_id = iter_kmh._Material_ID,
                        no = iter_kmh.no
                    };

                    list.Add(add_material);
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(list);
            }                     
        }

        [WebMethod]
        public static string TechDatesSave(Guid _dictNomenID, Guid OrderArticleID, DateTime? gen_date,
            DateTime? him_date, DateTime? svar_date, DateTime? techn_date)
        {
            try
            {
                using (ProductProvider provider = new ProductProvider())
                {
                    // select row with dates from DB
                    var unit_dates = provider.TechnDates.SingleOrDefault(it => it._dictNomenID == _dictNomenID && it.OrderArticleID == OrderArticleID);

                    // if row doesn't exists create new row
                    if (unit_dates == null)
                    {
                        unit_dates = new TechnDate()
                        {
                            _dictNomenID = _dictNomenID,
                            OrderArticleID = OrderArticleID
                        };
                        provider.TechnDates.InsertOnSubmit(unit_dates);
                    }

                    // clear dates if needed and save dates to row
                    if (gen_date.HasValue)
                    {
                        unit_dates.gen_date = (gen_date == DateTime.MinValue) ? null : gen_date;
                    }
                    if (him_date.HasValue)
                    {
                        unit_dates.him_date = (him_date == DateTime.MinValue) ? null : him_date;
                    }
                    if (svar_date.HasValue)
                    {
                        unit_dates.svar_date = (svar_date == DateTime.MinValue) ? null : svar_date;
                    }
                    if (techn_date.HasValue)
                    {
                        unit_dates.techn_date = (techn_date == DateTime.MinValue) ? null : techn_date;
                    }   

                    // if row contain all nulled fields delete it
                    if (unit_dates.techn_date == null &&
                        unit_dates.him_date == null &&
                        unit_dates.svar_date == null &&
                        unit_dates.gen_date == null)
                    {
                        provider.TechnDates.DeleteOnSubmit(unit_dates);
                    }

                    // save dates to DB
                    provider.SubmitChanges();
                }
                return new PostResult("Ok", 0).ToString();
            }
            catch (Exception e)
            {
                return new PostResult("Unknown exception: " + e.Message, -1).ToString();
            }            
        }

        public class Dicts
        {
            public List<DictItem> PVDs { set; get; }
            public List<DictItem> UMs { set; get; }
            public List<DictItem> SFs { set; get; }
            public List<DictItem> Ss { set; get; }
            public List<DictItem> STEs { set; get; }
            
            public Dicts()
            {
                PVDs = new List<DictItem>();
                UMs = new List<DictItem>();
                SFs = new List<DictItem>();
                Ss = new List<DictItem>();
                STEs = new List<DictItem>();
            }
        }

        [WebMethod]
        public static string RebuildListWares()
        {
            try 
            {
                using (SpecificationProvider provider = new SpecificationProvider())
                {
                    // получаем все приказы, которые содержат год и номер
                    var orders = from o in provider.OrderArticles
                                 where o.year.Length > 0
                                 && o.cco.Length > 0
                                 select o;

                    foreach (var order in orders)
                    {
                        // для каждого приказа получаем приказные продукты
                        List<Product> orderProducts = (from prod in provider.Products
                                                        where prod._dictNomenID == order._dictNomenID
                                                        select prod).ToList();

                        // находим последнюю версию приказного продукта
                        // и вставляем ссылку на неё в приказ
                        if (orderProducts.Count() > 0)
                        {
                            int max_order_version = orderProducts.Max(p => Convert.ToInt32(p.Version));
                            Product actual_product = orderProducts.Where(p => Convert.ToInt32(p.Version) == max_order_version).Single();

                            order.LastVersProductID = actual_product.ID;
                        }
                    }

                    provider.SubmitChanges();
                }
            }
            catch
            {
                return "Error!";
            }
            return "Rebuilding list of wares . . . . . . ok";
        }
        
        [WebMethod]
        public static string RequestListWares()
        {
            using (SpecificationProvider provider = new SpecificationProvider())
            {
                List<transfer_ware> returnList = new List<transfer_ware>(); 
                DataSet dataSet = provider.GetListOfWares(DateTime.Now);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    transfer_ware ware = new transfer_ware();

                    ware.ware_id = row.Field<Guid>("ProductID"); ;
                    ware.ware_pn1 = row.Field<string>("pn1");
                    ware.ware_pn2 = row.Field<string>("superpole");
                    
                    string postfix = "";
                    if (row.Field<Guid?>("id1").HasValue)
                    {
                        // карточка приказная
                        postfix = "1";
                        //tran.isprikaz = true;
                    }
                    else
                    {
                        // карточка стандартная
                        if (!row.Field<Guid?>("id").HasValue)
                        {
                            // нет никакой карточки
                            returnList.Add(ware);
                            continue;
                        }
                    }

                    ware.order_id = (Guid) row["order_id"];
                    ware.created = row.Field<DateTime>("created");
                    ware.LastVersCreatedDate = row.Field<DateTime?>("LastVersCreatedDate");
                    ware.order_number = row.Field<string>("cco").Trim();
                    ware.order_year = Convert.ToInt32(row.Field<string>("year").Trim());

                    ware.date = row.Field<DateTime?>("dtle" + postfix);
                    ware.note = row.Field<string>("cmt" + postfix);

                    //ware.gotov_him = row.Field<string>("h_got" + postfix).Trim() == "ГОТОВ" ? true : false;
                    //ware.gotov_tech = row.Field<string>("t_got" + postfix).Trim() == "ГОТОВ" ? true : false;
                    //ware.gotov_svar = row.Field<string>("s_got" + postfix).Trim() == "ГОТОВ" ? true : false;   

                    ware.gotov_him_date = row.Field<DateTime?>("him_date");
                    ware.gotov_svar_date = row.Field<DateTime?>("svar_date");
                    ware.gotov_techn_date = row.Field<DateTime?>("techn_date");

                    returnList.Add(ware);
                }


                MemoryStream stream = new MemoryStream();

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<transfer_ware>));
                ser.WriteObject(stream, returnList);

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();                
            }
        }

        [WebMethod]
        public static string SaveKmhCard(transfer card, int saveType, Guid order_id)
        {
            DateTime CurDateTime = DateTime.Now;

            // saveType == 1 (сохранить как основную)
            // saveType == 2 (сохранить как приказную)
            // saveType == 3 (как приказную и как основную)

            using (ProductProvider provider = new ProductProvider())
            {                
                // {!} здесь может быть проблемное место, потому что надо оборачивать действия
                // в одну транзакцию

                // проходим два раза по коду
                // 1 - стандартная карточка
                // 2 - карточка по приказу

                for (int index = 1; index <= 2; index++)
                {
                    if (saveType == 2 && index == 1) continue;
                    if (saveType == 1 && index == 2) continue;

                    // получаем все существующие стандарные карты для данного продукта
                    var all_kmh = from kmh in provider.Specification_1s
                                  where Object.Equals(kmh.OrderArticleID, index == 1 ? null : new Guid?(order_id))
                                  && kmh._Product_ID == card.prod_id
                                  select kmh;
                    
                    // переносим полученные данные в карту
                    Specification_1 savedCard = new Specification_1();
                    savedCard.id = Guid.NewGuid();                    
                    savedCard.OrderArticleID = (index == 1 ? null : new Guid?(order_id));
                    LoadKmh(savedCard, card);

                    // устанавливаем время действия карты
                    savedCard.FinishDT = PlusInfinity;
                    if (all_kmh.Count() == 0)
                    {
                        // если других карт нету, даты от -∞ до +∞
                        // savedCard.StartDT = MinusInfinity;
                        savedCard.StartDT = CurDateTime;
                        
                    }
                    else
                    {
                        // если другие карты есть, дата от CurDateTime+1 до +∞
                        savedCard.StartDT = CurDateTime;

                        // выбираем актуальные карты и завершаем дату их действия
                        List<Specification_1> actual_kmh = all_kmh.Where(c => c.StartDT <= CurDateTime && c.FinishDT > CurDateTime).ToList();
                        foreach (Specification_1 iter_kmh in actual_kmh)
                        {
                            iter_kmh.FinishDT = CurDateTime;
                        }
                    }

                    savedCard.dtle = CurDateTime;
                    try
                    {
                        savedCard.userID = (Guid)HttpContext.Current.Session["userID"];
                    }
                    catch { }
                    provider.Specification_1s.InsertOnSubmit(savedCard);
                }

                // Сохраняем изменения
                provider.SubmitChanges();
            }
            PostResult result = new PostResult("ok", 0) { TimeStamp = CurDateTime.ToUniversalTime() };
            return  result.ToString();
        }

        [WebMethod]
        public static string GetApplicability(Guid prodid)
        {
            List<transfer_appl> returnList = new List<transfer_appl>();
            using (UsageConfigurationProvider provider = new UsageConfigurationProvider())
            {
                DataSet dataSet = provider.GetApplicability(prodid);
                
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    returnList.Add(new transfer_appl()
                    {
                        prod_id = row.Field<Guid>("ProductOwnerID"),
                        pn1 = row.Field<string>("pn1"),
                        pn2 = row.Field<string>("pn2"),
                        count = Convert.ToInt32(row["Quantity"]),
                        order_number = row.Field<string>("OrderNumber"),
                        order_year = row.Field<string>("OrderYear"),
                        top_ware = (Convert.ToInt32(row["top"]) != 0),
                        version = Convert.ToInt32(row["Version"]),
                        actual = (Convert.ToInt32(row["Actual"]) != 0)
                    });
                }
            }                       

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(returnList);
        }

        [WebMethod]
        public static string SaveAddMaterials(List<transfer_add> list, Guid prodid, int saveType, Guid order_id, Guid ste_id)
        {
            DateTime CurDateTime = DateTime.Now;

            // saveType == 1 (сохранить как основную)
            // saveType == 2 (сохранить как приказную)
            // saveType == 3 (как приказную и как основную)

            using (ProductProvider provider = new ProductProvider())
            {
                // {!} здесь может быть проблемное место, потому что надо оборачивать действия
                // в одну транзакцию

                // проходим два раза по коду
                // 1 - стандартная карточка
                // 2 - карточка по приказу
                for (int index = 1; index <= 2; index++)
                {
                    if (saveType == 2 && index == 1) continue;
                    if (saveType == 1 && index == 2) continue;

                    // получаем актуальные на текущий момент дополнительные материалы
                    var all_kmh = from kmh in provider.Specification_2s
                                  where Object.Equals(kmh.OrderArticleID, index == 1 ? null : new Guid?(order_id))
                                  && kmh._Product_ID == prodid
                                  && kmh._dictS_TEID == ste_id                                 
                                  select kmh;
                    
                    // перебераем только актуальные карты
                    // и завершаем их по текущей дате
                    foreach (var kmh in all_kmh.Where(it => it.StartDT <= CurDateTime && it.FinishDT > CurDateTime))
                    {
                        kmh.FinishDT = CurDateTime;
                    }

                    // если в списке нет доп. материалов, то вставляем
                    // специальный материал с Guid = {00000000-0000-0000-0000-000000000000}
                    if (list.Count == 0)
                    {
                        list.Add(new transfer_add()
                        {
                            material_id = Guid.Empty
                        });
                    }

                    // создаём новые карты материалов
                    foreach (transfer_add new_kmh in list)
                    {
                        provider.Specification_2s.InsertOnSubmit(new Specification_2()
                        {
                            id = Guid.NewGuid(),
                            _Product_ID = prodid,
                            _Material_ID = new_kmh.material_id,                            
                            // если данный материал уже применялся, то дата начинается с текущего момента
                            // если же данный материал добавлен впервые то дата начаинается с -∞
                            // StartDT = all_kmh.Count(it => it._Material_ID == new_kmh.material_id) > 0 ? CurDateTime : MinusInfinity,
                            StartDT = CurDateTime,
                            FinishDT = PlusInfinity,
                            no = new_kmh.no,
                            _dictUMID = new_kmh.um_id,
                            _dictSID = new_kmh.s_id,
                            _dictS_TEID = ste_id,
                            OrderArticleID = (index == 1 ? null : new Guid?(order_id))
                        });
                    }

                }                

                // Сохраняем изменения
                provider.SubmitChanges();
            }
            PostResult result = new PostResult("ok", 0) { TimeStamp = CurDateTime.ToUniversalTime() };
            return result.ToString();
        }

        [WebMethod]
        public static string RequestDicts(string dicts)
        {            
            using (CommonDomain domain = new CommonDomain())
            {
                string[] required_dicts = dicts.Split(',');
                Dicts list_dicts = new Dicts();

                if (required_dicts.Contains("pvd"))
                {
                    foreach (var item in domain._dictPVDs.OrderBy(d => d.pvdn))
                    {
                        list_dicts.PVDs.Add(new DictItem()
                        {
                            ID = item.ID,
                            Name = item.pvdn
                        });
                    }
                }
                if (required_dicts.Contains("um"))
                {
                    foreach (var item in domain._dictUMs.OrderBy(d => d.umn1))
                    {
                        list_dicts.UMs.Add(new DictItem()
                        {
                            ID = item.ID,
                            Name = item.umn1
                        });
                    }
                }
                if (required_dicts.Contains("sf"))
                {
                    foreach (var item in domain._dictSFs.OrderBy(d => d.sfn))
                    {
                        list_dicts.SFs.Add(new DictItem()
                        {
                            ID = item.ID,
                            Name = item.sfn
                        });
                    }
                }
                if (required_dicts.Contains("s"))
                {
                    foreach (var item in domain._dicts.OrderBy(d => d.so))
                    {
                        list_dicts.Ss.Add(new DictItem()
                        {
                            ID = item.id,
                            Name = item.sn1
                        });
                    }
                }
                if (required_dicts.Contains("ste"))
                {
                    foreach (var item in domain._dictS_tes.OrderBy(d => d.type))
                    {
                        list_dicts.STEs.Add(new DictItem()
                        {
                            ID = item.id,
                            Name = item.type
                        });
                    }
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(list_dicts);                    
            }            
        }
    }
}
