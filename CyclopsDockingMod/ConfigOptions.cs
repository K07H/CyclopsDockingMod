using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using CyclopsDockingMod.Fixers;
using CyclopsDockingMod.Routing;
using CyclopsDockingMod.UI;
using UnityEngine;

namespace CyclopsDockingMod
{
	internal class ConfigOptions
    {
        public const string NoSignElements = "?";

        public static string DefaultSignElements = "?";

        public static string LblCyclopsDocked = "Cyclops docked: Energy {0}%";

        public static string LblNoCyclopsDocked = "No Cyclops docked";

        public static string LblClimbInCyclops = "Climb into Cyclops";

        public static bool EnableAutopilotFeature = true;

        private static string ConfigFilePath = null;

        private static readonly List<KeyValuePair<string, int>> autoPilotLang = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("noRouteSelected=", 80),
            new KeyValuePair<string, int>("createNewRoute=", 80),
            new KeyValuePair<string, int>("defaultRouteName=", 80),
            new KeyValuePair<string, int>("btnSelectionTooltip=", 80),
            new KeyValuePair<string, int>("btnRenameRouteTooltip=", 80),
            new KeyValuePair<string, int>("btnStartRecordTooltip=", 80),
            new KeyValuePair<string, int>("btnStartAutoPilotTooltip=", 80),
            new KeyValuePair<string, int>("btnStopAutoPilotTooltip=", 80),
            new KeyValuePair<string, int>("btnRemoveRouteTooltip=", 80),
            new KeyValuePair<string, int>("btnConfirmRouteRemovalTooltip=", 80),
            new KeyValuePair<string, int>("btnCancelRouteRemovalTooltip=", 80),
            new KeyValuePair<string, int>("selectRouteNotFound=", 200),
            new KeyValuePair<string, int>("cannotRenameWhileRecording=", 200),
            new KeyValuePair<string, int>("cannotChangeRouteWhilePiloting=", 200),
            new KeyValuePair<string, int>("cannotChangeRouteWhileRecording=", 200),
            new KeyValuePair<string, int>("cannotChangeRouteWhileUndocked=", 200),
            new KeyValuePair<string, int>("cannotRemoveRouteWhilePiloting=", 200),
            new KeyValuePair<string, int>("cannotRemoveRouteWhileRecording=", 200),
            new KeyValuePair<string, int>("cannotRenameWhileRemoving=", 200),
            new KeyValuePair<string, int>("cannotChangeRouteWhileRemoving=", 200),
            new KeyValuePair<string, int>("cannotStartAutoPilotWhileRemoving=", 200),
            new KeyValuePair<string, int>("autoPilotStart=", 80),
            new KeyValuePair<string, int>("autoPilotStop=", 80),
            new KeyValuePair<string, int>("recordingRouteStart=", 200),
            new KeyValuePair<string, int>("recordingRouteStop=", 200),
            new KeyValuePair<string, int>("recordingRouteCancelled=", 200),
            new KeyValuePair<string, int>("reachedRouteEnd=", 80)
        };

        private static void ProcessConfigLine(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				return;
			str = str.TrimStart(new char[] { ' ', '\t' });
			if (str.StartsWith("#"))
				return;
			if (str.StartsWith("autoDockingRange="))
			{
				int num;
				if (str.Length <= "autoDockingRange=".Length || str.Length > 10 + "autoDockingRange=".Length || !int.TryParse(str.Substring("autoDockingRange=".Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
				{
					Logger.Log("WARNING: Bad value « " + str.Substring("autoDockingRange=".Length) + " » for autoDockingRange (must be between 10 and 50). Default value will be used.");
					return;
				}
				if (num >= 10 && num <= 50)
				{
					CyclopsDockingModUI.CfgAutoDockingRange = num;
					CyclopsDockingModUI.CfgAutoDockingRangeOrig = num;
					SubControlFixer.AutoDockingTriggerSqrRange = Mathf.Pow((float)num, 2f);
					SubControlFixer.AutoDockingUndockSqrRange = Mathf.Pow((float)num + 5f, 2f);
					SubControlFixer.AutoDockingDetectSqrRange = Mathf.Pow((float)num + 6f, 2f);
					Logger.Log("INFO: Loaded autoDockingRange: " + num.ToString(CultureInfo.InvariantCulture) + "m");
					return;
				}
				Logger.Log("WARNING: Bad value « " + num.ToString(CultureInfo.InvariantCulture) + " » for autoDockingRange (must be between 10 and 50). Default value will be used.");
				return;
			}
			else if (str.StartsWith("enableManualDocking="))
			{
				if (str.Length <= "enableManualDocking=".Length || str.Length > 20 + "enableManualDocking=".Length)
				{
					Logger.Log("WARNING: Bad value « " + str.Substring("enableManualDocking=".Length) + " » for manual docking toggle (must be between 1 and 20 characters maximum). Default value will be used.");
					return;
				}
				bool flag;
				if (bool.TryParse(str.Substring("enableManualDocking=".Length), out flag))
				{
					CyclopsDockingModUI.CfgManualDockingMode = flag;
					CyclopsDockingModUI.CfgManualDockingModeOrig = flag;
					SubControlFixer.AutoDocking = !flag;
					Logger.Log("INFO: Loaded enableManualDocking: " + (flag ? "true" : "false"));
					return;
				}
				Logger.Log("WARNING: Bad value « " + str.Substring("enableManualDocking=".Length) + " » for manual docking toggle (wrong boolean string value). Default value will be used.");
				return;
			}
			else if (str.StartsWith("openModSettingsKey="))
			{
				KeyCode keyCode;
				if (str.Length > "openModSettingsKey=".Length && str.Length <= 20 + "openModSettingsKey=".Length && Enum.TryParse<KeyCode>(str.Substring("openModSettingsKey=".Length), false, out keyCode))
				{
					CyclopsDockingModUI.CfgOpenUIKeyText = keyCode.ToString();
					CyclopsDockingModUI.OpenUI = keyCode;
					Logger.Log("INFO: Loaded openModSettingsKey: " + keyCode.ToString());
					return;
				}
				Logger.Log("WARNING: Bad value « " + str.Substring("openModSettingsKey=".Length) + " » for openModSettingsKey (must be a valid KeyCode string and have between 1 and 20 characters maximum). Default value will be used.");
				return;
			}
			else if (str.StartsWith("manualDockingKey="))
			{
				KeyCode keyCode2;
				if (str.Length > "manualDockingKey=".Length && str.Length <= 20 + "manualDockingKey=".Length && Enum.TryParse<KeyCode>(str.Substring("manualDockingKey=".Length), false, out keyCode2))
				{
					CyclopsDockingModUI.CfgManualDockingKeyText = keyCode2.ToString();
					SubControlFixer.ManualDockingKey = keyCode2;
					Logger.Log("INFO: Loaded manualDockingKey: " + keyCode2.ToString());
					return;
				}
				Logger.Log("WARNING: Bad value « " + str.Substring("manualDockingKey=".Length) + " » for manualDockingKey (must be a valid KeyCode string and have between 1 and 20 characters maximum). Default value will be used.");
				return;
			}
			else if (str.StartsWith("powercellsChargeSpeed="))
			{
				int num2;
				if (str.Length <= "powercellsChargeSpeed=".Length || str.Length > 10 + "powercellsChargeSpeed=".Length || !int.TryParse(str.Substring("powercellsChargeSpeed=".Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
				{
					Logger.Log("WARNING: Bad value « " + str.Substring("powercellsChargeSpeed=".Length) + " » for powercellsChargeSpeed (must be between 1 and 100). Default value will be used.");
					return;
				}
				if (num2 >= 1 && num2 <= 100)
				{
					CyclopsDockingModUI.CfgRechargeSpeed = (float)num2;
					CyclopsDockingModUI.CfgRechargeSpeedOrig = CyclopsDockingModUI.CfgRechargeSpeed;
					SubRootFixer.CyclopsRechargeSpeed = CyclopsDockingModUI.CfgRechargeSpeed * 0.0004f;
					Logger.Log("INFO: Loaded powercellsChargeSpeed: " + num2.ToString(CultureInfo.InvariantCulture) + "/100");
					return;
				}
				Logger.Log("WARNING: Bad value « " + num2.ToString(CultureInfo.InvariantCulture) + " » for powercellsChargeSpeed (must be between 1 and 100). Default value will be used.");
				return;
			}
			else if (str.StartsWith("noCyclopsText="))
			{
				if (str.Length > "noCyclopsText=".Length && str.Length <= 40 + "noCyclopsText=".Length)
				{
					ConfigOptions.LblNoCyclopsDocked = str.Substring("noCyclopsText=".Length);
					Logger.Log("INFO: Loaded noCyclopsText: « " + ConfigOptions.LblNoCyclopsDocked + " »");
					return;
				}
				Logger.Log("WARNING: Bad value « " + str.Substring("noCyclopsText=".Length) + " » for noCyclopsText (must be between 1 and 40 characters maximum). Default value will be used.");
				return;
			}
			else if (str.StartsWith("cyclopsDockedText="))
			{
				if (str.Length > "cyclopsDockedText=".Length && str.Length <= 40 + "cyclopsDockedText=".Length && str.Contains("{0}"))
				{
					ConfigOptions.LblCyclopsDocked = str.Substring("cyclopsDockedText=".Length);
					Logger.Log("INFO: Loaded cyclopsDockedText: « " + ConfigOptions.LblCyclopsDocked + " »");
					return;
				}
				Logger.Log("WARNING: Bad value « " + str.Substring("cyclopsDockedText=".Length) + " » for cyclopsDockedText (string must contain the special symbol and must be between 1 and 40 characters maximum). Default value will be used.");
				return;
			}
			else if (str.StartsWith("climbIntoCyclopsText="))
			{
				if (str.Length > "climbIntoCyclopsText=".Length && str.Length <= 40 + "climbIntoCyclopsText=".Length)
				{
					ConfigOptions.LblClimbInCyclops = str.Substring("climbIntoCyclopsText=".Length);
					Logger.Log("INFO: Loaded climbIntoCyclopsText: « " + ConfigOptions.LblClimbInCyclops + " »");
					return;
				}
				Logger.Log("WARNING: Bad value « " + str.Substring("climbIntoCyclopsText=".Length) + " » for climbIntoCyclopsText (must be between 1 and 40 characters maximum). Default value will be used.");
				return;
			}
			else if (str.StartsWith("defaultTextSize="))
			{
				int num3;
				if (str.Length <= "defaultTextSize=".Length || str.Length > 20 + "defaultTextSize=".Length || !int.TryParse(str.Substring("defaultTextSize=".Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out num3))
				{
					Logger.Log("WARNING: Bad value « " + str.Substring("defaultTextSize=".Length) + " » for sign's text size (must be between -3 and 3). Default value will be used.");
					return;
				}
				if (num3 >= -3 && num3 <= 3)
				{
					CyclopsDockingModUI.CfgSignTextScale = num3;
					CyclopsDockingModUI.CfgSignTextScaleOrig = CyclopsDockingModUI.CfgSignTextScale;
					Logger.Log("INFO: Loaded defaultTextSize: " + CyclopsDockingModUI.CfgSignTextScale.ToString(CultureInfo.InvariantCulture));
					return;
				}
				Logger.Log("WARNING: Bad value « " + num3.ToString(CultureInfo.InvariantCulture) + " » for sign's text size (must be between -3 and 3). Default value will be used.");
				return;
			}
			else if (str.StartsWith("defaultTextColor="))
			{
				int num4;
				if (str.Length <= "defaultTextColor=".Length || str.Length > 20 + "defaultTextColor=".Length || !int.TryParse(str.Substring("defaultTextColor=".Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out num4))
				{
					Logger.Log("WARNING: Bad value « " + str.Substring("defaultTextColor=".Length) + " » for sign's text color (must be between 0 and 7). Default value will be used.");
					return;
				}
				if (num4 >= 0 && num4 <= 7)
				{
					CyclopsDockingModUI.CfgSignTextColorVal = num4;
					CyclopsDockingModUI.CfgSignTextColor = CyclopsDockingModUI.CfgSignTextColors[num4];
					CyclopsDockingModUI.CfgSignTextColorOrig = CyclopsDockingModUI.CfgSignTextColor;
					Logger.Log("INFO: Loaded defaultTextColor: " + CyclopsDockingModUI.CfgSignTextColor);
					return;
				}
				Logger.Log("WARNING: Bad value « " + num4.ToString(CultureInfo.InvariantCulture) + " » for sign's text color (must be between 0 and 7). Default value will be used.");
				return;
			}
			else if (str.StartsWith("defaultBackground="))
			{
				if (str.Length <= "defaultBackground=".Length || str.Length > 20 + "defaultBackground=".Length)
				{
					Logger.Log("WARNING: Bad value « " + str.Substring("defaultBackground=".Length) + " » for sign's background visibility (must be between 1 and 20 characters maximum). Default value will be used.");
					return;
				}
				bool flag2;
				if (bool.TryParse(str.Substring("defaultBackground=".Length), out flag2))
				{
					CyclopsDockingModUI.CfgSignBackgroundVisible = flag2;
					CyclopsDockingModUI.CfgSignBackgroundVisibleOrig = flag2;
					Logger.Log("INFO: Loaded defaultBackground: " + (flag2 ? "true" : "false"));
					return;
				}
				Logger.Log("WARNING: Bad value « " + str.Substring("defaultBackground=".Length) + " » for sign's background visibility (wrong boolean string value). Default value will be used.");
				return;
			}
			else if (str.StartsWith("ladderTintColor="))
			{
				int num5;
				if (str.Length <= "ladderTintColor=".Length || str.Length > 10 + "ladderTintColor=".Length || !int.TryParse(str.Substring("ladderTintColor=".Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out num5))
				{
					Logger.Log("WARNING: Bad value « " + str.Substring("ladderTintColor=".Length) + " » for ladder's tint color (must be between 0 and 9). Default value will be used.");
					return;
				}
				if (num5 >= 0 && num5 <= 9)
				{
					CyclopsDockingModUI.CfgLadderTintColorVal = num5;
					CyclopsDockingModUI.CfgLadderTintColor = CyclopsDockingModUI.CfgLadderTintColors[num5];
					CyclopsDockingModUI.CfgLadderTintColorOrig = CyclopsDockingModUI.CfgLadderTintColor;
					Logger.Log("INFO: Loaded ladderTintColor: " + CyclopsDockingModUI.CfgLadderTintColor);
					return;
				}
				Logger.Log("WARNING: Bad value « " + num5.ToString(CultureInfo.InvariantCulture) + " » for ladder's tint color (must be between 0 and 9). Default value will be used.");
				return;
			}
			else if (str.StartsWith("basePieceName="))
			{
				if (str.Length > "basePieceName=".Length && str.Length <= 40 + "basePieceName=".Length)
				{
					CyclopsHatchConnector.CyclopsHatchConnectorName = str.Substring("basePieceName=".Length);
					Logger.Log("INFO: Loaded basePieceName: « " + CyclopsHatchConnector.CyclopsHatchConnectorName + " »");
					return;
				}
				Logger.Log("WARNING: Bad value « " + str.Substring("basePieceName=".Length) + " » for basePieceName (must be between 1 and 40 characters maximum). Default value will be used.");
				return;
			}
			else if (str.StartsWith("basePieceDescription="))
			{
				if (str.Length > "basePieceDescription=".Length && str.Length <= 200 + "basePieceDescription=".Length)
				{
					CyclopsHatchConnector.CyclopsHatchConnectorDescription = str.Substring("basePieceDescription=".Length);
					Logger.Log("INFO: Loaded basePieceDescription: « " + CyclopsHatchConnector.CyclopsHatchConnectorDescription + " »");
					return;
				}
				Logger.Log("WARNING: Bad value « " + str.Substring("basePieceName=".Length) + " » for basePieceName (must be between 1 and 200 characters maximum). Default value will be used.");
				return;
			}
			else if (str.StartsWith("basePieceRecipe="))
			{
				if (str.Length <= "basePieceRecipe=".Length || str.Length > 200 + "basePieceRecipe=".Length)
				{
					Logger.Log("WARNING: Bad value « " + str.Substring("basePieceRecipe=".Length) + " » for basePieceRecipe (must be between 1 and 200 characters maximum). Default value will be used.");
					return;
				}
				List<TechType> list = ConfigOptions.ParseIngredients(str.Substring("basePieceRecipe=".Length));
				if (list != null && list.Count > 0)
				{
					CyclopsHatchConnector.ResourceMap = list;
					string text = "";
					foreach (TechType techType in list)
					{
						text = text + ((text.Length > 0) ? "," : "") + techType.AsString(false);
					}
					Logger.Log("INFO: Loaded basePieceRecipe: " + text);
					return;
				}
				Logger.Log("WARNING: Bad recipe provided for Cyclops docking base piece. Default recipe will be used.");
				return;
			}
			else if (str.StartsWith("enableAutoPilotFeature="))
			{
				if (str.Length <= "enableAutoPilotFeature=".Length || str.Length > 20 + "enableAutoPilotFeature=".Length)
				{
					Logger.Log("WARNING: Bad value « " + str.Substring("enableAutoPilotFeature=".Length) + " » for auto-pilot feature toggle (must be between 1 and 20 characters maximum). Default value will be used.");
					return;
				}
				bool flag3;
				if (bool.TryParse(str.Substring("enableAutoPilotFeature=".Length), out flag3))
				{
					ConfigOptions.EnableAutopilotFeature = flag3;
					Logger.Log("INFO: Loaded enableAutoPilotFeature: " + (flag3 ? "true" : "false"));
					return;
				}
				Logger.Log("WARNING: Bad value « " + str.Substring("enableAutoPilotFeature=".Length) + " » for auto-pilot feature toggle (wrong boolean string value). Default value will be used.");
				return;
			}
			else
			{
				if (!str.StartsWith("simpleDockingManeuver="))
				{
					foreach (KeyValuePair<string, int> keyValuePair in ConfigOptions.autoPilotLang)
					{
						if (!string.IsNullOrEmpty(keyValuePair.Key) && str.StartsWith(keyValuePair.Key))
						{
							if (str.Length <= keyValuePair.Key.Length || str.Length > keyValuePair.Value + keyValuePair.Key.Length)
							{
								Logger.Log(string.Concat(new string[]
								{
									"WARNING: Bad value « ",
									str.Substring(keyValuePair.Key.Length),
									" » for ",
									keyValuePair.Key.TrimEnd(new char[] { '=' }),
									" (must be between 1 and ",
									keyValuePair.Value.ToString(CultureInfo.InvariantCulture),
									" characters maximum). Default value will be used."
								}));
								break;
							}
							string text2 = str.Substring(keyValuePair.Key.Length);
							Logger.Log(string.Concat(new string[]
							{
								"INFO: Loaded ",
								keyValuePair.Key.Replace('=', ':'),
								" « ",
								text2,
								" »"
							}));
							if (keyValuePair.Key == "noRouteSelected=")
							{
								AutoPilot.Lbl_NoRouteSelected = text2;
								break;
							}
							if (keyValuePair.Key == "createNewRoute=")
							{
								AutoPilot.Lbl_CreateNewRoute = text2;
								break;
							}
							if (keyValuePair.Key == "defaultRouteName=")
							{
								AutoPilot.Lbl_DefaultRouteName = text2;
								break;
							}
							if (keyValuePair.Key == "btnSelectionTooltip=")
							{
								AutoPilot.Lbl_BtnSelection_Tooltip = text2;
								break;
							}
							if (keyValuePair.Key == "btnRenameRouteTooltip=")
							{
								AutoPilot.Lbl_BtnRenameRoute_Tooltip = text2;
								break;
							}
							if (keyValuePair.Key == "btnStartRecordTooltip=")
							{
								AutoPilot.Lbl_BtnAutoPilot_RecordTooltip = text2;
								break;
							}
							if (keyValuePair.Key == "btnStartAutoPilotTooltip=")
							{
								AutoPilot.Lbl_BtnAutoPilot_StartTooltip = text2;
								break;
							}
							if (keyValuePair.Key == "btnStopAutoPilotTooltip=")
							{
								AutoPilot.Lbl_BtnAutoPilot_StopTooltip = text2;
								break;
							}
							if (keyValuePair.Key == "btnRemoveRouteTooltip=")
							{
								AutoPilot.Lbl_BtnRemoveRoute_Tooltip = text2;
								break;
							}
							if (keyValuePair.Key == "btnConfirmRouteRemovalTooltip=")
							{
								AutoPilot.Lbl_BtnConfirmRemove_Tooltip = text2;
								break;
							}
							if (keyValuePair.Key == "btnCancelRouteRemovalTooltip=")
							{
								AutoPilot.Lbl_BtnCancelRemove_Tooltip = text2;
								break;
							}
							if (keyValuePair.Key == "selectRouteNotFound=")
							{
								AutoPilot.Lbl_SelectRouteNotFound = text2;
								break;
							}
							if (keyValuePair.Key == "cannotRenameWhileRecording=")
							{
								AutoPilot.Lbl_CantRenameWhileRecording = text2;
								break;
							}
							if (keyValuePair.Key == "cannotChangeRouteWhilePiloting=")
							{
								AutoPilot.Lbl_CantChangeRouteWhilePlaying = text2;
								break;
							}
							if (keyValuePair.Key == "cannotChangeRouteWhileRecording=")
							{
								AutoPilot.Lbl_CantChangeRouteWhileRecording = text2;
								break;
							}
							if (keyValuePair.Key == "cannotChangeRouteWhileUndocked=")
							{
								AutoPilot.Lbl_CantChangeRouteWhileUndocked = text2;
								break;
							}
							if (keyValuePair.Key == "cannotRemoveRouteWhilePiloting=")
							{
								AutoPilot.Lbl_CantRemoveWhilePlaying = text2;
								break;
							}
							if (keyValuePair.Key == "cannotRemoveRouteWhileRecording=")
							{
								AutoPilot.Lbl_CantRemoveWhileRecording = text2;
								break;
							}
							if (keyValuePair.Key == "cannotRenameWhileRemoving=")
							{
								AutoPilot.Lbl_CantRenameWhileRemoving = text2;
								break;
							}
							if (keyValuePair.Key == "cannotChangeRouteWhileRemoving=")
							{
								AutoPilot.Lbl_CantChangeRouteWhileRemoving = text2;
								break;
							}
							if (keyValuePair.Key == "cannotStartAutoPilotWhileRemoving=")
							{
								AutoPilot.Lbl_CantStartPilotingWhileRemoving = text2;
								break;
							}
							if (keyValuePair.Key == "autoPilotStart=")
							{
								AutoPilot.Lbl_AutoPilotStart = text2;
								break;
							}
							if (keyValuePair.Key == "autoPilotStop=")
							{
								AutoPilot.Lbl_AutoPilotStop = text2;
								break;
							}
							if (keyValuePair.Key == "recordingRouteStart=")
							{
								AutoPilot.Lbl_RecordingNewRoute = text2;
								break;
							}
							if (keyValuePair.Key == "recordingRouteStop=")
							{
								AutoPilot.Lbl_RecordingStopped = text2;
								break;
							}
							if (keyValuePair.Key == "recordingRouteCancelled=")
							{
								AutoPilot.Lbl_RecordingCancelled = text2;
								break;
							}
							if (keyValuePair.Key == "reachedRouteEnd=")
							{
								AutoPilot.Lbl_ReachedRouteEnd = text2;
								break;
							}
							break;
						}
					}
					return;
				}
				if (str.Length <= "simpleDockingManeuver=".Length || str.Length > 20 + "simpleDockingManeuver=".Length)
				{
					Logger.Log("WARNING: Bad value « " + str.Substring("simpleDockingManeuver=".Length) + " » for simple docking maneuver toggle (must be between 1 and 20 characters maximum). Default value will be used.");
					return;
				}
				bool flag4;
				if (bool.TryParse(str.Substring("simpleDockingManeuver=".Length), out flag4))
				{
					CyclopsDockingModUI.CfgSimpleDockingManeuver = flag4;
					CyclopsDockingModUI.CfgSimpleDockingManeuverOrig = flag4;
					SubControlFixer.SimpleDocking = flag4;
					Logger.Log("INFO: Loaded simpleDockingManeuver: " + (flag4 ? "true" : "false"));
					return;
				}
				Logger.Log("WARNING: Bad value « " + str.Substring("simpleDockingManeuver=".Length) + " » for simple docking maneuver toggle (wrong boolean string value). Default value will be used.");
				return;
			}
		}

		private static List<TechType> ParseIngredients(string str)
		{
			if (!string.IsNullOrWhiteSpace(str))
			{
				string[] array = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				if (array != null)
				{
					List<TechType> list = new List<TechType>();
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						TechType techType;
						if (UWE.Utils.TryParseEnum<TechType>(array2[i], out techType) && techType != TechType.None)
							list.Add(techType);
					}
					if (list.Count <= 0)
						return null;
					return list;
				}
			}
			return null;
		}

		public static void LoadConfig()
		{
			if (ConfigOptions.ConfigFilePath == null)
				ConfigOptions.ConfigFilePath = FilesHelper.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config.txt");
			if (ConfigOptions.ConfigFilePath == null)
			{
				Logger.Log("WARNING: Cannot find configuration file path. Default settings will be used.");
				return;
			}
			if (!File.Exists(ConfigOptions.ConfigFilePath))
			{
				Logger.Log("WARNING: Cannot find configuration at \"" + ConfigOptions.ConfigFilePath + "\". Default settings will be used.");
				return;
			}
			string text = File.ReadAllText(ConfigOptions.ConfigFilePath, Encoding.UTF8);
			if (text == null)
			{
				Logger.Log("WARNING: Could not read configuration file at \"" + ConfigOptions.ConfigFilePath + "\". Default settings will be used.");
				return;
			}
			string[] array = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (array != null)
			{
				Logger.Log("INFO: Loading configuration from \"" + ConfigOptions.ConfigFilePath + "\"...");
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
					ConfigOptions.ProcessConfigLine(array2[i]);
				return;
			}
			Logger.Log("WARNING: Configuration file at \"" + ConfigOptions.ConfigFilePath + "\" is empty. Default settings will be used.");
		}

		public static void UpdateConfigFile(string oldStr, string newStr)
		{
			if (!string.IsNullOrEmpty(oldStr) && !string.IsNullOrEmpty(newStr) && !string.IsNullOrEmpty(ConfigOptions.ConfigFilePath) && File.Exists(ConfigOptions.ConfigFilePath))
			{
				try
				{
					string text = File.ReadAllText(ConfigOptions.ConfigFilePath, Encoding.UTF8);
					if (!string.IsNullOrEmpty(text))
					{
						string text2 = null;
						if (oldStr.EndsWith("=" + Environment.NewLine))
						{
							string text3 = oldStr.Substring(0, oldStr.Length - 2);
							int num = text.IndexOf(text3);
							if (num > 0)
							{
								int num2 = text.IndexOf(Environment.NewLine, num + text3.Length);
								if (num2 > num && num2 < text.Length)
								{
									string text4 = text.Substring(num, num2 - num) + Environment.NewLine;
									Logger.Log(string.Concat(new string[]
									{
										"INFO: Replacing configuration [",
										text4.Replace(Environment.NewLine, ""),
										"] by [",
										newStr.Replace(Environment.NewLine, ""),
										"]."
									}));
									text2 = text.Replace(text4, newStr);
								}
							}
						}
						else
							text2 = text.Replace(oldStr, newStr);
						if (text2 != null)
							File.WriteAllText(ConfigOptions.ConfigFilePath, text2, Encoding.UTF8);
					}
				}
				catch
				{
					string text5 = "An error happened while updating config file at \"" + ConfigOptions.ConfigFilePath + "\"!";
					Logger.Log("ERROR: " + text5);
					ErrorMessage.AddDebug(text5);
				}
			}
		}
	}
}
