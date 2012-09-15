using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Aspect.Domain;
using Aspect.Model.Query;

namespace Aspect.Model.ProductDomain
{
    public class CustomClassificationProductProvider : ProductProvider
    {
        public override System.Data.DataSet GetList(Guid treeNodeID, Guid userID, OrderExpression order, List<SearchExpression> searchExpression, PagingInfo pagingInfo)
        {
            Query.Query query = new Query.CustomClassificationQuery(this);
            //query.ClassificationTreeID = treeNodeID;
            query.Columns = this.GetUserProperties(userID, treeNodeID, FieldPlaceHolder.Grid);
            query.UserFields = this.GetUserFields(userID, treeNodeID, FieldPlaceHolder.Grid);
            query.OrderExpression = order;
            query.SearchExpression = this.ValidateSearchExpression(query.Columns, query.UserFields, searchExpression);
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
        public override string getQuery(Guid treeNodeID, Guid userID, OrderExpression order, List<SearchExpression> searchExpression)
        {
            Query.Query query = new Query.CustomClassificationQuery(this);
            //query.ClassificationTreeID = treeNodeID;
            query.Columns = this.GetUserProperties(userID, treeNodeID, FieldPlaceHolder.Grid);
            query.UserFields = this.GetUserFields(userID, treeNodeID, FieldPlaceHolder.Grid);
            query.OrderExpression = order;
            query.SearchExpression = this.ValidateSearchExpression(query.Columns, query.UserFields, searchExpression);
            string sql = query.BuildListQuery(treeNodeID);
            return sql;
        }

        public override void AddProducts(Guid userID, Guid treeID, List<Guid> ids)
        {
            CustomClassificationTree tree = CustomClassificationTrees.SingleOrDefault(c => c.ID == treeID);
            if (tree != null)
            {
                CustomClassificationNode node = tree.CustomClassificationNode;
                if (!tree.CustomClassificationNodeID.HasValue)
                {
                    node = AddNode(tree);
                }
                foreach (Guid id in ids)
                {
                    //check if product already exists in the node

                    if (CustomClassificationNodeProducts.Where(c => c.CustomClassificationNodeID == node.ID && c.ProductID == id).Count() == 0)
                    {
                        CustomClassificationNodeProduct entity = new CustomClassificationNodeProduct()
                        {
                            ID = Guid.NewGuid(),
                            ProductID = id,
                            CustomClassificationNodeID = node.ID
                        };
                        Aspect.Utility.TraceHelper.Log(userID, "Продукт: {0}. Добавление в пользовательскую классификацию. В CustomClassificationNode {1}. ", id, node.ID);
                        CustomClassificationNodeProducts.InsertOnSubmit(entity);
                        this.SubmitChanges();
                    }
                }
            }
            //CustomClassificationTree tr;
        }
        private CustomClassificationNode AddNode(CustomClassificationTree tree)
        {
            if (tree.CustomClassificationNodeID.HasValue)
            {
                return tree.CustomClassificationNode;
            }
            else
            {
                CustomClassificationNode entity = new CustomClassificationNode()
                {
                    ID = Guid.NewGuid(),
                    Name = tree.Name
                };
                CustomClassificationNodes.InsertOnSubmit(entity);
                this.SubmitChanges();
                tree.CustomClassificationNode = entity;
                //tree.CustomClassificationNodeID = entity.ID;
                this.SubmitChanges();
                return entity;
            }
        }
    }
}
