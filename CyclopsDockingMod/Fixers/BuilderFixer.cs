namespace CyclopsDockingMod.Fixers;
using UnityEngine;
using UWE;

public static class BuilderFixer
{
    public const string BaseConnectorL = "BaseConnectorLc";

    public static bool BaseConnector = false;

    public static bool CreateGhost_Prefix(ref bool __result)
    {
        if (Builder.ghostModel != null || !uGUI_BuilderMenuFixer.SelectedDocking)
            return true;
        BaseConnector = true;
        GameObject gameObject = Builder.prefab;
        Constructable component = gameObject.GetComponent<Constructable>();
        ConstructableBase component2 = gameObject.GetComponent<ConstructableBase>();
        Builder.constructableTechType = component.techType;
        Builder.placeMinDistance = component.placeMinDistance;
        Builder.placeMaxDistance = component.placeMaxDistance;
        Builder.placeDefaultDistance = component.placeDefaultDistance;
        Builder.allowedSurfaceTypes = component.allowedSurfaceTypes;
        Builder.forceUpright = component.forceUpright;
        Builder.allowedInSub = component.allowedInSub;
        Builder.allowedInBase = component.allowedInBase;
        Builder.allowedOutside = component.allowedOutside;
        Builder.allowedOnConstructables = component.allowedOnConstructables;
        Builder.allowedUnderwater = component.allowedUnderwater;
        Builder.rotationEnabled = component.rotationEnabled;
        Builder.rotatableBasePiece = component2 != null && component2.rotatableBasePiece;
        Builder.alignWithSurface = component.alignWithSurface;
        Builder.attachedToBase = component.attachedToBase;
        if (component2 != null)
        {
            GameObject model = Object.Instantiate(gameObject).GetComponent<ConstructableBase>().model;
            uGUI_BuilderMenuFixer.SelectedDocking = false;
            GameObject gameObject2 = new("BaseConnectorLc");
            gameObject2.transform.parent = model.transform;
            gameObject2.transform.localPosition = Vector3.zero;
            gameObject2.transform.localRotation = Quaternion.identity;
            gameObject2.transform.localScale = Vector3.one;
            BaseFixer.SetupCyclopsDockingHatchModel(model.transform, CyclopsHatchConnector.CyclopsDockingAnim.NONE);
            Builder.ghostModel = model;
            Builder.ghostModel.GetComponent<BaseGhost>().SetupGhost();
            Builder.ghostModelPosition = Vector3.zero;
            Builder.ghostModelRotation = Quaternion.identity;
            Builder.ghostModelScale = Vector3.one;
            Builder.renderers = MaterialExtensions.AssignMaterial(Builder.ghostModel, Builder.ghostStructureMaterial, true);
            Builder.InitBounds( Builder.ghostModel);
        }
        else
        {
            Builder.ghostModel = Object.Instantiate(component.model);
            Builder.ghostModel.SetActive(true);
            Transform component3 = component.GetComponent<Transform>();
            Transform component4 = component.model.GetComponent<Transform>();
            Quaternion quaternion = Quaternion.Inverse(component3.rotation);
            Builder.ghostModelPosition = quaternion * (component4.position - component3.position);
            Builder.ghostModelRotation = quaternion * component4.rotation;
            Builder.ghostModelScale = component4.lossyScale;
            Collider[] componentsInChildren = Builder.ghostModel.GetComponentsInChildren<Collider>();
            for (int i = 0; i < componentsInChildren.Length; i++)
                Object.Destroy(componentsInChildren[i]);
            Builder.renderers = MaterialExtensions.AssignMaterial(Builder.ghostModel, Builder.ghostStructureMaterial, true);
            string poweredPrefabName = CraftData.GetPoweredPrefabName(Builder.constructableTechType);
            if (!string.IsNullOrEmpty(poweredPrefabName))
            {
                CoroutineHost.StartCoroutine(Builder.CreatePowerPreviewAsync(Builder.ghostModel, poweredPrefabName));
            }
            Builder.InitBounds(Builder.prefab);
        }
        __result = true;
        return false;
    }

    public static void End_Postfix()
    {
        if (BaseConnector)
            BaseConnector = false;
    }
}
