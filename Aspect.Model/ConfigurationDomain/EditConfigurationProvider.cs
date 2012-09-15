using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspect.Domain;
using System.Web.Script.Serialization;
using NLog;
using System.Data.Linq;

namespace Aspect.Model.ConfigurationDomain
{
    public class EditConfigurationProvider : CommonDomain
    {
        public List<GridColumn> GetGridColumns()
        {
            List<GridColumn> columns = new List<GridColumn>();
            columns.Add(EditableGridColumn.PositionColumn);
            columns.Add(GridColumn.IdentifierColumn);
            columns.Add(GridColumn.SpecColumn);
            columns.Add(EditableGridColumn.QuantityColumn);
            //
            EditableGridColumn column = EditableGridColumn._dictUMColumn;
            column.DataSource = new EditableGridColumn.Source();
            column.DataSource.DataSource = this._dictUMs.ToList().ConvertAll(
                delegate(_dictUM n)
                {
                    return new Pair<Guid, string>(n.ID, n.PublicName );
                });

            column.DataSource.ValueField = "First";
            column.DataSource.TextField = "Second";
            columns.Add(column);
            //            
            columns.Add(GridColumn.OrderWeightColumn);
            columns.Add(EditableGridColumn.GroupToChangeColumn);
            columns.Add(EditableGridColumn.GroupNumberColumn);
            columns.Add(EditableGridColumn.QuantityInclusiveColumn);
            columns.Add(EditableGridColumn.CommentColumn);
            columns.Add(GridColumn.MainVersionColumn);
            columns.Add(GridColumn.VersionColumn);
            columns.Add(GridColumn.NsColumn);
            columns.Add(GridColumn.DictFormatColumn);            
            columns.Add(GridColumn.OrderNumberColumn);
            columns.Add(GridColumn.OrderYearColumn);
            columns.Add(EditableGridColumn.AutoUpdateColumn);
            columns.Add(EditableGridColumn.ZoneColumn);
            columns.Add(GridColumn.UserLEColumn);
            columns.Add(GridColumn.DateLEColumn);
            columns.Add(GridColumn.DesignationColumn);
            columns.Add(GridColumn.TitlenameColumn);            
            return columns;
        }
        
        /*
        private List<GridColumn> GetEditableColumns()
        {
            List<GridColumn> list = new List<GridColumn>();
            list.Add(EditableGridColumn.PositionColumn);
            list.Add(EditableGridColumn.QuantityColumn);

            EditableGridColumn column = EditableGridColumn._dictUMColumn;
            column.DataSource = new EditableGridColumn.Source();

            //List<_dictUM> list1 = this._dictUMs.ToList();

            //column.DataSource.DataSource = this._dictUMs.ToList();
            column.DataSource.DataSource = this._dictUMs.ToList().ConvertAll(
                delegate(_dictUM n)
                {
                    return new Pair<Guid, string>(n.ID, n.PublicName);
                });

            column.DataSource.ValueField = "First";
            column.DataSource.TextField = "Second";
            //column.DataSource.ValueField = this._dictUMs
            list.Add(column);

            list.Add(EditableGridColumn.GroupNumberColumn);
            list.Add(EditableGridColumn.GroupToChangeColumn);
            list.Add(EditableGridColumn.AutoUpdateColumn);
            list.Add(EditableGridColumn.QuantityInclusiveColumn);
            list.Add(EditableGridColumn.ZoneColumn);
            list.Add(EditableGridColumn.CommentColumn);
            return list;
        }
        */
        public System.Data.DataSet GetList(Guid productID)
        {
            //Query.Query query = new Aspect.Model.ConfigurationDomain.Query.Query();
            string sql = BuildConfigurationQuery(productID);
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                return provider.ExecuteCommand(sql);
            }
        }

        public System.Data.DataSet GetAppendList(Dictionary<Guid, Guid> multiBuffer)
        {
            int countProducts = multiBuffer.Where(item => Guid.Empty.Equals(item.Value)).Count();
            int countProductsWithConfiguration = multiBuffer.Where(item => !Guid.Empty.Equals(item.Value)).Count(); ;
            System.Data.DataSet dataSet = new System.Data.DataSet();
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                /*
                 * Надо обязательно исправить, в случае когда в буфере будут и конфигурации 
                 * и просто продукты -- получится не хорошо...
                 */
                if (countProducts > 0)
                {
                    string sqlProducts = BuildAppendProductsQuery(multiBuffer);
                    dataSet.Merge(provider.ExecuteCommand(sqlProducts));
                }

                if (countProductsWithConfiguration > 0)
                {
                    string sqlConfigurations = BuildAppendConfigurationsQuery(multiBuffer);
                    dataSet.Merge(provider.ExecuteCommand(sqlConfigurations));
                }
            }
            return dataSet;
        }

        private class ConfListComparerByID
        {
            public List<Configuration>  listNewed;
            private List<Configuration>  listOldChanged;
            public List<Configuration>  listChanged;
            public List<Configuration>  listDeleted;

            /*
             * сравнение двух конфигураций
             */
            public static bool isIdentialConf(Configuration conf1, Configuration conf2)
            {
                if (conf1.AutoUpdate != conf2.AutoUpdate) return false;
                if (conf1.Comment != conf2.Comment) return false;
                if (conf1.GroupNumber != conf2.GroupNumber) return false;
                if (conf1.GroupToChange != conf2.GroupToChange) return false;
                if (conf1.Position != conf2.Position) return false;
                if (conf1.Quantity != conf2.Quantity) return false;
                if (conf1.QuantityInclusive  != conf2.QuantityInclusive) return false;
                if (conf1._dictUMID != conf2._dictUMID) return false;
                if (conf1.Zone != conf2.Zone) return false;
                return true;
            }

            public ConfListComparerByID(List<Configuration> list1, List<Configuration> list2)
            {
                // поиск новых конфигураций
                listNewed = list2.Where(cnf2 => !list1.Exists(cnf1 => cnf1.ID == cnf2.ID)).ToList();
                                
                // поиск удалённых конфигураций
                listDeleted = list1.Where(cnf1 => !list2.Exists(cnf2 => cnf2.ID == cnf1.ID)).ToList();

                // поиск изменившихся
                listChanged = new List<Configuration>();
                listOldChanged = new List<Configuration>();
                foreach (Configuration confEtalon in list1)
                {
                    foreach (Configuration confItem in list2)
                    {
                        if (confEtalon.ID == confItem.ID)
                        {
                            if (!isIdentialConf(confEtalon, confItem))
                            {
                                listChanged.Add(confItem);
                                listOldChanged.Add(confEtalon);
                                break;
                            }                            
                        }
                    }
                }
            }

            public Configuration getConfBeforeChanged(Configuration condAfterChanged)
            {
                return listOldChanged.Single(conf => conf.ID == condAfterChanged.ID);
            }
        }

        public bool SaveProductConfiguration(Guid productID, List<Configuration> list, Guid userID)
        {
            //try
            //{                
            List<Configuration> todelete = Configurations.Where(c => c.ProductOwnerID == productID).ToList();
            ConfListComparerByID comparer = new ConfListComparerByID(todelete, list);
            DateTime dateTimeNow = DateTime.Now;            

            foreach (Configuration conf in comparer.listNewed)
            {
                conf.dt_upd = dateTimeNow;
                conf.ID = Guid.NewGuid();                
            }

            foreach (Configuration conf in comparer.listChanged)
            {
                conf.dt_upd = dateTimeNow;
            }

            #region LOGGING
            StringBuilder logEntry = new StringBuilder("Delete configuration - ");
            foreach (Configuration t in todelete)
            {
                string str = string.Format("{11}ProductOwnerID: {10}; ProductID: {0}; _dictUMID: {1}; AutoUpdate: {2}; Comment: {3}; GroupNumber: {4}; GroupToChange: {5}; Quantity: {6}; QuantityInclusive: {7}; version: {8}; Zone: {9};", t.ProductID,
                    t._dictUMID.HasValue ? t._dictUMID.ToString() : "NULL",
                    t.AutoUpdate,
                    t.Comment == null ? String.Empty : t.Comment.Trim(),
                    t.GroupNumber.HasValue ? t.GroupNumber.ToString() : "NULL",
                    t.GroupToChange.HasValue ? t.GroupToChange.ToString() : "NULL",
                    t.Quantity,
                    t.QuantityInclusive,
                    t.version.HasValue ? t.version.ToString() : "NULL",
                    t.Zone,
                    productID, System.Environment.NewLine);
                logEntry.Append(str);
            }
            if (todelete.Count > 0)
            {
                Aspect.Utility.TraceHelper.Log(userID, logEntry.ToString());
            }
            #endregion
            #region logging_advance
            {
                Logger loggerAdd = LogManager.GetLogger(String.Format("{0},{1}", userID, "Configuration,Add"));
                Logger loggerChange = LogManager.GetLogger(String.Format("{0},{1}", userID, "Configuration,Change"));
                Logger loggerDel = LogManager.GetLogger(String.Format("{0},{1}", userID, "Configuration,Delete"));
                System.Reflection.PropertyInfo[] confProps = typeof(Aspect.Domain.Configuration).GetProperties();
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                // для созданых конфигурация сохраняем новые поля
                foreach (Configuration conf in comparer.listNewed)
                {
                    //loggerAdd.Trace("{{ProductID='{0}', ConfigurationID='{1}'}}", productID, conf.ID);
                    List<object> props = new List<object>();
                    foreach (System.Reflection.PropertyInfo propInfo in confProps)
                    {
                        if (!propInfo.PropertyType.Name.Equals("Product") &&
                            !propInfo.PropertyType.Name.Equals("_dictUM"))
                        {
                            props.Add(new
                            {
                                Name = propInfo.Name,
                                OldValue = "",
                                NewValue = propInfo.GetValue(conf, null)
                            });
                        }
                    }

                    loggerAdd.Trace(serializer.Serialize(new
                    {
                        Properties = props
                    }));
                }

                // для удалённых конфигурация сохраняем новые поля
                foreach (Configuration conf in comparer.listDeleted)
                {
                    List<object> props = new List<object>();
                    foreach (System.Reflection.PropertyInfo propInfo in confProps)
                    {
                        if (!propInfo.PropertyType.Name.Equals("Product") &&
                            !propInfo.PropertyType.Name.Equals("_dictUM"))
                        {
                            props.Add(new
                            {
                                Name = propInfo.Name,
                                OldValue = propInfo.GetValue(conf, null),
                                NewValue = ""
                            });
                        }
                        
                    }

                    loggerDel.Trace(serializer.Serialize(new
                    {
                        Properties = props
                    }));
                }

                // для изменённых конфигурация сохраняем новые поля
                foreach (Configuration conf in comparer.listChanged)
                {
                    List<object> props = new List<object>();
                    foreach (System.Reflection.PropertyInfo propInfo in confProps)
                    {
                        if (!propInfo.PropertyType.Name.Equals("Product") &&
                            !propInfo.PropertyType.Name.Equals("_dictUM"))
                        {
                            props.Add(new
                            {
                                Name = propInfo.Name,
                                OldValue = propInfo.GetValue(comparer.getConfBeforeChanged(conf), null),
                                NewValue = propInfo.GetValue(conf, null)
                            });
                        }
                    }

                    loggerChange.Trace(serializer.Serialize(new
                    {
                        Properties = props
                    }));
                }
            }
            #endregion logging_advance

            Configurations.DeleteAllOnSubmit(comparer.listDeleted);
            this.SubmitChanges();

            #region LOGGING
            logEntry = new StringBuilder("Add configuration - ");
            foreach (Configuration t in list)
            {
                string str = string.Format(@"{11}ProductOwnerID: {10}; ProductID: {0}; _dictUMID: {1}; AutoUpdate: {2}; Comment: {3}; GroupNumber: {4}; GroupToChange: {5}; Quantity: {6}; QuantityInclusive: {7}; version: {8}; Zone: {9};", t.ProductID,
                    t._dictUMID.HasValue ? t._dictUMID.ToString() : "NULL",
                    t.AutoUpdate,
                    t.Comment == null ? String.Empty : t.Comment.Trim(),
                    t.GroupNumber.HasValue ? t.GroupNumber.ToString() : "NULL",
                    t.GroupToChange.HasValue ? t.GroupToChange.ToString() : "NULL",
                    t.Quantity,
                    t.QuantityInclusive,
                    t.version.HasValue ? t.version.ToString() : "NULL",
                    t.Zone,
                    productID, System.Environment.NewLine);
                logEntry.Append(str);
            }
            if (list.Count > 0)
            {
                Aspect.Utility.TraceHelper.Log(userID, logEntry.ToString());
            }
            #endregion

            foreach (Configuration conf in comparer.listChanged)
            {
                var confRow = (from p in Configurations
                               where p.ID == conf.ID
                               select p).Single();
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
                confRow.UserID = userID;
                this.SubmitChanges();
            }

            Configurations.InsertAllOnSubmit(comparer.listNewed);
            this.SubmitChanges();
            return true;
            /*}
            catch (Exception ex)
            {
                throw ex;
            }*/
        }

        #region Query
        /// <summary>
        /// 0 - from
        /// 1 - join
        /// 2 - where
        /// 3 - order
        /// </summary>
        protected string SqlConfigurationTemplate
        {
            get
            {
                return @"
SELECT 
c.ProductID AS ID,
c.ID AS ConfigurationID,
dn.superpole AS Identifier, 
dh.hsn1 AS Spec,
fh.fdn1 AS DictFormat, 
userrow.Name AS UserName,
dn.pn1 AS dn_pn1,
dn.pn2 AS dn_pn2,

pp1.Value AS MainVersion, 
pp2.Value AS OrderNumber, 
pp3.Value AS OrderYear, 
pp4.Value AS Version, 
pp5.Value AS OrderWeight,
pp6.Value AS p_ns,

c.Position AS Position, 
c.Quantity AS Quantity, 
c._dictUMID AS _dictUMID, 
c.GroupNumber AS GroupNumber, 
c.GroupToChange AS GroupToChange, 
c.AutoUpdate AS AutoUpdate, 
c.QuantityInclusive AS QuantityInclusive, 
c.Zone AS Zone, 
c.Comment AS Comment,
c.dt_upd AS dt_upd
FROM Product p
INNER JOIN Configuration c ON p.ID = c.ProductOwnerID
INNER JOIN Product p2 on p2.ID = c.ProductID

LEFT JOIN ProductProperty pp1 ON p2.id = pp1.ProductID AND pp1.PropertyID = 'BBE170B0-28E4-4738-B365-1038B03F4552' -- Основная версия
LEFT JOIN ProductProperty pp2 ON p2.id = pp2.ProductID AND pp2.PropertyID = '9A38E338-DD60-4636-BFE3-6A98BAF8AE87' -- Номер приказа
LEFT JOIN ProductProperty pp3 ON p2.id = pp3.ProductID AND pp3.PropertyID = '2CCD4FF3-6D43-4A35-9784-969FAB46B5CC' -- Год приказа
LEFT JOIN ProductProperty pp4 ON p2.id = pp4.ProductID AND pp4.PropertyID = '0789DB1A-9BAA-4574-B405-AE570C746C03' -- Версия
LEFT JOIN ProductProperty pp5 ON p2.id = pp5.ProductID AND pp5.PropertyID = 'AC37F816-E4C1-4751-99ED-6180D7CCA142' -- Вес по кризазу
LEFT JOIN ProductProperty pp6 ON p2.id = pp6.ProductID AND pp6.PropertyID = '00ACC1C7-6857-4317-8713-8B8D9479C5CC' -- Наличие состава
LEFT JOIN [User] userrow ON c.userID = userrow.ID

LEFT JOIN _dictNomen dn ON p2._dictNomenID = dn.ID
LEFT JOIN _dictHS dh ON dn._dictHSID = dh.ID
LEFT JOIN _dictFD fh ON dn._dictFDID = fh.ID
where p.ID = '{0}'
order by dh.hso, CAST(c.Position AS DECIMAL(10,2)), dn.superpole
";
            }
        }

        protected string SqlAppendConfigurationTemplate
        {
            get
            {
                return @"
SELECT 
p.ID AS ID,
CAST('{2}' as uniqueidentifier) AS ConfigurationID,
dn.superpole AS Identifier, 
dh.hsn1 AS Spec, 
fh.fdn1 AS DictFormat,
dn.pn1 AS dn_pn1,
dn.pn2 AS dn_pn2,

pp1.Value AS MainVersion, 
pp2.Value AS OrderNumber, 
pp3.Value AS OrderYear, 
pp4.Value AS Version,
pp6.Value AS p_ns,

'0' AS Position, 
CAST(1 AS decimal(18,5)) AS Quantity, 
CAST('{1}' AS uniqueidentifier) AS _dictUMID, 
CAST(0 as int) AS GroupNumber, 
CAST(0 as int) AS GroupToChange, 
CAST(0 as bit) AS AutoUpdate, 
CAST(1 as int) AS QuantityInclusive, 
CAST(NULL as nvarchar(10)) AS Zone, 
CAST(NULL as nvarchar(250)) AS Comment
FROM Product p

LEFT JOIN _dictNomen dn ON p._dictNomenID = dn.ID
LEFT JOIN _dictHS dh ON dn._dictHSID = dh.ID
LEFT JOIN _dictFD fh ON dn._dictFDID = fh.ID

LEFT JOIN ProductProperty pp1 ON p.id = pp1.ProductID AND pp1.PropertyID = 'BBE170B0-28E4-4738-B365-1038B03F4552' -- Основная версия
LEFT JOIN ProductProperty pp2 ON p.id = pp2.ProductID AND pp2.PropertyID = '9A38E338-DD60-4636-BFE3-6A98BAF8AE87' -- Номер приказа
LEFT JOIN ProductProperty pp3 ON p.id = pp3.ProductID AND pp3.PropertyID = '2CCD4FF3-6D43-4A35-9784-969FAB46B5CC' -- Год приказа
LEFT JOIN ProductProperty pp4 ON p.id = pp4.ProductID AND pp4.PropertyID = '0789DB1A-9BAA-4574-B405-AE570C746C03' -- Версия
LEFT JOIN ProductProperty pp5 ON p.id = pp5.ProductID AND pp5.PropertyID = 'AC37F816-E4C1-4751-99ED-6180D7CCA142' -- Вес по кризазу
LEFT JOIN ProductProperty pp6 ON p.id = pp6.ProductID AND pp6.PropertyID = '00ACC1C7-6857-4317-8713-8B8D9479C5CC' -- Наличие состава

WHERE p.ID in ({0})
order by dh.hso, dn.superpole
";
            }
        }

        protected string SqlAppendConfigurationTemplateWithConfig
        {
            get
            {
                return @"
SELECT 
p.ID AS ID,
CAST('{3}' as uniqueidentifier) AS ConfigurationID,
dn.superpole AS Identifier, 
dh.hsn1 AS Spec, 
fh.fdn1 AS DictFormat,
userrow.Name AS UserName,
dn.pn1 AS dn_pn1,
dn.pn2 AS dn_pn2,

pp1.Value AS MainVersion, 
pp2.Value AS OrderNumber, 
pp3.Value AS OrderYear, 
pp4.Value AS Version,
pp6.Value AS p_ns,

cfg.Position AS Position, 
cfg.Quantity AS Quantity, 
cfg._dictUMID AS _dictUMID, 
cfg.GroupNumber AS GroupNumber, 
cfg.GroupToChange AS GroupToChange, 
cfg.AutoUpdate AS AutoUpdate, 
cfg.QuantityInclusive AS QuantityInclusive, 
cfg.Zone AS Zone, 
cfg.Comment AS Comment,
cfg.dt_upd AS dt_upd
FROM Product p

INNER JOIN Configuration cfg on cfg.ProductID = p.ID

LEFT JOIN _dictNomen dn ON p._dictNomenID = dn.ID
LEFT JOIN _dictHS dh ON dn._dictHSID = dh.ID
LEFT JOIN _dictFD fh ON dn._dictFDID = fh.ID

LEFT JOIN ProductProperty pp1 ON p.id = pp1.ProductID AND pp1.PropertyID = 'BBE170B0-28E4-4738-B365-1038B03F4552' -- Основная версия
LEFT JOIN ProductProperty pp2 ON p.id = pp2.ProductID AND pp2.PropertyID = '9A38E338-DD60-4636-BFE3-6A98BAF8AE87' -- Номер приказа
LEFT JOIN ProductProperty pp3 ON p.id = pp3.ProductID AND pp3.PropertyID = '2CCD4FF3-6D43-4A35-9784-969FAB46B5CC' -- Год приказа
LEFT JOIN ProductProperty pp4 ON p.id = pp4.ProductID AND pp4.PropertyID = '0789DB1A-9BAA-4574-B405-AE570C746C03' -- Версия
LEFT JOIN ProductProperty pp5 ON p.id = pp5.ProductID AND pp5.PropertyID = 'AC37F816-E4C1-4751-99ED-6180D7CCA142' -- Вес по кризазу
LEFT JOIN ProductProperty pp6 ON p.id = pp6.ProductID AND pp6.PropertyID = '00ACC1C7-6857-4317-8713-8B8D9479C5CC' -- Наличие состава
LEFT JOIN [User] userrow ON cfg.userID = userrow.ID

WHERE p.ID in ({0})
AND cfg.ID in ({2})
order by dh.hso, CAST(cfg.Position AS DECIMAL(10,2)), dn.superpole
";
            }
        }

        private static string ProductOwnerID = "p.ID";

        private string BuildConfigurationQuery(Guid productID)
        {
            string where = WhereClause.BuildExpression(ProductOwnerID, productID.ToString());
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(SqlConfigurationTemplate,
                productID);
            return sql.ToString();
        }

        private string BuildAppendProductsQuery(Dictionary<Guid, Guid> multiBuffer)
        {
            List<string> products = new List<string>();
            foreach (KeyValuePair<Guid, Guid> item in multiBuffer)
            {
                if (Guid.Empty.Equals(item.Value))
                {
                    products.Add(string.Format("'{0}'", item.Key.ToString()));
                }
            }
            /*string[] products = multiBuffer.Keys.ToList().ConvertAll(delegate(Guid n)
            {
                return string.Format("'{0}'", n.ToString());
            }).ToArray();*/

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(SqlAppendConfigurationTemplate,
                    string.Join(",", products.ToArray()),
                    _dictUM.defaultValue,
                    Guid.Empty);
            return sql.ToString();
        }

        private string BuildAppendConfigurationsQuery(Dictionary<Guid, Guid> multiBuffer)
        {
            List<string> products = new List<string>(), configurations = new List<string>();
            foreach (KeyValuePair<Guid, Guid> item in multiBuffer)
            {
                if (!Guid.Empty.Equals(item.Value))
                {
                    products.Add(string.Format("'{0}'", item.Key.ToString()));
                    configurations.Add(string.Format("'{0}'", item.Value.ToString()));
                }
            }

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(SqlAppendConfigurationTemplateWithConfig,
                    string.Join(",", products.ToArray()),
                    _dictUM.defaultValue,
                    string.Join(",", configurations.ToArray()),
                    Guid.Empty);
            return sql.ToString();
        }
        #endregion
    }
}
