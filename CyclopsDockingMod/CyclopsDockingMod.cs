using System.Collections.Generic;
using System.Reflection;
using CyclopsDockingMod.Fixers;
using CyclopsDockingMod.UI;
using HarmonyLib;
using UnityEngine;

namespace CyclopsDockingMod
{
	public static class CyclopsDockingMod
    {
        private static Harmony HarmonyInstance = null;

        public static readonly List<IBaseItem> BaseItems = new List<IBaseItem>();

        public static TechType CyclopsHatchConnector = TechType.None;

        public static GameObject SettingsUIObj = null;

        public static void Start()
		{
			if (!CyclopsDockingMod.InitializeHarmony())
				return;
			ConfigOptions.LoadConfig();
			CyclopsDockingMod.RegisterBaseParts();
			CyclopsDockingMod.PatchAll();
			CyclopsDockingMod.SetupInGameModOptions();
		}

		public static bool InitializeHarmony()
		{
			if ((CyclopsDockingMod.HarmonyInstance = new Harmony("com.osubmarin.cyclopsdockingmod")) == null)
			{
				Logger.Error("Unable to initialize Harmony!");
				return false;
			}
			return true;
		}

		public static void RegisterBaseParts()
		{
			BaseItem baseItem = new CyclopsHatchConnector();
			if (baseItem.GameObject != null)
			{
				baseItem.RegisterItem();
				CyclopsDockingMod.BaseItems.Add(baseItem);
			}
		}

		public static void PatchAll()
		{
			MethodInfo method = typeof(uGUI_MainMenu).GetMethod("LoadMostRecentSavedGame", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method2 = typeof(uGUI_MainMenuFixer).GetMethod("LoadMostRecentSavedGame_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method, new HarmonyMethod(method2), null, null, null, null);
			MethodInfo method3 = typeof(uGUI_MainMenu).GetMethod("OnErrorConfirmed", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method4 = typeof(uGUI_MainMenuFixer).GetMethod("OnErrorConfirmed_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method3, new HarmonyMethod(method4), null, null, null, null);
			MethodInfo method5 = typeof(MainMenuLoadButton).GetMethod("Load", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method6 = typeof(MainMenuLoadButtonFixer).GetMethod("Load_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method5, new HarmonyMethod(method6), null, null, null, null);
			MethodInfo method7 = typeof(IngameMenu).GetMethod("SaveGame", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method8 = typeof(IngameMenuFixer).GetMethod("SaveGame_Postfix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method7, null, new HarmonyMethod(method8), null, null, null);
			MethodInfo method9 = typeof(IngameMenu).GetMethod("QuitGame", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method10 = typeof(IngameMenuFixer).GetMethod("QuitGame_Postfix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method9, null, new HarmonyMethod(method10), null, null, null);
			MethodInfo method11 = typeof(uGUI_BuilderMenu).GetMethod("BeginAsync", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method12 = typeof(uGUI_BuilderMenuFixer).GetMethod("BeginAsync_Postfix", BindingFlags.Static | BindingFlags.Public);
			if (method11 != null && method12 != null)
				CyclopsDockingMod.HarmonyInstance.Patch(method11, null, new HarmonyMethod(method12), null, null, null);
			else
				Logger.Error("Failed to find BeginAsync methods.");
			MethodInfo method13 = typeof(Builder).GetMethod("CreateGhost", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo method14 = typeof(BuilderFixer).GetMethod("CreateGhost_Prefix", BindingFlags.Static | BindingFlags.Public);
			if (method13 != null && method14 != null)
				CyclopsDockingMod.HarmonyInstance.Patch(method13, new HarmonyMethod(method14), null, null, null, null);
			else
				Logger.Error("Failed to find CreateGhost methods.");
			MethodInfo method15 = typeof(BaseGhost).GetMethod("Place", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method16 = typeof(BaseGhostFixer).GetMethod("Place_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method15, new HarmonyMethod(method16), null, null, null, null);
			MethodInfo method17 = typeof(Base).GetMethod("BuildConnectorGeometry", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method18 = typeof(BaseFixer).GetMethod("BuildConnectorGeometry_Postfix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method17, null, new HarmonyMethod(method18), null, null, null);
			MethodInfo method19 = typeof(Base).GetMethod("BuildCorridorGeometry", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method20 = typeof(BaseFixer).GetMethod("BuildCorridorGeometry_Postfix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method19, null, new HarmonyMethod(method20), null, null, null);
			MethodInfo method21 = typeof(SubControl).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method22 = typeof(SubControlFixer).GetMethod("Update_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method21, new HarmonyMethod(method22), null, null, null, null);
			MethodInfo method23 = typeof(Stabilizer).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method24 = typeof(StabilizerFixer).GetMethod("FixedUpdate_Postfix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method23, null, new HarmonyMethod(method24), null, null, null);
			MethodInfo method25 = typeof(CinematicModeTriggerBase).GetMethod("OnHandClick", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method26 = typeof(CinematicModeTriggerBaseFixer).GetMethod("OnHandClick_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method25, new HarmonyMethod(method26), null, null, null, null);
			MethodInfo method27 = typeof(CyclopsEntryHatch).GetMethod("OnTriggerExit", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method28 = typeof(CyclopsEntryHatchFixer).GetMethod("OnTriggerExit_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method27, new HarmonyMethod(method28), null, null, null, null);
			MethodInfo method29 = typeof(SubRoot).GetMethod("UpdateThermalReactorCharge", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method30 = typeof(SubRootFixer).GetMethod("UpdateThermalReactorCharge_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method29, new HarmonyMethod(method30), null, null, null, null);
			MethodInfo method31 = typeof(Constructable).GetMethod("Construct", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method32 = typeof(ConstructableFixer).GetMethod("Construct_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method31, new HarmonyMethod(method32), null, null, null, null);
			MethodInfo method33 = typeof(Constructable).GetMethod("DeconstructAsync", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method34 = typeof(ConstructableFixer).GetMethod("DeconstructAsync_Postfix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method33, null, new HarmonyMethod(method34), null, null, null);
			MethodInfo method35 = typeof(ConstructableBase).GetMethod("InitializeModelCopy", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method36 = typeof(ConstructableFixer).GetMethod("InitializeModelCopy_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method35, new HarmonyMethod(method36), null, null, null, null);
			MethodInfo method37 = typeof(Base).GetMethod("CanSetConnector", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method38 = typeof(BaseFixer).GetMethod("CanSetConnector_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method37, new HarmonyMethod(method38), null, null, null, null);
			MethodInfo method39 = typeof(Base).GetMethod("CanConnectToCell", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method40 = typeof(BaseFixer).GetMethod("CanConnectToCell_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method39, new HarmonyMethod(method40), null, null, null, null);
			MethodInfo method41 = typeof(Base).GetMethod("HasSpaceFor", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method42 = typeof(BaseFixer).GetMethod("HasSpaceFor_Prefix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method41, new HarmonyMethod(method42), null, null, null, null);
			MethodInfo method43 = typeof(Builder).GetMethod("End", BindingFlags.Static | BindingFlags.Public);
			MethodInfo method44 = typeof(BuilderFixer).GetMethod("End_Postfix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method43, null, new HarmonyMethod(method44), null, null, null);
			MethodInfo method45 = typeof(SubRoot).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method46 = typeof(SubRootFixer).GetMethod("Start_Postfix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method45, null, new HarmonyMethod(method46), null, null, null);
			MethodInfo method47 = typeof(CyclopsHelmHUDManager).GetMethod("StartPiloting", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method48 = typeof(CyclopsHelmHUDManagerFixer).GetMethod("StartPiloting_Postfix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method47, null, new HarmonyMethod(method48), null, null, null);
			/*
			MethodInfo method49 = typeof(uGUI_SignInput).GetMethod("UpdateScale", BindingFlags.Instance | BindingFlags.NonPublic);
			if (method49 != null)
			{
				MethodInfo method50 = typeof(MyuGUI_SignInputFixer).GetMethod("MyUpdateScale_Postfix", BindingFlags.Static | BindingFlags.Public);
				CyclopsDockingMod.HarmonyInstance.Patch(method49, null, new HarmonyMethod(method50), null, null, null);
			}
			*/
			MethodInfo[] methods = typeof(BuilderTool).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
			if (methods != null)
			{
				MethodInfo methodInfo = null;
				MethodInfo methodInfo2 = null;
				foreach (MethodInfo methodInfo3 in methods)
				{
					string text = GeneralExtensions.FullDescription(methodInfo3);
					if (text == "void BuilderTool::OnHover(Constructable constructable)")
						methodInfo = methodInfo3;
					else if (text == "void BuilderTool::OnHover(BaseDeconstructable deconstructable)")
						methodInfo2 = methodInfo3;
					if (methodInfo != null && methodInfo2 != null)
						break;
				}
				MethodInfo method51 = typeof(BuilderToolFixer).GetMethod("ConstructOnHover_Prefix", BindingFlags.Static | BindingFlags.Public);
				MethodInfo method52 = typeof(BuilderToolFixer).GetMethod("DeconstructOnHover_Prefix", BindingFlags.Static | BindingFlags.Public);
				CyclopsDockingMod.HarmonyInstance.Patch(methodInfo, new HarmonyMethod(method51), null, null, null, null);
				CyclopsDockingMod.HarmonyInstance.Patch(methodInfo2, new HarmonyMethod(method52), null, null, null, null);
			}
			if (ConfigOptions.EnableAutopilotFeature)
			{
				MethodInfo method53 = typeof(CyclopsEngineChangeState).GetMethod("OnMouseEnter", BindingFlags.Instance | BindingFlags.Public);
				MethodInfo method54 = typeof(CyclopsEngineChangeStateFixer).GetMethod("OnMouseEnter_Prefix", BindingFlags.Static | BindingFlags.Public);
				CyclopsDockingMod.HarmonyInstance.Patch(method53, new HarmonyMethod(method54), null, null, null, null);
				MethodInfo method55 = typeof(CyclopsEngineChangeState).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
				MethodInfo method56 = typeof(CyclopsEngineChangeStateFixer).GetMethod("Update_Prefix", BindingFlags.Static | BindingFlags.Public);
				CyclopsDockingMod.HarmonyInstance.Patch(method55, new HarmonyMethod(method56), null, null, null, null);
				MethodInfo method57 = typeof(CyclopsEngineChangeState).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.Public);
				MethodInfo method58 = typeof(CyclopsEngineChangeStateFixer).GetMethod("OnClick_Prefix", BindingFlags.Static | BindingFlags.Public);
				CyclopsDockingMod.HarmonyInstance.Patch(method57, new HarmonyMethod(method58), null, null, null, null);
			}
			MethodInfo method59 = typeof(UWE.Utils).GetMethod("UpdateCusorLockState", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo method60 = typeof(UtilsFixer).GetMethod("UpdateCusorLockState_Prefix", BindingFlags.Static | BindingFlags.Public);
			MethodInfo method61 = typeof(UtilsFixer).GetMethod("UpdateCusorLockState_Postfix", BindingFlags.Static | BindingFlags.Public);
			CyclopsDockingMod.HarmonyInstance.Patch(method59, new HarmonyMethod(method60), new HarmonyMethod(method61), null, null, null);
		}

		private static void SetupInGameModOptions()
		{
			CyclopsDockingMod.SettingsUIObj = new GameObject("CyclopsDockingModUIObj");
			Object.DontDestroyOnLoad(CyclopsDockingMod.SettingsUIObj);
			CyclopsDockingMod.SettingsUIObj.AddComponent<SceneCleanerPreserve>();
			CyclopsDockingMod.SettingsUIObj.AddComponent<CyclopsDockingModUI>();
		}
	}
}
