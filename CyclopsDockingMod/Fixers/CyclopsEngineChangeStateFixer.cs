using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CyclopsDockingMod.Routing;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CyclopsDockingMod.Fixers
{
	internal static class CyclopsEngineChangeStateFixer
    {
        internal static readonly FieldInfo _tooltipTextField = typeof(CyclopsEngineChangeState).GetField("tooltipText", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _invalidButtonField = typeof(CyclopsEngineChangeState).GetField("invalidButton", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _mouseHoverField = typeof(CyclopsEngineChangeState).GetField("mouseHover", BindingFlags.Instance | BindingFlags.NonPublic);

        internal static readonly FieldInfo _spinSpeedField = typeof(CyclopsEngineChangeState).GetField("spinSpeed", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool OnMouseEnter_Prefix(CyclopsEngineChangeState __instance)
		{
			if ((bool)CyclopsEngineChangeStateFixer._invalidButtonField.GetValue(__instance))
				return true;
			if (Player.main.currentSub != __instance.subRoot)
				return true;
			string text = (string)CyclopsEngineChangeStateFixer._tooltipTextField.GetValue(__instance);
			if (text != null)
			{
				if (text.StartsWith("CyclopsAutoPilot"))
                {
                    bool isPlayingRoute = false;
                    bool isCreatingRoute = false;
                    string pid = __instance.subRoot.GetComponent<PrefabIdentifier>()?.Id;
                    if (pid != null && AutoPilot.SubsPlayingRoutes != null && AutoPilot.SubsPlayingRoutes.ContainsKey(pid))
                    {
                        isPlayingRoute = AutoPilot.SubsPlayingRoutes[pid].IsPlayingRoute;
                        isCreatingRoute = AutoPilot.SubsPlayingRoutes[pid].SelectedRoute == -2;
                    }
					CyclopsEngineChangeStateFixer._tooltipTextField.SetValue(__instance, isPlayingRoute ? "CyclopsAutoPilotOff" : (isCreatingRoute ? "CyclopsAutoPilotCreate" : "CyclopsAutoPilotOn"));
					CyclopsEngineChangeStateFixer._mouseHoverField.SetValue(__instance, true);
					return false;
				}
				else if (text.StartsWith("CyclopsSelectRoute") || text.StartsWith("CyclopsEditRoute") || text.StartsWith("CyclopsRemoveRoute") || text.StartsWith("CyclopsConfirmRemove") || text.StartsWith("CyclopsCancelRemove"))
				{
					CyclopsEngineChangeStateFixer._mouseHoverField.SetValue(__instance, true);
					return false;
				}
			}
			return true;
		}

		private static string RegularTooltip(string tooltipText)
		{
			if (tooltipText.StartsWith("CyclopsSelectRoute"))
				return AutoPilot.Lbl_BtnSelection_Tooltip;
			if (tooltipText.StartsWith("CyclopsEditRoute"))
				return AutoPilot.Lbl_BtnRenameRoute_Tooltip;
			if (tooltipText.StartsWith("CyclopsRemoveRoute"))
				return AutoPilot.Lbl_BtnRemoveRoute_Tooltip;
			if (tooltipText.StartsWith("CyclopsConfirmRemove"))
				return AutoPilot.Lbl_BtnConfirmRemove_Tooltip;
			if (tooltipText.StartsWith("CyclopsCancelRemove"))
				return AutoPilot.Lbl_BtnCancelRemove_Tooltip;
			return null;
		}

		public static bool Update_Prefix(CyclopsEngineChangeState __instance)
		{
			string text = (string)CyclopsEngineChangeStateFixer._tooltipTextField.GetValue(__instance);
			if (text != null)
			{
				if (text.StartsWith("CyclopsAutoPilot"))
				{
					if ((bool)CyclopsEngineChangeStateFixer._mouseHoverField.GetValue(__instance))
					{
						if (text == "CyclopsAutoPilotCreate")
						{
                            if (AutoPilot.IsRecording)
#if SUBNAUTICA_EXP
                                HandReticle.main.SetText(HandReticle.TextType.Hand, AutoPilot.Lbl_BtnAutoPilot_StopRecordTooltip, false, GameInput.Button.LeftHand);
#else
                                HandReticle.main.SetInteractText(AutoPilot.Lbl_BtnAutoPilot_RecordTooltip, false, HandReticle.Hand.Left);
#endif
                            else
#if SUBNAUTICA_EXP
                                HandReticle.main.SetText(HandReticle.TextType.Hand, AutoPilot.Lbl_BtnAutoPilot_RecordTooltip, false, GameInput.Button.LeftHand);
#else
                                HandReticle.main.SetInteractText(AutoPilot.Lbl_BtnAutoPilot_RecordTooltip, false, HandReticle.Hand.Left);
#endif
                        }
						else if (text == "CyclopsAutoPilotOn")
#if SUBNAUTICA_EXP
							HandReticle.main.SetText(HandReticle.TextType.Hand, AutoPilot.Lbl_BtnAutoPilot_StartTooltip, false, GameInput.Button.LeftHand);
#else
                            HandReticle.main.SetInteractText(AutoPilot.Lbl_BtnAutoPilot_StartTooltip, false, HandReticle.Hand.Left);
#endif
						else
#if SUBNAUTICA_EXP
							HandReticle.main.SetText(HandReticle.TextType.Hand, AutoPilot.Lbl_BtnAutoPilot_StopTooltip, false, GameInput.Button.LeftHand);
#else
                            HandReticle.main.SetInteractText(AutoPilot.Lbl_BtnAutoPilot_StopTooltip, false, HandReticle.Hand.Left);
#endif
					}
					string pid = __instance.subRoot.GetComponent<PrefabIdentifier>()?.Id;
					float num = (((pid != null && AutoPilot.SubsPlayingRoutes.ContainsKey(pid) && (AutoPilot.SubsPlayingRoutes[pid].IsPlayingRoute || AutoPilot.SubsPlayingRoutes[pid].UndockStartPlaying)) || AutoPilot.UndockStartRecord || AutoPilot.IsRecording) ? 3f : 0f);
					float num2 = Mathf.MoveTowards((float)CyclopsEngineChangeStateFixer._spinSpeedField.GetValue(__instance), num, Time.deltaTime * 15f);
					CyclopsEngineChangeStateFixer._spinSpeedField.SetValue(__instance, num2);
					__instance.screwIcon.Rotate(new Vector3(0f, 0f, -(num2 * Time.deltaTime * 60f)), Space.Self);
					return false;
				}
				else
				{
					string text2 = CyclopsEngineChangeStateFixer.RegularTooltip(text);
					if (text2 != null)
					{
						if ((bool)CyclopsEngineChangeStateFixer._mouseHoverField.GetValue(__instance))
#if SUBNAUTICA_EXP
                            HandReticle.main.SetText(HandReticle.TextType.Hand, text2, false, GameInput.Button.LeftHand);
#else
                            HandReticle.main.SetInteractText(text2, false, HandReticle.Hand.Left);
#endif
                        return false;
					}
				}
			}
			return true;
		}

		private static void StartEngineAndSetNormalSpeed(CyclopsEngineChangeState engineState)
		{
			Transform parent = engineState.gameObject.transform.parent;
			Transform transform = parent.Find("EngineOff_Button");
			if (transform != null)
			{
				CyclopsEngineChangeState component = transform.GetComponent<CyclopsEngineChangeState>();
				if (!component.motorMode.engineOn)
				{
					engineState.subRoot.voiceNotificationManager.ClearQueue();
					component.OnClick();
				}
				if (component.motorMode.cyclopsMotorMode != CyclopsMotorMode.CyclopsMotorModes.Standard)
				{
					Transform transform2 = parent.Find("Speed_02");
					if (transform2 == null)
						return;
					CyclopsMotorModeButton component2 = transform2.GetComponent<CyclopsMotorModeButton>();
					if (component2 == null)
						return;
					component2.Invoke("OnClick", 8f);
				}
			}
		}

		private static bool CyclopsAutoPilotClick(CyclopsEngineChangeState engineState, string pid)
		{
			if (AutoPilot.InRemoveConfirm)
			{
				ErrorMessage.AddDebug(AutoPilot.Lbl_CantStartPilotingWhileRemoving);
				return false;
			}
			SubRoutePlaying subRoutePlaying = AutoPilot.SubsPlayingRoutes[pid];
			if (subRoutePlaying.SelectedRoute < 0)
			{
				if (subRoutePlaying.SelectedRoute == -2)
				{
					if (AutoPilot.IsRecording)
						AutoPilot.StopRecording(engineState.subRoot.GetComponent<SubControl>(), null, true);
					else if (AutoPilot.UndockStartRecord)
						AutoPilot.StopRecording(engineState.subRoot.GetComponent<SubControl>(), null, false);
					else
					{
						if (SubControlFixer.Docked(pid))
						{
							AutoPilot.UndockStartRecord = true;
							SubControlFixer.StartUndocking();
						}
						else
							AutoPilot.StartRecording(engineState.subRoot.GetComponent<SubControl>(), null, 2, null);
						CyclopsEngineChangeStateFixer.StartEngineAndSetNormalSpeed(engineState);
					}
				}
				else
					ErrorMessage.AddDebug(AutoPilot.Lbl_NoRouteSelected);
			}
			else
			{
				bool flag = false;
				foreach (Route route in AutoPilot.Routes)
				{
					if (route.Id == subRoutePlaying.SelectedRoute)
					{
						flag = true;
						if (subRoutePlaying.IsPlayingRoute || subRoutePlaying.UndockStartPlaying)
						{
							ErrorMessage.AddDebug(string.Format(AutoPilot.Lbl_AutoPilotStop, route.Name));
							AutoPilot.StopPlayingRoute(pid, engineState.subRoot.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI"), true, true);
						}
						else
						{
							ErrorMessage.AddDebug(string.Format(AutoPilot.Lbl_AutoPilotStart, route.Name));
							if (SubControlFixer.Docked(pid))
							{
								AutoPilot.SubsPlayingRoutes[pid].UndockStartPlaying = true;
								SubControlFixer.StartUndocking();
							}
							else
								AutoPilot.StartPlayingRoute(pid);
							CyclopsEngineChangeStateFixer.StartEngineAndSetNormalSpeed(engineState);
						}
					}
				}
				if (!flag)
					ErrorMessage.AddDebug(AutoPilot.Lbl_SelectRouteNotFound);
			}
			CyclopsEngineChangeStateFixer._invalidButtonField.SetValue(engineState, true);
			engineState.Invoke("ResetInvalidButton", 1f);
			return false;
		}

		private static bool CyclopsSelectRouteClick(CyclopsEngineChangeState engineState, string pid)
		{
			SubRoutePlaying subRoutePlaying = AutoPilot.SubsPlayingRoutes[pid];
			if (subRoutePlaying.IsPlayingRoute || subRoutePlaying.UndockStartPlaying)
			{
				ErrorMessage.AddDebug(AutoPilot.Lbl_CantChangeRouteWhilePlaying);
				return false;
			}
			if (AutoPilot.IsRecording || AutoPilot.UndockStartRecord)
			{
				ErrorMessage.AddDebug(AutoPilot.Lbl_CantChangeRouteWhileRecording);
				return false;
			}
			if (AutoPilot.InRemoveConfirm)
			{
				ErrorMessage.AddDebug(AutoPilot.Lbl_CantChangeRouteWhileRemoving);
				return false;
			}
			if (!SubControlFixer.Docked(pid))
			{
				ErrorMessage.AddDebug(AutoPilot.Lbl_CantChangeRouteWhileUndocked);
				return false;
			}
			List<Route> list = new List<Route> { new Route(-1, AutoPilot.Lbl_NoRouteSelected, null, 0f, 2, null, null) };
			foreach (Route route in AutoPilot.Routes)
				if (route.BasePartPosStt != null && route.BasePartPosStt != null && FastHelper.IsNear(SubControlFixer.DockedSubs[pid].position, route.BasePartPosStt.Value))
					list.Add(route);
			list = list.OrderBy((Route t) => t.Id).ToList<Route>();
			list.Insert(1, new Route(-2, AutoPilot.Lbl_CreateNewRoute, null, 0f, 2, null, null));
			Transform parent = engineState.transform.parent;
			int i = 0;
			while (i < list.Count)
			{
				if (list[i].Id == subRoutePlaying.SelectedRoute)
				{
					if (i + 1 < list.Count)
					{
						AutoPilot.SelectRoute(pid, list[i + 1].Id, parent, list[i + 1].Name);
						break;
					}
					AutoPilot.SelectRoute(pid, list[0].Id, parent, list[0].Name);
					break;
				}
				else
					i++;
			}
			if (i >= list.Count)
				AutoPilot.SelectRoute(pid, list[0].Id, parent, list[0].Name);
			return false;
		}

		private static bool RouteExists(int routeId)
		{
			using (List<Route>.Enumerator enumerator = AutoPilot.Routes.GetEnumerator())
			{
				while (enumerator.MoveNext())
					if (enumerator.Current.Id == routeId)
						return true;
			}
			return false;
		}

		private static bool CyclopsEditRoute(CyclopsEngineChangeState engineState, string pid)
		{
			if (AutoPilot.IsRecording || AutoPilot.UndockStartRecord)
			{
				ErrorMessage.AddDebug(AutoPilot.Lbl_CantRenameWhileRecording);
				return false;
			}
			if (AutoPilot.InRemoveConfirm)
			{
				ErrorMessage.AddDebug(AutoPilot.Lbl_CantRenameWhileRemoving);
				return false;
			}
			Transform transform = engineState.gameObject.transform.parent.Find("CyclopsRouteLabel");
			if (transform != null && AutoPilot.SubsPlayingRoutes[pid].SelectedRoute >= 0)
			{
				if (!CyclopsEngineChangeStateFixer.RouteExists(AutoPilot.SubsPlayingRoutes[pid].SelectedRoute))
				{
					ErrorMessage.AddDebug(AutoPilot.Lbl_SelectRouteNotFound);
					return false;
				}
				foreach (object obj in transform)
				{
					Transform transform2 = (Transform)obj;
					if (transform2.name == "DepthText")
					{
						MyInputField component = transform2.gameObject.GetComponent<MyInputField>();
						if (component != null)
						{
							component.OnPointerClick(new PointerEventData(EventSystem.current));
							break;
						}
						break;
					}
				}
			}
			return false;
		}

		private static bool CyclopsRemoveRoute(CyclopsEngineChangeState engineState, string pid)
		{
			if (AutoPilot.IsRecording || AutoPilot.UndockStartRecord)
			{
				ErrorMessage.AddDebug(AutoPilot.Lbl_CantRemoveWhileRecording);
				return false;
			}
			SubRoutePlaying subRoutePlaying = AutoPilot.SubsPlayingRoutes[pid];
			if (subRoutePlaying.IsPlayingRoute || subRoutePlaying.UndockStartPlaying)
			{
				ErrorMessage.AddDebug(AutoPilot.Lbl_CantRemoveWhilePlaying);
				return false;
			}
			if (!AutoPilot.InRemoveConfirm && subRoutePlaying.SelectedRoute >= 0)
			{
				if (!CyclopsEngineChangeStateFixer.RouteExists(subRoutePlaying.SelectedRoute))
				{
					ErrorMessage.AddDebug(AutoPilot.Lbl_SelectRouteNotFound);
					return false;
				}
				Transform transform = engineState.transform.parent.Find("CyclopsConfirmRemoveBtn");
				if (transform != null)
				{
					transform.gameObject.SetActive(true);
					CyclopsEngineChangeStateFixer._tooltipTextField.SetValue(engineState, "CyclopsCancelRemove");
					AutoPilot.InRemoveConfirm = true;
				}
			}
			return false;
		}

		private static bool CyclopsConfirmRemoveRoute(CyclopsEngineChangeState engineState, string pid, bool confirmed)
		{
			if (AutoPilot.InRemoveConfirm)
			{
				Transform parent = engineState.transform.parent;
				Transform transform = parent.Find("CyclopsConfirmRemoveBtn");
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
					CyclopsEngineChangeStateFixer._tooltipTextField.SetValue(engineState, "CyclopsRemoveRoute");
					AutoPilot.InRemoveConfirm = false;
				}
				if (confirmed)
				{
					SubRoutePlaying route = AutoPilot.SubsPlayingRoutes[pid];
					if (route != null && route.SelectedRoute >= 0)
					{
						if (!CyclopsEngineChangeStateFixer.RouteExists(route.SelectedRoute))
							ErrorMessage.AddDebug(AutoPilot.Lbl_SelectRouteNotFound);
						else
							AutoPilot.Routes.RemoveAll((Route r) => r.Id == route.SelectedRoute);
					}
					AutoPilot.SelectRoute(pid, -1, parent, AutoPilot.Lbl_NoRouteSelected);
				}
			}
			return false;
		}

		public static bool OnClick_Prefix(CyclopsEngineChangeState __instance)
		{
			if ((bool)CyclopsEngineChangeStateFixer._invalidButtonField.GetValue(__instance))
				return true;
			if (Player.main.currentSub != __instance.subRoot)
				return true;
			string text = (string)CyclopsEngineChangeStateFixer._tooltipTextField.GetValue(__instance);
			if (text != null)
			{
				PrefabIdentifier component = __instance.subRoot.GetComponent<PrefabIdentifier>();
				if (component == null)
					return false;
				if (text.StartsWith("CyclopsAutoPilot"))
					return CyclopsEngineChangeStateFixer.CyclopsAutoPilotClick(__instance, component.Id);
				if (text.StartsWith("CyclopsSelectRoute"))
					return CyclopsEngineChangeStateFixer.CyclopsSelectRouteClick(__instance, component.Id);
				if (text.StartsWith("CyclopsEditRoute"))
					return CyclopsEngineChangeStateFixer.CyclopsEditRoute(__instance, component.Id);
				if (text.StartsWith("CyclopsRemoveRoute"))
					return CyclopsEngineChangeStateFixer.CyclopsRemoveRoute(__instance, component.Id);
				if (text.StartsWith("CyclopsConfirmRemove"))
					return CyclopsEngineChangeStateFixer.CyclopsConfirmRemoveRoute(__instance, component.Id, true);
				if (text.StartsWith("CyclopsCancelRemove"))
					return CyclopsEngineChangeStateFixer.CyclopsConfirmRemoveRoute(__instance, component.Id, false);
			}
			return true;
		}
	}
}
