using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Aspect.Domain
{
    public class PagingInfo
    {
        public int Start = 0;
        public int Limit = 0;
        public bool Enabled = true;

        public PagingInfo(int Start, int Limit)
        {
            this.Start = Start;
            this.Limit = Limit;
        }
        public PagingInfo(bool Enabled)
        {
            this.Enabled = Enabled;
        }
        public PagingInfo()
        {
        }
    }

    public abstract class ContentDomain : CommonDomain
    {        
        
        public abstract System.Data.DataSet GetList(Guid treeNodeID, Guid userID, OrderExpression order, List<SearchExpression> searchExpression, PagingInfo pagingInfo);

        public abstract System.Data.DataRow GetEntity(Guid ID, Guid userID, Guid treeNodeID);

        public abstract List<IUserField> GetUserFields(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID);

        public abstract void AddUserField(Guid userID, Guid treeNodeID, Guid DcitionaryPropertyID, Guid DictionaryTreeID, Guid fieldPlaceHolderID, int order);

        public abstract void DeleteUserFields(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID);

        public virtual void DeleteUserFields(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID, List<Guid> propertyIDs)
        {
        }

        public virtual void DeleteUserProperties(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID, List<Guid> propertyIDs)
        {
        }

        public virtual void AddUserProperty(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID, Guid propertyID, int order)
        {
        }

        public virtual void AddProducts(Guid userID, Guid treeID, List<Guid> ids)
        {
        }

        public abstract List<GridColumn> GetGridColumns(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID);
        
        public abstract void SetGridColumnOrder(Guid id, int order);

        public List<SearchSource> ValidateSearchExpression(List<IUserField> fields, List<SearchExpression> searchExpression)
        {
            return ValidateSearchExpression(new List<Property>(), fields, searchExpression);
        }

        public List<SearchSource> ValidateSearchExpression(List<Property> columns, List<IUserField> fields, List<SearchExpression> searchExpression)
        {
            List<SearchSource> result = new List<SearchSource>();
            foreach (SearchExpression item in searchExpression)
            {
                if (item.FieldCond == Condition.Inset) //InList
                {
                    if (!string.IsNullOrEmpty(item.FieldName)) result.Add(new SearchSource()
                    {
                        FieldValue = item.FieldValue,
                        FeildType = TypeEnum.InList,
                        FieldCond = item.FieldCond,
                        ColumnName = item.FieldName
                    });
                    continue;
                }
                if (item.FieldCond == Condition.Beable) //IsBeing
                {
                    if (columns.Exists(p => p.ID == new Guid(item.FieldValue))) result.Add(new SearchSource()
                    {
                        FieldValue = item.FieldValue,
                        Alias = columns.First(p => p.ID == new Guid(item.FieldValue)).Alias,
                        FeildType = TypeEnum.IsBeing,
                        FieldCond = item.FieldCond,
                        ColumnName = "Value"
                    });
                    continue;
                }
                IEnumerable<Property> list1 = columns.Where(p => p.ID == item.FieldID);
                IEnumerable<IUserField> list2 = fields.Where(f => f.ID == item.FieldID);
                if (list1.Count() > 0)
                {
                    Property prop = list1.First();
                    result.Add(new SearchSource()
                    {
                        ColumnName = "Value",
                        //FieldName = "Value",
                        FieldValue = item.FieldValue,
                        Alias = prop.Alias,
                        FieldCond = item.FieldCond,
                        FeildType = prop.Type.Value
                    });
                }
                else if (list2.Count() > 0)
                {
                    IUserField prop = list2.First();
                    result.Add(new SearchSource()
                    {
                        FeildType = prop.FieldType,
                        FieldValue = item.FieldValue,
                        FieldCond = item.FieldCond,
                        ColumnName = prop.DictionaryProperty.ColumnName,
                        Alias = prop.DictionaryTree.Alias
                    });
                }
            }
            return result;
        }

        public DictionaryTree GetDictionaryTreeNode(Guid id)
        {
            return DictionaryTrees.SingleOrDefault(d => d.ID == id);
        }

        public virtual List<Property> GetUserProperties(Guid userID, Guid classificationTreeID, Guid fieldPlaceHolderID)
        {
            return new List<Property>();
        }

        public virtual List<UserProperty> GetUserPropertyColumns(Guid userID, Guid classificationTreeID, Guid fieldPlaceHolderID)
        {
            return new List<UserProperty>();
        }

        private List<Guid> deniedNodes = null;
        public List<Property> GetAvailableProperties(List<Guid> roles)
        {
            if (deniedNodes == null)
            {
                var d = from uv in RoleViewPermissions
                        where uv.PermissionEntityID == PermissionEntity.Property && !uv.Read
                        && roles.Contains(uv.RoleID)
                        select uv.EntityID;
                deniedNodes = d.ToList();
            }
            List<Property> list = Properties.ToList();
            list = list.Where(p => !deniedNodes.Contains(p.ID)).ToList();
            return list;
        }

        public List<Property> GetAvailablePropertiesToModify(List<Guid> roles)
        {
            if (deniedNodes == null)
            {
                var d = from uv in RoleViewPermissions
                        where uv.PermissionEntityID == PermissionEntity.Property && !uv.Modify
                        && roles.Contains(uv.RoleID)
                        select uv.EntityID;
                deniedNodes = d.ToList();
            }
            List<Property> list = Properties.ToList();
            list = list.Where(p => !deniedNodes.Contains(p.ID)).ToList();
            return list;
        }

        public virtual string getQuery(Guid treeNodeID, Guid userID, OrderExpression order, List<SearchExpression> searchExpression)
        {
            return string.Empty;
        }

        public virtual bool AllowEdit(List<Guid> roles, Guid entityID)
        {
            return true;
        }
        public virtual bool AllowDelete(List<Guid> roles, Guid entityID)
        {
            return true;
        }
    }
}
