namespace CyclopsDockingMod.Fixers;

internal static class MyuGUI_SignInputFixer
{
    public static void MyUpdateScale_Postfix(uGUI_SignInput __instance)
    {
        if (__instance.enabled && __instance.gameObject != null && __instance.gameObject.GetComponent<MySignFixerComponent>() == null && __instance.transform != null && __instance.transform.parent != null && __instance.transform.parent.GetComponent<Sign>() != null)
            __instance.gameObject.AddComponent<MySignFixerComponent>();
    }
}
