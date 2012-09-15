using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Aspect.Domain;
using Aspect.Model.Query;

namespace Aspect.Model.ConfigurationDomain
{
    public class ConfigurationProvider : ContentDomain
    {
        private List<ConfigurationUserProperty> ConfigurationUserPropertyColumns(Guid userID, Guid formGridViewID, Guid fieldPlaceHolderID)
        {
            var q = from us in ConfigurationUserProperties
                    where us.UserID == userID && us.FormGridViewID == formGridViewID && us.FieldPlaceHolderID == fieldPlaceHolderID
                    select us;
            return q.Distinct().ToList();
        }

        public override List<Property> GetUserProperties(Guid userID, Guid formGridViewID, Guid fieldPlaceHolderID)
        {
            var q = from u in Properties
                    join us in ConfigurationUserProperties on u.ID equals us.PropertyID
                    where us.UserID == userID && us.FormGridViewID == formGridViewID && us.FieldPlaceHolderID == fieldPlaceHolderID
                    select u;
            return q.Distinct().ToList();
        }

        public override List<IUserField> GetUserFields(Guid userID, Guid formGridViewID, Guid fieldPlaceHolderID)
        {
            List<ConfigurationUserField> list = this.ConfigurationUserFields.Where(us => us.UserID == userID && us.FormGridViewID == formGridViewID && us.FieldPlaceHolderID == fieldPlaceHolderID).OrderBy(us => us.DictionaryTreeID).ToList();
            return list.ConvertAll(delegate(ConfigurationUserField uf) { return (IUserField)uf; });
        }

        public override void SetGridColumnOrder(Guid id, int order)
        {
            ConfigurationUserField field = this.ConfigurationUserFields.SingleOrDefault(u => u.ID == id);
            if (field != null)
            {
                field.Sequence = order;
                this.SubmitChanges();
                return;
            }

            ConfigurationUserProperty property = this.ConfigurationUserProperties.SingleOrDefault(p => p.ID == id);
            if (property != null)
            {
                property.Sequence = order;
                this.SubmitChanges();
                return;
            }
            return;
        }

        public override List<GridColumn> GetGridColumns(Guid userID, Guid formGridViewID, Guid fieldPlaceHolderID)
        {
            List<GridColumn> result = new List<GridColumn>();
            List<IUserField> userFields = this.GetUserFields(userID, formGridViewID, fieldPlaceHolderID);

            result.AddRange(userFields.ConvertAll(
                    delegate(IUserField n)
                    {
                        return new GridColumn()
                        {
                            ID= n.ID,
                            Name = n.DictionaryProperty.Name,
                            //DataItem = n.DictionaryProperty.Name,
                            DataItem = string.Format("{0}.{1}", n.DictionaryTree.Alias, n.DictionaryProperty.ColumnName),
                            Group = n.DictionaryTree.Name,
                            IsDictionary = true,
                            Order = n.Sequence
                        };
                    })
           );


            //List<Property> props = this.GetUserProperties(userID, formGridViewID, fieldPlaceHolderID);
            List<ConfigurationUserProperty> props = ConfigurationUserPropertyColumns(userID, formGridViewID, fieldPlaceHolderID);

            result.AddRange(props.ConvertAll(
                    delegate(ConfigurationUserProperty n)
                    {
                        return new GridColumn()
                        {
                            //ID = n.Property.ID,
                            ID = n.ID,
                            Name = n.Property.Name,
                            //DataItem = n.Name,
                            DataItem = string.Format("{1}.{0}", n.Property.Name, n.Property.Alias),
                            Group = n.Property.Name,//"Свойства"
                            Order = n.Sequence,
                            GridColumnType = n.Property.Type.Value
                        };
                    })
           );
            if (fieldPlaceHolderID.Equals(FieldPlaceHolder.Grid))
            {
                result.Add(GridColumn.SetOrder(EditableGridColumn.QuantityColumn, 1000));
                result.Add(GridColumn.SetOrder(GridColumn.EdIzmColumn, 1001));
            }
            return result.OrderBy(r => r.Order).ToList();
            //return result;
        }

        public override void AddUserField(Guid userID, Guid formGridViewID, Guid DcitionaryPropertyID, Guid DictionaryTreeID, Guid fieldPlaceHolderID, int order)
        {
            ConfigurationUserField entity = new ConfigurationUserField()
            {
                ID = Guid.NewGuid(),
                FormGridViewID = formGridViewID,
                DcitionaryPropertyID = DcitionaryPropertyID,
                DictionaryTreeID = DictionaryTreeID,
                UserID = userID,
                FieldPlaceHolderID = fieldPlaceHolderID,
                Sequence = order
            };
            this.ConfigurationUserFields.InsertOnSubmit(entity);
            this.SubmitChanges();
        }

        public override void DeleteUserFields(Guid userID, Guid formGridViewID, Guid fieldPlaceHolderID)
        {
            ConfigurationUserFields.DeleteAllOnSubmit(ConfigurationUserFields.Where(u => u.UserID == userID && u.FormGridViewID == formGridViewID && u.FieldPlaceHolderID == fieldPlaceHolderID).AsEnumerable());
            this.SubmitChanges();            
        }

        public override void DeleteUserFields(Guid userID, Guid formGridViewID, Guid fieldPlaceHolderID, List<Guid> propertyIDs)
        {
            /*ConfigurationUserFields.DeleteAllOnSubmit(ConfigurationUserFields.Where(u => u.UserID == userID && u.FormGridViewID == formGridViewID && u.FieldPlaceHolderID == fieldPlaceHolderID).AsEnumerable());
            this.SubmitChanges();*/
            ConfigurationUserFields.DeleteAllOnSubmit(ConfigurationUserFields.Where(u => u.UserID == userID && u.FormGridViewID == formGridViewID && u.FieldPlaceHolderID == fieldPlaceHolderID && propertyIDs.Contains(u.DcitionaryPropertyID)).AsEnumerable());
            this.SubmitChanges();
        }

        public override void AddUserProperty(Guid userID, Guid formGridViewID, Guid fieldPlaceHolderID, Guid propertyID, int order)
        {
            ConfigurationUserProperty entity = new ConfigurationUserProperty()
            {
                ID = Guid.NewGuid(),
                FormGridViewID = formGridViewID,
                PropertyID = propertyID,
                UserID = userID,
                FieldPlaceHolderID = fieldPlaceHolderID,
                Sequence = order
            };
            ConfigurationUserProperties.InsertOnSubmit(entity);
            this.SubmitChanges();
        }

        public override void DeleteUserProperties(Guid userID, Guid formGridViewID, Guid fieldPlaceHolderID, List<Guid> propertyIDs)
        {
            ConfigurationUserProperties.DeleteAllOnSubmit(ConfigurationUserProperties.Where(u => u.UserID == userID && u.FormGridViewID == formGridViewID && u.FieldPlaceHolderID == fieldPlaceHolderID && propertyIDs.Contains(u.PropertyID)).AsEnumerable());
            this.SubmitChanges();
        }

        public override System.Data.DataSet GetList(Guid treeNodeID, Guid userID, OrderExpression order, List<SearchExpression> searchExpression, PagingInfo pagingInfo)
        {
            throw new NotImplementedException();
        }

        public virtual System.Data.DataSet GetList(Guid formGridViewID, Guid productID, Guid userID, /*OrderExpression order ,*/ List<SearchExpression> searchExpression)
        {
            //
            //List<SearchExpression> searchExpression = new List<SearchExpression>();
            OrderExpression order = new OrderExpression();
            //
            Query.Query query = new Query.Query(this);
            //query.ClassificationTreeID = treeNodeID;
            query.Columns = this.GetUserProperties(userID, formGridViewID, FieldPlaceHolder.Grid);
            query.UserFields = this.GetUserFields(userID, formGridViewID, FieldPlaceHolder.Grid);
            query.OrderExpression = order;
            query.SearchExpression = this.ValidateSearchExpression(query.Columns, query.UserFields, searchExpression);
            string sql = query.BuildListQuery(productID);
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                return provider.ExecuteCommand(sql);
            }
        }
        public string getQuery(Guid formGridViewID, Guid productID, Guid userID/*, OrderExpression order , List<SearchExpression> searchExpression*/)
        {
            //
            List<SearchExpression> searchExpression = new List<SearchExpression>();
            OrderExpression order = new OrderExpression();
            //
            Query.Query query = new Query.Query(this);
            //query.ClassificationTreeID = treeNodeID;
            query.Columns = this.GetUserProperties(userID, formGridViewID, FieldPlaceHolder.Grid);
            query.UserFields = this.GetUserFields(userID, formGridViewID, FieldPlaceHolder.Grid);
            query.OrderExpression = order;
            query.SearchExpression = this.ValidateSearchExpression(query.Columns, query.UserFields, searchExpression);
            string sql = query.BuildListQuery(productID);
            return sql;
        }
        public override string getQuery(Guid treeNodeID, Guid userID, OrderExpression order, List<SearchExpression> searchExpression)
        {
            return base.getQuery(treeNodeID, userID, order, searchExpression);
        }

        public override System.Data.DataRow GetEntity(Guid ID, Guid userID, Guid formGridViewID)
        {

            ProductDomain.Query.Query query = new ProductDomain.Query.Query(this);
            query.Columns = this.GetUserProperties(userID, formGridViewID, FieldPlaceHolder.GridCard);
            query.UserFields = this.GetUserFields(userID, formGridViewID, FieldPlaceHolder.GridCard);
            string sql = query.BuildEntityQuery(ID);
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                System.Data.DataSet ds = provider.ExecuteCommand(sql);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) return ds.Tables[0].Rows[0];
                else return null;
            }
        }

        public Configuration CopyConfiguration(Configuration srcConf)
        {
            Configuration dstConf = new Configuration();

            dstConf.ProductID = srcConf.ProductID;
            dstConf.ProductOwnerID = srcConf.ProductOwnerID;
            
            dstConf.AutoUpdate = srcConf.AutoUpdate;
            dstConf.Comment = srcConf.Comment;
            dstConf.GroupNumber = srcConf.GroupNumber;
            dstConf.GroupToChange = srcConf.GroupToChange;
            dstConf.Position = srcConf.Position;
            dstConf.Quantity = srcConf.Quantity;
            dstConf.version = srcConf.version;
            dstConf.QuantityInclusive = srcConf.QuantityInclusive;
            dstConf._dictUMID = srcConf._dictUMID;
            dstConf.Zone = srcConf.Zone;

            return dstConf;
        }
    }
}
