using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspect.Domain;
using System.Web.Configuration;
using Aspect.Model.ProductDomain;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;

namespace Aspect.UI.Web.Popup
{
    public partial class ProductActions : Basic.PageBase
    {
        protected HyperLink Edit;
        protected HyperLink EditReadOnly;
        protected HyperLink Usage;
        protected HyperLink UsageWaves;
        protected HyperLink View;
        protected HyperLink Tree;
        protected HyperLink TreeEx;
        protected HyperLink TreeWithKmh;
        protected HyperLink EditObject;
        protected HyperLink NewObject;
        protected HyperLink NewObjectWithConfs;
        protected HyperLink ViewObject;
        protected HyperLink AttachedFiles;
        protected HyperLink EditKmh;

        public bool TechnologyEnable
        {
            get
            {
                return Convert.ToBoolean(WebConfigurationManager.AppSettings["TechnologyEnable"]);
            }
        }

        public string AttachedFilesApplication
        {
            get
            {
                return WebConfigurationManager.AppSettings["AttechedFilesApplication"];
            }
        }

        private string MD5Hash(string instr)
        {
            string strHash = string.Empty;

            foreach (byte b in new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(instr)))
            {
                strHash += b.ToString();
            }
            return strHash;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                Guid cid = Guid.Empty;
                Guid eid = Guid.Empty;
                try
                {
                    cid = new Guid(Request["CID"]);
                    eid = new Guid(Request["ID"]);
                }
                catch (Exception)
                {
                    Edit.Visible = false;
                    Usage.Visible = false;
                    View.Visible = false;
                    Tree.Visible = false;
                    TreeEx.Visible = false;
                    EditObject.Visible = false;
                    NewObject.Visible = false;
                    ViewObject.Visible = false;
                    return;
                }                              

                if (cid != Guid.Empty) //dictionary entity
                {
                    if (cid == new Guid("316c6bc7-d883-44c8-aae0-602f49c73595"))
                    {
                        // добавляем пункт прикрепляемые файлы                        
                        string secretKey = MD5Hash(String.Format("{0}{1}{2}", AttachedFilesApplication, User.ID, new Guid(Request["ID"])));
                        string attachedUrl = @"javascript:CallAttachedFiles('{0}','{1}','{2}','{3}')";
                        AttachedFiles.NavigateUrl = string.Format(attachedUrl, AttachedFilesApplication, this.User.ID, new Guid(Request["ID"]), secretKey);
                    }
                    else
                    {
                        AttachedFiles.Visible = false;
                    }

                    NewObjectWithConfs.Visible = false;
                    Edit.Visible = false;
					EditReadOnly.Visible = false;
                    Usage.Visible = false;
                    UsageWaves.Visible = false;
                    View.Visible = false;
                    Tree.Visible = false;
                    TreeEx.Visible = false;
                    TreeWithKmh.Visible = false;
                    EditKmh.Visible = false;
                    EditObject.NavigateUrl = string.Format("~/Editarea/EditDict.aspx?ID={0}&new=false&DictID={1}", Request["ID"], cid);
                    NewObject.NavigateUrl = string.Format("~/Editarea/EditDict.aspx?ID={0}&new=true&DictID={1}", Request["ID"], cid);                    
                    ViewObject.NavigateUrl = string.Format("~/Editarea/ViewDict.aspx?ID={0}&new=false&DictID={1}", Request["ID"], cid);

                    using (Aspect.Model.DictionaryDomain.DictionaryProvider provider = new Aspect.Model.DictionaryDomain.DictionaryProvider())
                    {
                        if (!provider.AllowEdit(this.Roles, cid))
                        {
                            EditObject.Visible = false;
                            NewObject.Visible = false;
                        }
                    }
                }
                else //product entity
                {
                    NewObject.Text = "Добавить новую версию";
                    NewObjectWithConfs.NavigateUrl = string.Format("{0}?ID={1}", NewObjectWithConfs.NavigateUrl, Request["ID"]);
                    Edit.NavigateUrl = string.Format("{0}?ID={1}", Edit.NavigateUrl, Request["ID"]);
                    EditReadOnly.NavigateUrl = string.Format("{0}?ID={1}&mode=view", EditReadOnly.NavigateUrl, Request["ID"]);
                    Usage.NavigateUrl = string.Format("{0}?ID={1}", Usage.NavigateUrl, Request["ID"]);
                    UsageWaves.NavigateUrl = string.Format("{0}?prodid={1}", UsageWaves.NavigateUrl, Request["ID"]);
                    View.NavigateUrl = string.Format("{0}?ID={1}", View.NavigateUrl, Request["ID"]);
                    Tree.NavigateUrl = string.Format("{0}?ID={1}", Tree.NavigateUrl, Request["ID"]);
                    TreeEx.NavigateUrl = string.Format("{0}?ID={1}", TreeEx.NavigateUrl, Request["ID"]);                    

                    EditObject.NavigateUrl = string.Format("{0}?ID={1}&new=false", EditObject.NavigateUrl, Request["ID"]);
                    NewObject.NavigateUrl = string.Format("{0}?ID={1}&new=true", NewObject.NavigateUrl, Request["ID"]);                    

                    ViewObject.NavigateUrl = string.Format("{0}?ID={1}&new=false", ViewObject.NavigateUrl, Request["ID"]);                    

                    using (Aspect.Model.ProductDomain.ProductProvider provider = new Aspect.Model.ProductDomain.ProductProvider())
                    {
                        if (!provider.AllowEdit(this.Roles, eid))
                        {
                            EditObject.Visible = false;
                            NewObject.Visible = false;
                            Edit.Visible = false;
                        }

                        // добавляем пункт прикрепляемые файлы
                        Product prod = provider.GetProduct(new Guid(Request["ID"]));
                        string secretKey = MD5Hash(String.Format("{0}{1}{2}", AttachedFilesApplication, User.ID, prod._dictNomenID));
                        string attachedUrl = @"javascript:CallAttachedFiles('{0}','{1}','{2}','{3}')";
                        AttachedFiles.NavigateUrl = string.Format(attachedUrl, AttachedFilesApplication, this.User.ID, prod._dictNomenID, secretKey);

                        // ищем продукт в приказных изделиях
                        if (!String.IsNullOrEmpty(Request.QueryString["order_id"]))
                        {
                            TreeWithKmh.NavigateUrl = string.Format("{0}?prodid={1}&orderid={2}", TreeWithKmh.NavigateUrl, Request["ID"], Request["order_id"]);
                        }
                        else
                        {
                            Product product = provider.GetProduct(eid);
                            if (!String.IsNullOrEmpty(product.OrderNumber) && !String.IsNullOrEmpty(product.OrderYear))
                            {
                                // приказное изделия
                                var orders = from order in provider.OrderArticles
                                             where order.year == product.OrderYear
                                             && order.cco == product.OrderNumber
                                             select order;
                                if (orders.Count() > 0)
                                {
                                    // есть приказ соотвествующий номеру и году
                                    TreeWithKmh.NavigateUrl = string.Format("{0}?prodid={1}&orderid={2}", TreeWithKmh.NavigateUrl, Request["ID"], orders.First().ID);
                                    EditKmh.NavigateUrl = string.Format("/Technology/EditorKmh.aspx?prodid={0}&orderid={1}", Request["ID"], orders.First().ID);
                                }
                                else
                                {
                                    // приказа нет, поэтому будем считать его стандартным
                                    TreeWithKmh.NavigateUrl = string.Format("{0}?prodid={1}", TreeWithKmh.NavigateUrl, Request["ID"]);
                                    EditKmh.NavigateUrl = string.Format("/Technology/EditorKmh.aspx?prodid={0}", Request["ID"]);
                                }
                            }
                            else
                            {
                                // не приказное изделие
                                TreeWithKmh.NavigateUrl = string.Format("{0}?prodid={1}", TreeWithKmh.NavigateUrl, Request["ID"]);
                                EditKmh.NavigateUrl = string.Format("/Technology/EditorKmh.aspx?prodid={0}", Request["ID"]);
                            }
                        }
                        
                        if (provider.IsMainVersion(eid))
                        {
                            /*
                             * для основных версий запрещается редактирование
                             * характеристик и состава
                             */ 
                            EditObject.Visible = false;
                            Edit.Visible = false;
                        }
                        else
                        {
                            /*
                             * для НЕосновных версий запрещается создание
                             * новых версий и новых версий с составом
                             */
                            NewObject.Visible = false;
                            NewObjectWithConfs.Visible = false;
                        }

                        if (provider.isPrikazVersion(eid) || provider.isLessOfMainVersion(eid))
                        {
                            /*
                             * для приказных версий запрещается редактирование
                             * характеристик и состава
                             */
                            EditObject.Visible = false;
                            Edit.Visible = false;
                        }

                        if (provider.IsNotConfigurationable(eid))
                        {
                            /*
                             * для простых продуктов (материалы, прочие, ...)
                             * запрещены любые манипуляции с составом
                             */
                            NewObjectWithConfs.Visible = false;
                            Edit.Visible = false;
                            EditReadOnly.Visible = false;
                            View.Visible = false;
                            Tree.Visible = false;
                            TreeEx.Visible = false;
                        }

                        if (!TechnologyEnable)
                        {
                            TreeWithKmh.Visible = false;
                        }
                    }
                }

                // применяем разрешения для пунктов меню
                List<KeyValuePair<HyperLink, Guid>> menuList = new List<KeyValuePair<HyperLink,Guid>>();
                menuList.Add(new KeyValuePair<HyperLink,Guid>(EditObject, new Guid("8e227deb-6c24-4904-997a-0e9aeb768a3c")));
                menuList.Add(new KeyValuePair<HyperLink,Guid>(ViewObject, new Guid("8f6c3479-0905-4f05-a4cb-f06de253be73")));
                menuList.Add(new KeyValuePair<HyperLink,Guid>(NewObject, new Guid("20a57357-7f79-4af5-87e0-35192d16d736")));
                menuList.Add(new KeyValuePair<HyperLink,Guid>(AttachedFiles, new Guid("a96e10df-fb98-42a4-8c48-c8d5c40d1536")));
                menuList.Add(new KeyValuePair<HyperLink,Guid>(NewObjectWithConfs, new Guid("fe711646-62ce-4f34-b1e3-67e2376a5ff3")));
                menuList.Add(new KeyValuePair<HyperLink,Guid>(Edit, new Guid("71ea2f2b-a5ab-4cad-b1c1-d025fe92eb4e")));
                menuList.Add(new KeyValuePair<HyperLink,Guid>(EditReadOnly, new Guid("972431d2-cca3-46f1-bc79-b4f8250fb68f")));
                menuList.Add(new KeyValuePair<HyperLink,Guid>(Usage, new Guid("65e7bea5-4a86-4b02-bfcc-3ae0ae0192d8")));
                menuList.Add(new KeyValuePair<HyperLink, Guid>(UsageWaves, new Guid("F0370805-D740-4B0B-A9E6-786905F172F8")));
                menuList.Add(new KeyValuePair<HyperLink,Guid>(View, new Guid("079e49a6-0d19-42b6-a8fe-6b1237d84c78")));
                menuList.Add(new KeyValuePair<HyperLink,Guid>(Tree, new Guid("4a4f9ffe-1686-4f01-8c35-065cea788a54")));
                menuList.Add(new KeyValuePair<HyperLink,Guid>(TreeWithKmh, new Guid("98d79f99-6b09-4846-9af8-6bacb71b7bef")));                               

                using (ProductProvider provider = new ProductProvider())
                {
                    List<Guid> userRoles = (from role in provider.UserRoles
                                            where role.UserID == User.ID
                                            select role.RoleID).ToList();

                    var permisions = from perm in provider.RoleViewPermissions
                                     where perm.PermissionEntityID == new Guid("0097C313-2EAD-4E1A-B12C-BB31F110A367") // MenuActionItem
                                     && userRoles.Contains(perm.RoleID)
                                     select perm;

                    foreach (var menuItem in menuList)
                    {
                        if (permisions.Count(perm => perm.EntityID == menuItem.Value && perm.Read) > 0)
                        {
                            menuItem.Key.Enabled = true;                            
                        }
                        else 
                        {
                            menuItem.Key.Enabled = false;
                            menuItem.Key.ForeColor = Color.LightGray;
                        }
                    }

                }

                                

            }
            //Response.Write(Request["ID"]);
        }
    }
}
