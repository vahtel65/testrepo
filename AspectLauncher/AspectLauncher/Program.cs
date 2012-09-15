using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace AspectLauncher
{
    static class Program
    {
        private static bool IsAlreadyRunning()
        {            
            Process currentProcess = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcesses();
            
            foreach (Process process in processes)
            {
                if(process.Id != currentProcess.Id)
                    if(currentProcess.ProcessName == process.ProcessName)
                        return true;  
            }
            return false;
        }


        [STAThread]
        static void Main()
        {
            string target = "http://192.168.10.3/";
            System.Diagnostics.Process.Start(target);


            if (IsAlreadyRunning()) return ;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
