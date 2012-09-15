using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Aspect.Utility
{
	//Values should be equal to TraceEventType values
    public enum Severity
    {
        // Fatal error or application crash
		Critical = TraceEventType.Critical,
        // Recoverable error
        Error = TraceEventType.Error,
        // Noncritical problem
        Warning = TraceEventType.Warning,
        // Informational message
        Info = TraceEventType.Information,
        // Debugging trace
        Verbose = TraceEventType.Verbose
    }

	public static class TraceHelper
	{
/*        public static string Log(Exception exception, Guid sessionID)
        {
            return Log(exception, sessionID, Severity.Critical);
        }

        public static string Log(Exception exception, Guid sessionID, Severity severity)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append(exception.ToString() + "\n");
            while(exception != null)
            {
                msg.Append(exception.ToString() + "\n");
                exception = exception.InnerException;
            }
            Log(msg.ToString(), sessionID, severity, true);
            return msg.ToString();
        }*/
        public static void Log(Guid userID, string message)
        {
            Log(userID, Severity.Info, message);
        }
        public static void Log(Guid userID, string format, params object[] args)
        {
            Log(userID, Severity.Info, format, args);
        }
        public static void Log(Guid userID, Severity severity, string format, params object[] args)
        {
            string message = string.Format(format, args);
            Log(userID, severity, message);
        }

        public static void Log(Guid userID, Severity severity, string message)
		{
            string msg = string.Format("{0}\t{1}\t{2}\t{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), userID.ToString("N"), Enum.GetName(typeof(Severity), severity), message);
            

				Trace.WriteLine(msg, Enum.GetName(typeof(Severity), severity));
		}
    }
}
