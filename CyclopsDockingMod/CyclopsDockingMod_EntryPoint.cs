namespace CyclopsDockingMod;

using System;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;

[BepInPlugin("com.osubmarin.cyclopsdockingmod", "CyclopsDockingMod", "2.0.7")]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
[DisallowMultipleComponent]
public class CyclopsDockingMod_EntryPoint : BaseUnityPlugin
{
    private static bool _initialized = false;

    private static bool _success = true;

    public static ManualLogSource _logger = null;

    private void Awake()
    {
        _logger = Logger;
        if (!_initialized)
        {
            _initialized = true;
            Logger.LogInfo("INFO: Initializing Cyclops Docking mod...");
            try
            {
                CyclopsDockingMod.Start();
            }
            catch (Exception ex)
            {
                _success = false;
                Logger.LogInfo(string.Format("ERROR: Exception caught! Message=[{0}] StackTrace=[{1}]", ex.Message, ex.StackTrace));
                if (ex.InnerException != null)
                    Logger.LogInfo(string.Format("ERROR: Inner exception => Message=[{0}] StackTrace=[{1}]", ex.InnerException.Message, ex.InnerException.StackTrace));
            }
            Logger.LogInfo(_success ? "INFO: Cyclops Docking mod initialized successfully." : "ERROR: Cyclops Docking mod initialization failed.");
        }
    }
}
