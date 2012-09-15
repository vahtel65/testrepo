using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.Web.Script.Serialization;
using Aspect.Domain;

namespace Aspect.UI.Web.Loging
{
    public class LogLine
    {
        public Guid UserID;
        public string Object;
        public string Action;
        public DateTime Time;
        public string Level;
        public string Data;

        public LogLine(string line)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string[] parts = line.Split('|');
            this.Time = Convert.ToDateTime(parts[0]);
            this.Level = parts[1];            
            string[] ev = parts[2].Split(',');
            this.UserID = new Guid(ev[0]);
            this.Object = ev[1];
            this.Action = ev[2];
            this.Data = parts[3];
        }
    }

    public partial class View : Basic.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Title = "Просмотр лог файлов";
                listFiles_BindData();
            }
        }

        private void listFiles_BindData()
        {
            List<object> data = new List<object>();
            DirectoryInfo advLogFolder = new DirectoryInfo("c:\\Logs\\Advance");
            foreach (FileInfo file in advLogFolder.GetFiles())
            {
                data.Add(new { FileName = file.Name });
            }
            listFilesStore.DataSource = data;
            listFilesStore.DataBind();
        }

        protected void RefreshLogLines(object sender, StoreRefreshDataEventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CommonDomain provider = new CommonDomain();

            List<object> data = new List<object>();            
            string[] lines = File.ReadAllLines(string.Format("c:\\Logs\\Advance\\{0}", listFilesSelect.Value.ToString()));
            int lineIndex = 0;
            foreach (string line in lines)
            {                
                LogLine logLine = new LogLine(line);
                data.Add(new
                {
                    evTime = logLine.Time.ToString("HH:mm:ss"),
                    evUser = provider.Users.Single(us => us.ID == logLine.UserID).Name,
                    evLevel = logLine.Level,
                    evObject = logLine.Object,
                    evAction = logLine.Action,
                    evIndex = lineIndex
                });
                ++lineIndex;
            }
            logLinesStore.DataSource = data;
            logLinesStore.DataBind();
        }
    }
}
