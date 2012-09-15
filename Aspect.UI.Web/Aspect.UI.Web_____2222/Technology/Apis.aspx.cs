using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using Aspect.Model.SpecificationDomain;
using System.Globalization;

namespace Aspect.UI.Web.Technology
{
    [ScriptService]
    public partial class Apis : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        [WebMethod]
        public static string ApplicabilityMaterials(string material_id)
        {
            DateTime CurrentDT = DateTime.Now;
            List<object[]> rows = new List<object[]>();
            List<UniColumn> columns = new List<UniColumn>();

            // prepare columns
            columns.Add(new UniColumn()
            {
                uniType = UniColumn.UniType.ProductMenu,
                header = "#",
                dataBind = "productID"
            });
            columns.Add(new UniColumn()
            {
                uniType = UniColumn.UniType.String,
                header = "Обозначение",
                dataBind = "pn1"
            });
            columns.Add(new UniColumn()
            {
                uniType = UniColumn.UniType.String,
                header = "Наименование",
                dataBind = "pn2"
            });
            columns.Add(new UniColumn()
            {
                uniType = UniColumn.UniType.String,
                header = "Год приказа",
                dataBind = "yorder"
            });
            columns.Add(new UniColumn()
            {
                uniType = UniColumn.UniType.String,
                header = "Номер приказа",
                dataBind = "norder"
            });
            columns.Add(new UniColumn()
            {
                uniType = UniColumn.UniType.Boolean,
                header = "Основной материал",
                dataBind = "main_material"
            });
            columns.Add(new UniColumn()
            {
                uniType = UniColumn.UniType.Decimal,
                header = "Норма расхода",
                dataBind = "no"
            });
            columns.Add(new UniColumn()
            {
                uniType = UniColumn.UniType.String,
                header = "Единица измерения",
                dataBind = "um"
            });
            columns.Add(new UniColumn()
            {
                uniType = UniColumn.UniType.String,
                header = "Цех потребления",
                dataBind = "s"
            });

            using (SpecificationProvider provider = new SpecificationProvider())
            {
                for (int specificationTable = 1; specificationTable <= 2; specificationTable++)
                {                    
                    DataSet dataSet = provider.GetApplicabilityList(CurrentDT, specificationTable, new Guid(material_id));
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        rows.Add(new object[] { 
                        row["productID"],
                        row["pn1"],                // обозначение
                        row["pn2"],                // наименование
                        row["yorder"],             // год приказа
                        row["norder"],             // номер приказа
                        (specificationTable == 1), // основной материал
                        Convert.ToDouble(row["no"]).ToString(CultureInfo.InvariantCulture),   // норма расхода
                        row["um"],                 // единица измерения
                        row["s"]                   // цех потребления
                    });
                    }

                }                
            }           

            UniTransfer transfer = new UniTransfer()
            {
                columns = columns,
                rows = rows,
                statusAnswer = 0,
                messageAnswer = "Ok"
            };

            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = 50000000;
                return serializer.Serialize(transfer);
            } catch (Exception e)
            {
                return e.Message;
            }
        }
            
    }
}
