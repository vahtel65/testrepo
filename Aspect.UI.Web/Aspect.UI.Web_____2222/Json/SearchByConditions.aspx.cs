using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
/* LINQ */
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Aspect.Domain;

namespace Aspect.UI.Web.Json
{
    #region DISTINCBY_HELPER
    public static class ProjectionComparer
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TValue> selector)
        {
            var comparer = ProjectionComparer<TSource>.CompareBy<TValue>(
                selector, EqualityComparer<TValue>.Default);
            return new HashSet<TSource>(source, comparer);
        }
    }
    public static class ProjectionComparer<TSource>
    {
        public static IEqualityComparer<TSource> CompareBy<TValue>(
            Func<TSource, TValue> selector)
        {
            return CompareBy<TValue>(selector, EqualityComparer<TValue>.Default);
        }
        public static IEqualityComparer<TSource> CompareBy<TValue>(
            Func<TSource, TValue> selector,
            IEqualityComparer<TValue> comparer)
        {
            return new ComparerImpl<TValue>(selector, comparer);
        }
        sealed class ComparerImpl<TValue> : IEqualityComparer<TSource>
        {
            private readonly Func<TSource, TValue> selector;
            private readonly IEqualityComparer<TValue> comparer;
            public ComparerImpl(
                Func<TSource, TValue> selector,
                IEqualityComparer<TValue> comparer)
            {
                if (selector == null) throw new ArgumentNullException("selector");
                if (comparer == null) throw new ArgumentNullException("comparer");
                this.selector = selector;
                this.comparer = comparer;
            }

            bool IEqualityComparer<TSource>.Equals(TSource x, TSource y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return comparer.Equals(selector(x), selector(y));
            }

            int IEqualityComparer<TSource>.GetHashCode(TSource obj)
            {
                return obj == null ? 0 : comparer.GetHashCode(selector(obj));
            }
        }
    }
    #endregion

    /*
     * Классы для разбора входящих данных
     */
    public class SearchCondition
    {
        public string Concat;       // {and, or}
        public string Condition;    // {=>, <=, >, <, =, !=, ==, !==}
        public string Alias;        // {Property.ID, DictionaryProperty.ID}
        public string Value;
    }

    public class SearchRequest
    {        
        public int start;
        public int limit;
        public string dir;          // {ASC, DECT}
        public string sort;         // {=alias}
        public int maxResult;
        public List<SearchCondition> searchConditions;

        public SearchRequest(HttpRequest request)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            searchConditions = js.Deserialize<List<SearchCondition>>(request["searchConditions"]);
            // очищаем от лишних пробелов
            searchConditions.ForEach(cond => cond.Value = cond.Value.Trim());
            start = Convert.ToInt32(request["start"]);
            limit = Convert.ToInt32(request["limit"]);
            dir = request["dir"];
            sort = request["sort"];
            maxResult = Convert.ToInt32(request["maxResult"]);
        }
    }

    /*
     * Поля продуктов и поля словарей
     */

    public class BaseField
    {
        /* Main fields */
        public Guid Id;
        public string Name;
        public Guid TypeId;
        public string Alias;

        /* Guid types from table [Aspect].[Type] */
        #region Available types
        static public Guid guidString = new Guid("4F55F13D-5C7D-4D45-9B82-4FDD9C65CCE3");
        static public Guid guidDecimal = new Guid("A669D88E-ED67-42FE-8A18-A54DB3614134");
        static public Guid guidInteger = new Guid("F1DDB372-44E2-4C9A-ADB2-A7C326AFA3DC");
        static public Guid guidBoolean = new Guid("BE264A7D-8B3E-46E0-95DF-C8A51A1E2FFF");
        static public Guid guidDateTime = new Guid("E48D93BE-C55D-4A62-9D3A-DD48F2BE9CE8");
        #endregion

        public string getConvertType()
        {
            if (TypeId.Equals(guidDecimal)) return "decimal";
            if (TypeId.Equals(guidInteger)) return "int";
            if (TypeId.Equals(guidDateTime)) return "datetime";
            return "";
        }

        public string getJsonType()
        {
            if (TypeId.Equals(guidDecimal)) return "float";
            if (TypeId.Equals(guidInteger)) return "int";
            if (TypeId.Equals(guidDateTime)) return "datetime";
            if (TypeId.Equals(guidBoolean)) return "boolean";
            if (TypeId.Equals(guidString)) return "string";
            return "";
        }

        public virtual string getSelect() { return ""; }
        public virtual string getJoin() { return ""; }
        public virtual string getWhere(SearchCondition cond) { return ""; }
    }

    public class DictionaryField : BaseField
    {
        public string Table;
        public string Column;

        public static List<DictionaryField> getAllDictionaryFields()
        {
            List<DictionaryField> listFields = new List<DictionaryField>();
            using (CommonDomain domain = new CommonDomain())
            {
                var properties = from p in domain.DictionaryProperties
                                 select p;
                foreach (DictionaryProperty prop in properties)
                {
                    DictionaryField field = new DictionaryField();
                    field.Id = prop.ID;
                    field.Name = prop.Name;
                    field.TypeId = prop.TypeID;
                    field.Table = prop.Dictionary.TableName;
                    field.Column = prop.ColumnName;
                    field.Alias = String.Format("{0}_{1}", prop.Dictionary.TableName, prop.ColumnName);
                    listFields.Add(field);
                }
                return listFields;
            }
        }

        public override string getSelect()
        {
            // "_dictNomen.superpole AS superpole"
            return String.Format("{0}.{1} AS {0}_{1}", Table, Column);
        }

        public override string getJoin()
        {            
            // "_dictNomen ON _dictNomen.ID = Product._dictNomenID"
            string[] listProduct = {"_dictPVD", "_dictSF", "_dictUM", "_dictNomen"};
            string[] listDictNomen = { "_dictFD", "_dictGOST", "_dictHS", "_dictMarka", "_dictP1ST", "_dictSortam" };
            string tableId = "";
            if (listProduct.Contains(Table)) tableId = "Product";
            if (listDictNomen.Contains(Table)) tableId = "_dictNomen";
            return String.Format("{0} ON {0}.ID = {1}.{0}ID", Table, tableId);
        }

        public override string getWhere(SearchCondition cond)
        {
            if (TypeId.Equals(guidDecimal))
            {
                switch (cond.Condition)
                {
                    case ">": return String.Format("(CONVERT(decimal,'{0}') < CONVERT(decimal,REPLACE({1}.{2}, ',', '.')))", cond.Value, Table, Column);
                    case "<": return String.Format("(CONVERT(decimal,'{0}') > CONVERT(decimal,REPLACE({1}.{2}, ',', '.')))", cond.Value, Table, Column);
                    case "=": return String.Format("(CONVERT(decimal,'{0}') = CONVERT(decimal,REPLACE({1}.{2}, ',', '.')))", cond.Value, Table, Column);
                    case "!=": return String.Format("(CONVERT(decimal,'{0}') <> CONVERT(decimal,REPLACE({1}.{2}, ',', '.')))", cond.Value, Table, Column);
                }
            }
            if (TypeId.Equals(guidInteger) || TypeId.Equals(guidDateTime))
            {
                switch (cond.Condition)
                {
                    case ">": return String.Format("(CONVERT({0},'{1}') < CONVERT({0}, {2}.{3})", getConvertType(), cond.Value, Table, Column);
                    case "<": return String.Format("(CONVERT({0},'{1}') > CONVERT({0}, {2}.{3}))", getConvertType(), cond.Value, Table, Column);
                    case "=": return String.Format("(CONVERT({0},'{1}') = CONVERT({0}, {2}.{3}))", getConvertType(), cond.Value, Table, Column);
                    case "!=": return String.Format("(CONVERT({0},'{1}') <> CONVERT({0}, {2}.{3}))", getConvertType(), cond.Value, Table, Column);
                }
            }
            if (TypeId.Equals(guidBoolean))
            {
                switch (cond.Condition)
                {
                    case "=": return String.Format("('{0}' = {1}.{2})", cond.Value, Table, Column);
                    case "!=": return String.Format("('{0}' <> {1}.{2})", cond.Value, Table, Column);
                }
            }
            if (TypeId.Equals(guidString))
            {
                switch (cond.Condition)
                {
                    case "~": return String.Format("( CHARINDEX('{0}', {1}.{2}) > 0)", cond.Value, Table, Column);
                    case "!~": return String.Format("( CHARINDEX('{0}', {1}.{2}) = 0)", cond.Value, Table, Column);
                    case "=": return String.Format("('{0}' = {1}.{2})", cond.Value, Table, Column);
                    case "!=": return String.Format("('{0}' <> {1}.{2})", cond.Value, Table, Column);
                }
            }
            return "";
        }
    }
    
    public class PropertyField : BaseField, IEquatable<PropertyField>
    {
        public PropertyField()
        {
        }

        public virtual bool Equals(PropertyField other)
        {
            return (Id == other.Id);
        }

        public PropertyField(PropertyField previous)
        {
            Id = previous.Id;
            Name = previous.Name;
            TypeId = previous.TypeId;
            Alias = previous.Alias;
        }

        public override string getSelect()
        {
            return String.Format("{0}.Value AS {0}", Alias);
        }

        public override string getJoin()
        {
            return String.Format("ProductProperty AS {0} ON {0}.ProductID = Product.ID AND {0}.PropertyID = '{1}'", Alias, Id.ToString().ToUpper());
        }

        public override string getWhere(SearchCondition cond)
        {
            if (TypeId.Equals(guidDecimal))
            {
                switch (cond.Condition)
                {
                    case ">": return String.Format("(CONVERT(decimal,'{0}') < CONVERT(decimal,REPLACE({1}.Value, ',', '.')))", cond.Value, Alias);
                    case "<": return String.Format("(CONVERT(decimal,'{0}') > CONVERT(decimal,REPLACE({1}.Value, ',', '.')))", cond.Value, Alias);
                    case "=": return String.Format("(CONVERT(decimal,'{0}') = CONVERT(decimal,REPLACE({1}.Value, ',', '.')))", cond.Value, Alias);
                    case "!=": return String.Format("(CONVERT(decimal,'{0}') <> CONVERT(decimal,REPLACE({1}.Value, ',', '.')))", cond.Value, Alias);
                }
            }
            if (TypeId.Equals(guidInteger) || TypeId.Equals(guidDateTime))
            {
                switch (cond.Condition)
                {
                    case ">": return String.Format("(CONVERT({0},'{1}') < CONVERT({0}, {2}.Value))", getConvertType(), cond.Value, Alias);
                    case "<": return String.Format("(CONVERT({0},'{1}') > CONVERT({0}, {2}.Value))", getConvertType(), cond.Value, Alias);
                    case "=": return String.Format("(CONVERT({0},'{1}') = CONVERT({0}, {2}.Value))", getConvertType(), cond.Value, Alias);
                    case "!=": return String.Format("(CONVERT({0},'{1}') <> CONVERT({0}, {2}.Value))", getConvertType(), cond.Value, Alias);
                }
            }
            if (TypeId.Equals(guidBoolean))
            {
                switch (cond.Condition)
                {
                    case "=": return String.Format("('{0}' = {1}.Value )", cond.Value, Alias);
                    case "!=": return String.Format("('{0}' <> {1}.Value )", cond.Value, Alias);
                }
            }
            if (TypeId.Equals(guidString))
            {
                switch (cond.Condition)
                {
                    case "~": return String.Format("( CHARINDEX('{0}', {1}.Value) > 0 )", cond.Value, Alias);
                    case "!~": return String.Format("( CHARINDEX('{0}', {1}.Value) = 0 )", cond.Value, Alias);
                    case "=": return String.Format("('{0}' = {1}.Value )", cond.Value, Alias);
                    case "!=": return String.Format("('{0}' <> {1}.Value )", cond.Value, Alias);
                }
            }
            return "";
        }

        public static List<PropertyField> getAllPropertyFields()
        {
            List<PropertyField> listFields = new List<PropertyField>();
            using (CommonDomain domain = new CommonDomain())
            {
                var properties = from p in domain.Properties
                                 select p;
                foreach (Property property in properties)
                {
                    PropertyField field = new PropertyField();
                    field.Id = property.ID;
                    field.Name = property.Name;
                    field.TypeId = property.TypeID;
                    field.Alias = property.Alias;
                    listFields.Add(field);
                }
                return listFields;
            }
        }
    }    

    class SearchResponse
    {
        public List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
        public int totalCount;
    }

    class SearchBuilder
    {
        public List<BaseField> m_listFields = new List<BaseField>();

        public static SearchBuilder InitInstance()
        {
            SearchBuilder searchBuilder = (SearchBuilder)HttpRuntime.Cache["SearchBuilder"];
            if(searchBuilder == null)
            {
                 searchBuilder = new SearchBuilder();
                 HttpRuntime.Cache.Add("SearchBuilder", searchBuilder,null,System.Web.Caching.Cache.NoAbsoluteExpiration, 
                     new TimeSpan(0,30,0), System.Web.Caching.CacheItemPriority.Normal,null);
            }
            return searchBuilder;
        }

        public SearchBuilder()
        {
            m_listFields.AddRange(PropertyField.getAllPropertyFields().Cast<BaseField>());
            m_listFields.AddRange(DictionaryField.getAllDictionaryFields().Cast<BaseField>());
        }

        public BaseField getFieldByAlias(string Alias)
        {
            List<BaseField> list = m_listFields.Where(f => f.Alias == Alias).ToList();
            if (list.Count == 0) new Exception("Нет ни одного поля с данным Alias");
            if (list.Count > 1) new Exception("Найденно несколько полей с данным Alias");
            return list.First();
        }

        public string Perform(SearchRequest searchRequest)
        {
            List<string> selectPart = new List<string>();
            List<string> joinPart = new List<string>();
            List<string> wherePart = new List<string>();

            /* формирование столбцов для запроса */
            selectPart.Add("Product.ID as uid");
            selectPart.Add(getFieldByAlias("_dictNomen_superpole").getSelect());
            selectPart.Add(getFieldByAlias("p_vers").getSelect());
            foreach (SearchCondition cond in searchRequest.searchConditions)
            {
                selectPart.Add(getFieldByAlias(cond.Alias).getSelect());
            }
            selectPart = selectPart.Distinct().ToList();

            /* формирование дополнительных таблиц для запроса */
            joinPart.Add(getFieldByAlias("_dictNomen_superpole").getJoin());
            joinPart.Add(getFieldByAlias("p_vers").getJoin());
            foreach (SearchCondition cond in searchRequest.searchConditions)
            {
                joinPart.Add(getFieldByAlias(cond.Alias).getJoin());
            }
            joinPart = joinPart.Distinct().ToList();

            /* формирование условий для запроса*/
            bool first = true;
            foreach (SearchCondition cond in searchRequest.searchConditions)
            {
                if (first)
                {
                    wherePart.Add(getFieldByAlias(cond.Alias).getWhere(cond));
                    first = false;
                }
                else
                {
                    wherePart.Add(cond.Concat + " " + getFieldByAlias(cond.Alias).getWhere(cond));
                }
            }

            string pre_select = "SELECT ";
            if (searchRequest.maxResult != 0)
            {
                pre_select = String.Format("{0} TOP {1} ", pre_select, Convert.ToInt32(searchRequest.maxResult));
            }

            string select = selectPart.Aggregate((a, b) => a + ", " + b) + "\n";
            string from = "FROM Product\n";
            string join = "INNER JOIN " + joinPart.Aggregate((a, b) => a + "\nINNER JOIN " + b) + "\n";
            string where = "WHERE " + wherePart.Aggregate((a, b) => a + " " + b);

            return pre_select + select + from + join + where;
        }
    }

    public partial class SearchByConditions : System.Web.UI.Page
    {
        public string jsonResponse;

        protected void Page_Load(object sender, EventArgs e)
        {
            SearchBuilder searchBuilder = SearchBuilder.InitInstance();
            SearchRequest searchRequest = new SearchRequest(Request);
           
            string sqlRequest = searchBuilder.Perform(searchRequest);

            /* проверка корректности FieldId полей, полученных из json */
            /*if (searchRequest.searchConditions.Items.Where(cond => !dictProperties.ContainsKey(cond.FieldId)).Count() > 0)
            {
                new Exception("Переданный json содержит не допустипый FieldId");
            };*/

            using (CommonDataProvider provider = new CommonDataProvider())
            {                                
                DataSet dataset = provider.ExecuteCommand(sqlRequest);
                SearchResponse response = new SearchResponse();
                response.totalCount = dataset.Tables[0].Rows.Count;

                for (int i = searchRequest.start; i < searchRequest.start + searchRequest.limit; i++)
                {
                    if (i < dataset.Tables[0].Rows.Count)
                    {
                        Dictionary<string, string> jsonrow = new Dictionary<string, string>();
                        foreach (DataColumn column in dataset.Tables[0].Columns)
                        {
                            jsonrow[column.Caption] = dataset.Tables[0].Rows[i][column].ToString();
                            if (column.Caption != "uid")
                            {
                                if (searchBuilder.getFieldByAlias(column.Caption).TypeId == BaseField.guidDecimal)
                                {
                                    jsonrow[column.Caption] = jsonrow[column.Caption].Replace(',', '.');
                                }   
                            }
                        }
                        response.rows.Add(jsonrow);
                    }
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                jsonResponse = js.Serialize(response);
            }
        }
    }
}
