using System.Collections.Generic;
using CyclopsDockingMod.Fixers;
#if SUBNAUTICA_NAUTI
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Diagnostics.CodeAnalysis;
#else
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
#endif
using Ingredient = CraftData.Ingredient;
using UnityEngine;

namespace CyclopsDockingMod
{
	public class CyclopsHatchConnector : BaseItem
    {
        public static List<TechType> ResourceMap = new List<TechType>(new TechType[]
        {
            TechType.Titanium,
            TechType.Titanium,
            TechType.Titanium,
            TechType.Lubricant,
            TechType.Lead
        });

        public const float ConstructAnimDuration = 8f;

        public const float ConstructAnimFadeIn = 0.5f;

        public const string ModelName = "CyclopsDockingHatchClean";

        public static string CyclopsHatchConnectorName = "Cyclops docking hatch";

        public static string CyclopsHatchConnectorDescription = "A module for mooring the Cyclops to a base. Allows quick base/Cyclops transitions and recharges Cyclops powercells.";

        private static GameObject _cyclopsDockingHatch = null;

        private static Texture _normal1 = null;

		private static Texture _illum1 = null;

        public enum CyclopsDockingAnim
        {
            NONE,
            CONSTRUCT,
            DOCKING,
            DOCKED,
            UNDOCKING,
            UNDOCKED
        }

#if SUBNAUTICA_NAUTI
        [SetsRequiredMembers]
        public CyclopsHatchConnector() : base("CyclopsHatchConnector", CyclopsHatchConnectorName, CyclopsHatchConnectorDescription, "CyclopsDockingHatchIconG")
#else
        public CyclopsHatchConnector()
#endif
        {
#if SUBNAUTICA_NAUTI
            GameObject = new GameObject(base.ClassID);
#else
			base.ClassID = "CyclopsHatchConnector";
			base.PrefabFileName = "WorldEntities/Environment/Wrecks/" + base.ClassID;
			base.GameObject = new GameObject(base.ClassID);
			base.TechType = TechTypeHandler.AddTechType(base.ClassID, CyclopsHatchConnector.CyclopsHatchConnectorName, CyclopsHatchConnector.CyclopsHatchConnectorDescription, true);
#endif
			CyclopsDockingMod.CyclopsHatchConnector = base.TechType;
			this.IsHabitatBuilder = true;
#if SUBNAUTICA_NAUTI
            base.Recipe = new RecipeData
#else
            base.Recipe = new TechData
#endif
			{
				craftAmount = 1,
				Ingredients = this.SortIngredients()
			};
		}

		private List<Ingredient> SortIngredients()
		{
			Dictionary<TechType, int> dictionary = new Dictionary<TechType, int>();
			foreach (TechType techType in CyclopsHatchConnector.ResourceMap)
			{
				if (dictionary.ContainsKey(techType))
				{
					Dictionary<TechType, int> dictionary2 = dictionary;
					TechType techType2 = techType;
					int num = dictionary2[techType2];
					dictionary2[techType2] = num + 1;
				}
				else
					dictionary.Add(techType, 1);
			}
			List<Ingredient> list = new List<Ingredient>();
			foreach (KeyValuePair<TechType, int> keyValuePair in dictionary)
				list.Add(new Ingredient(keyValuePair.Key, keyValuePair.Value));
			return list;
		}

		public override void RegisterItem()
		{
			if (!this.IsRegistered)
			{
#if SUBNAUTICA_NAUTI
				CraftDataHandler.SetRecipeData(TechType, Recipe);
#else
                CraftDataHandler.SetTechData(base.TechType, base.Recipe);
#endif
				CraftDataHandler.AddBuildable(base.TechType);
				CraftDataHandler.AddToGroup(TechGroup.BasePieces, TechCategory.BasePiece, base.TechType);
#if SUBNAUTICA_NAUTI
				this.Register();
#else
				PrefabHandler.RegisterPrefab(this);
#endif
				SpriteHandler.RegisterSprite(base.TechType, AssetsHelper.Assets.LoadAsset<Sprite>("CyclopsDockingHatchIconG"));

				this.IsRegistered = true;
			}
		}

		public override GameObject GetGameObject()
		{
			if (GameObject == null)
				GameObject = new GameObject(ClassID);
			GameObject gameObject = Object.Instantiate<GameObject>(GameObject);
			gameObject.name = ClassID;
			gameObject.AddComponent<PrefabIdentifier>().ClassId = ClassID;
			gameObject.AddComponent<TechTag>().type = TechType;
			Constructable constructable = gameObject.AddComponent<Constructable>();
			constructable.allowedInBase = true;
			constructable.allowedInSub = true;
			constructable.allowedOutside = true;
			constructable.allowedOnCeiling = false;
			constructable.allowedOnGround = true;
			constructable.allowedOnConstructables = true;
			constructable.allowedUnderwater = true;
			constructable.attachedToBase = true;
			constructable.deconstructionAllowed = true;
			constructable.rotationEnabled = false;
			constructable.model = gameObject;
			constructable.techType = TechType;
			constructable.surfaceType = VFXSurfaceTypes.metal;
			constructable.placeMinDistance = 0.6f;
			constructable.enabled = true;
			return gameObject;
		}

		public static void PlayDockingAnim(GameObject go, CyclopsHatchConnector.CyclopsDockingAnim toPlay)
		{
			string text;
			if (toPlay == CyclopsHatchConnector.CyclopsDockingAnim.CONSTRUCT)
				text = "construct_";
			else if (toPlay == CyclopsHatchConnector.CyclopsDockingAnim.DOCKING)
				text = "";
			else if (toPlay == CyclopsHatchConnector.CyclopsDockingAnim.DOCKED)
				text = "docked_";
			else if (toPlay == CyclopsHatchConnector.CyclopsDockingAnim.UNDOCKING)
				text = "undock_";
			else
			{
				if (toPlay != CyclopsHatchConnector.CyclopsDockingAnim.UNDOCKED)
					return;
				text = null;
			}
			go.GetComponent<Animator>().CrossFadeInFixedTime((text == null) ? "NoAnim" : (text + "armsrotation"), 0.5f);
			foreach (object obj in go.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.name == "SmallBaseTube")
					transform.GetComponent<Animator>().CrossFadeInFixedTime((text == null) ? "NoAnim" : (text + "smallbasetube"), 0.5f);
				else if (transform.name == "arms")
				{
					foreach (object obj2 in transform)
					{
						Transform transform2 = (Transform)obj2;
						if (transform2.name == "arm_up" || transform2.name == "arm_down" || transform2.name == "arm_up_bis" || transform2.name == "arm_down_bis")
							transform2.GetComponent<Animator>().CrossFadeInFixedTime((text == null) ? "NoAnim" : (text + transform2.name), 0.5f);
					}
				}
			}
			if (toPlay == CyclopsHatchConnector.CyclopsDockingAnim.DOCKED)
				StabilizerFixer.RefreshStabilizers();
		}

		public static GameObject InstantiateCyclopsDocking(CyclopsHatchConnector.CyclopsDockingAnim toPlay)
		{
			if (CyclopsHatchConnector._cyclopsDockingHatch == null)
			{
				CyclopsHatchConnector._cyclopsDockingHatch = AssetsHelper.Assets.LoadAsset<GameObject>("CyclopsDockingHatchClean");
				CyclopsHatchConnector._normal1 = AssetsHelper.Assets.LoadAsset<Texture>("submarine_launch_bay_01_02_normal_207");
				CyclopsHatchConnector._illum1 = AssetsHelper.Assets.LoadAsset<Texture>("submarine_launch_bay_01_02_241_illumb");
			}
			GameObject gameObject = Object.Instantiate<GameObject>(CyclopsHatchConnector._cyclopsDockingHatch);
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			Shader shader = Shader.Find("MarmosetUBER");
			if (componentsInChildren != null)
			{
				foreach (Renderer renderer in componentsInChildren)
				{
					if (renderer.materials != null)
					{
						foreach (Material material in renderer.materials)
						{
							material.shader = shader;
							if (material.name.StartsWith("submarine_launch_bay_01_02_101"))
							{
								material.SetTexture("_BumpMap", CyclopsHatchConnector._normal1);
								material.SetTexture("_Illum", CyclopsHatchConnector._illum1);
								material.SetFloat("_EmissionLM", 1f);
								material.EnableKeyword("MARMO_NORMALMAP");
								material.EnableKeyword("MARMO_EMISSION");
								material.EnableKeyword("_ZWRITE_ON");
							}
						}
					}
				}
			}
			CyclopsHatchConnector.PlayDockingAnim(gameObject, toPlay);
			return gameObject;
		}
	}
}
