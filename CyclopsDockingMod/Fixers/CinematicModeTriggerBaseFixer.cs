namespace CyclopsDockingMod.Fixers;

public static class CinematicModeTriggerBaseFixer
{
    public static bool OnHandClick_Prefix(CinematicModeTriggerBase __instance, GUIHand hand)
    {
        if (__instance.triggerType != CinematicModeTriggerBase.TriggerType.HandTarget)
            return true;
        if (!__instance.isValidHandTarget)
            return true;
        Player component = hand.GetComponent<Player>();
        if (component == null)
            return true;
        if (__instance.name != "InternalDiveHatch" || !component.IsInSub())
            return true;
        SubRoot subRoot = Utils.FindAncestorWithComponent<SubRoot>(__instance.gameObject);
        if (subRoot == null)
            return true;
        PrefabIdentifier component2 = subRoot.gameObject.GetComponent<PrefabIdentifier>();
        if (component2 == null)
            return true;
        if (!SubControlFixer.Docked(component2.Id))
            return true;
        if (SubControlFixer.DockedSubs[component2.Id].root == null)
        {
            BaseRoot baseRoot = BaseFixer.GetBaseRoot(SubControlFixer.DockedSubs[component2.Id].id);
            if (baseRoot != null)
                SubControlFixer.DockedSubs[component2.Id].root = baseRoot;
        }
        if (SubControlFixer.DockedSubs[component2.Id].root == null)
            return true;
        if (SubControlFixer.DockedSubs[component2.Id].dock == null)
            SubControlFixer.DockedSubs[component2.Id].dock = component2.Id;
        if (SubControlFixer.DockedSubs[component2.Id].sub == null)
            SubControlFixer.DockedSubs[component2.Id].sub = subRoot;
        component.SetPosition(SubControlFixer.DockedSubs[component2.Id].position - BasePart.P_CyclopsDockingHatchCF);
        component.currentEscapePod = null;
        component.escapePod.Update(false);
        component.SetCurrentSub(SubControlFixer.DockedSubs[component2.Id].root, false);
        component.currentWaterPark = null;
        component.precursorOutOfWater = false;
        component.SetDisplaySurfaceWater(true);
        return false;
    }
}
