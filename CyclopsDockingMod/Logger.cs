namespace CyclopsDockingMod;
using System;
using System.Globalization;

internal static class Logger
{
    internal static void Log(string text, params object[] args)
    {
        string text2 = "[CyclopsDockingMod] " + ((args != null && args.Length != 0) ? string.Format(CultureInfo.InvariantCulture, text, args) : text);
        if (Plugin.Logger != null)
        {
            Plugin.Logger.LogMessage(text2);
            return;
        }
        Console.WriteLine(text2);
    }
}
