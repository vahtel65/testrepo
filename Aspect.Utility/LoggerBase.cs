using System;
using System.IO;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace Aspect.Utility
{
	/// <summary>
	/// File logger implementation.
	/// </summary>
	public class LoggerBase
	{
		public const int DefaultMaxLogSize = 2 * 1024 * 1024; // 2 Mb
		//
		// private class members.
		//
		protected string logFileName = string.Empty;
		protected int indent = 2;
		protected int curIndent = 0;
		protected bool isDestroying = false;
		protected DateTime lastWriteTime = DateTime.Now;
		protected int maxSize = DefaultMaxLogSize;
		protected System.Diagnostics.TraceSwitch traceSwitch = null;
		protected string eventSource = "Aspect";
		// the following is used to not create a file until there is nothing to write to log
		protected string pendingStartLines = string.Empty;

		public enum Action
		{
			Trace,
			Enter,
			Leave,
			Error,
			Warning
		}

		//
		// Max number of indent spaces.
		//
		private const int MaxIdent = 100;

		/// <summary>
		/// A constructor.
		/// </summary>
		public LoggerBase(string filename, string switchName, string eventSource)
		{

			this.FileName = filename;
			switchName = string.IsNullOrEmpty(switchName) ? String.Empty : switchName;
			traceSwitch = new System.Diagnostics.TraceSwitch(switchName, string.Empty);
			if (switchName.Length == 0)
				traceSwitch.Level = System.Diagnostics.TraceLevel.Info;
			this.eventSource = eventSource;
		}

		/// <summary>
		/// A constructor.
		/// </summary>
		public LoggerBase(string filename, System.Diagnostics.TraceLevel level, string eventSource)
		{
			this.FileName = filename;
			traceSwitch = new System.Diagnostics.TraceSwitch(string.Empty, string.Empty);
			traceSwitch.Level = level;
			this.eventSource = eventSource;
		}

		/// <summary>
		///     Default destructor.
		/// </summary>
		~LoggerBase()
		{
			isDestroying = true;
			this.FileName = string.Empty;
		}

		/// <value>
		///     Property TraceSwitch is used to determine the trace swith. 
		/// </value>
		public System.Diagnostics.TraceSwitch TraceSwitch
		{
			get { return traceSwitch; }
		}

		/// <value>
		///     Property MaxSize is used to determine the maximum log file size. 
		/// </value>
		public int MaxSize
		{
			get { return maxSize; }
			set { maxSize = value; }
		}

		/// <value>
		///     Property FileName specifies the file name to write log messages to. 
		/// </value>
		public string FileName
		{
			get { return logFileName; }
			set
			{
				bool sameName = value.CompareTo(logFileName) == 0;
				if (!sameName)
				{
					curIndent = 0;
					if (!isDestroying)
						internalTrace("File name changed to " + value, Action.Trace);
					if (pendingStartLines.Length == 0)
						internalTrace("========================= log finished " + DateTime.Now + " ===================", Action.Trace);
					logFileName = value;
					pendingStartLines = "========================= log started " + DateTime.Now + " ===================";
					string affinityStr = string.Empty;
					try
					{
						affinityStr = "----- processor affinity is " + System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity;
					}
					catch (Exception)
					{
						affinityStr = "----- cannot obtain processor affinity information.";
					}
					pendingStartLines += Environment.NewLine;
					pendingStartLines += affinityStr;
				}
			}
		}


		/// <summary>
		///     Builds and outputs a line to the file.
		///     <param name="message">A message to be written to the file.</param>
		///     <param name="action">Log action type.</param>
		/// </summary>
		protected void internalTrace(string message, Action action)
		{
			if (FileName.Length > 0)
				try
				{
					if (pendingStartLines.Length > 0)
					{
						string _msg = pendingStartLines;
						pendingStartLines = string.Empty;
						internalTrace(_msg, Action.Trace);
					}
					DateTime now = DateTime.Now;
					string line = Thread.CurrentThread.GetHashCode().ToString("X8") + " "; // now.ToString(" HH:mm:ss.fff: ");
					string prefix = string.Empty;
					switch (action)
					{
						case Action.Enter:
							prefix = "ENTER: ";
							break;
						case Action.Leave:
							prefix = "LEAVE: ";
							if (curIndent >= indent)
								curIndent -= indent;
							break;
						case Action.Error:
							prefix = "ERROR: ";
							break;
						case Action.Warning:
							prefix = "WARN: ";
							break;
					}
					if (curIndent > 0)
					{
						string offs = string.Empty;
						line += offs.PadRight(curIndent, ' ');
					}

					// check if the file exceeds max file size
					//if (lastWriteTime.Minute != now.Minute)
					//{
						FileInfo fi = new FileInfo(FileName);
						try
						{
                            //---
                            DateTime lwt = fi.LastWriteTime;
                            if (lwt.Date < DateTime.Today)
                            {
                                string backName = string.Format("{0}.{1}.{2}.{3}.log", FileName, lwt.Year, lwt.Month, lwt.Day);
                                if (File.Exists(backName))
                                {
                                    backName = string.Format("{0}.{1}.log", backName, DateTime.Now.ToString());
                                }
                                fi.MoveTo(backName);
                                //string backName = FileName + string.Format("",
                            }
                            //---
							/*if (fi.Length >= MaxSize)
							{
								string bakName = FileName + ".bak";
								if (File.Exists(bakName))
									(new FileInfo(bakName)).Delete();
								fi.MoveTo(bakName);
							}*/
						}
						catch (Exception)
						{ }
					//}

					StreamWriter w = new StreamWriter(FileName, true);
					if (lastWriteTime.DayOfWeek != now.DayOfWeek)
					{
						line = Environment.NewLine + "          Date is: " + now + Environment.NewLine + line;
					}
					lastWriteTime = now;
					try
					{
						StreamWriter.Synchronized(w).WriteLine(line + prefix + message);
					}
					finally
					{
						w.Close();
					}

					switch (action)
					{
						case Action.Enter:
							if (curIndent < MaxIdent)
								curIndent += indent;
							break;
						case Action.Leave:
							break;
						case Action.Error:
							break;
						case Action.Warning:
							break;
					}
				}
				catch (Exception)
				{ }
		}

		/// <summary>
		///     Writes an ordinary informational line to the log.
		///     <param name="message">A message to be written to the file.</param>
		/// </summary>
		public virtual void trace(string message)
		{
			if (traceSwitch.TraceInfo)
				internalTrace(message, Action.Trace);
		}

		/// <summary>
		///     Formats and writes an ordinary informational line to the log.
		///     <param name="format">A format string to be written to the file.</param>
		///     <param name="args">An array of values to be used to form the final string.</param>
		/// </summary>
		public virtual void trace(string format, params object[] args)
		{
			try
			{
				if (traceSwitch.TraceInfo)
					trace(String.Format(format, args));
			}
			catch (Exception e)
			{
				error(e);
			}
		}


		protected void TraceEventLog(string message, EventLogEntryType entryType)
		{
			try
			{
				EventLog.WriteEntry(eventSource, message, entryType);
			}
			catch
			{ }
		}


		/// <summary>
		///     Writes an error line to the log.
		///     <param name="message">A message to be written to the file.</param>
		/// </summary>
		public virtual void error(String message)
		{
			if (traceSwitch.TraceError)
				internalTrace(message, Action.Error);
			TraceEventLog(message, EventLogEntryType.Error);
		}


		/// <summary>
		///     Formats and writes an error line to the log.
		///     <param name="format">A format string to be written to the file.</param>
		///     <param name="args">An array of values to be used to form the final string.</param>
		/// </summary>
		public virtual void error(string format, params object[] args)
		{
			try
			{
				if (traceSwitch.TraceError)
					error(String.Format(format, args));
			}
			catch (Exception e)
			{
				error(e);
			}
		}

		/// <summary>
		///     Formats and writes an exception info to the log.
		///     <param name="e">An exception object.</param>
		/// </summary>
		public virtual void error(Exception e)
		{
			if (e != null)
			{
				try
				{
					string fmt = "\nMessage: {0}\nType   : {1}\nSource : {2}\nTrace  : {3}";
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat(fmt, new object[] { e.Message, e.GetType(), e.Source, e.StackTrace });

					Exception inner = e.InnerException;
					while (inner != null)
					{
						sb.Append("\n\n--- Inner Exception: ---");
						sb.AppendFormat(fmt, new object[] { inner.Message, inner.GetType(), inner.Source, inner.StackTrace });
						inner = inner.InnerException;
					}
					if (traceSwitch.TraceError)
						internalTrace(sb.ToString(), Action.Error);
					TraceEventLog(sb.ToString(), EventLogEntryType.Error);
				}
				catch (Exception)
				{ }
			}
		}

		/// <summary>
		///     Writes an error line to the log.
		///     <param name="message">A message to be written to the file.</param>
		/// </summary>
		public virtual void warn(String message)
		{
			if (traceSwitch.TraceWarning)
				internalTrace(message, Action.Warning);
		}

		/// <summary>
		///     Formats and writes an error line to the log.
		///     <param name="format">A format string to be written to the file.</param>
		///     <param name="args">An array of values to be used to form the final string.</param>
		/// </summary>
		public virtual void warn(string format, params object[] args)
		{
			try
			{
				if (traceSwitch.TraceWarning)
					warn(String.Format(format, args));
			}
			catch (Exception e)
			{
				error(e);
			}
		}

		/// <summary>
		///     Writes a line to the log and increases log indent.
		///     <param name="message">A message to be written to the file.</param>
		/// </summary>
		public virtual void enter(String message)
		{
			if (traceSwitch.TraceInfo)
				internalTrace(message, Action.Enter);
		}

		/// <summary>
		///     Formats and writes a line to the log, then increases log indent.
		///     <param name="format">A format string to be written to the file.</param>
		///     <param name="args">An array of values to be used for line formatting.</param>
		/// </summary>
		public virtual void enter(string format, params object[] args)
		{
			try
			{
				if (traceSwitch.TraceInfo)
					enter(String.Format(format, args));
			}
			catch (Exception e)
			{
				error(e);
			}
		}

		/// <summary>
		///     Decreases log indent and writes a line to the log.
		///     <param name="message">A message to be written to the file.</param>
		/// </summary>
		public virtual void leave(String message)
		{
			if (traceSwitch.TraceInfo)
				internalTrace(message, Action.Leave);
		}

		/// <summary>
		///     Decreases log indent, Formats and writes a line to the log.
		///     <param name="format">A format string to be written to the file.</param>
		///     <param name="args">An array of values to be used for line formatting.</param>
		/// </summary>
		public virtual void leave(string format, params object[] args)
		{
			try
			{
				if (traceSwitch.TraceInfo)
					leave(String.Format(format, args));
			}
			catch (Exception e)
			{
				error(e);
			}
		}
	}
}
