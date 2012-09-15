using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Aspect.Domain;

namespace Aspect.Model.ConfigurationDomain
{
    public class ConfigurationSource
    {
        public Product Product { get; set; }
        public Configuration Configuration { get; set; }
        /*public string Version { get; set; }
        public string MainVersion { get; set; }
        public string OrderNumber { get; set; }
        public string OrderYear { get; set; }*/
        //public bool HasConfiguration { get; set; }
    }
    
    public class UsageConfigurationProvider : CommonDomain
    {
        public List<Product> GetProductVersions(Guid productID)
        {
            Product entity = this.GetProduct(productID);
            if (entity != null)
            {
                var q = from p in Products
                        join n in _dictNomens on p._dictNomenID equals n.ID
                        where n.ID == entity._dictNomenID
                        orderby p.CreatedDate
                        select p;
                return q.ToList();
            }
            return new List<Product>();
        }
        public List<ConfigurationSource> GetProductUsage(Guid productID)
        {
            var q = (from c in Configurations
                    where c.ProductID == productID && (!c.GroupNumber.HasValue || c.GroupNumber == 0)
                    select new ConfigurationSource()
                    {
                        Configuration = c,
                        Product = c.Product1,
                        /*MainVersion = c.Product1.ProductProperties.SingleOrDefault(p=>p.PropertyID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552")).Value,
                        OrderNumber = c.Product1.ProductProperties.SingleOrDefault(p=>p.PropertyID == new Guid("9A38E338-DD60-4636-BFE3-6A98BAF8AE87")).Value,
                        OrderYear = c.Product1.ProductProperties.SingleOrDefault(p=>p.PropertyID == new Guid("2CCD4FF3-6D43-4A35-9784-969FAB46B5CC")).Value*/
                    }).ToList();
            return q.Distinct().ToList();
        }

        public List<ConfigurationSource> GetProductUsageChange(Guid productID)
        {
            var q = (from c in Configurations
                    where c.ProductID == productID && (c.GroupNumber.HasValue && c.GroupNumber != 0)
                    select new ConfigurationSource()
                    {
                        Configuration = c,
                        Product = c.Product1
                    }).ToList();
            List<ConfigurationSource> result = q.Distinct().
                OrderBy(i => i.Product._dictNomen._dictH.hso).
                ThenBy(i => Convert.ToInt32(i.Configuration.Position)).
                ThenBy(i => i.Product._dictNomen.superpole).ToList();
            return result;
        }
        public List<ConfigurationSource> GetProductSpecification(Guid productID)
        {
            var q = (from c in Configurations
                    where c.ProductOwnerID == productID && (!c.GroupNumber.HasValue || c.GroupNumber == 0)
                    orderby c.Position
                    select new ConfigurationSource()
                    {
                        Configuration = c,
                        Product = c.Product
                    }).ToList();
            List<ConfigurationSource> result = q.Distinct().
                OrderBy(i => i.Product._dictNomen._dictH.hso).
                ThenBy(i => Convert.ToInt32(i.Configuration.Position)).
                ThenBy(i => i.Product._dictNomen.superpole).ToList();
            //UpdateHasConfiguration(result);
            return result;
        }
        public List<ConfigurationSource> GetProductSpecificationChange(Guid productID)
        {
            var q = (from c in Configurations
                    where c.ProductOwnerID == productID && (c.GroupNumber.HasValue && c.GroupNumber != 0)
                    select new ConfigurationSource()
                    {
                        Configuration = c,
                        Product = c.Product
                    }).ToList();
            List<ConfigurationSource> result = q.Distinct().
                OrderBy(i => i.Product._dictNomen._dictH.hso).
                ThenBy(i => Convert.ToInt32(i.Configuration.Position)).
                ThenBy(i => i.Product._dictNomen.superpole).ToList();

            //UpdateHasConfiguration(result);
            return result;
        }

        #region SQL Queqy for GetApplicability()
        
        /// <summary>
        /// {0} - Product ID
        /// </summary>
        private string QueryGetApplicability
        {
            get
            {
                return @"with Hierachy(ProductOwnerID, ProductID, Quantity, Level)
                        as
                        (
                        select ProductOwnerID, ProductID, Quantity, 0 as Level
                            from Configuration c
                            where c.ProductID = '{0}' -- insert parameter here    
                            union all
                            
                            select c.ProductOwnerID, c.ProductID, c.Quantity,  ch.Level + 1
                            from Configuration c
                            inner join Hierachy ch
                            on c.ProductID = ch.ProductOwnerID
                        )
                        select Hierachy.ProductOwnerID, Hierachy.ProductID, Hierachy.Quantity,Hierachy.Level, 
	                        vers.Value as Version,
	                        actual.Value as Actual,
	                        _dictNomen.pn1 as pn1,
	                        _dictNomen.superpole as pn2,
	                        order_number.Value as OrderNumber,
	                        order_year.Value as OrderYear
                        from Hierachy
                        left join ProductProperty vers on vers.PropertyID = '0789DB1A-9BAA-4574-B405-AE570C746C03' AND vers.ProductID = Hierachy.ProductOwnerID
                        left join ProductProperty actual on actual.PropertyID = 'BBE170B0-28E4-4738-B365-1038B03F4552' AND actual.ProductID = Hierachy.ProductOwnerID
                        left join ProductProperty order_year on order_year.PropertyID = '2CCD4FF3-6D43-4A35-9784-969FAB46B5CC' AND order_year.ProductID = Hierachy.ProductOwnerID
                        left join ProductProperty order_number on order_number.PropertyID = '9A38E338-DD60-4636-BFE3-6A98BAF8AE87' AND order_number.ProductID = Hierachy.ProductOwnerID
                        inner join Product on Product.ID = Hierachy.ProductOwnerID
                        inner join _dictNomen on _dictNomen.ID = Product._dictNomenID";
            }
        }

        /// <summary>
        /// {0} - Product ID
        /// </summary>
        private string QueryGetApplicability2
        {
            get
            {
                return @"IF OBJECT_ID (N'dbo.#1', N'U') IS NOT NULL    DROP TABLE dbo.#1;
                        IF OBJECT_ID (N'dbo.#2', N'U') IS NOT NULL    DROP TABLE dbo.#2;
                        IF OBJECT_ID (N'dbo.#3', N'U') IS NOT NULL    DROP TABLE dbo.#3;

                        with Hierachy(ProductOwnerID, ProductID, Quantity, Level)
	                        as
	                        (
	                        select ProductOwnerID, ProductID, cast(Quantity as int) as Quantity, 0 as Level
		                        from Configuration c
		                        where c.ProductID = '{0}' -- insert parameter here    
		                        union all
                        		
		                        select c.ProductOwnerID, c.ProductID, cast(c.Quantity*ch.Quantity as int) as Quantity,  ch.Level + 1
		                        from Configuration c
		                        inner join Hierachy ch
		                        on c.ProductID = ch.ProductOwnerID
	                        )
	                        select Hierachy.ProductOwnerID, Hierachy.ProductID, Hierachy.Quantity,Hierachy.Level
	                        into #1
	                        from Hierachy
	                        select ProductOwnerID, SUM( Quantity) as Quantity
		                        into #2
		                        from #1
		                        group by ProductOwnerID
	                        select ProductOwnerID, Quantity ,
		                        vers.Value as Version,
		                        actual.Value as Actual,
		                        _dictNomen.pn1 as pn1,
		                        _dictNomen.superpole as pn2,
		                        order_number.Value as OrderNumber,
		                        order_year.Value as OrderYear,
		                        cast(0 as bit) as [top]
	                        into #3
	                        from #2
	                        left join ProductProperty vers on vers.PropertyID = '0789DB1A-9BAA-4574-B405-AE570C746C03' AND vers.ProductID = #2.ProductOwnerID
	                        left join ProductProperty actual on actual.PropertyID = 'BBE170B0-28E4-4738-B365-1038B03F4552' AND actual.ProductID = #2.ProductOwnerID
	                        left join ProductProperty order_year on order_year.PropertyID = '2CCD4FF3-6D43-4A35-9784-969FAB46B5CC' AND order_year.ProductID = #2.ProductOwnerID
	                        left join ProductProperty order_number on order_number.PropertyID = '9A38E338-DD60-4636-BFE3-6A98BAF8AE87' AND order_number.ProductID = #2.ProductOwnerID
	                        inner join Product on Product.ID = #2.ProductOwnerID
	                        inner join _dictNomen on _dictNomen.ID = Product._dictNomenID
	                        update #3  set [top]=1 where not exists (select * from Configuration  where ProductID=#3.ProductOwnerID) 
	                        select * from #3";
            }
        }

        #endregion

        public DataSet GetApplicability(Guid prod_id)
        {
            using (CommonDataProvider provider = new CommonDataProvider())
            {
                return provider.ExecuteCommand(String.Format(QueryGetApplicability2, prod_id));                   
            }            
        }

        /*public void UpdateHasConfiguration(List<ConfigurationSource> list)
        {
            foreach (ConfigurationSource s in list)
            {
                s.HasConfiguration = Configurations.Where(c => c.ProductOwnerID == s.Product.ID).Count() > 0;
            }
        }*/
    }
}
