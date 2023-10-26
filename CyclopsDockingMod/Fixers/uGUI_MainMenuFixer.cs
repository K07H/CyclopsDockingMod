namespace CyclopsDockingMod.Fixers;
using System;
using global::CyclopsDockingMod.Routing;

public static class uGUI_MainMenuFixer
{
    public static bool OnErrorConfirmed_Prefix(bool confirmed, string saveGame)
    {
        if (confirmed)
        {
            BaseFixer.LoadBaseParts(saveGame);
            if (ConfigOptions.EnableAutopilotFeature)
                AutoPilot.LoadRoutes(saveGame);
        }
        return true;
    }

    public static bool LoadMostRecentSavedGame_Prefix()
    {
        SaveLoadManager.GameInfo gameInfo = null;
        string text = null;
        try
        {
            string[] activeSlotNames = SaveLoadManager.main.GetActiveSlotNames();
            long num = 0L;
            int i = 0;
            int num2 = activeSlotNames.Length;
            while (i < num2)
            {
                SaveLoadManager.GameInfo gameInfo2 = SaveLoadManager.main.GetGameInfo(activeSlotNames[i]);
                if (gameInfo2.dateTicks > num)
                {
                    gameInfo = gameInfo2;
                    num = gameInfo2.dateTicks;
                    text = activeSlotNames[i];
                }
                i++;
            }
        }
        catch (Exception ex)
        {
            Logger.Log("ERROR: Exception caught while retrieving game info. Exception=[" + ex.ToString() + "]", Array.Empty<object>());
            gameInfo = null;
        }
        if (gameInfo != null)
        {
            BaseFixer.LoadBaseParts(text);
            if (ConfigOptions.EnableAutopilotFeature)
                AutoPilot.LoadRoutes(text);
        }
        return true;
    }
}
