using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspect.Domain;
using Aspect.UI.Web.Loging;
using System.Web.Script.Serialization;

namespace Aspect.UI.Web.Callback
{
    public partial class ViewLogLine : System.Web.UI.Page
    {
        [Serializable] 
        public class ConfigurationDataItem
        {
            public string Name { get; set; }
            public object OldValue { get; set; }
            public object NewValue { get; set; } 
        };
            
        public class ConfigurationData
        {
            public List<ConfigurationDataItem> Properties;
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines(string.Format("c:\\Logs\\Advance\\{0}", Request["logFile"].ToString()));

            LogLine logLine = new LogLine(lines[Convert.ToInt32(Request["lineNumber"])]);
            if (logLine.Object == "Configuration")
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ConfigurationData data = serializer.Deserialize<ConfigurationData>(logLine.Data);
                ProductCard1.DataSource = data.Properties.ToList(); // .Select(it => new Pair<string,object>(it.Name, it.NewValue)).ToList();
                ProductCard1.DataBind();
            }
        }
    }
}
