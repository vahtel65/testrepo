using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aspect.Model.ProductDomain.Query
{
    public class ClassificationWhereClauseBuilder : Aspect.Domain.WhereClause
    {
        #region Templates
        private string classificationWhereClauseTemplate = @"ClassificationTreeProduct.ClassificationTreeID IN ({0})";
        #endregion

        /*private StringBuilder expression;
        /// <summary>
        /// Classification WHERE Clause
        /// </summary>
        public override StringBuilder Expression
        {
            get
            {
                if (expression == null) expression = new StringBuilder();
                return expression;
            }
        }*/
        
        public ClassificationWhereClauseBuilder(Guid classificationTreeID, Aspect.Domain.CommonDomain provider)
        {
            if (classificationTreeID != Guid.Empty)
            {
                List<string> list = new List<string>();
                foreach (Aspect.Domain.GetClassificationTreeChildsResult item in provider.GetClassificationTreeChilds(classificationTreeID))
                {
                    if (item.id != null) list.Add(string.Format("'{0}'", item.id.ToString()));
                }
                list.Add(string.Format("'{0}'", classificationTreeID.ToString()));
                string[] ids = list.ToArray();
                Expression.AppendFormat(classificationWhereClauseTemplate, string.Join(",", ids));
            }
        }
        public ClassificationWhereClauseBuilder(List<Guid?> classificationTreeIDs, Aspect.Domain.CommonDomain provider)
        {
            if (classificationTreeIDs.Count > 0)
            {
                List<Guid?> ids = new List<Guid?>();
                foreach (Guid? item in classificationTreeIDs)
                {
                    if (item.HasValue)
                    {
                        ids.AddRange(
                            provider.GetClassificationTreeChilds(item.Value).ToList().ConvertAll(
                            delegate(Aspect.Domain.GetClassificationTreeChildsResult n){
                            return n.id; }));
                        ids.Add(item);
                    }
                    //if(item != null) list.Add(string.Format("'{0}'", item.ToString()));
                }
                string[] list = ids.Distinct().Where(d => d.HasValue).ToList().ConvertAll(delegate(Guid? n){
                    return string.Format("'{0}'", n.Value.ToString());
                }).ToArray();
                if (list.Count() > 0)
                {
                    Expression.AppendFormat(classificationWhereClauseTemplate, string.Join(",", list));
                }
            }
        }

    }
}
