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
				PrefabIdentifier component = __instance.subRoot.GetComponent<PrefabIdentifier>();
				if (component != null)
					flag = SubControlFixer.Docked(component.Id);
				if (!flag)
					flag = AutoPilot.SubsPlayingRoutes.ContainsKey(component.Id) && AutoPilot.SubsPlayingRoutes[component.Id].IsPlayingRoute;
				if (!flag)
					flag = AutoPilot.IsRecording;
				AutoPilot.RefreshHud(__instance.subRoot, flag);
			}
		}
	}
}
