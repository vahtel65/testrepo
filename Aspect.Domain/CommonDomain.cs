namespace Aspect.Domain
{
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using System;
    using System.Web.Configuration;
 
    partial class UserField
    {
    }

    public partial class CommonDomain
    {
        public CommonDomain() :
            base(global::Aspect.Domain.Properties.Settings.Default.AspectConnectionString, mappingSource)        
        {
            OnCreated();
        }

        partial void OnCreated()
        {
            this.CommandTimeout = Convert.ToInt32(WebConfigurationManager.AppSettings["TimeoutInnerConnection"]);
        }

        public string GetNomenSuperFieldValue(Guid _dictP1STID, Guid _dictGOSTID, string PN1, string PN2_2)
        {
            List<GetNomenSuperFieldResult> list  = this.GetNomenSuperField(
                _dictP1STID.Equals(Guid.Empty) ? (Guid?)null : (Guid?)_dictP1STID,
                _dictGOSTID.Equals(Guid.Empty) ? (Guid?)null : (Guid?)_dictGOSTID, 
                PN1, 
                PN2_2).ToList();
            string result = string.Empty;
            foreach (GetNomenSuperFieldResult item in list)
            {
                result = item.superfield;
                break;
            }
            return result;
        }
        public string GetNomenSuperFieldValueUa(Guid _dictP1STID, Guid _dictGOSTID, string PN1, string PN2_2)
        {
            List<GetNomenSuperFieldUaResult> list = this.GetNomenSuperFieldUa(
                _dictP1STID.Equals(Guid.Empty) ? (Guid?)null : (Guid?)_dictP1STID,
                _dictGOSTID.Equals(Guid.Empty) ? (Guid?)null : (Guid?)_dictGOSTID,
                PN1,
                PN2_2).ToList();
            string result = string.Empty;
            foreach (GetNomenSuperFieldUaResult item in list)
            {
                result = item.superfield;
                break;
            }
            return result;
        }

        public Product GetProduct(Guid id)
        {
            return this.Products.SingleOrDefault(p => p.ID == id);
        }        

        public List<ITreeNode> GetProductParents(Guid id)
        {
            var q = from m in ClassificationTreeProducts
                    where m.ProductID == id
                    select m.ClassificationTree as ITreeNode;

            List<ITreeNode> list = new List<ITreeNode>(); //q.ToList();
            foreach (ITreeNode item in q)
            {
                list.Add(item);
                if (item.ParentID.HasValue) GetClassificationParent(item.ParentID.Value, list);
            }

            var s = from m in CustomClassificationTrees
                    join k in CustomClassificationNodeProducts on m.CustomClassificationNodeID equals k.CustomClassificationNodeID
                    where k.ProductID == id
                    select m as ITreeNode;

            List<ITreeNode> clist = new List<ITreeNode>();
            foreach (ITreeNode item in s)
            {
                clist.Add(item);
                if (item.ParentID.HasValue) GetCustomClassificationParent(item.ParentID.Value, clist);
            }


            var t = from z in CustomClassificationTrees
                    where z.ClassificationTreeID.HasValue && list.Select(s1 => s1.ID).Contains(z.ClassificationTreeID.Value)
                    select z as ITreeNode;

            List<ITreeNode> mlist = new List<ITreeNode>();
            foreach (ITreeNode item in t)
            {
                //mlist.Add(item);
                if (item.ParentID.HasValue) GetCustomClassificationParent(item.ParentID.Value, mlist);
            }
            list.AddRange(clist);
            list.AddRange(mlist);
            return list.Distinct().ToList();
        }

        private void GetClassificationParent(Guid id, List<ITreeNode> list)
        {
            ClassificationTree item = ClassificationTrees.SingleOrDefault(d => d.ID == id);
            list.Add(item);
            if (item.ParentID.HasValue) GetClassificationParent(item.ParentID.Value, list);
        }

        private void GetCustomClassificationParent(Guid id, List<ITreeNode> list)
        {
            CustomClassificationTree item = CustomClassificationTrees.SingleOrDefault(d => d.ID == id);
            list.Add(item);
            if (item.ParentID.HasValue) GetCustomClassificationParent(item.ParentID.Value, list);
        }

        public Guid AddNewProduct(Guid parentProduct, Guid userID)
        {
            return AddNewProduct(parentProduct, false, Guid.Empty, userID);
        }

        public Guid AddNewProduct(Guid parentProduct, bool withDictNomen, Guid dictNomenID, Guid UserID)
        {            
            Product parent = Products.SingleOrDefault(p => p.ID == parentProduct);
            List<ClassificationTreeProduct> trees = parent.ClassificationTreeProducts.ToList();
            Guid id = Guid.NewGuid();
            if (parent != null)
            {
                //insert product entity
                string sql;
                if (withDictNomen)
                {
                    sql = "INSERT INTO Product(ID,Name,CreatedDate,_dictNomenID)VALUES('{0}','{1}',CONVERT(datetime, '{2}', 120),'{3}')";
                    sql = string.Format(sql, id, parent.PublicName.Trim(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), Guid.Empty.Equals(dictNomenID) ? parent._dictNomenID : dictNomenID);
                }
                else
                {
                    sql = "INSERT INTO Product(ID,Name,CreatedDate)VALUES('{0}','{1}',CONVERT(datetime, '{2}', 120))";
                    sql = string.Format(sql, id, parent.PublicName.Trim(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }

                using (CommonDataProvider provider = new CommonDataProvider())
                {
                    provider.ExecuteNonQuery(sql);
                }

                //add to global classification tree
                List<ClassificationTreeProduct> classif = new List<ClassificationTreeProduct>();
                foreach (ClassificationTreeProduct item in trees)
                {
                    ClassificationTreeProduct entity = new ClassificationTreeProduct()
                    {
                        ClassificationTreeID = item.ClassificationTreeID,
                        ID = Guid.NewGuid(),
                        ProductID = id
                    };
                    classif.Add(entity);
                }
                this.ClassificationTreeProducts.InsertAllOnSubmit(classif);
                this.SubmitChanges();

                return id;
            }
            return Guid.Empty;
        }
    }
    public class ProductParent
    {
        public Guid ID { get; set; }
        public Guid? ParentID { get; set; }
        public string Name { get; set; }
    }
}
