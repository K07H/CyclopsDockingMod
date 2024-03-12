using System;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace CyclopsDockingMod
{
    [BepInPlugin("com.osubmarin.cyclopsdockingmod", "CyclopsDockingMod", "2.1.0")]
#if SUBNAUTICA_NAUTI
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
#else
	[BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
#endif
	[DisallowMultipleComponent]
	public class CyclopsDockingMod_EntryPoint : BaseUnityPlugin
    {
        private static bool _initialized = false;

        private static bool _success = true;

        public static ManualLogSource _logger = null;

        private void Awake()
		{
			CyclopsDockingMod_EntryPoint._logger = base.Logger;
			if (!CyclopsDockingMod_EntryPoint._initialized)
			{
				CyclopsDockingMod_EntryPoint._initialized = true;
				base.Logger.LogMessage("Initializing Cyclops Docking mod...");
				try
				{
					CyclopsDockingMod.Start();
				}
				catch (Exception ex)
				{
					CyclopsDockingMod_EntryPoint._success = false;
					base.Logger.LogError(string.Format("Exception caught! Message=[{0}] StackTrace=[{1}]", ex.Message, ex.StackTrace));
					if (ex.InnerException != null)
						base.Logger.LogError(string.Format("Inner exception => Message=[{0}] StackTrace=[{1}]", ex.InnerException.Message, ex.InnerException.StackTrace));
				}
				if (CyclopsDockingMod_EntryPoint._success)
					base.Logger.LogMessage("Cyclops Docking mod initialized successfully.");
				else
					base.Logger.LogError("Cyclops Docking mod initialization failed.");
			}
		}
	}
}
