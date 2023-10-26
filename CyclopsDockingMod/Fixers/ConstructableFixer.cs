namespace CyclopsDockingMod.Fixers;

using System.Collections;
using System.Collections.Generic;
using global::CyclopsDockingMod.Routing;
using UnityEngine;

public static class ConstructableFixer
{
    private static bool IsBaseConnector(Transform root)
    {
        foreach (Transform tr in root)
            if (tr.name.StartsWith("BaseGhost"))
            {
                foreach (Transform chTr in tr)
                    if (chTr.name.StartsWith(BuilderFixer.BaseConnectorL))
                        return true;
                return false;
            }
        return false;
    }

    private static void SetBaseRoot(Constructable constructable, BasePart basePart)
    {
        GameObject gameObject = constructable.gameObject;
        basePart.root = gameObject.GetComponent<BaseRoot>();
        if (basePart.root == null)
        {
            foreach (object obj in gameObject.transform)
            {
                Transform transform = (Transform)obj;
                if (transform.name.StartsWith("BaseGhost"))
                {
                    GameObject gameObject2 = transform.gameObject;
                    basePart.root = ((gameObject2 != null) ? gameObject2.GetComponent<BaseRoot>() : null);
                    break;
                }
            }
        }
    }

    private static KeyValuePair<List<BasePart>, List<Base>> BaseSelect(Constructable construct, Vector3 pos)
    {
        List<BasePart> list = new List<BasePart>();
        List<Base> list2 = new List<Base>();
        foreach (BasePart basePart in BaseFixer.BaseParts)
        {
            if (FastHelper.IsNear(basePart.position, pos))
            {
                list.Add(basePart);
                if (basePart.root == null)
                    SetBaseRoot(construct, basePart);
                BaseRoot root = basePart.root;
                Base b = ((root != null) ? root.GetComponent<Base>() : null);
                if (b != null)
                    list2.Add(b);
            }
        }
        return new KeyValuePair<List<BasePart>, List<Base>>(list, list2);
    }

    public static bool Construct_Prefix(Constructable __instance, ref bool __result)
    {
        if (__instance._constructed)
            return true;
        if (__instance.gameObject == null || __instance.gameObject.transform == null)
            return true;
        if (!IsBaseConnector(__instance.gameObject.transform))
            return true;

        __instance.resourceMap = CyclopsHatchConnector.ResourceMap;
        int count = __instance.resourceMap.Count;
        int num = __instance.GetResourceID();
        __instance.constructedAmount += Time.deltaTime / (count * Constructable.GetConstructInterval());
        __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
        int num2 = __instance.GetResourceID();
        if (num2 != num)
        {
            TechType techType = __instance.resourceMap[num2 - 1];
            if (!Inventory.main.DestroyItem(techType, false) && GameModeUtils.RequiresIngredients())
            {
                __instance.constructedAmount = (float)num / (float)count;
                __result = false;
                return false;
            }
        }
        __instance.UpdateMaterial();
        if (__instance.constructedAmount >= 1f)
            __instance.SetState(true, true);
        __result = true;
        return false;
    }

    public static IEnumerator DeconstructAsync_Postfix(IEnumerator values, Constructable __instance, IOut<bool> result, IOut<string> reason)
    {
        if (__instance.constructed || __instance.deconstructCoroutineRunning || __instance.techType != TechType.BaseConnector || CyclopsDockingMod.CyclopsHatchConnector == TechType.None)
        {
            yield return values;
            yield break;
        }
        GameObject gameObject = __instance.gameObject;
        Transform tr = ((gameObject != null) ? gameObject.transform : null);
        if (tr == null || !IsBaseConnector(tr))
        {
            yield return values;
            yield break;
        }
        __instance.deconstructCoroutineRunning = true;
        __instance.resourceMap = CyclopsHatchConnector.ResourceMap;
        int resourceCount = __instance.resourceMap.Count;
        int num = __instance.GetResourceID();;
        float num2 = 2f;
        __instance.constructedAmount -= Time.deltaTime * num2 / (resourceCount * Constructable.GetConstructInterval());
        __instance.constructedAmount = Mathf.Clamp01(__instance.constructedAmount);
        int nextResID = __instance.GetResourceID();;
        if (nextResID != num && GameModeUtils.RequiresIngredients())
        {
            TechType techType = __instance.resourceMap[nextResID];
            bool resourceCanBePickedUp = Inventory.main.HasRoomFor(techType);
            if (resourceCanBePickedUp)
            {
                TaskResult<GameObject> prefabResult = new TaskResult<GameObject>();
                yield return CraftData.InstantiateFromPrefabAsync(techType, prefabResult, false);
                Pickupable component = prefabResult.Get().GetComponent<Pickupable>();
                if (!Inventory.main.Pickup(component, false))
                {
                    resourceCanBePickedUp = false;
                    Object.Destroy(component.gameObject);
                }
                prefabResult = null;
                prefabResult = null;
            }
            if (!resourceCanBePickedUp)
            {
                __instance.constructedAmount = ((float)nextResID + 0.001f) / (float)resourceCount;
                result.Set(false);
                reason.Set(Language.main.Get("InventoryFull"));
                __instance.deconstructCoroutineRunning = false;
                yield break;
            }
        }
        __instance.UpdateMaterial();
        yield return __instance.ProgressDeconstruction();
        if (__instance.constructedAmount <= 0f)
        {
            KeyValuePair<List<BasePart>, List<Base>> keyValuePair = BaseSelect(__instance, tr.position);
            if (keyValuePair.Key.Count > 0)
            {
                foreach (BasePart basePart in keyValuePair.Key)
                {
                    List<int> list = new List<int>();
                    foreach (Route route2 in AutoPilot.Routes)
                    {
                        if (route2.BasePartPosEnd != null && route2.BasePartPosEnd != null && FastHelper.IsNear(basePart.position, route2.BasePartPosEnd.Value))
                        {
                            AutoPilot.StopSubsPlayingRoute(route2.BasePartPosEnd.Value, true);
                            route2.WayPoints.Add(route2.BasePartPosEnd.Value);
                            route2.BasePartPosEnd = null;
                        }
                        if (route2.BasePartPosStt != null && route2.BasePartPosStt != null && FastHelper.IsNear(basePart.position, route2.BasePartPosStt.Value))
                        {
                            AutoPilot.StopSubsPlayingRoute(route2.BasePartPosStt.Value, false);
                            list.Add(route2.Id);
                        }
                    }
                    List<string> list2 = new List<string>();
                    foreach (KeyValuePair<string, BasePart> keyValuePair2 in SubControlFixer.DockedSubs)
                        if (keyValuePair2.Value != null && FastHelper.IsNear(keyValuePair2.Value.position, tr.position))
                            list2.Add(keyValuePair2.Key);
                    foreach (string text in list2)
                        SubControlFixer.DockedSubs.Remove(text);
                    if (list.Count > 0)
                    {
                        using (List<int>.Enumerator enumerator5 = list.GetEnumerator())
                        {
                            while (enumerator5.MoveNext())
                            {
                                int rtd = enumerator5.Current;
                                AutoPilot.Routes.RemoveAll((Route route) => route.Id == rtd);
                            }
                        }
                    }
                    BaseFixer.BaseParts.Remove(basePart);
                }
            }
            Object.Destroy(__instance.gameObject);
            if (keyValuePair.Value.Count > 0)
                foreach (Base b in keyValuePair.Value)
                    b.RebuildGeometry();
        }
        result.Set(true);
        reason.Set(null);
        __instance.deconstructCoroutineRunning = false;
        yield break;
    }

    public static bool InitializeModelCopy_Prefix(ConstructableBase __instance, ref bool __result)
    {
        foreach (BasePart basePart in BaseFixer.BaseParts)
        {
            GameObject gameObject = __instance.gameObject;
            if (!(((gameObject != null) ? gameObject.transform : null) != null) || !FastHelper.IsNear(__instance.gameObject.transform.position, basePart.position))
            {
                GameObject model = __instance.model;
                if (!(((model != null) ? model.transform : null) != null) || !FastHelper.IsNear(__instance.model.transform.position, basePart.position))
                    continue;
            }
            __instance.ReplaceMaterials(__instance.model);
            GameObject model2 = __instance.model;
            GameObject gameObject2 = new GameObject("BaseConnectorLc");
            gameObject2.transform.parent = model2.transform;
            gameObject2.transform.localPosition = Vector3.zero;
            gameObject2.transform.localRotation = Quaternion.identity;
            gameObject2.transform.localScale = Vector3.one;
            BaseFixer.SetupCyclopsDockingHatchModel(model2.transform, CyclopsHatchConnector.CyclopsDockingAnim.NONE);
            __instance.modelCopy = model2;
            __result = true;
            return false;
        }
        return true;
    }
}
