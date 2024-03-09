using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using CyclopsDockingMod.Controllers;
using CyclopsDockingMod.UI;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static ClipMapManager;
using static OVRHaptics;
using UnityEngine.UIElements;

namespace CyclopsDockingMod.Fixers
{
	public static class BaseFixer
    {
        private static readonly MethodInfo _GetCorridorDef = typeof(Base).GetMethod("GetCorridorDef", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _CanConnectToCell = typeof(Base).GetMethod("CanConnectToCell", BindingFlags.Instance | BindingFlags.NonPublic);

        private static MethodInfo _SpawnPiece = null;

        private static MethodInfo _GetFace = null;

        private static System.Type CorridorDefType = null;

        private static FieldInfo _piece = null;

        private static FieldInfo _rotation = null;

        private static System.Type BasePieceType = null;

        private static object ConnectorLadderEnum = null;

        private static object CorridorIShapeLadderTopEnum = null;

        private static object CorridorTShapeLadderTopEnum = null;

        private static object CorridorXShapeLadderTopEnum = null;

        private static object CorridorCoverIShapeTopIntOpenedEnum = null;

        private static object CorridorCoverTShapeTopIntOpenedEnum = null;

        private static object CorridorCoverXShapeTopIntOpenedEnum = null;

        private static object CorridorIShapeEnum = null;

        private static object CorridorTShapeEnum = null;

        private static object CorridorXShapeEnum = null;

        public static readonly List<BasePart> BaseParts = new List<BasePart>();

        private static bool _typesInitialized = false;

        private static GameObject _sign = null;

        public static void LoadBaseParts(string saveGame)
		{
			SubControlFixer.DockedSubs.Clear();
			BaseFixer.BaseParts.Clear();
			string saveFolderPath = FilesHelper.GetSaveFolderPath(saveGame);
			if (saveFolderPath != null)
			{
				if (!Directory.Exists(saveFolderPath))
				{
					Logger.Info("No save directory found for base parts at \"" + saveFolderPath + "\".");
					return;
				}
				string text = FilesHelper.Combine(saveFolderPath, "baseparts.json");
                if (File.Exists(text))
                {
                    Logger.Info("Loading base parts from \"" + text + "\".");
					BasePartSaveData[] array = JArray.Parse(File.ReadAllText(text, Encoding.UTF8)).ToObject<BasePartSaveData[]>();
                    if (array != null && array.Length != 0)
                    {
						foreach (BasePartSaveData data in array)
                        {
                            string dock = ((string.IsNullOrWhiteSpace(data.basePart.dock) || data.basePart.dock == "?") ? null : data.basePart.dock);
                            BasePart basePart = new BasePart(data.basePart.id, new Int3(data.basePart.cell.x, data.basePart.cell.y, data.basePart.cell.z), data.basePart.index, new Vector3(data.basePart.position.x, data.basePart.position.y, data.basePart.position.z), data.basePart.type, dock, BaseFixer.GetBaseRoot(data.basePart.id), BaseFixer.GetSubRoot(dock), data.signConfig.config1, data.signConfig.config2, data.signConfig.config3, BaseFixer.RestoreSignElements(data.signConfig.config4));
                            BaseFixer.BaseParts.Add(basePart);
                            if (dock != null)
                            {
                                SubControlFixer.DockedSubs[dock] = basePart;
                            }
                        }
                    }
                    Logger.Info("Base parts loaded. Player built {0} custom base parts.", new object[] { BaseFixer.BaseParts.Count });
                    return;
                }
                text = FilesHelper.Combine(saveFolderPath, "baseparts.txt");
				if (File.Exists(text))
				{
					Logger.Info("Loading old format base parts from \"" + text + "\".");
					string[] array;
					try
					{
						array = File.ReadAllLines(text, Encoding.UTF8);
					}
					catch (System.Exception ex)
					{
						Logger.Error("Exception caught while loading base parts. Exception=[" + ex.ToString() + "]");
						return;
					}
					if (array != null && array.Length != 0)
					{
						foreach (string text2 in array)
						{
							if (text2.Length > 10 && text2.Contains("/"))
							{
                                string[] array3 = text2.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
								int cellX;
								int cellY;
								int cellZ;
								int index;
								float posX;
								float posY;
								float posZ;
								int type;
								int signConfig1;
								int signConfig2;
								if (array3 != null && array3.Length == 14 && int.TryParse(array3[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out cellX) && int.TryParse(array3[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out cellY) && int.TryParse(array3[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out cellZ) && int.TryParse(array3[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out index) && index >= 0 && float.TryParse(array3[5], NumberStyles.Float, CultureInfo.InvariantCulture, out posX) && float.TryParse(array3[6], NumberStyles.Float, CultureInfo.InvariantCulture, out posY) && float.TryParse(array3[7], NumberStyles.Float, CultureInfo.InvariantCulture, out posZ) && int.TryParse(array3[8], NumberStyles.Integer, CultureInfo.InvariantCulture, out type) && type >= 0 && int.TryParse(array3[10], NumberStyles.Integer, CultureInfo.InvariantCulture, out signConfig1) && int.TryParse(array3[11], NumberStyles.Integer, CultureInfo.InvariantCulture, out signConfig2))
								{
									string dock = ((string.IsNullOrWhiteSpace(array3[9]) || array3[9] == "?") ? null : array3[9]);
									BasePart basePart = new BasePart(array3[0], new Int3(cellX, cellY, cellZ), index, new Vector3(posX, posY, posZ), type, dock, BaseFixer.GetBaseRoot(array3[0]), BaseFixer.GetSubRoot(dock), signConfig1, signConfig2, string.Compare(array3[12], "True", true, CultureInfo.InvariantCulture) == 0, BaseFixer.RestoreSignElements(array3[13]));
									BaseFixer.BaseParts.Add(basePart);
									if (dock != null)
									{
										SubControlFixer.DockedSubs[dock] = basePart;
									}
								}
							}
						}
					}
					Logger.Info("Base parts loaded. Player built {0} custom base parts.", new object[] { BaseFixer.BaseParts.Count });
					return;
				}
				Logger.Info("No base parts saved at \"" + text + "\".");
				return;
			}
			else
			{
				Logger.Info("Could not find save slot for base parts.");
			}
		}

		public static void SaveBaseParts()
		{
			string text = "";
			List<BasePartSaveData> basePartSaveData = new List<BasePartSaveData>();
			foreach (BasePart basePart in BaseFixer.BaseParts)
			{
				if (basePart.type >= 0 && !string.IsNullOrEmpty(basePart.id))
				{
					BaseRoot root = basePart.root;
					Transform transform;
					if (root == null)
					{
						transform = null;
					}
					else
					{
						GameObject gameObject = root.gameObject;
						transform = ((gameObject != null) ? gameObject.transform : null);
					}
                    System.Tuple<int, int, bool, string> signConfig = BaseFixer.GetSignConfig(transform, basePart.position);
					basePartSaveData.Add(new BasePartSaveData
					{
						basePart =
						{
							id = basePart.id,
							cell = {
								x = basePart.cell.x,
								y = basePart.cell.y,
								z = basePart.cell.z
							},
                            index = basePart.index,
							position = {
								x = basePart.position.x,
								y = basePart.position.y,
								z = basePart.position.z
							},
                            type = basePart.type,
							dock = string.IsNullOrEmpty(basePart.dock) ? "?" : basePart.dock
						},
						signConfig =
                        {
                            config1 = signConfig.Item1,
                            config2 = signConfig.Item2,
                            config3 = signConfig.Item3,
                            config4 = signConfig.Item4
                        }
					});
					text += string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}/{10}/{11}/{12}/{13}{14}", new object[]
					{
						basePart.id,
						basePart.cell.x.ToString(CultureInfo.InvariantCulture),
						basePart.cell.y.ToString(CultureInfo.InvariantCulture),
						basePart.cell.z.ToString(CultureInfo.InvariantCulture),
						basePart.index.ToString(CultureInfo.InvariantCulture),
						basePart.position.x.ToString(CultureInfo.InvariantCulture),
						basePart.position.y.ToString(CultureInfo.InvariantCulture),
						basePart.position.z.ToString(CultureInfo.InvariantCulture),
						basePart.type.ToString(CultureInfo.InvariantCulture),
						string.IsNullOrEmpty(basePart.dock) ? "?" : basePart.dock,
						signConfig.Item1.ToString(CultureInfo.InvariantCulture),
						signConfig.Item2.ToString(CultureInfo.InvariantCulture),
						signConfig.Item3 ? "True" : "False",
						signConfig.Item4,
						System.Environment.NewLine
					});
                }
            }
            string saveFolderPath = FilesHelper.GetSaveFolderPath();
            if (saveFolderPath.Contains("/test/"))
            {
                Logger.Error("Unable to find save folder path at [" + saveFolderPath + "].");
                return;
            }
            if (!Directory.Exists(saveFolderPath))
            {
				if (basePartSaveData.Count == 0) return; //do not need to remove old save data
                try
                {
                    Directory.CreateDirectory(saveFolderPath);
                }
                catch (System.Exception ex)
                {
                    Logger.Error($"Exception caught while creating folder at [{saveFolderPath}]. Exception=[{ex.ToString()}]");
                }
            }
            if (!Directory.Exists(saveFolderPath))
            {
                Logger.Error("Unable to create save folder at [" + saveFolderPath + "].");
                return;
            }
            string text2 = FilesHelper.Combine(saveFolderPath, "baseparts.json");
            string text3 = FilesHelper.Combine(saveFolderPath, "baseparts.txt");
            if (basePartSaveData.Count > 0)
            {
                Logger.Info($"Saving {BaseFixer.BaseParts.Count} base parts to \"{text2}\" with legacy \"{text3}\" for backwards compatibility.");
                try
                {
                    File.WriteAllText(text2, JArray.FromObject(basePartSaveData).ToString(), Encoding.UTF8);
                }
                catch (System.Exception ex2)
                {
                    Logger.Error($"Exception caught while saving base parts at [{text2}]. Exception=[{ex2.ToString()}]");
                }
                try
                {
                    File.WriteAllText(text3, text, Encoding.UTF8);
                }
                catch (System.Exception ex2)
                {
                    Logger.Error($"Exception caught while saving base parts at [{text3}]. Exception=[{ex2.ToString()}]");
                }
            }
			else
			{
                if (File.Exists(text2))
                {
                    Logger.Info($"Deleting old baseparts info \"{text2}\"");
                    try
                    {
                        File.Delete(text2);
                    }
                    catch (System.Exception ex2)
                    {
                        Logger.Error($"Exception caught while deleting old baseparts info \"{text2}\". Exception=[{ex2.ToString()}]");
                    }
                }
                if (File.Exists(text3))
                {
                    Logger.Info($"Deleting old legacy baseparts info \"{text3}\"");
                    try
                    {
                        File.Delete(text3);
                    }
                    catch (System.Exception ex2)
                    {
                        Logger.Error($"Exception caught while deleting old baseparts info \"{text3}\". Exception=[{ex2.ToString()}]");
                    }
                }
            }
        }

		private static bool[] RestoreSignElements(string str)
		{
			if (!string.IsNullOrEmpty(str) && str != "?")
			{
				string[] array = str.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
				if (array != null)
				{
					List<bool> list = new List<bool>();
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						bool flag;
						if (bool.TryParse(array2[i], out flag))
							list.Add(flag);
					}
					if (list.Count > 0)
						return list.ToArray();
				}
			}
			return null;
		}

		private static string FormatElements(bool[] states)
		{
			string text = "";
			if (states != null)
				foreach (bool flag in states)
					text = text + ((text.Length > 0) ? ";" : "") + flag.ToString(CultureInfo.InvariantCulture);
			if (!string.IsNullOrEmpty(text))
				return text;
			return "?";
		}

		private static System.Tuple<int, int, bool, string> GetSignConfig(Transform t, Vector3 pos)
		{
			if (t != null)
			{
				Vector3 vector = pos - BasePart.P_CyclopsDockingHatchC;
				foreach (object obj in t)
				{
					Transform transform = (Transform)obj;
					if (transform.name.StartsWith("BaseCell") && FastHelper.IsNear(transform.position, vector))
					{
						foreach (object obj2 in transform)
						{
							Transform transform2 = (Transform)obj2;
							if (transform2.name.StartsWith("Sign"))
							{
								Sign component = transform2.GetComponent<Sign>();
								if (component != null)
									return new System.Tuple<int, int, bool, string>(component.signInput.scaleIndex, component.signInput.colorIndex, component.signInput.backgroundToggle.isOn, BaseFixer.FormatElements(component.signInput.elementsState));
							}
						}
					}
				}
			}
			return new System.Tuple<int, int, bool, string>(CyclopsDockingModUI.CfgSignTextScale, CyclopsDockingModUI.CfgSignTextColorVal, CyclopsDockingModUI.CfgSignBackgroundVisible, ConfigOptions.DefaultSignElements);
		}

		private static Vector3 GetISignPos(Base.FaceType faceType, bool cond)
		{
			float num;
			if (faceType == Base.FaceType.Reinforcement)
				num = -1.1f;
			else if (faceType == Base.FaceType.Planter)
				num = -1.01f;
			else
				num = -1.13f;
			float num2 = ((faceType == Base.FaceType.Hatch) ? 0.8f : 0.56f);
			if (cond)
				return new Vector3(0f, num2, num);
			return new Vector3(num, num2, 0f);
		}

		private static void SetTSignPos(Transform tr, Base.FaceType faceType, int t)
		{
			float num;
			if (faceType == Base.FaceType.Reinforcement)
				num = -1.1f;
			else if (faceType == Base.FaceType.Planter)
				num = -1.01f;
			else
				num = -1.13f;
			float num2 = ((faceType == Base.FaceType.Hatch) ? 0.8f : 0.56f);
			switch (t)
			{
			case 0:
				tr.localPosition = new Vector3(0f, num2, num);
				tr.localEulerAngles = Vector3.zero;
				return;
			case 1:
				tr.localPosition = new Vector3(num * -1f, num2, 0f);
				tr.localEulerAngles = new Vector3(0f, 270f, 0f);
				return;
			case 2:
				tr.localPosition = new Vector3(0f, num2, num * -1f);
				tr.localEulerAngles = new Vector3(0f, 180f, 0f);
				return;
			default:
				tr.localPosition = new Vector3(num, num2, 0f);
				tr.localEulerAngles = new Vector3(0f, 90f, 0f);
				return;
			}
		}

		private static void SetupCyclopsDockingSign(BasePart bp, Transform t, BasePieceConfig cfg, System.Tuple<Base.FaceType, Base.FaceType, Base.FaceType, Base.FaceType> faces)
		{
			if (BaseFixer._sign == null)
			{
				AsyncOperationHandle<GameObject> asyncOperationHandle = AddressablesUtility.LoadAsync<GameObject>("Submarine/Build/Sign.prefab");
				asyncOperationHandle.WaitForCompletion();
				BaseFixer._sign = asyncOperationHandle.Result;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(BaseFixer._sign);
			Constructable component = gameObject.GetComponent<Constructable>();
			if (component != null)
				component.deconstructionAllowed = false;
			Sign component2 = gameObject.GetComponent<Sign>();
			component2.scaleIndex = bp.signScale;
			component2.colorIndex = bp.signColor;
			component2.backgroundEnabled = bp.signBackground;
			if (bp.signElements != null)
				component2.elements = bp.signElements;
			component2.signInput.scaleIndex = bp.signScale;
			component2.signInput.colorIndex = bp.signColor;
			component2.signInput.backgroundToggle.isOn = bp.signBackground;
			if (bp.signElements != null)
				component2.signInput.elementsState = bp.signElements;
			BaseFixer.SetEnergyLbl(gameObject, component2, BaseFixer.GetEnergyLbl(bp, gameObject));
			foreach (object obj in gameObject.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.name.StartsWith("Trigger"))
				{
					transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.15f);
					break;
				}
			}
			bp.signGo = gameObject;
			gameObject.transform.parent = t;
			gameObject.transform.localScale = Vector3.one;
			if (cfg.IShape)
			{
				bool flag = cfg.Rotation.y > 0f;
				gameObject.transform.localPosition = BaseFixer.GetISignPos(flag ? faces.Item3 : faces.Item4, flag);
				gameObject.transform.localEulerAngles = new Vector3(0f, flag ? 0f : 90f, 0f);
			}
			else if (cfg.TShape)
			{
				if (FastHelper.NearlyEquals(cfg.Rotation.y, 0f) && cfg.Rotation.w > 0.5f)
					BaseFixer.SetTSignPos(gameObject.transform, faces.Item3, 0);
				else if (cfg.Rotation.y < -0.5f && FastHelper.NearlyEquals(cfg.Rotation.w, 0f))
					BaseFixer.SetTSignPos(gameObject.transform, faces.Item2, 1);
				else if (FastHelper.NearlyEquals(cfg.Rotation.y, 0f) && cfg.Rotation.w < -0.5f)
					BaseFixer.SetTSignPos(gameObject.transform, faces.Item1, 2);
				else
					BaseFixer.SetTSignPos(gameObject.transform, faces.Item4, 3);
			}
			else if (cfg.XShape)
			{
				gameObject.transform.localPosition = new Vector3(0f, 1.1f, -0.3f);
				gameObject.transform.localEulerAngles = Vector3.zero;
			}
			gameObject.SetActive(true);
			if (t.gameObject.GetComponent<SignController>() == null)
				t.gameObject.AddComponent<SignController>();
		}

		public static int GetCyclopsEnergy(SubRoot s)
		{
			if (!(s != null))
				return -1;
			int num = Mathf.RoundToInt(100f / s.powerRelay.GetMaxPower() * s.powerRelay.GetPower());
			if (num < 0)
				return 0;
			if (num <= 100)
				return num;
			return 100;
		}

		private static string GetEnergyLbl(BasePart bp, GameObject signPrefab)
		{
			string text;
			if (!string.IsNullOrEmpty(bp.dock))
			{
				int cyclopsEnergy = BaseFixer.GetCyclopsEnergy(bp.sub);
				text = string.Format(ConfigOptions.LblCyclopsDocked, cyclopsEnergy);
				if (cyclopsEnergy < 0)
				{
					RefreshEnergyController refreshEnergyController = signPrefab.AddComponent<RefreshEnergyController>();
					refreshEnergyController._lastCheck = Time.time;
					refreshEnergyController._bp = bp;
					LoopRefreshEnergyController loopRefreshEnergyController = signPrefab.AddComponent<LoopRefreshEnergyController>();
					loopRefreshEnergyController._lastCheck = refreshEnergyController._lastCheck;
					loopRefreshEnergyController._bp = bp;
				}
			}
			else
				text = ConfigOptions.LblNoCyclopsDocked;
			return text;
		}

		private static void SetEnergyLbl(GameObject signPrefab, Sign sign, string lbl)
        {
            foreach (Transform sTr in signPrefab.transform)
                if (sTr.name.StartsWith("UI"))
                {
                    foreach (Transform subTr in sTr)
                        if (subTr.name.StartsWith("Base"))
                        {
                            ContentSizeFitter csfBase = subTr.GetComponent<ContentSizeFitter>();
                            if (csfBase != null)
                                csfBase.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                            foreach (Transform subSub in subTr)
                                if (subSub.name.StartsWith("InputField"))
                                {
                                    uGUI_InputField inputField = subSub.GetComponent<uGUI_InputField>();
                                    if (inputField != null)
                                        inputField.characterLimit = 40;
                                    break;
                                }
                            break;
                        }
                    break;
                }
            sign.text = lbl;
            sign.signInput.text = lbl;
        }

		public static void SetupCyclopsDockingHatchModel(Transform t, CyclopsHatchConnector.CyclopsDockingAnim toPlay)
        {
            GameObject go = CyclopsHatchConnector.InstantiateCyclopsDocking(toPlay);
            foreach (Transform ch in t)
                if (ch.name.StartsWith("BaseConnectorTube"))
                {
                    SkyApplier[] sas = ch.GetComponents<SkyApplier>();
                    if (sas != null && sas.Length > 0)
                        foreach (SkyApplier sat in sas)
                            if (sat.renderers != null)
                                foreach (Renderer rend in sat.renderers)
                                    if (rend.name.StartsWith("BaseCorridor"))
                                    {
                                        foreach (Transform tr in go.transform)
                                            if (tr.name.StartsWith("SmallBaseTube"))
                                            {
                                                MeshRenderer tb = tr.GetComponentInChildren<MeshRenderer>();
                                                if (tb != null)
                                                {
                                                    tb.material = rend.material;
                                                    SkyApplier sa = go.AddComponent<SkyApplier>();
                                                    sa.renderers = new Renderer[] { tb };
                                                    sa.anchorSky = sat.anchorSky;
                                                    sa.customSkyPrefab = sat.customSkyPrefab;
                                                    sa.dynamic = sat.dynamic;
                                                    sa.emissiveFromPower = sat.emissiveFromPower;
                                                    sa.updaterIndex = sat.updaterIndex;
                                                }
                                                break;
                                            }
                                        break;
                                    }
                    break;
                }
            if (go != null)
            {
                go.transform.parent = t;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }
        }

		private static void AdjustCyclopsDockingHatch(Transform t, string coverName)
		{
			foreach (object obj in t)
			{
				Transform transform = (Transform)obj;
				if (transform.name.StartsWith(coverName))
				{
					Renderer[] allComponentsInChildren = transform.GetAllComponentsInChildren<Renderer>();
					for (int i = 0; i < allComponentsInChildren.Length; i++)
						allComponentsInChildren[i].enabled = false;
				}
				else if (transform.name.StartsWith("BaseCorridorLadderTop"))
				{
					foreach (object obj2 in transform)
					{
						Transform transform2 = (Transform)obj2;
						if (transform2.name.StartsWith("logic"))
						{
							BaseLadder component = transform2.GetComponent<BaseLadder>();
							if (component != null)
                                Object.DestroyImmediate(component);
							transform2.gameObject.AddComponent<LadderController>();
						}
						else if (CyclopsDockingModUI.CfgLadderTintColorVal != 0 && CyclopsDockingModUI.CfgLadderTintColorVal < BasePart.Colors.Length && transform2.name.StartsWith("models"))
						{
							foreach (object obj3 in transform2)
							{
								Transform transform3 = (Transform)obj3;
								if (transform3.name.StartsWith("ladder_extendible_geo"))
								{
									MeshRenderer component2 = transform3.GetComponent<MeshRenderer>();
									if (component2 != null)
									{
										component2.material.SetColor("_Color", BasePart.Colors[CyclopsDockingModUI.CfgLadderTintColorVal]);
										break;
									}
									break;
								}
							}
						}
					}
				}
			}
		}

		private static void InitializeTypes()
		{
			if (!BaseFixer._typesInitialized)
			{
				if (BaseFixer.CorridorDefType == null)
					BaseFixer.CorridorDefType = System.Type.GetType("Base+CorridorDef, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", false, true);
				if ((BaseFixer._piece == null || BaseFixer._rotation == null) && BaseFixer.CorridorDefType != null)
				{
					BaseFixer._piece = BaseFixer.CorridorDefType.GetField("piece");
					BaseFixer._rotation = BaseFixer.CorridorDefType.GetField("rotation");
				}
				if (BaseFixer.BasePieceType == null)
					BaseFixer.BasePieceType = System.Type.GetType("Base+Piece, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", false, true);
				if (BaseFixer.BasePieceType != null)
				{
					if (BaseFixer.ConnectorLadderEnum == null)
						BaseFixer.ConnectorLadderEnum = System.Enum.Parse(BaseFixer.BasePieceType, "ConnectorLadder", false);
					if (BaseFixer.CorridorIShapeLadderTopEnum == null)
						BaseFixer.CorridorIShapeLadderTopEnum = System.Enum.Parse(BaseFixer.BasePieceType, "CorridorIShapeLadderTop", false);
					if (BaseFixer.CorridorTShapeLadderTopEnum == null)
						BaseFixer.CorridorTShapeLadderTopEnum = System.Enum.Parse(BaseFixer.BasePieceType, "CorridorTShapeLadderTop", false);
					if (BaseFixer.CorridorXShapeLadderTopEnum == null)
						BaseFixer.CorridorXShapeLadderTopEnum = System.Enum.Parse(BaseFixer.BasePieceType, "CorridorXShapeLadderTop", false);
					if (BaseFixer.CorridorCoverIShapeTopIntOpenedEnum == null)
						BaseFixer.CorridorCoverIShapeTopIntOpenedEnum = System.Enum.Parse(BaseFixer.BasePieceType, "CorridorCoverIShapeTopIntOpened", false);
					if (BaseFixer.CorridorCoverTShapeTopIntOpenedEnum == null)
						BaseFixer.CorridorCoverTShapeTopIntOpenedEnum = System.Enum.Parse(BaseFixer.BasePieceType, "CorridorCoverTShapeTopIntOpened", false);
					if (BaseFixer.CorridorCoverXShapeTopIntOpenedEnum == null)
						BaseFixer.CorridorCoverXShapeTopIntOpenedEnum = System.Enum.Parse(BaseFixer.BasePieceType, "CorridorCoverXShapeTopIntOpened", false);
					if (BaseFixer.CorridorIShapeEnum == null)
						BaseFixer.CorridorIShapeEnum = System.Enum.Parse(BaseFixer.BasePieceType, "CorridorIShape", false);
					if (BaseFixer.CorridorTShapeEnum == null)
						BaseFixer.CorridorTShapeEnum = System.Enum.Parse(BaseFixer.BasePieceType, "CorridorTShape", false);
					if (BaseFixer.CorridorXShapeEnum == null)
						BaseFixer.CorridorXShapeEnum = System.Enum.Parse(BaseFixer.BasePieceType, "CorridorXShape", false);
				}
				if (BaseFixer._SpawnPiece == null || BaseFixer._GetFace == null)
				{
					MethodInfo[] methods = typeof(Base).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
					if (methods != null)
					{
						foreach (MethodInfo methodInfo in methods)
						{
							if (methodInfo.Name == "SpawnPiece" && methodInfo.GetParameters().Length == 5)
								BaseFixer._SpawnPiece = methodInfo;
							else if (methodInfo.Name == "GetFace" && methodInfo.GetParameters().Length == 2)
								BaseFixer._GetFace = methodInfo;
							if (BaseFixer._SpawnPiece != null && BaseFixer._GetFace != null)
								break;
						}
					}
				}
				BaseFixer._typesInitialized = true;
			}
		}

		public static BaseRoot GetBaseRoot(string pid)
		{
			Base[] array = Object.FindObjectsOfType<Base>();
			if (array != null)
			{
				foreach (Base b in array)
				{
					PrefabIdentifier component = b.gameObject.GetComponent<PrefabIdentifier>();
					if (component != null && component.Id == pid)
						return b.GetComponent<BaseRoot>();
				}
			}
			return null;
		}

		public static SubRoot GetSubRoot(string pid)
		{
			if (!string.IsNullOrEmpty(pid))
			{
				SubRoot[] array = Object.FindObjectsOfType<SubRoot>();
				if (array != null)
					foreach (SubRoot subRoot in array)
					{
						PrefabIdentifier component = subRoot.GetComponent<PrefabIdentifier>();
						if (component != null && component.Id == pid)
							return subRoot;
					}
			}
			return null;
		}

		private static System.Tuple<Base.FaceType, Base.FaceType, Base.FaceType, Base.FaceType> GetFaces(Base b, int index)
		{
			return new System.Tuple<Base.FaceType, Base.FaceType, Base.FaceType, Base.FaceType>((Base.FaceType)BaseFixer._GetFace.Invoke(b, new object[]
			{
				index,
				Base.Direction.North
			}), (Base.FaceType)BaseFixer._GetFace.Invoke(b, new object[]
			{
				index,
				Base.Direction.East
			}), (Base.FaceType)BaseFixer._GetFace.Invoke(b, new object[]
			{
				index,
				Base.Direction.South
			}), (Base.FaceType)BaseFixer._GetFace.Invoke(b, new object[]
			{
				index,
				Base.Direction.West
			}));
		}

		public static void BuildCorridorGeometry_Postfix(Base __instance, Int3 cell, int index)
		{
			if (__instance.IsFaceUsed(index, Base.Direction.Above))
			{
				Transform cellObject = __instance.GetCellObject(cell);
				if (cellObject != null)
				{
					foreach (BasePart basePart in BaseFixer.BaseParts)
					{
						if (basePart.IsDockingHatch(cellObject.position))
						{
							if (!BaseFixer._typesInitialized)
								BaseFixer.InitializeTypes();
							object obj = BaseFixer._GetCorridorDef.Invoke(__instance, new object[] { index });
							object value = BaseFixer._piece.GetValue(obj);
							BasePieceConfig basePieceConfig = new BasePieceConfig((int)value == (int)BaseFixer.CorridorXShapeEnum, (int)value == (int)BaseFixer.CorridorTShapeEnum, (int)value == (int)BaseFixer.CorridorIShapeEnum, Quaternion.identity);
							if (basePieceConfig.IsValid())
							{
								basePieceConfig.SetSquare((Quaternion)BaseFixer._rotation.GetValue(obj));
								if (!__instance.isGhost)
								{
									basePart.Reset();
									MethodBase spawnPiece = BaseFixer._SpawnPiece;
									object[] array = new object[5];
									array[0] = (basePieceConfig.XShape ? BaseFixer.CorridorXShapeLadderTopEnum : (basePieceConfig.TShape ? BaseFixer.CorridorTShapeLadderTopEnum : BaseFixer.CorridorIShapeLadderTopEnum));
									array[1] = cell;
									array[2] = basePieceConfig.Rotation;
									spawnPiece.Invoke(__instance, array);
									MethodBase spawnPiece2 = BaseFixer._SpawnPiece;
									object[] array2 = new object[5];
									array2[0] = (basePieceConfig.XShape ? BaseFixer.CorridorCoverXShapeTopIntOpenedEnum : (basePieceConfig.TShape ? BaseFixer.CorridorCoverTShapeTopIntOpenedEnum : BaseFixer.CorridorCoverIShapeTopIntOpenedEnum));
									array2[1] = cell;
									array2[2] = basePieceConfig.Rotation;
									array2[3] = Base.Direction.Above;
									spawnPiece2.Invoke(__instance, array2);
									BaseFixer.AdjustCyclopsDockingHatch(cellObject, basePieceConfig.XShape ? "BaseCorridorCoverXShapeTopIntClosed" : (basePieceConfig.TShape ? "BaseCorridorCoverTShapeTopIntClosed" : "BaseCorridorCoverIShapeTopIntClosed"));
									BaseFixer.SetupCyclopsDockingSign(basePart, cellObject, basePieceConfig, BaseFixer.GetFaces(__instance, index));
								}
							}
						}
					}
				}
			}
		}

		public static void BuildConnectorGeometry_Postfix(Base __instance, Int3 cell, int index)
		{
			PrefabIdentifier component = __instance.GetComponent<PrefabIdentifier>();
			if (component != null)
			{
				Transform cellObject = __instance.GetCellObject(cell);
				if (BaseGhostFixer.LBaseConnector != null && BaseGhostFixer.LBaseConnector != null && BaseGhostFixer.LBaseConnector.Value.x == cell.x && BaseGhostFixer.LBaseConnector.Value.y == cell.y && BaseGhostFixer.LBaseConnector.Value.z == cell.z)
				{
					BaseGhostFixer.LBaseConnector = null;
					if (cellObject != null)
					{
						BaseFixer.BaseParts.Add(new BasePart(component.Id, cell, index, cellObject.position, 0, null, __instance.GetComponent<BaseRoot>(), null, CyclopsDockingModUI.CfgSignTextScale, CyclopsDockingModUI.CfgSignTextColorVal, CyclopsDockingModUI.CfgSignBackgroundVisible, null));
						BaseFixer.SetupCyclopsDockingHatchModel(cellObject, CyclopsHatchConnector.CyclopsDockingAnim.CONSTRUCT);
						if (!BaseFixer._typesInitialized)
							BaseFixer.InitializeTypes();
						MethodBase spawnPiece = BaseFixer._SpawnPiece;
						object[] array = new object[5];
						array[0] = BaseFixer.ConnectorLadderEnum;
						array[1] = cell;
						array[2] = Quaternion.identity;
						spawnPiece.Invoke(__instance, array);
						__instance.Invoke("RebuildGeometry", 8.6f);
						return;
					}
				}
				else if (cellObject != null)
				{
					foreach (BasePart basePart in BaseFixer.BaseParts)
					{
						if (basePart.id == component.Id && (int)basePart.position.x == (int)cellObject.position.x && (int)basePart.position.y == (int)cellObject.position.y && (int)basePart.position.z == (int)cellObject.position.z)
						{
							if (basePart.root == null)
								basePart.root = __instance.GetComponent<BaseRoot>();
							basePart.Reset();
							BaseFixer.SetupCyclopsDockingHatchModel(cellObject, (!string.IsNullOrEmpty(basePart.dock)) ? CyclopsHatchConnector.CyclopsDockingAnim.DOCKED : CyclopsHatchConnector.CyclopsDockingAnim.NONE);
							if (!BaseFixer._typesInitialized)
								BaseFixer.InitializeTypes();
							MethodBase spawnPiece2 = BaseFixer._SpawnPiece;
							object[] array2 = new object[5];
							array2[0] = BaseFixer.ConnectorLadderEnum;
							array2[1] = cell;
							array2[2] = Quaternion.identity;
							spawnPiece2.Invoke(__instance, array2);
							break;
						}
					}
				}
			}
		}

		public static bool CanSetConnector_Prefix(Base __instance, ref bool __result, Int3 cell)
		{
			if (!BuilderFixer.BaseConnector || __instance.GetCell(cell) != Base.CellType.Empty)
				return true;
			Int3 adjacent = Base.GetAdjacent(cell, Base.Direction.Above);
			if (__instance.GetCell(adjacent) > Base.CellType.Empty)
			{
				__result = false;
				return false;
			}
			Int3 adjacent2 = Base.GetAdjacent(cell, Base.Direction.Below);
			Base.CellType cell2 = __instance.GetCell(adjacent2);
			__result = cell2 != Base.CellType.Empty && (bool)BaseFixer._CanConnectToCell.Invoke(__instance, new object[]
			{
				adjacent2,
				Base.Direction.Above
			});
			return false;
		}

		public static bool CanConnectToCell_Prefix(Base __instance, ref bool __result, Int3 cell, Base.Direction direction)
		{
			if (__instance.GetCell(cell) == Base.CellType.Connector)
			{
				bool flag = false;
				Transform cellObject = __instance.GetCellObject(cell);
				if (cellObject != null)
				{
					using (List<BasePart>.Enumerator enumerator = BaseFixer.BaseParts.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (FastHelper.IsNear(enumerator.Current.position, cellObject.position))
							{
								flag = true;
								break;
							}
						}
					}
				}
				if (flag || BuilderFixer.BaseConnector)
				{
					__result = false;
					return false;
				}
			}
			return true;
		}

		public static bool HasSpaceFor_Prefix(Base __instance, ref bool __result, Int3 cell, Int3 size)
        {
            Transform b = __instance.GetCellObject(Base.GetAdjacent(cell, Base.Direction.Below));
            if (b != null)
                foreach (Transform tr in b)
                    if (tr.name.StartsWith(CyclopsHatchConnector.ModelName))
                    {
                        __result = false;
                        return false;
                    }
            return true;
        }
	}

    public struct BasePartSaveData
	{
        public struct BasePart
        {
            public string id;
			public struct Vec3i { public int x, y, z; }
            public Vec3i cell;
            public int index;
            public struct Vec3f { public float x, y, z; }
            public Vec3f position;
            public int type;
            public string dock;
        }
        public struct SignConfig
        {
            public int config1;
            public int config2;
            public bool config3;
            public string config4;
        }

        public BasePart basePart;
        public SignConfig signConfig;
    }
}
