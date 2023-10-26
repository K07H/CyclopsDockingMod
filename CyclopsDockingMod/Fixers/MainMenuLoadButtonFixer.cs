﻿namespace CyclopsDockingMod.Fixers;
using global::CyclopsDockingMod.Routing;

public static class MainMenuLoadButtonFixer
{
    public static bool Load_Prefix(MainMenuLoadButton __instance)
    {
        if (!__instance.IsEmpty())
        {
            BaseFixer.LoadBaseParts(__instance.saveGame);
            if (ConfigOptions.EnableAutopilotFeature)
                AutoPilot.LoadRoutes(__instance.saveGame);
        }
        return true;
    }
}
