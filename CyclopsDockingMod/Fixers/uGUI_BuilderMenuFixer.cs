namespace CyclopsDockingMod.Fixers;

using System.Collections;
using UnityEngine;

public static class uGUI_BuilderMenuFixer
{
    public static bool SelectedDocking = false;

    public static IEnumerator BeginAsync_Postfix(IEnumerator values, uGUI_BuilderMenu __instance, TechType techType)
    {
        if (techType == CyclopsDockingMod.CyclopsHatchConnector)
        {
            SelectedDocking = true;
            techType = TechType.BaseConnector;
        }
        GameObject gameObject = __instance.TryGetCachedPrefab(techType);
        if (gameObject != null)
        {
            Builder.Begin(techType, gameObject);
            __instance.beginCoroutine = null;
            yield break;
        }
        yield return Builder.BeginAsync(techType);
        yield break;
    }
}
