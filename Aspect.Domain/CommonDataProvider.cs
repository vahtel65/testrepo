using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Web.Configuration;

namespace Aspect.Domain
{
    public class CommonDataProvider : IDisposable
    {
        private SqlConnection connection;
        protected SqlConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = new SqlConnection(global::Aspect.Domain.Properties.Settings.Default.AspectConnectionString);
                    connection.Open();
                }
                return connection;
            }

        }

        // параметр приложениея, отвечающий за таймаут ожидания данных 
        // от MSSQL Server-а в уже установленном соединении
        public int TimeoutInnerConnection
        {
            get
            {
                return Convert.ToInt32(WebConfigurationManager.AppSettings["TimeoutInnerConnection"]);
            }
        }

        public DataSet ExecuteCommand(string sql)
        {
            DataSet resultSet = new DataSet();
            using (SqlCommand command = new SqlCommand(sql, Connection))
            {
                command.CommandTimeout = TimeoutInnerConnection;
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(resultSet);
                }
            }
            return resultSet;
        }

        public object ExecuteScalar(string sql)
        {
            DataSet resultSet = new DataSet();
            using (SqlCommand command = new SqlCommand(sql, Connection))
            {
                command.CommandTimeout = TimeoutInnerConnection;
                /*if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();*/
                return command.ExecuteScalar();
            }
        }
        public void ExecuteNonQuery(string sql)
        {
            DataSet resultSet = new DataSet();
            using (SqlCommand command = new SqlCommand(sql, Connection))
            {
                command.CommandTimeout = TimeoutInnerConnection;
                /*if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();*/
                command.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            if (connection != null && connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }
    }
}