using System;
using System.Globalization;

namespace CyclopsDockingMod
{
	internal static class Logger
	{
		internal static void Log(string text, params object[] args)
		{
			string text2 = "[CyclopsDockingMod] " + ((args != null && args.Length != 0) ? string.Format(CultureInfo.InvariantCulture, text, args) : text);
			if (CyclopsDockingMod_EntryPoint._logger != null)
			{
				CyclopsDockingMod_EntryPoint._logger.LogMessage(text2);
				return;
			}
			Console.WriteLine(text2);
		}
	}
}
