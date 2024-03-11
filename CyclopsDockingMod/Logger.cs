using System;
using System.Globalization;
using BepInEx.Logging;

namespace CyclopsDockingMod
{
	internal static class Logger
	{
		internal static void Debug(string text, params object[] args)
		{
			Log(LogLevel.Debug, text, args);
		}

		internal static void Info(string text, params object[] args)
		{
			Log(LogLevel.Info, text, args);
		}

		internal static void Message(string text, params object[] args)
		{
			Log(LogLevel.Message, text, args);
		}

		internal static void Warning(string text, params object[] args)
		{
			Log(LogLevel.Warning, text, args);
		}

		internal static void Error(string text, params object[] args)
		{
			Log(LogLevel.Error, text, args);
		}

		internal static void Fatal(string text, params object[] args)
		{
			Log(LogLevel.Fatal, text, args);
		}

		internal static void Log(LogLevel level, string text, params object[] args)
		{
			if (args != null && args.Length > 0)
				text = string.Format(CultureInfo.InvariantCulture, text, args);
			if (CyclopsDockingMod_EntryPoint._logger != null)
				CyclopsDockingMod_EntryPoint._logger.Log(level, text);
			else
				Console.WriteLine($"[CyclopsDockingMod] {level} {text}");
		}
	}
}
