namespace CyclopsDockingMod.Fixers;

public static class BaseGhostFixer
{
    public static Int3? LBaseConnector;

    public static bool Place_Prefix(BaseGhost __instance)
    {
        LBaseConnector = ((__instance.gameObject.FindChild("BaseConnectorLc") == null) ? null : new Int3?(__instance.TargetOffset));
        return true;
    }
}
