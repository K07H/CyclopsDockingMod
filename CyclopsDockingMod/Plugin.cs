namespace CyclopsDockingMod;

using System;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.osubmarin.cyclopsdockingmod")]
[BepInIncompatibility("com.ahk1221.smlhelper")]
[DisallowMultipleComponent]
public class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo("INFO: Initializing Cyclops Docking mod...");
        try
        {
            CyclopsDockingMod.Start();
            Logger.LogInfo("Cyclops Docking mod initialized successfully.");
        }
        catch (Exception ex)
        {
            Logger.LogError("Cyclops Docking mod initialization failed.");
            Logger.LogError(string.Format("Exception caught! Message=[{0}] StackTrace=[{1}]", ex.Message, ex.StackTrace));
            ex = ex.InnerException;
            do
            {
                if(ex != null)
                {
                    Logger.LogError(string.Format("Inner exception => \nMessage=[{0}] \nStackTrace=[{1}]", ex.Message, ex.StackTrace));
                    ex = ex.InnerException;
                }
            }
            while (ex != null);
        }
    }
}
