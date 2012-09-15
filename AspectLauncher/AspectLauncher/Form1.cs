using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AspectLauncher
{
    public partial class Form1 : Form
    {
        private TcpServer _tcpServer = new TcpServer();

        public Form1()
        {
            InitializeComponent();

            _tcpServer.Start();
        }

        private void exitMenuItem(object sender, EventArgs e)
        {
            _tcpServer.Stop();
            this.Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Hide();
        }        
    }
}
