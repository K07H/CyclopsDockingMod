using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CyclopsDockingMod.Controllers;
using CyclopsDockingMod.Fixers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyclopsDockingMod.Routing
{
	internal static class AutoPilot
    {
        private static readonly Sprite _helm1 = AssetsHelper.Assets.LoadAsset<Sprite>("helmicon1");

        private static readonly Sprite _helm2 = AssetsHelper.Assets.LoadAsset<Sprite>("helmicon2");

        private static readonly Sprite _bg1 = AssetsHelper.Assets.LoadAsset<Sprite>("helmbg1");

        private static readonly Sprite _bg2 = AssetsHelper.Assets.LoadAsset<Sprite>("helmbg2");

        private static readonly Sprite _route1 = AssetsHelper.Assets.LoadAsset<Sprite>("routeiconlr1");

        private static readonly Sprite _route2 = AssetsHelper.Assets.LoadAsset<Sprite>("routeiconlr2");

        private static readonly Sprite _edit1 = AssetsHelper.Assets.LoadAsset<Sprite>("editrouteicon1");

        private static readonly Sprite _edit2 = AssetsHelper.Assets.LoadAsset<Sprite>("editrouteicon2");

        private static readonly Sprite _remove1 = AssetsHelper.Assets.LoadAsset<Sprite>("removerouteicon1");

        private static readonly Sprite _remove2 = AssetsHelper.Assets.LoadAsset<Sprite>("removerouteicon2");

        private static readonly Sprite _confirm1 = AssetsHelper.Assets.LoadAsset<Sprite>("confirmrouteicon1");

        private static readonly Sprite _confirm2 = AssetsHelper.Assets.LoadAsset<Sprite>("confirmrouteicon2");

        internal static bool KeepDrivingWhenEjected = true;

        internal static bool KeepDrivingOnPartialLOD = true;

        internal static bool IsRecording = false;

        internal static bool UndockStartRecord = false;

        internal static bool InRemoveConfirm = false;

        internal static readonly Dictionary<string, SubRoutePlaying> SubsPlayingRoutes = new Dictionary<string, SubRoutePlaying>();

        internal static readonly List<Route> Routes = new List<Route>();

        internal static string Lbl_NoRouteSelected = "No route selected";

        internal static string Lbl_CreateNewRoute = "New route";

        internal static string Lbl_DefaultRouteName = "Route {0}";

        internal static string Lbl_BtnSelection_Tooltip = "Change selection";

        internal static string Lbl_BtnRenameRoute_Tooltip = "Rename route";

        internal static string Lbl_BtnAutoPilot_RecordTooltip = "Start recording route";

        internal static string Lbl_BtnAutoPilot_StartTooltip = "Start auto-pilot";

        internal static string Lbl_BtnAutoPilot_StopTooltip = "Stop auto-pilot";

        internal static string Lbl_BtnRemoveRoute_Tooltip = "Remove route";

        internal static string Lbl_BtnConfirmRemove_Tooltip = "Confirm route removal";

        internal static string Lbl_BtnCancelRemove_Tooltip = "Cancel route removal";

        internal static string Lbl_SelectRouteNotFound = "Route not found: Please select another.";

        internal static string Lbl_CantRenameWhileRecording = "Cannot rename route while recording.";

        internal static string Lbl_CantChangeRouteWhilePlaying = "Cannot change route while auto-piloting.";

        internal static string Lbl_CantChangeRouteWhileRecording = "Cannot change route while recording.";

        internal static string Lbl_CantChangeRouteWhileUndocked = "Cyclops needs to be docked.";

        internal static string Lbl_CantRemoveWhileRecording = "Cannot remove route while recording.";

        internal static string Lbl_CantRemoveWhilePlaying = "Cannot remove route while auto-piloting.";

        internal static string Lbl_CantRenameWhileRemoving = "Cannot rename route (pending route removal).";

        internal static string Lbl_CantChangeRouteWhileRemoving = "Cannot change route (pending route removal).";

        internal static string Lbl_CantStartPilotingWhileRemoving = "Cannot start auto-pilot (pending route removal).";

        internal static string Lbl_AutoPilotStop = "Auto-pilot stop: {0}";

        internal static string Lbl_AutoPilotStart = "Auto-pilot start: {0}";

        internal static string Lbl_RecordingNewRoute = "Recording route...";

        internal static string Lbl_RecordingStopped = "Recording stopped: {0} created.";

        internal static string Lbl_RecordingCancelled = "Recording cancelled.";

        internal static string Lbl_ReachedRouteEnd = "Reached route end.";

        internal static string Lbl_WaitUndockCompletion = "Pending undock maneuver, please wait.";

        internal static string Lbl_ForceEject = "Docking started 20sec ago: Forcing eject.";

        internal static string Lbl_ForceUndock = "Eject asked 3 times during docking: Forcing undock.";

        private static GameObject AddHudButton(GameObject original, Transform parent, Vector3 localPos, string objName, string tooltip, Sprite sprite, Sprite alternate, bool enableAnim = false, bool small = false)
		{
			CyclopsEngineChangeState component = original.GetComponent<CyclopsEngineChangeState>();
			if (component != null)
			{
				CyclopsEngineChangeStateFixer._spinSpeedField.SetValue(component, 0f);
				component.screwIcon.localEulerAngles = new Vector3(component.screwIcon.localEulerAngles.x, component.screwIcon.localEulerAngles.y, 0f);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, parent, true);
			gameObject.name = objName;
			gameObject.transform.localPosition = localPos;
			Image component2 = gameObject.GetComponent<Image>();
			if (component2 != null)
			{
				component2.sprite = AutoPilot._bg2;
				component2.overrideSprite = AutoPilot._bg1;
			}
			Image component3 = gameObject.transform.Find("EngineOn_Icon").GetComponent<Image>();
			if (component3 != null)
			{
				component3.sprite = sprite;
				component3.overrideSprite = alternate;
			}
			if (small)
				gameObject.transform.localScale *= 0.7f;
			gameObject.SetActive(true);
			gameObject.GetComponent<Animator>().enabled = enableAnim;
			CyclopsEngineChangeStateFixer._tooltipTextField.SetValue(gameObject.GetComponent<CyclopsEngineChangeState>(), tooltip);
			return gameObject;
		}

		private static void AddHudLabel(GameObject original, Transform parent, Vector3 relativeLocalPos, Vector3 angles, string str)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, parent, true);
			gameObject.name = "CyclopsRouteLabel";
			foreach (object obj in gameObject.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.name == "DepthText")
				{
					TMP_Text component = transform.GetComponent<TMP_Text>();
					float num = component.fontSize - 2f;
					MyInputField myInputField = transform.gameObject.AddComponent<MyInputField>();
					myInputField.textComponent = component;
					myInputField.inputType = TMP_InputField.InputType.Standard;
					myInputField.interactable = true;
					myInputField.characterLimit = 40;
					myInputField.characterValidation = TMP_InputField.CharacterValidation.None;
					myInputField.contentType = TMP_InputField.ContentType.Standard;
					myInputField.lineType = TMP_InputField.LineType.MultiLineSubmit;
					myInputField.readOnly = false;
					myInputField.uppercase = false;
					myInputField.text = str;
					myInputField.textComponent.text = str;
					myInputField.textComponent.overflowMode = TextOverflowModes.Overflow;
					myInputField.textComponent.alignment = TextAlignmentOptions.MidlineRight;
					myInputField.textComponent.fontSize = num;
					myInputField.enabled = true;
					break;
				}
			}
			gameObject.transform.eulerAngles = angles;
			gameObject.transform.localPosition += relativeLocalPos;
			gameObject.SetActive(true);
		}

		private static void RestoreBtn(Transform engineOnUI, Transform centerHUD)
		{
			if (engineOnUI == null || centerHUD == null)
				return;
			GameObject gameObject = null;
			foreach (object obj in engineOnUI)
			{
				Transform transform = (Transform)obj;
				if (transform.name.StartsWith("EngineOff_Button"))
					gameObject = transform.gameObject;
				else if (transform.name.StartsWith("CyclopsAutoPilotBtn"))
					return;
			}
			if (gameObject == null)
				return;
			GameObject gameObject2 = null;
			foreach (object obj2 in centerHUD)
			{
				Transform transform2 = (Transform)obj2;
				if (transform2.name.StartsWith("DepthStatus"))
					gameObject2 = transform2.gameObject;
				else if (transform2.name.StartsWith("CyclopsRouteLabel"))
					return;
			}
			if (gameObject2 == null)
				return;
			AutoPilot.AddHudButton(gameObject, engineOnUI, new Vector3(-20f, 760f, 0.004157424f), "CyclopsAutoPilotBtn", "CyclopsAutoPilotOn", AutoPilot._helm1, AutoPilot._helm2, true, false);
			AutoPilot.AddHudButton(gameObject, engineOnUI, new Vector3(-200f, 760f, 0.004157424f), "CyclopsRouteSelectBtn", "CyclopsSelectRoute", AutoPilot._route2, AutoPilot._route1, false, false);
			AutoPilot.AddHudLabel(gameObject2, engineOnUI, new Vector3(-1780f, -400f, 260f), gameObject.transform.eulerAngles, AutoPilot.Lbl_NoRouteSelected);
			GameObject gameObject3 = AutoPilot.AddHudButton(gameObject, engineOnUI, new Vector3(-230f, 900f, 0.004157424f), "CyclopsRouteEditBtn", "CyclopsEditRoute", AutoPilot._edit2, AutoPilot._edit1, false, true);
			GameObject gameObject4 = AutoPilot.AddHudButton(gameObject, engineOnUI, new Vector3(-115f, 900f, 0.004157424f), "CyclopsRouteRemoveBtn", "CyclopsRemoveRoute", AutoPilot._remove2, AutoPilot._remove1, false, true);
			GameObject gameObject5 = AutoPilot.AddHudButton(gameObject, engineOnUI, new Vector3(0f, 900f, 0.004157424f), "CyclopsConfirmRemoveBtn", "CyclopsConfirmRemove", AutoPilot._confirm2, AutoPilot._confirm1, false, true);
			gameObject3.SetActive(false);
			gameObject4.SetActive(false);
			gameObject5.SetActive(false);
		}

		private static void ShowHud(Transform cyclopsHud, bool show = true)
		{
			if (cyclopsHud != null)
			{
				foreach (object obj in cyclopsHud)
				{
					Transform transform = (Transform)obj;
					if (transform.name.StartsWith("CyclopsAutoPilotBtn"))
						transform.gameObject.SetActive(show);
					else if (transform.name.StartsWith("CyclopsRouteSelectBtn"))
						transform.gameObject.SetActive(show);
					else if (transform.name.StartsWith("CyclopsRouteLabel"))
						transform.gameObject.SetActive(show);
				}
			}
		}

		internal static void RefreshHud(SubRoot sr, bool docked = true)
		{
			Transform transform = sr.gameObject.transform.Find("HelmHUD");
			if (transform != null)
			{
				Transform transform2 = transform.Find("HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI");
				Transform transform3 = transform.Find("HelmHUDVisuals/Canvas_CenterHUD");
				AutoPilot.RestoreBtn(transform2, transform3);
				AutoPilot.ShowHud(transform2, docked);
			}
		}

		private static int GetNextRouteId()
		{
			int num = 0;
			foreach (Route route in AutoPilot.Routes)
				if (num <= route.Id)
					num = route.Id + 1;
			return num;
		}

		private static float GetRouteLength(List<Vector3> wayPoints)
		{
			float num = 0f;
			if (wayPoints.Count > 1)
			{
				Vector3? vector = null;
				foreach (Vector3 vector2 in wayPoints)
				{
					if (vector != null)
						num += Vector3.Distance(vector.Value, vector2);
					vector = new Vector3?(vector2);
				}
			}
			return num;
		}

		private static Route AddRoute(List<Vector3> wayPoints, string name = null, int speed = 2, Vector3? basePartPosStt = null, Vector3? basePartPosEnd = null)
		{
			int nextRouteId = AutoPilot.GetNextRouteId();
			string text = ((!string.IsNullOrWhiteSpace(name)) ? name : string.Format(AutoPilot.Lbl_DefaultRouteName, (nextRouteId + 1).ToString()));
			Route route = new Route(nextRouteId, text, wayPoints, AutoPilot.GetRouteLength(wayPoints), speed, basePartPosStt, basePartPosEnd);
			AutoPilot.Routes.Add(route);
			return route;
		}

		internal static void SelectRoute(string pid, int rid, Transform cyclopsHud, string str)
		{
			if (AutoPilot.SubsPlayingRoutes.ContainsKey(pid))
				AutoPilot.SubsPlayingRoutes[pid].SelectedRoute = rid;
			else
			{
				AutoPilot.SubsPlayingRoutes.Add(pid, new SubRoutePlaying
				{
					IsPlayingRoute = false,
					UndockStartPlaying = false,
					SelectedRoute = rid
				});
			}
			if (cyclopsHud != null)
			{
				foreach (object obj in cyclopsHud)
				{
					Transform transform = (Transform)obj;
					if (transform.name.StartsWith("CyclopsRouteEditBtn") || transform.name.StartsWith("CyclopsRouteRemoveBtn"))
						transform.gameObject.SetActive(rid >= 0);
					else if (transform.name.StartsWith("CyclopsRouteLabel"))
						foreach (object obj2 in transform.transform)
						{
							Transform transform2 = (Transform)obj2;
							if (transform2.name == "DepthText")
							{
								MyInputField component = transform2.GetComponent<MyInputField>();
								if (component != null)
									component.text = str;
								Text component2 = transform2.GetComponent<Text>();
								if (component2 != null)
								{
									component2.text = str;
									break;
								}
								break;
							}
						}
				}
			}
		}

		internal static void StopPlayingRoute(string pid, Transform cyclopsHud = null, bool reachedEnd = true, bool endsOnNothing = false)
		{
			if (reachedEnd)
			{
				AutoPilot.SelectRoute(pid, -1, cyclopsHud, AutoPilot.Lbl_NoRouteSelected);
				if (endsOnNothing)
					AutoPilot.ShowHud(cyclopsHud, false);
			}
			if (AutoPilot.SubsPlayingRoutes.ContainsKey(pid))
			{
				AutoPilot.SubsPlayingRoutes[pid].LastTime = -1f;
				AutoPilot.SubsPlayingRoutes[pid].LastPos = null;
				AutoPilot.SubsPlayingRoutes[pid].LastAngle = null;
				AutoPilot.SubsPlayingRoutes[pid].IsStuck = false;
				AutoPilot.SubsPlayingRoutes[pid].AlternateUnstuck = true;
				AutoPilot.SubsPlayingRoutes[pid].AlternateUnstuckBis = false;
				AutoPilot.SubsPlayingRoutes[pid].IsPlayingRoute = false;
			}
		}

		private static void StopSubPlayingRoute(string pid)
		{
			SubRoot subRoot = BaseFixer.GetSubRoot(pid);
			if (subRoot != null)
				AutoPilot.StopPlayingRoute(pid, subRoot.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI"), true, true);
		}

		internal static void StopSubsPlayingRoute(Vector3 pos, bool isRouteEndPoint)
		{
			foreach (KeyValuePair<string, SubRoutePlaying> keyValuePair in AutoPilot.SubsPlayingRoutes)
			{
				if (isRouteEndPoint)
				{
					SubRoutePlaying value = keyValuePair.Value;
					bool flag;
					if (value == null)
						flag = false;
					else
					{
						Route playing = value.Playing;
						Vector3? vector = ((playing != null) ? playing.BasePartPosEnd : null);
						flag = vector != null;
					}
					if (flag && keyValuePair.Value.Playing.BasePartPosEnd != null && FastHelper.IsNear(keyValuePair.Value.Playing.BasePartPosEnd.Value, pos) && keyValuePair.Value.IsPlayingRoute)
						AutoPilot.StopSubPlayingRoute(keyValuePair.Key);
				}
				else
				{
					SubRoutePlaying value2 = keyValuePair.Value;
					bool flag2;
					if (value2 == null)
						flag2 = false;
					else
					{
						Route playing2 = value2.Playing;
						Vector3? vector = ((playing2 != null) ? playing2.BasePartPosStt : null);
						flag2 = vector != null;
					}
					if (flag2 && keyValuePair.Value.Playing.BasePartPosStt != null && FastHelper.IsNear(keyValuePair.Value.Playing.BasePartPosStt.Value, pos) && keyValuePair.Value.IsPlayingRoute)
						AutoPilot.StopSubPlayingRoute(keyValuePair.Key);
				}
			}
		}

		internal static void StartPlayingRoute(string pid)
		{
			if (AutoPilot.SubsPlayingRoutes.ContainsKey(pid))
			{
				int selectedRoute = AutoPilot.SubsPlayingRoutes[pid].SelectedRoute;
				foreach (Route route in AutoPilot.Routes)
				{
					if (route.Id == selectedRoute)
					{
						AutoPilot.SubsPlayingRoutes[pid].CurrentRouteIndex = 0;
						AutoPilot.SubsPlayingRoutes[pid].Playing = route;
						AutoPilot.SubsPlayingRoutes[pid].IsPlayingRoute = true;
						break;
					}
				}
			}
		}

		internal static void ComputeUnstuck(SubRoutePlaying route, Vector3 pos, Vector3 subDirAngles)
		{
			if (route.LastPos == null)
			{
				route.LastPos = new Vector3?(pos);
				route.LastAngle = new float?(subDirAngles.y);
				route.LastTime = Time.time;
				return;
			}
			if (!route.IsStuck && Time.time > route.LastTime + 3f)
			{
				float num = FastHelper.AngleDiff(subDirAngles.y, route.LastAngle.Value);
				if (FastHelper.IsNear(route.LastPos.Value, pos) && num > -1f && num < 1f)
				{
					route.IsStuck = true;
					route.AlternateUnstuck = !route.AlternateUnstuck;
					if (route.AlternateUnstuck)
						route.AlternateUnstuckBis = !route.AlternateUnstuckBis;
				}
				route.LastTime = Time.time;
				route.LastPos = null;
				route.LastAngle = null;
				return;
			}
			if (route.IsStuck && Time.time > route.LastTime + 6f)
			{
				route.IsStuck = false;
				route.LastTime = Time.time;
				route.LastPos = null;
				route.LastAngle = null;
			}
		}

		internal static void StartRecording(SubControl subCtrl, string routeName = null, int routeSpeed = 2, BasePart bp = null)
		{
			if (AutoPilot.IsRecording)
				return;
			RecordRouteController recordRouteController = subCtrl.gameObject.GetComponent<RecordRouteController>();
			if (recordRouteController == null)
				recordRouteController = subCtrl.gameObject.AddComponent<RecordRouteController>();
			if (recordRouteController != null)
			{
				ErrorMessage.AddDebug(AutoPilot.Lbl_RecordingNewRoute);
				recordRouteController.Reset();
				recordRouteController.RouteName = routeName;
				recordRouteController.RouteSpeed = routeSpeed;
				if (bp != null)
					recordRouteController.BasePartPosStt = new Vector3?(bp.position);
				AutoPilot.IsRecording = true;
				recordRouteController.IsRecording = true;
			}
		}

		internal static void StopRecording(SubControl subCtrl, BasePart bp = null, bool saveRoute = true)
		{
			if (!AutoPilot.IsRecording && !AutoPilot.UndockStartRecord)
				return;
			RecordRouteController component = subCtrl.gameObject.GetComponent<RecordRouteController>();
			if (component != null)
			{
				component.IsRecording = false;
				AutoPilot.IsRecording = false;
				if (bp != null)
					component.BasePartPosEnd = new Vector3?(bp.position);
				if (saveRoute)
				{
					Route route = AutoPilot.AddRoute(component.RecordedWayPoints, component.RouteName, component.RouteSpeed, component.BasePartPosStt, component.BasePartPosEnd);
					ErrorMessage.AddDebug(string.Format(AutoPilot.Lbl_RecordingStopped, route.Name));
				}
				else
					ErrorMessage.AddDebug(AutoPilot.Lbl_RecordingCancelled);
				Transform transform = subCtrl.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI");
				AutoPilot.SelectRoute(subCtrl.GetComponent<PrefabIdentifier>().Id, -1, transform, AutoPilot.Lbl_NoRouteSelected);
				if (bp == null)
					AutoPilot.ShowHud(transform, false);
			}
		}

		internal static void LoadRoutes(string saveGame)
		{
			AutoPilot.Routes.Clear();
			string saveFolderPath = FilesHelper.GetSaveFolderPath(saveGame);
			if (saveFolderPath != null)
			{
				if (!Directory.Exists(saveFolderPath))
				{
					Logger.Message("No save directory found for Cyclops auto-pilot routes at \"" + saveFolderPath + "\".");
					return;
				}
				string text = FilesHelper.Combine(saveFolderPath, "routes.txt");
				if (File.Exists(text))
				{
					Logger.Message("Loading Cyclops auto-pilot routes from \"" + text + "\".");
					string[] array;
					try
					{
						array = File.ReadAllLines(text, Encoding.UTF8);
					}
					catch (Exception ex)
					{
						Logger.Error("Exception caught while loading Cyclops auto-pilot routes. Exception=[" + ex.ToString() + "]");
						return;
					}
					if (array != null && array.Length != 0)
					{
						foreach (string text2 in array)
							if (text2.Length > 10 && text2.Contains("/"))
							{
								Route route = Route.Deserialize(text2);
								if (route != null)
									AutoPilot.Routes.Add(route);
							}
					}
					Logger.Message("Cyclops auto-pilot routes loaded. Player made {0} custom routes.", new object[] { AutoPilot.Routes.Count });
					return;
				}
				Logger.Message("No Cyclops auto-pilot routes saved at \"" + text + "\".");
				return;
			}
			else
				Logger.Message("Could not find save slot for Cyclops auto-pilot routes.");
		}

		internal static void SaveRoutes()
		{
			string text = "";
			foreach (Route route in AutoPilot.Routes)
			{
				text = text + route.Serialize() + Environment.NewLine;
			}
			if (!string.IsNullOrEmpty(text))
			{
				string saveFolderPath = FilesHelper.GetSaveFolderPath();
				if (saveFolderPath.Contains("/test/"))
				{
					Logger.Error("Unable to find save folder path at [" + saveFolderPath + "].");
					return;
				}
				if (!Directory.Exists(saveFolderPath))
				{
					try
					{
						Directory.CreateDirectory(saveFolderPath);
					}
					catch (Exception ex)
					{
						Logger.Error($"Exception caught while creating folder at [{saveFolderPath}]. Exception=[{ex.ToString()}]");
					}
				}
				if (!Directory.Exists(saveFolderPath))
				{
					Logger.Error("Unable to create save folder at [" + saveFolderPath + "].");
					return;
				}
				string text2 = FilesHelper.Combine(saveFolderPath, "routes.txt");
				Logger.Message("Saving {0} Cyclops auto-pilot routes to \"{1}\".", new object[]
				{
					AutoPilot.Routes.Count,
					text2
				});
				try
				{
					File.WriteAllText(text2, text, Encoding.UTF8);
				}
				catch (Exception ex2)
				{
					Logger.Error($"Exception caught while saving Cyclops auto-pilot routes at [{text2}]. Exception=[{ex2.ToString()}]");
				}
			}
		}
	}
}
