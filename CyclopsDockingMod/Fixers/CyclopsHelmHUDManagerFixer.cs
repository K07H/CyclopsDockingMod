using CyclopsDockingMod.Routing;

namespace CyclopsDockingMod.Fixers
{
	public class CyclopsHelmHUDManagerFixer
	{
		public static void StartPiloting_Postfix(CyclopsHelmHUDManager __instance)
		{
			if (ConfigOptions.EnableAutopilotFeature)
			{
				bool flag = false;
				PrefabIdentifier pid = __instance.subRoot.GetComponent<PrefabIdentifier>();
				if (pid != null)
				{
					flag = SubControlFixer.Docked(pid.Id);
					if (!flag)
						flag = AutoPilot.SubsPlayingRoutes.ContainsKey(pid.Id) && AutoPilot.SubsPlayingRoutes[pid.Id].IsPlayingRoute;
				}
				if (!flag)
					flag = AutoPilot.IsRecording;
				AutoPilot.RefreshHud(__instance.subRoot, flag);
			}
		}
	}
}
