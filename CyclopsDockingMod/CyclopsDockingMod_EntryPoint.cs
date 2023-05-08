using System;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace CyclopsDockingMod
{
	[BepInPlugin("com.osubmarin.cyclopsdockingmod", "CyclopsDockingMod", "2.0.7")]
	[BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
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
				base.Logger.LogInfo("INFO: Initializing Cyclops Docking mod...");
				try
				{
					CyclopsDockingMod.Start();
				}
				catch (Exception ex)
				{
					CyclopsDockingMod_EntryPoint._success = false;
					base.Logger.LogInfo(string.Format("ERROR: Exception caught! Message=[{0}] StackTrace=[{1}]", ex.Message, ex.StackTrace));
					if (ex.InnerException != null)
						base.Logger.LogInfo(string.Format("ERROR: Inner exception => Message=[{0}] StackTrace=[{1}]", ex.InnerException.Message, ex.InnerException.StackTrace));
				}
				base.Logger.LogInfo(CyclopsDockingMod_EntryPoint._success ? "INFO: Cyclops Docking mod initialized successfully." : "ERROR: Cyclops Docking mod initialization failed.");
			}
		}
	}
}
