using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Aspect.Domain;
using Aspect.Model.Query;

namespace Aspect.Model.ProductDomain
{
    public class ProductProvider : ContentDomain
    {
        internal const string ProductIDColumn = "Product.ID";
        //public const string ProductName = "ProductName";
        public const string ProductTable = "Product";

        public override List<UserProperty> GetUserPropertyColumns(Guid userID, Guid classificationTreeID, Guid fieldPlaceHolderID)
        {
            var q = from us in UserProperties
                    where us.UserID == userID && us.ClassificationTreeID == classificationTreeID && us.FieldPlaceHolderID == fieldPlaceHolderID
                    select us;
            return q.Distinct().ToList();
        }

        public override List<Property> GetUserProperties(Guid userID, Guid classificationTreeID, Guid fieldPlaceHolderID)
        {
            var q = from u in Properties
                    join us in UserProperties on u.ID equals us.PropertyID
                    where us.UserID == userID && us.ClassificationTreeID == classificationTreeID && us.FieldPlaceHolderID == fieldPlaceHolderID
                    select u;
            return q.Distinct().ToList();
        }

        public Property GetColumnPropertyByID(Guid PropertyID)
        {        
            var q = from u in Properties where u.ID == PropertyID select u;
            return q.Single();
        }

        public override void SetGridColumnOrder(Guid id, int order)
        {
            UserField field = this.UserFields.SingleOrDefault(u => u.ID == id);
            if (field != null)
            {
                field.Sequence = order;
                this.SubmitChanges();
                return;
            }

            UserProperty property = this.UserProperties.SingleOrDefault(p => p.ID == id);
            if (property != null)
            {
                property.Sequence = order;
                this.SubmitChanges();
                return;
            }
            return;
        }

        public override List<GridColumn> GetGridColumns(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID)
        {
            List<GridColumn> result = new List<GridColumn>();
            List<IUserField> userFields = this.GetUserFields(userID, treeNodeID, fieldPlaceHolderID);

            result.AddRange(userFields.ConvertAll(
                    delegate(IUserField n)
                    {
                        return new GridColumn()
                        {
                            ClassificationID = n.DictionaryTree.ClassificationID,
                            ID = n.ID,
                            SourceID = n.DictionaryProperty.ID,
                            IsDictionary = true,
                            Name = n.DictionaryProperty.Name,
                            Alias = n.DictionaryProperty.Name,
                            GridColumnType = n.DictionaryProperty.Type.Value,
                            //DataItem = n.DictionaryProperty.Name,
                            DataItem = string.Format("{0}.{1}", n.DictionaryTree.Alias, n.DictionaryProperty.ColumnName),
                            OrderExpression = string.Format("{0}.{1}", n.DictionaryTree.Alias, n.DictionaryProperty.ColumnName),
                            Group = n.DictionaryTree.Name,
                            Order = n.Sequence
                        };
                    })
           );


            //List<Property> props = this.GetUserProperties(userID, treeNodeID, fieldPlaceHolderID);
            List<UserProperty> props = this.GetUserPropertyColumns(userID, treeNodeID, fieldPlaceHolderID);

            result.AddRange(props.ConvertAll(
                    delegate(UserProperty n)
                    {
                        return new GridColumn()
                        {
                            ClassificationID = n.Property.ClassificationID,
                            ID = n.ID,
                            SourceID = n.Property.ID,
                            Name = n.Property.Name,
                            Alias = n.Property.Caption,
                            DataItem = string.Format("{1}.{0}", n.Property.Name, n.Property.Alias),
                            OrderExpression = string.Format("{0}.Value", n.Property.Alias),
                            Group = n.Property.Caption,
                            Order = n.Sequence,
                            GridColumnType = n.Property.Type.Value
                        };
                    })
           );
            //result.AddRange(props.ConvertAll(delegate(Property n) { return (IBoundField)n; }));
            //return result;
            return result.OrderBy(r => r.Order).ToList();
        }

        public override List<IUserField> GetUserFields(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID)
        {
            List<UserField> list = this.UserFields.Where(us => us.UserID == userID && us.ClassificationTreeID == treeNodeID && us.FieldPlaceHolderID == fieldPlaceHolderID).OrderBy(us => us.DictionaryTreeID).ToList();
            return list.ConvertAll(delegate(UserField uf) { return (IUserField)uf; });
        }

        public override void AddUserField(Guid userID, Guid treeNodeID, Guid DcitionaryPropertyID, Guid DictionaryTreeID, Guid fieldPlaceHolderID, int order)
        {
            UserField entity = new UserField()
            {
                ID = Guid.NewGuid(),
                ClassificationTreeID = treeNodeID,
                DcitionaryPropertyID = DcitionaryPropertyID,
                DictionaryTreeID = DictionaryTreeID,
                UserID = userID,
                FieldPlaceHolderID = fieldPlaceHolderID,
                Sequence = order
            };
            this.UserFields.InsertOnSubmit(entity);
            this.SubmitChanges();
        }

        public override void DeleteUserFields(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID)
        {
            UserFields.DeleteAllOnSubmit(UserFields.Where(u => u.UserID == userID && u.ClassificationTreeID == treeNodeID && u.FieldPlaceHolderID == fieldPlaceHolderID).AsEnumerable());
            this.SubmitChanges();
        }

        public override void AddUserProperty(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID, Guid propertyID, int order)
        {
            UserProperty entity = new UserProperty()
            {
                ID = Guid.NewGuid(),
                ClassificationTreeID = treeNodeID,
                PropertyID = propertyID,
                UserID = userID,
                FieldPlaceHolderID = fieldPlaceHolderID,
                Sequence = order
            };
            UserProperties.InsertOnSubmit(entity);
            this.SubmitChanges();
        }

        public override void DeleteUserFields(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID, List<Guid> propertyIDs)
        {
            UserFields.DeleteAllOnSubmit(UserFields.Where(u => u.UserID == userID && u.ClassificationTreeID == treeNodeID && u.FieldPlaceHolderID == fieldPlaceHolderID && propertyIDs.Contains(u.DcitionaryPropertyID)).AsEnumerable());
            this.SubmitChanges();
        }

        public override void DeleteUserProperties(Guid userID, Guid treeNodeID, Guid fieldPlaceHolderID, List<Guid> propertyIDs)
        {
            /*foreach (Guid item in propertyIDs)
            {
                UserProperties.DeleteOnSubmit(UserProperties.Single(u => u.UserID == userID && u.ClassificationTreeID == treeNodeID && u.FieldPlaceHolderID == fieldPlaceHolderID && u.PropertyID == item));
            }*/
            UserProperties.DeleteAllOnSubmit(UserProperties.Where(u => u.UserID == userID && u.ClassificationTreeID == treeNodeID && u.FieldPlaceHolderID == fieldPlaceHolderID && propertyIDs.Contains(u.PropertyID)).AsEnumerable());
            //UserProperties.DeleteAllOnSubmit(UserProperties.Where(u => u.UserID == userID && u.ClassificationTreeID == treeNodeID && u.FieldPlaceHolderID == fieldPlaceHolderID).AsEnumerable());
            this.SubmitChanges();
        }

        public override System.Data.DataRow GetEntity(Guid ID, Guid userID, Guid treeNodeID)
        {
            Query.Query query = new Query.Query(this);
            query.Columns = this.GetUserProperties(userID, treeNodeID, FieldPlaceHolder.GridCard);
            query.UserFields = this.GetUserFields(userID, treeNodeID, FieldPlaceHolder.GridCard);
            string sql = query.BuildEntityQuery(ID);
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
            //query.ClassificationTreeID = treeNodeID;
            query.Columns = this.GetUserProperties(userID, treeNodeID, FieldPlaceHolder.Grid);

            foreach (SearchExpression expr in searchExpression)
            {
                // основная версия               
                if (expr.FieldID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552") && !query.Columns.Exists(p => p.ID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552")))
                {
                    query.Columns.Add(this.GetColumnPropertyByID(new Guid("BBE170B0-28E4-4738-B365-1038B03F4552")));
                }
                // IsBeing
                if (expr.FieldCond == Condition.Beable && !query.Columns.Exists(p => p.ID == new Guid(expr.FieldValue)))
                {
                    query.Columns.Add(this.GetColumnPropertyByID(new Guid(expr.FieldValue)));
                }
                // InList
                if (expr.FieldCond == Condition.Inset && expr.FieldName == "ID")
                {
                    expr.FieldName = "Product.ID";
                }
            }
            
            query.UserFields = this.GetUserFields(userID, treeNodeID, FieldPlaceHolder.Grid);
            // default order by nomen super pole
            if (order.OrderClause == string.Empty)
            {
                //IUserField uf = query.UserFields.SingleOrDefault(u => u.DictionaryProperty.ID == new Guid("CAE5AFF1-1103-45CF-8135-7834DC9FAD35"));
				IUserField uf = query.UserFields.FirstOrDefault(u => u.DictionaryProperty.ID == new Guid("CAE5AFF1-1103-45CF-8135-7834DC9FAD35") && u.DictionaryTreeID == new Guid("316C6BC7-D883-44C8-AAE0-602F49C73595"));

                
                if (uf != null)
                {
                    order = new OrderExpression()
                    {
                        Expression = string.Format("{0}.{1}", uf.DictionaryTree.Alias, uf.DictionaryProperty.ColumnName),
                        SortDirection = SortDirection.asc
                    };
                }
            }
            //
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

        public override void AddProducts(Guid userID, Guid treeID, List<Guid> ids)
        {
            ClassificationTree tree = ClassificationTrees.SingleOrDefault(c => c.ID == treeID);
            if (tree != null)
            {
                foreach (Guid id in ids)
                {
                    ClassificationTreeProduct entity = ClassificationTreeProducts.SingleOrDefault(c => c.ProductID == id);
                    if (entity != null)
                    {
                        Aspect.Utility.TraceHelper.Log(userID, "Продукт: {0}. Перенос в стандартной классификации из: {1}. В {2}. ", id, entity.ClassificationTreeID, treeID);
                        entity.ClassificationTreeID = treeID;
                        this.SubmitChanges();
                    }
                }
            }
        }

        public override bool AllowEdit(List<Guid> roles, Guid entityID)
        {
            var d = from uv in RoleViewPermissions
                    join ctp in ClassificationTreeProducts on uv.EntityID equals ctp.ClassificationTreeID
                    where uv.PermissionEntityID == PermissionEntity.ClassificationTree && !uv.Modify
                    && roles.Contains(uv.RoleID) && ctp.ProductID == entityID
                    select uv.EntityID;
            if (d.Count() > 0) return false;

            var d1 = from uv in RoleViewPermissions
                     join cct in CustomClassificationTrees on uv.EntityID equals cct.ID
                    join ctp in CustomClassificationNodeProducts on cct.CustomClassificationNodeID equals ctp.CustomClassificationNodeID
                    where uv.PermissionEntityID == PermissionEntity.ClassificationTree && !uv.Modify
                    && roles.Contains(uv.RoleID) && ctp.ProductID == entityID
                    select uv.EntityID;

            if (d1.Count() > 0) return false;

            return true;
        }

        public bool IsMainVersion(Guid entityID)
        {
            // проверка на "Основную версию"
            var mainVersion = from uv in ProductProperties
                              where uv.ProductID == entityID && uv.PropertyID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552")
                              select uv.Value;
            foreach (var item in mainVersion)
                if (item == "1") return true;

            return false;
        }

        public bool isPrikazVersion(Guid entityID)
        {
            // проверка на "Приказную версию"
            var yearPrikazs = from uv in ProductProperties
                              where uv.ProductID == entityID && uv.PropertyID == new Guid("2CCD4FF3-6D43-4A35-9784-969FAB46B5CC")
                                && uv.Value.Length > 0
                              select uv.PropertyID;            
            var nomerPrikazs = from uv in ProductProperties
                               where uv.ProductID == entityID && uv.PropertyID == new Guid("9A38E338-DD60-4636-BFE3-6A98BAF8AE87")
                                &&  uv.Value.Length > 0
                               select uv.PropertyID;
            if (nomerPrikazs.Count() > 0 && yearPrikazs.Count() > 0) return true;

            return false;
        }

        public int getMaxVersionByNomenID(Guid NomenID)
        {
            // возвращает максимальную версию среди всех продуктов, относящихся
            // к данной номенклатуре
            var versions = from allprod in Products
                           join property in ProductProperties on allprod.ID equals property.ProductID
                           where allprod._dictNomenID == NomenID
                           && property.PropertyID == new Guid("0789DB1A-9BAA-4574-B405-AE570C746C03")
                           select property.Value;
            int maxVersion = 0;
            try
            {
                maxVersion = versions.Select(ver => Convert.ToInt32(ver)).Max();
            }
            catch
            {
            }
            return maxVersion;
        }

        public int getMaxVersionByProductID(Guid ProductID)
        { 
            // возвращает максимальную версию среди всех продуктов, относящихся
            // к данной номенклатуре
            var versions = from prod in Products
                           where prod.ID == ProductID
                           select prod._dictNomen into nomen
                           from allprod in Products 
                           join property in ProductProperties on allprod.ID equals property.ProductID                                
                           where allprod._dictNomen == nomen
                           && property.PropertyID == new Guid("0789DB1A-9BAA-4574-B405-AE570C746C03")
                           select property.Value;
            int maxVersion = 0;
            try
            {
                maxVersion = versions.Select(ver => Convert.ToInt32(ver)).Max();
            }
            catch
            {
            }
            return maxVersion;
        }

        /// <summary>
        /// функция проверяет, является ли продукт ДТ, МТ или ПИ
        /// </summary>
        public bool IsNotConfigurationable(Guid entiryID)
        {
            Guid p = (from prod in Products
                    where prod.ID == entiryID
                    join hs in _dictHs on prod._dictNomen._dictHSID equals hs.ID
                    select hs.ID).Single();
            Guid[] zapret = {
                new Guid("94EBCF6E-1B89-4B9D-A4E8-F69E7F38466B"), // Дт - детали
                new Guid("86154C96-461D-4A0C-803C-125F90B6FFEE"), // Мт - материалы
                new Guid("477CD48D-251A-4C51-8232-FFA67476AE22")  // Пи - прочие
                // new Guid("7503D82C-F99F-4DEA-8A2C-01694E6D5A46")  // Ст - стандартные
                // new Guid("5DE347AA-667C-41D5-8C56-1BEAEA535613")  // Км - комплекты
                // new Guid("017A023D-2E22-46DB-9915-2D4A513B79B8")  // Дк - документы
                // new Guid("52E2A438-6BAE-4BF1-9219-555EC5A18C6D")  // Сб - сборочные единицы
            };
            return zapret.Contains(p);
        }

        /// <summary>
        /// Копирование всей спецификации из одного продукта в другой
        /// </summary>
        public void CopyConfiguration(Guid fromProductID, Guid toProductID, Guid UserID)
        {
            // копирование по аналогу вместе с составом
            using (CommonDomain domain = new CommonDomain())
            {
                var confs = from p in domain.Configurations
                            where p.ProductOwnerID == fromProductID
                            select p;
                foreach (Aspect.Domain.Configuration conf in confs)
                {
                    Aspect.Domain.Configuration confRow = new Aspect.Domain.Configuration();
                    confRow.AutoUpdate = conf.AutoUpdate;
                    confRow.Comment = conf.Comment;
                    confRow.GroupNumber = conf.GroupNumber;
                    confRow.GroupToChange = conf.GroupToChange;
                    confRow.Position = conf.Position;
                    confRow.Quantity = conf.Quantity;
                    confRow.QuantityInclusive = conf.QuantityInclusive;
                    confRow.Zone = conf.Zone;
                    confRow._dictUMID = conf._dictUMID;
                    confRow.dt_upd = conf.dt_upd;
                    if (Guid.Empty.Equals(conf.UserID))
                    {
                        confRow.UserID = UserID;
                    }
                    else
                    {
                        confRow.UserID = conf.UserID;
                    }
                    confRow.ID = Guid.NewGuid();
                    confRow.ProductOwnerID = toProductID;
                    confRow.ProductID = conf.ProductID;
                    domain.Configurations.InsertOnSubmit(confRow);
                }
                domain.SubmitChanges();
            }
        }

        /// <summary>
        /// Копирование продукта внутри того же класса с заданием
        /// новой версии. Копируются внешние и внутренние свойства 
        /// продукта. Можно указать новое значение словаря _dictNomen.
        /// </summary>
        /// <param name="oldProductID"></param>
        /// <param name="newVersion"></param>
        /// <returns>Guid созданного при копировании продукта</returns>
        public Guid CopyProduct(Guid oldProductID, int newVersion, Guid newDictNomen, Guid userID)
        {
            /* создать запись в dbo.Products */
            Guid newProductID;
            Product newProduct, oldProduct;

            using (CommonDomain commonDomain = new CommonDomain())
            {
                // копируем запись в таблице Product
                oldProduct = commonDomain.Products.SingleOrDefault(p => p.ID == oldProductID);
                newProductID = commonDomain.AddNewProduct(oldProductID, true, Guid.Empty.Equals(newDictNomen) ? oldProduct._dictNomenID.Value : newDictNomen, userID);                
                newProduct = commonDomain.Products.SingleOrDefault(p => p.ID == newProductID);

                // копируем ссылки на словари
                newProduct._dictNomen1ID = oldProduct._dictNomen1ID;
                newProduct._dictPVDID = oldProduct._dictPVDID;
                newProduct._dictSFID = oldProduct._dictSFID;
                newProduct._dictUMID = oldProduct._dictUMID;

                /*newProduct.cod_Asp = oldProduct.cod_Asp;
                newProduct.cod_Nomen_Asp = oldProduct.cod_Nomen_Asp;
                newProduct.cod_Nomen1_Asp = oldProduct.cod_Nomen1_Asp;*/
                commonDomain.SubmitChanges();
            }

            /* скопировать все поля в dbo.ProductProperty */
            using (CommonDomain domain = new CommonDomain())
            {
                List<ProductProperty> props = domain.ProductProperties.Where(p => p.ProductID == oldProductID).ToList();
                foreach (ProductProperty prop in props)
                {
                    ProductProperty newProp = new ProductProperty();
                    newProp.ID = Guid.NewGuid();
                    newProp.PropertyID = prop.PropertyID;
                    newProp.ProductID = newProduct.ID;
                    newProp.Value = prop.Value;
                    if (prop.PropertyID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552")) // основная версия
                    {
                        newProp.Value = "0";
                    }
                    if (prop.PropertyID == new Guid("0789DB1A-9BAA-4574-B405-AE570C746C03")) // версия
                    {
                        newProp.Value = newVersion.ToString();
                    }
                    if (prop.PropertyID == new Guid("3EA753C8-2B06-41DA-B3DB-421D110B079E")) // дата изменения продукта
                    {
                        // управляется триггером
                        continue;
                    }
                    domain.ProductProperties.InsertOnSubmit(newProp);
                }
                domain.SubmitChanges();
            }

            /* вернуть ID созданного продукта */
            return newProductID;
        }
        
        /// <summary>
        /// Создание продукта с аналогичным составом и с наибольшей новой версией.
        /// </summary>
        /// <param name="oldProductID"></param>
        /// <returns></returns>
        public Guid CreateNewVersionOfProduct(Guid oldProductID, Guid userID)
        {
            // копирование продукта в том же классе
            Guid newProductID = this.CopyProduct(oldProductID, getMaxVersionByProductID(oldProductID) + 1, Guid.Empty, userID);

            /* скопировать конфигурацию в dbo.Configuration */
            CopyConfiguration(oldProductID, newProductID, userID);
                        
            /* вернуть ID созданного продукта */
            return newProductID;
        }

        public bool isLessOfMainVersion(Guid entiryID)
        {
            // проверка на существование "Основной версии" с большей версией
            
            // получение номенклатурного номера продукта      
            var dictNomenIDs = from uv in Products
                              where uv.ID == entiryID
                              select uv._dictNomenID;
            Guid dictNomenID = (Guid) dictNomenIDs.First();
            
            // получение всех продуктов с данным номенклатурным номером
            var allProducts = from uv in Products
                              where uv._dictNomenID == dictNomenID
                              select uv.ID;            
            List<Guid> allProduct = allProducts.ToList();            
            List<Guid> mainProduct = new List<Guid>();
            
            // получение продуктов, версии которых "Основные"
            foreach(Guid productID in allProduct)
            {
                var mainValues = from uv in ProductProperties
                                where uv.ProductID == productID
                                    && uv.PropertyID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552")
                                select uv.Value;
                if (mainValues.First() == "1") mainProduct.Add(productID);
            }                      
            
            // получение максимальной версии продукта с "Основной версией"
            int maxVersion = -1;
            foreach (Guid productID in mainProduct)
            {
                var productVersions = from uv in ProductProperties
                                      where uv.ProductID == productID
                                        && uv.PropertyID == new Guid("0789DB1A-9BAA-4574-B405-AE570C746C03")
                                      select uv.Value;
                if (maxVersion < Convert.ToInt32(productVersions.First()))
                {
                    maxVersion = Convert.ToInt32(productVersions.First());
                }
            }

            // получение версии данного продукта
            var thisVersions = from uv in ProductProperties
                               where uv.ProductID == entiryID
                                && uv.PropertyID == new Guid("0789DB1A-9BAA-4574-B405-AE570C746C03")
                               select uv.Value;
            int thisVersion = Convert.ToInt32(thisVersions.First());
        
            // сравнение версий
            if (maxVersion > thisVersion)
                return true;
            else
                return false;
        }

        public override bool AllowDelete(List<Guid> roles, Guid entityID)
        {
            var d = from uv in RoleViewPermissions
                    join ctp in ClassificationTreeProducts on uv.EntityID equals ctp.ClassificationTreeID
                    where uv.PermissionEntityID == PermissionEntity.ClassificationTree && !uv.Delete
                    && roles.Contains(uv.RoleID) && ctp.ProductID == entityID
                    select uv.EntityID;
            if (d.Count() > 0) return false;

            var d1 = from uv in RoleViewPermissions
                     join cct in CustomClassificationTrees on uv.EntityID equals cct.ID
                     join ctp in CustomClassificationNodeProducts on cct.CustomClassificationNodeID equals ctp.CustomClassificationNodeID
                     where uv.PermissionEntityID == PermissionEntity.ClassificationTree && !uv.Delete
                     && roles.Contains(uv.RoleID) && ctp.ProductID == entityID
                     select uv.EntityID;

            if (d1.Count() > 0) return false;

            return true;
        }

        /// <summary>
        /// Устанавливает свойство "Основавная версия"
        /// </summary>        
        public bool SetMainVersion (Guid UserID, List<Guid> ProductIDs)
        {
            // Получаем права пользователя
            List<Guid> roles = new List<Guid>();
            using (Aspect.Model.Authentication.Provider provider = new Aspect.Model.Authentication.Provider())
            {
                roles = provider.GetUserRoles(UserID);
            }

            // Проверка -- Разрешено ли пользователю менять основную версию
            List<Property> props = this.GetAvailablePropertiesToModify(roles);
            if (!props.Any(p => p.ID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552")))
            {
                // текущему пользователю нельзя менять значение поля "Основная версия"
                return false;
            }

            // Устанавливаем поле "Основная версия" в "1"
            List<ProductProperty> propsMainVersion = (from p in ProductProperties
                                                      where p.PropertyID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552")
                                                      && ProductIDs.Contains(p.ProductID)
                                                      && p.Value == "0"
                                                      select p).ToList();
            propsMainVersion.ForEach(p => p.Value = "1");
            SubmitChanges();

            // Переносим вес из продуктов в _dictNomen, если там он отсутствует (0 или null)            
            foreach (Guid ProductID in ProductIDs)
            {
                try
                {
                    // Пытаемся получить свойство с весом если оно есть
                     decimal? pw = ( from p in Products
                                 join pp in ProductProperties on p.ID equals pp.ProductID
                                 where p.ID == ProductID && pp.PropertyID == new Guid("AC37F816-E4C1-4751-99ED-6180D7CCA142")
                                 select Convert.ToDecimal(pp.Value) ).Single();                    
                    // Если свойство есть переносим его
                    if (pw.HasValue && pw.Value != 0)
                    {
                        _dictNomen dict = ( from p in Products
                                            join d in _dictNomens on p._dictNomenID equals d.ID
                                            where p.ID == ProductID
                                            select d).Single();
                        if (!dict.pw.HasValue || (dict.pw.HasValue && dict.pw.Value == 0))
                        {
                            dict.pw = pw.Value;
                            SubmitChanges();
                        }
                    }
                }
                catch
                {
                   // перехватываем исключение, так как веса у продукта может вовсе и не быть 
                }
            }           
            
            return true;
        }

        /// <summary>
        /// Перемещение продукта из класса (МатРесурсы) в заданный класс.
        /// </summary>
        /// <param name="dictNomenID"></param>
        /// <param name="newClassificationTreeID"></param>
        public void ChangeProductClassification(Guid dictNomenID, Guid newClassificationTreeID)
        {
            // находим единственный продукт, соответствующий данной номенклатуре
            // если продуктов несколько вырабатается исключение
            Guid productID = Products.Where(p => p._dictNomenID == dictNomenID).Select(p => p.ID).Single();

            // удалям продукт из класса (МатРесурсы)
            ClassificationTreeProducts.DeleteOnSubmit(
                ClassificationTreeProducts.Where(cls => cls.ProductID == productID
                && cls.ClassificationTreeID == new Guid("55C7B455-0638-4ACB-AC2E-5B4992E48462")).Single());

            // вставляем продукт в новый класс
            ClassificationTreeProduct newClassification = new ClassificationTreeProduct();
            newClassification.ID = Guid.NewGuid();
            newClassification.ProductID = productID;
            newClassification.ClassificationTreeID = newClassificationTreeID;
            ClassificationTreeProducts.InsertOnSubmit(newClassification);

            // подтверждаем изменения
            SubmitChanges();            
        }

        /// <summary>
        /// Установка "Веса по приказу" для первой версии продукта.
        /// </summary>
        /// <param name="dictNomenID"></param>
        /// <param name="newClassificationTreeID"></param>
        public void SetProductWeight(Guid dictNomenID, decimal weight)
        {
            // находим единственный продукт, соответствующий данной номенклатуре
            // если продуктов несколько вырабатается исключение
            Guid productID = Products.Where(p => p._dictNomenID == dictNomenID).Select(p => p.ID).Single();

            Guid weightPropertyID = new Guid ("AC37F816-E4C1-4751-99ED-6180D7CCA142");            
            try
            {
                // ищем для продукта свойство "Вес по приказу" и устанавливаем его
                ProductProperty weightProperty = ProductProperties.Where(pp => pp.ProductID == productID && pp.PropertyID == weightPropertyID).Single();
                weightProperty.Value = weight.ToString().Replace(".", ",");
            }
            catch
            {
                // если свойство не найдено, создаём его и устанавливаем
                ProductProperty weightProperty = new ProductProperty();
                weightProperty.ID = Guid.NewGuid();
                weightProperty.ProductID = productID;
                weightProperty.PropertyID = weightPropertyID;
                weightProperty.Value = weight.ToString().Replace(".", ",");
                ProductProperties.InsertOnSubmit(weightProperty);
            }

            // подтверждаем изменения
            SubmitChanges();
        }

        public Product GetActualOrderProduct(Guid order_id)
        {
            // получаем приказ
            var order = OrderArticles.SingleOrDefault(ord => ord.ID == order_id);

            // получаем список всех продуктов связанных с номенклатурой
            List<Product> order_products = (from prod in Products
                                           where prod._dictNomenID == order._dictNomenID
                                           select prod).ToList();

            // отсеиваем неприказные продукты
            order_products = order_products.Where(prod => prod.OrderNumber.Length > 0 && prod.OrderYear.Length > 0).ToList();

            // получаем максимальную версию продукта
            int max_version = order_products.Max(prod => Convert.ToInt32(prod.Version));

            // возвращаем продукт с максимальной версией
            return order_products.Where(prod => Convert.ToInt32(prod.Version) == max_version).Single();
        }
    }
}
