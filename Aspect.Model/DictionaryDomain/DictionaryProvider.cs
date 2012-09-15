using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data.Linq;
using System.Linq;
using System.Text;

using Aspect.Domain;
using Aspect.Model.Query;

namespace Aspect.Model.DictionaryDomain
{
    public class DictionaryProvider : Aspect.Domain.ContentDomain, ITreeProvider
    {
        private List<Guid> deniedNodes = null;
        protected virtual List<Guid> GetDeniedNodes(List<Guid> roles)
        {
            if (deniedNodes == null)
            {
                var d = from uv in RoleViewPermissions
                        where uv.PermissionEntityID == PermissionEntity.DictionaryTree && !uv.Read
                        && roles.Contains(uv.RoleID)
                        select uv.EntityID;
                deniedNodes = d.ToList();
            }
            return deniedNodes;
        }

        public List<ITreeNode> GetList(Guid parentID, Guid userID, List<Guid> roles)
        {
            var q = from c in DictionaryTrees
                    where c.ParentID == parentID || (!c.ParentID.HasValue && parentID.Equals(Guid.Empty))
                    select c as ITreeNode;
            List<ITreeNode> list = q.ToList();
            list = list.Where(l => !GetDeniedNodes(roles).Contains(l.ID)).ToList();
            return list;
            //List<DictionaryTree> list = q.ToList();
            //return list.ConvertAll(delegate(DictionaryTree n) { return (ITreeNode)n; });
        }

        public List<DictionaryTree> GetDictionaryTreeList(Guid parentID, List<Guid> roles)
        {
            var q = from c in DictionaryTrees
                    where c.ParentID == parentID || (!c.ParentID.HasValue && parentID.Equals(Guid.Empty))
                    select c;
            List<DictionaryTree> list = q.ToList();
            list = list.Where(l => !GetDeniedNodes(roles).Contains(l.ID)).ToList();
            return list;//q.ToList();
        }


        private List<Guid> deniedProperties = null;
        public List<DictionaryProperty> GetAvailableDictionaryProperties(List<Guid> roles, Guid dictionaryID)
        {
            if (deniedProperties == null)
            {
                var d = from uv in RoleViewPermissions
                        where uv.PermissionEntityID == PermissionEntity.DictionaryProperty && !uv.Read
                        && roles.Contains(uv.RoleID)
                        select uv.EntityID;
                deniedProperties = d.ToList();
            }
            List<DictionaryProperty> list = DictionaryProperties.Where(d => d.DictionaryID == dictionaryID).ToList();
            list = list.Where(p => !deniedProperties.Contains(p.ID)).ToList();
            return list;
        }

        public ITreeNode GetTreeNode(Guid id)
        {
            return DictionaryTrees.SingleOrDefault(s => s.ID == id);
        }

        protected Guid TreeNodeID { get; set; }

        public override void SetGridColumnOrder(Guid id, int order)
        {
            DictionaryUserField field = this.DictionaryUserFields.SingleOrDefault(u => u.ID == id);
            if (field != null)
            {
                field.Sequence = order;
                this.SubmitChanges();
            }
            return;
        }

        public override List<GridColumn> GetGridColumns(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID)
        {
            List<GridColumn> result = new List<GridColumn>();
            List<IUserField> userFields = this.GetUserFields(userID, treeNodeID, fieldPlaceHolderID);

            return userFields.ConvertAll(
                delegate(IUserField n)
                {
                    return new GridColumn()
                    {
                        ID = n.ID,
                        SourceID = n.DictionaryProperty.ID,
                        Name = n.DictionaryProperty.Name,
                        Alias = n.DictionaryProperty.Name,
                        //DataItem = n.DictionaryProperty.Name,
                        GridColumnType = n.DictionaryProperty.Type.Value,
                        DataItem = string.Format("{0}.{1}", n.DictionaryTree.Alias, n.DictionaryProperty.ColumnName),
                        OrderExpression = string.Format("{0}.{1}", n.DictionaryTree.Alias, n.DictionaryProperty.ColumnName),
                        Group = n.DictionaryTree.Name,
                        Order = n.Sequence,
                        IsDictionary = true
                    };
                }).OrderBy(r => r.Order).ToList();
            //List<DictionaryProperty> list = this.GetUserFields(userID, treeNodeID, fieldPlaceHolderID).Select(uf => uf.DictionaryProperty).ToList();
            //return list.ConvertAll(delegate(DictionaryProperty n) { return (IBoundField)n; });
        }

        public override List<IUserField> GetUserFields(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID)
        {
            List<DictionaryUserField> list = this.DictionaryUserFields.Where(us => us.UserID == userID && us.DictionaryTreeNodeID == treeNodeID && us.FieldPlaceHolderID == fieldPlaceHolderID).OrderBy(us => us.DictionaryTreeID).ToList();
            return list.ConvertAll(delegate(DictionaryUserField uf) { return (IUserField)uf; });
        }

        public override void AddUserField(Guid userID, Guid treeNodeID, Guid DcitionaryPropertyID, Guid DictionaryTreeID, Guid fieldPlaceHolderID, int order)
        {
            DictionaryUserField entity = new DictionaryUserField()
            {
                ID = Guid.NewGuid(),
                DictionaryTreeNodeID = treeNodeID,
                DcitionaryPropertyID = DcitionaryPropertyID,
                DictionaryTreeID = DictionaryTreeID,
                UserID = userID,
                FieldPlaceHolderID = fieldPlaceHolderID,
                Sequence = order
            };
            this.DictionaryUserFields.InsertOnSubmit(entity);
            this.SubmitChanges();
        }

        public override void DeleteUserFields(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID)
        {
            DictionaryUserFields.DeleteAllOnSubmit(DictionaryUserFields.Where(u => u.UserID == userID && u.DictionaryTreeNodeID == treeNodeID && u.FieldPlaceHolderID == fieldPlaceHolderID).AsEnumerable());
            this.SubmitChanges();
        }

        public override void DeleteUserFields(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID, List<Guid> propertyIDs)
        {
            DictionaryUserFields.DeleteAllOnSubmit(DictionaryUserFields.Where(u => u.UserID == userID && u.DictionaryTreeNodeID == treeNodeID && u.FieldPlaceHolderID == fieldPlaceHolderID && propertyIDs.Contains(u.DcitionaryPropertyID)).AsEnumerable());            
            this.SubmitChanges();
        }

        public override System.Data.DataRow GetEntity(Guid ID, Guid userID, Guid treeNodeID)
        {
            Query.Query query = new Query.Query(this);
            query.UserFields = this.GetUserFields(userID, treeNodeID, FieldPlaceHolder.GridCard);
            string sql = query.BuildEntityQuery(ID, treeNodeID);
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                System.Data.DataSet ds = provider.ExecuteCommand(sql);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) return ds.Tables[0].Rows[0];
                else return null;
            }
        }

        public override System.Data.DataSet GetList(Guid treeNodeID, Guid userID, OrderExpression order, List<SearchExpression> searchExpression, PagingInfo pagingInfo)
        {
            Query.Query query = new Query.Query(this);
            query.UserFields = this.GetUserFields(userID, treeNodeID, FieldPlaceHolder.Grid);
            query.OrderExpression = order;

            foreach (SearchExpression expr in searchExpression)
            {                
                // InList
                if (expr.FieldID == new Guid("11111111-0000-1111-0000-6097b75f5d3d") && expr.FieldName == "ID")
                {
                    Aspect.Domain.DictionaryTree entity = this.DictionaryTrees.Single(d => d.ID == treeNodeID);
                    expr.FieldName = string.Format("[{0}].[{1}]", entity.Alias, entity.PK);
                }
            }
            
            query.SearchExpression = this.ValidateSearchExpression(query.UserFields, searchExpression);
            string sql = query.BuildListQuery(treeNodeID);
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                if (pagingInfo.Enabled)
                {
                    StringBuilder cursor = new StringBuilder();
                    cursor.AppendLine("DECLARE @handle int, @rows int;");
                    cursor.AppendLine("EXEC sp_cursoropen @handle OUT,");
                    cursor.AppendFormat("'{0}',", sql.Replace("'", "''"));
                    cursor.AppendLine("1,1,@rows OUT SELECT @rows;");
                    cursor.AppendFormat("EXEC sp_cursorfetch @handle, 16, {0}, {1}\n", pagingInfo.Start + 1, pagingInfo.Limit);
                    cursor.AppendLine("EXEC sp_cursorclose @handle;");
                    return provider.ExecuteCommand(cursor.ToString());
                }
                else
                {
                    return provider.ExecuteCommand(sql);
                }

            }
        }

        public object GetProductDicitonaryValue(Guid productID, string property)
        {
            string sql = "select [{0}] from Product where ID = '{1}'";
            sql = string.Format(sql, property, productID);
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                return provider.ExecuteScalar(sql);
            }
        }

        public void SetProductDictioanryValue(Guid productID, string property, string value, Guid userID)
        {
            object oldValue = this.GetProductDicitonaryValue(productID, property);
            if (oldValue == DBNull.Value || value != oldValue.ToString())
            {
                Aspect.Utility.TraceHelper.Log(userID, "Продукт: {0}. Свойство справочника изменино: {1}. Старое значение {2}. Новое значение {3}", productID, property, oldValue == DBNull.Value ? "NULL" : oldValue.ToString(), value);
            }            
            string sql = string.Format("update Product set [{0}] = '{1}' where ID = '{2}'", property, value, productID);
            string sql2 = string.Format("update Product set [UserID] = '{0}' where ID = '{1}'", userID, productID);
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                provider.ExecuteNonQuery(sql);
                provider.ExecuteNonQuery(sql2);
            }
        }

        public object GetProductDictionaryText(Guid entityID, Guid dictTreeID)
        {
            DictionaryTree tree = this.DictionaryTrees.SingleOrDefault(d => d.ID == dictTreeID);
            if (tree == null) return string.Empty;
            return this.GetProductDictionaryText(entityID, tree);
        }

        public object GetProductDictionaryText(Guid entityID, DictionaryTree entity)
        {
            string sql = "select [{0}] from {1} where [{2}] = '{3}'";
            sql = string.Format(sql, entity.Dictionary.IdentifierField, entity.Dictionary.TableName, entity.PK, entityID);
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                return provider.ExecuteScalar(sql);
            }
        }

        public void DeleteProductDictionaryValue(Guid productID, string property, Guid userID)
        {
            object oldValue = this.GetProductDicitonaryValue(productID, property);
            if (oldValue != DBNull.Value)
            {
                Aspect.Utility.TraceHelper.Log(userID, "Продукт: {0}. Свойство справочника удалено: {1}. Старое значение {2}. Новое значение NULL", productID, property, oldValue == DBNull.Value ? "NULL" : oldValue.ToString());
            }
            string sql = "update Product set [{0}] = NULL where ID = '{1}'";
            sql = string.Format(sql, property, productID);
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                provider.ExecuteNonQuery(sql);
            }
        }

        public override string getQuery(Guid treeNodeID, Guid userID, OrderExpression order, List<SearchExpression> searchExpression)
        {
            Query.Query query = new Query.Query(this);
            query.UserFields = this.GetUserFields(userID, treeNodeID, FieldPlaceHolder.Grid);
            query.OrderExpression = order;
            query.SearchExpression = this.ValidateSearchExpression(query.UserFields, searchExpression);
            string sql = query.BuildListQuery(treeNodeID);
            return sql;
        }

        //---------------------
        public List<DictionaryProperty> GetAvailableDictionaryPropertiesToModify(Guid dictionaryTreeID, List<Guid> roles)
        {

            if (deniedProperties == null)
            {
                var d = from uv in RoleViewPermissions
                        where uv.PermissionEntityID == PermissionEntity.DictionaryProperty && (!uv.Read || !uv.Modify)
                        && roles.Contains(uv.RoleID)
                        select uv.EntityID;
                deniedProperties = d.ToList();
            }
            //List<DictionaryProperty> list = DictionaryProperties.Where(d => d.DictionaryID == dictionaryID).ToList();
            //list = list.Where(p => !deniedProperties.Contains(p.ID)).ToList();
            //return list;

            DictionaryTree tree = DictionaryTrees.SingleOrDefault(d => d.ID == dictionaryTreeID);
            if (tree != null)
            {
                List<DictionaryProperty> list = DictionaryProperties.Where(d => d.DictionaryID == tree.DictionaryID).ToList();
                list = list.Where(p => !deniedProperties.Contains(p.ID)).ToList();
                return list;
            }
            return new List<DictionaryProperty>();
        }
        public object GetDictionaryItemValue(Guid dictionaryTreeID, Guid entityID, string column)
        {
            DictionaryTree tree = DictionaryTrees.SingleOrDefault(d => d.ID == dictionaryTreeID);
            if (tree != null)
            {
                string sql = string.Format("select [{0}] from [{1}] where [{2}] = '{3}'", column, tree.Dictionary.TableName, tree.PK, entityID);
                using (CommonDataProvider domain = new CommonDataProvider())
                {
                    return domain.ExecuteScalar(sql);
                }
            }
            return null;
        }
        public enum ColumnTypeEnum
        {
            Varchar,
            Numeric,
            Decimal,
            Integer,
            Uniqueidentifier,
            Datetime,
            Unknown
        }

        private ColumnTypeEnum GetColumnType(CommonDataProvider provider, string tableName, string columnName)
        {
            string sql = string.Format("select data_type from information_schema.columns where table_name = '{0}' AND column_name = '{1}'", tableName, columnName);
            string val = provider.ExecuteScalar(sql).ToString();
            if (val == "char" || val == "nchar" || val == "nvarchar" || val == "varchar")
            {
                return ColumnTypeEnum.Varchar;
            }
            else if (val == "int")
            {
                return ColumnTypeEnum.Integer;
            }
            else if (val == "numeric")
            {
                return ColumnTypeEnum.Numeric;
            }
            else if (val == "decimal")
            {
                return ColumnTypeEnum.Decimal;
            }
            else if (val == "uniqueidentifier")
            {
                return ColumnTypeEnum.Uniqueidentifier;
            }
            else
            {
                return ColumnTypeEnum.Unknown;
            }
        }
        public bool IsSimpleDetal(Guid dictNomenID)
        {
            List<_dictNomen> nomens = _dictNomens.Where(d => d.ID == dictNomenID).ToList();
            if (nomens.Count == 0) return false;

            Guid[] zapret = {
                new Guid("94EBCF6E-1B89-4B9D-A4E8-F69E7F38466B"),
                new Guid("86154C96-461D-4A0C-803C-125F90B6FFEE"),
                new Guid("477CD48D-251A-4C51-8232-FFA67476AE22")
            };
            return zapret.Contains(nomens.First()._dictHSID.Value);
        }

        private int GetColumnLength(CommonDataProvider provider, string tableName, string columnName)
        {
            string sql = string.Format("select character_maximum_length from information_schema.columns where table_name = '{0}' AND column_name = '{1}'", tableName, columnName);
            object val = provider.ExecuteScalar(sql);
            if (val == DBNull.Value) return 0;
            return Convert.ToInt32(val);
        }
        public void SetDictionaryItemValue(Guid dictionaryTreeID, Guid entityID, string column, string value, Guid userID)
        {
            DictionaryTree tree = DictionaryTrees.SingleOrDefault(d => d.ID == dictionaryTreeID);
            if (tree != null)
            {
                using (CommonDataProvider domain = new CommonDataProvider())
                {
                    ColumnTypeEnum type = this.GetColumnType(domain, tree.Dictionary.TableName, column);
                    int length = this.GetColumnLength(domain, tree.Dictionary.TableName, column);
                    string sql;
                    switch (type)
                    {
                        case ColumnTypeEnum.Integer:
                        case ColumnTypeEnum.Numeric:
                        case ColumnTypeEnum.Decimal:
                            value = value.Replace(",", ".");
                            if (string.IsNullOrEmpty(value)) value = "NULL";
                            sql = string.Format("UPDATE {1} SET [{0}] = {2} WHERE [{3}] = '{4}'", column, tree.Dictionary.TableName, value, tree.PK, entityID);
                            break;
                        case ColumnTypeEnum.Datetime:
                            sql = string.Format("UPDATE {1} SET [{0}] = '{2}' WHERE [{3}] = '{4}'", column, tree.Dictionary.TableName, Convert.ToDateTime(value).ToString(CultureInfo.InvariantCulture), tree.PK, entityID);
                            break;
                        case ColumnTypeEnum.Varchar:
                            if (value.Length > length) value = value.Substring(0, length);
                            sql = string.Format("UPDATE {1} SET [{0}] = '{2}' WHERE [{3}] = '{4}'", column, tree.Dictionary.TableName, value, tree.PK, entityID);
                            break;
                        case ColumnTypeEnum.Uniqueidentifier:
                        case ColumnTypeEnum.Unknown:
                        default:
                            sql = string.Format("UPDATE {1} SET [{0}] = '{2}' WHERE [{3}] = '{4}'", column, tree.Dictionary.TableName, value, tree.PK, entityID);
                            break;
                    }

                    domain.ExecuteNonQuery(sql);

                    // setting UserID
                    sql = string.Format("UPDATE {0} SET [UserID] = '{1}' WHERE [{2}] = '{3}'", tree.Dictionary.TableName, userID, tree.PK, entityID);
                    domain.ExecuteNonQuery(sql);
                }
            }
        }
        public void DeleteDictionaryItemValue(Guid dictionaryTreeID, Guid entityID, string column)
        {
            DictionaryTree tree = DictionaryTrees.SingleOrDefault(d => d.ID == dictionaryTreeID);
            if (tree != null)
            {
                string sql = string.Format("UPDATE {1} SET [{0}] = NULL WHERE [{2}] = '{3}'", column, tree.Dictionary.TableName, tree.PK, entityID);
                using (CommonDataProvider domain = new CommonDataProvider())
                {
                    domain.ExecuteNonQuery(sql);
                }
            }
        }
        public Guid AddNewDictionaryEntity(Guid dictionaryTreeID, Guid entityID)
        {
            DictionaryTree tree = DictionaryTrees.SingleOrDefault(d => d.ID == dictionaryTreeID);
            Guid id = Guid.NewGuid();
            if (tree != null)
            {
                string sql = string.Format("insert into {0}({1})values('{2}')", tree.Dictionary.TableName, tree.PK, id);
                using (CommonDataProvider domain = new CommonDataProvider())
                {
                    domain.ExecuteNonQuery(sql);
                    return id;
                }
            }
            return Guid.Empty;
        }
        //---------------------

        public override bool AllowEdit(List<Guid> roles, Guid dictID)
        {
            var d = from uv in RoleViewPermissions
                    join ctp in DictionaryTrees on uv.EntityID equals ctp.ID
                    where uv.PermissionEntityID == PermissionEntity.DictionaryTree && !uv.Modify
                    && roles.Contains(uv.RoleID) && ctp.ID == dictID
                    select uv.EntityID;
            if (d.Count() > 0) return false;

            return true;
        }

        public override bool AllowDelete(List<Guid> roles, Guid dictID)
        {
            var d = from uv in RoleViewPermissions
                    join ctp in DictionaryTrees on uv.EntityID equals ctp.ID
                    where uv.PermissionEntityID == PermissionEntity.DictionaryTree && !uv.Delete
                    && roles.Contains(uv.RoleID) && ctp.ID == dictID
                    select uv.EntityID;
            if (d.Count() > 0) return false;

            return true;
        }
    }
}
