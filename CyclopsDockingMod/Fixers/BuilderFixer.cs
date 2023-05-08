using System.Collections;
using System.Reflection;
using UnityEngine;
using UWE;

namespace CyclopsDockingMod.Fixers
{
	public static class BuilderFixer
    {
        public const string BaseConnectorL = "BaseConnectorLc";

        public static bool BaseConnector = false;

        private static readonly FieldInfo _ghostModel = typeof(Builder).GetField("ghostModel", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _placeMaxDistance = typeof(Builder).GetField("placeMaxDistance", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _renderers = typeof(Builder).GetField("renderers", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _ghostStructureMaterial = typeof(Builder).GetField("ghostStructureMaterial", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _prefab = typeof(Builder).GetField("prefab", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _constructableTechType = typeof(Builder).GetField("constructableTechType", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _placeMinDistance = typeof(Builder).GetField("placeMinDistance", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _placeDefaultDistance = typeof(Builder).GetField("placeDefaultDistance", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _allowedSurfaceTypes = typeof(Builder).GetField("allowedSurfaceTypes", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _forceUpright = typeof(Builder).GetField("forceUpright", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _allowedInSub = typeof(Builder).GetField("allowedInSub", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _allowedInBase = typeof(Builder).GetField("allowedInBase", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _allowedOutside = typeof(Builder).GetField("allowedOutside", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _allowedOnConstructables = typeof(Builder).GetField("allowedOnConstructables", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _allowedUnderwater = typeof(Builder).GetField("allowedUnderwater", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _rotationEnabled = typeof(Builder).GetField("rotationEnabled", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _rotatableBasePiece = typeof(Builder).GetField("rotatableBasePiece", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _alignWithSurface = typeof(Builder).GetField("alignWithSurface", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _attachedToBase = typeof(Builder).GetField("attachedToBase", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _ghostModelPosition = typeof(Builder).GetField("ghostModelPosition", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _ghostModelRotation = typeof(Builder).GetField("ghostModelRotation", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _ghostModelScale = typeof(Builder).GetField("ghostModelScale", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo _InitBounds = typeof(Builder).GetMethod("InitBounds", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo _CreatePowerPreviewAsync = typeof(Builder).GetMethod("CreatePowerPreviewAsync", BindingFlags.Static | BindingFlags.NonPublic);

        public static bool CreateGhost_Prefix(ref bool __result)
		{
			if (BuilderFixer._ghostModel.GetValue(null) != null || !uGUI_BuilderMenuFixer.SelectedDocking)
				return true;
			BuilderFixer.BaseConnector = true;
			GameObject gameObject = (GameObject)BuilderFixer._prefab.GetValue(null);
			Constructable component = gameObject.GetComponent<Constructable>();
			ConstructableBase component2 = gameObject.GetComponent<ConstructableBase>();
			BuilderFixer._constructableTechType.SetValue(null, component.techType);
			BuilderFixer._placeMinDistance.SetValue(null, component.placeMinDistance);
			BuilderFixer._placeMaxDistance.SetValue(null, component.placeMaxDistance);
			BuilderFixer._placeDefaultDistance.SetValue(null, component.placeDefaultDistance);
			BuilderFixer._allowedSurfaceTypes.SetValue(null, component.allowedSurfaceTypes);
			BuilderFixer._forceUpright.SetValue(null, component.forceUpright);
			BuilderFixer._allowedInSub.SetValue(null, component.allowedInSub);
			BuilderFixer._allowedInBase.SetValue(null, component.allowedInBase);
			BuilderFixer._allowedOutside.SetValue(null, component.allowedOutside);
			BuilderFixer._allowedOnConstructables.SetValue(null, component.allowedOnConstructables);
			BuilderFixer._allowedUnderwater.SetValue(null, component.allowedUnderwater);
			BuilderFixer._rotationEnabled.SetValue(null, component.rotationEnabled);
			BuilderFixer._rotatableBasePiece.SetValue(null, component2 != null && component2.rotatableBasePiece);
			BuilderFixer._alignWithSurface.SetValue(null, component.alignWithSurface);
			BuilderFixer._attachedToBase.SetValue(null, component.attachedToBase);
			if (component2 != null)
			{
				GameObject model = UnityEngine.Object.Instantiate<GameObject>(gameObject).GetComponent<ConstructableBase>().model;
				uGUI_BuilderMenuFixer.SelectedDocking = false;
				GameObject gameObject2 = new GameObject("BaseConnectorLc");
				gameObject2.transform.parent = model.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				gameObject2.transform.localScale = Vector3.one;
				BaseFixer.SetupCyclopsDockingHatchModel(model.transform, CyclopsHatchConnector.CyclopsDockingAnim.NONE);
				BuilderFixer._ghostModel.SetValue(null, model);
				((GameObject)BuilderFixer._ghostModel.GetValue(null)).GetComponent<BaseGhost>().SetupGhost();
				BuilderFixer._ghostModelPosition.SetValue(null, Vector3.zero);
				BuilderFixer._ghostModelRotation.SetValue(null, Quaternion.identity);
				BuilderFixer._ghostModelScale.SetValue(null, Vector3.one);
				BuilderFixer._renderers.SetValue(null, MaterialExtensions.AssignMaterial((GameObject)BuilderFixer._ghostModel.GetValue(null), (Material)BuilderFixer._ghostStructureMaterial.GetValue(null), true));
				BuilderFixer._InitBounds.Invoke(null, new object[] { (GameObject)BuilderFixer._ghostModel.GetValue(null) });
			}
			else
			{
				BuilderFixer._ghostModel.SetValue(null, UnityEngine.Object.Instantiate<GameObject>(component.model));
				((GameObject)BuilderFixer._ghostModel.GetValue(null)).SetActive(true);
				Transform component3 = component.GetComponent<Transform>();
				Transform component4 = component.model.GetComponent<Transform>();
				Quaternion quaternion = Quaternion.Inverse(component3.rotation);
				BuilderFixer._ghostModelPosition.SetValue(null, quaternion * (component4.position - component3.position));
				BuilderFixer._ghostModelRotation.SetValue(null, quaternion * component4.rotation);
				BuilderFixer._ghostModelScale.SetValue(null, component4.lossyScale);
				Collider[] componentsInChildren = ((GameObject)BuilderFixer._ghostModel.GetValue(null)).GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
					UnityEngine.Object.Destroy(componentsInChildren[i]);
				BuilderFixer._renderers.SetValue(null, MaterialExtensions.AssignMaterial((GameObject)BuilderFixer._ghostModel.GetValue(null), (Material)BuilderFixer._ghostStructureMaterial.GetValue(null), true));
				string poweredPrefabName = CraftData.GetPoweredPrefabName((TechType)BuilderFixer._constructableTechType.GetValue(null));
				if (!string.IsNullOrEmpty(poweredPrefabName))
				{
					CoroutineHost.StartCoroutine((IEnumerator)BuilderFixer._CreatePowerPreviewAsync.Invoke(null, new object[]
					{
						(GameObject)BuilderFixer._ghostModel.GetValue(null),
						poweredPrefabName
					}));
				}
				BuilderFixer._InitBounds.Invoke(null, new object[] { (GameObject)BuilderFixer._prefab.GetValue(null) });
			}
			__result = true;
			return false;
		}

		public static void End_Postfix()
		{
			if (BuilderFixer.BaseConnector)
				BuilderFixer.BaseConnector = false;
		}
	}
}
