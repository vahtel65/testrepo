using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Aspect.Utility
{
	public class SplitterTraceListener : TraceListener
	{
		private bool isListenerInit = false;

		private const int DefaultMaxFileSize = 2 * 1024 * 1024;

		private LoggerBase _logger = null;

		protected string LogPath 
		{
			get
			{
				return Attributes["LogPath"];
			}
		}

		public int MaxFileSize 
		{ 
			get 
			{
				int maxSize = DefaultMaxFileSize;

				if (!string.IsNullOrEmpty(Attributes["MaxFileSize"]) && 
					!int.TryParse(Attributes["MaxFileSize"], out maxSize))
					maxSize = DefaultMaxFileSize; //2 MB

				return maxSize;
			} 
		}

		public SplitterTraceListener(string file)
			: base(file)
		{
			if (!string.IsNullOrEmpty(file))
			{
				_logger = new LoggerBase(file, string.Empty, string.Empty);
			}

		}

		public void InitilizeListener(Type type)
		{
			if (!string.IsNullOrEmpty(LogPath))
			{
				string fileName = Path.Combine(LogPath, string.Format("{0}.{1}.log", type.Namespace, type.Name));
				_logger = new LoggerBase(fileName, string.Empty, string.Empty);
			}
		}

		public void InitilizeListener()
		{
			if (_logger != null)
				_logger.MaxSize = MaxFileSize;

			isListenerInit = true;
		}

		public override void Write(string message, string category)
		{
			Write(message);
		}

		public override void WriteLine(string message, string category)
		{
			Write(message);
		}

		public override void Write(string message)
		{
			if (isListenerInit)
				InitilizeListener();

			if (_logger != null)
				_logger.trace(message);
		}

		public override void WriteLine(string message)
		{
			Write(message);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			Write(message);
		}

		protected override string[] GetSupportedAttributes()
		{
			string[] atr = base.GetSupportedAttributes();
			
			List<string> supportedAttributes = new List<string>();
			if (atr != null) supportedAttributes.AddRange(atr);
			supportedAttributes.Add("LogPath");
			supportedAttributes.Add("MaxFileSize");
			return supportedAttributes.ToArray();
		}

	}
}
