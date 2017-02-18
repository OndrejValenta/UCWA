using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using UCWA.Provider;

namespace UCWA.TestConsole
{
	class Program
	{
		private static ILogger logger = LogManager.GetCurrentClassLogger();
		static void Main(string[] args)
		{
			UCWAConnector connector = new UCWAConnector();

			try
			{
				connector.Logon_Step01();
			}
			catch (Exception ex)
			{
				logger.Error(ex);

			}

			if (Debugger.IsAttached)
			{
				Console.ReadKey();
			}
		}
	}
}
