using CyclopsDockingMod.Routing;

namespace CyclopsDockingMod.Fixers
{
	public static class IngameMenuFixer
	{
		public static void SaveGame_Postfix()
		{
			BaseFixer.SaveBaseParts();
			if (ConfigOptions.EnableAutopilotFeature)
				AutoPilot.SaveRoutes();
		}

		public static void QuitGame_Postfix(bool quitToDesktop)
		{
			if (GameModeUtils.IsPermadeath())
			{
				BaseFixer.SaveBaseParts();
				if (ConfigOptions.EnableAutopilotFeature)
					AutoPilot.SaveRoutes();
			}
		}
	}
}
