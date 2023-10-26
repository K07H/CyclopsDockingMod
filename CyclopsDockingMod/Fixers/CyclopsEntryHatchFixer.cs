namespace CyclopsDockingMod.Fixers;
using System.Reflection;
using UnityEngine;

public static class CyclopsEntryHatchFixer
{
    public static bool OnTriggerExit_Prefix(CyclopsEntryHatch __instance, Collider col)
    {
        if (col.gameObject.Equals(Player.main.gameObject) && __instance.hatchOpen)
        {
            Transform transform = __instance.gameObject.transform;
            Transform transform2 = ((transform != null) ? transform.parent : null);
            if (transform2 != null)
            {
                PrefabIdentifier component = transform2.GetComponent<PrefabIdentifier>();
                if (component != null && SubControlFixer.Docked(component.Id))
                    return false;
            }
        }
        return true;
    }
}
