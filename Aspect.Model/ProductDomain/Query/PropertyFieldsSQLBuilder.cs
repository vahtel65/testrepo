using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Model.ProductDomain.Query
{
    /// <summary>
    /// Builds From and Join string for dynamic properties
    /// </summary>
    internal class PropertyFieldsSQLBuilder : Aspect.Model.Query.FieldsSQLBuilder
    {
        #region Templates
        //private string propertyFromTemplate = ",{0}.Value AS [{1}]";
        private string propertyFromTemplate = ",{0}.Value AS [{0}.{1}]";
        private string propertyJoinTemplate = @"
left join (
		SELECT pp.Value, pp.ProductID 
		FROM ProductProperty pp 
		INNER join [Property] p ON pp.PropertyID = p.ID  WHERE p.ID = '{0}' ) {1} 
	ON {1}.ProductID = product.ID
";
        #endregion

        public PropertyFieldsSQLBuilder(List<Aspect.Domain.Property> columns)
        {
            From = new StringBuilder();
            Join = new StringBuilder();
            foreach (Aspect.Domain.Property item in columns)
            {
                From.AppendFormat(propertyFromTemplate, item.Alias, item.Name);
                Join.AppendFormat(propertyJoinTemplate, item.ID, item.Alias);
            }
        }

    }
}
