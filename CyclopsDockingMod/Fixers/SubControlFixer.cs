using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CyclopsDockingMod.Routing;
using UnityEngine;

namespace CyclopsDockingMod.Fixers
{
	public static class SubControlFixer
    {
        private static readonly FieldInfo _sub = typeof(SubControl).GetField("sub", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _throttle = typeof(SubControl).GetField("throttle", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _lastTimeThrottled = typeof(SubControl).GetField("lastTimeThrottled", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _canAccel = typeof(SubControl).GetField("canAccel", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _throttleHandlers = typeof(SubControl).GetField("throttleHandlers", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _hatchOpen = typeof(CyclopsEntryHatch).GetField("hatchOpen", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _EngineStartCameraShake = typeof(CyclopsEngineChangeState).GetMethod("EngineStartCameraShake", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FMODAsset _cyclopsDockDoorsClose = AssetsHelper.CreateAsset("2093", "6abdf213-0d28-412a-b0c7-a622c0a03912", "event:/sub/cyclops/docking_doors_close");

        private static readonly FMODAsset _cyclopsDockDoorsOpen = AssetsHelper.CreateAsset("2094", "c39c942e-793e-47c7-ab74-6175420abf25", "event:/sub/cyclops/docking_doors_open");

        private static readonly FMODAsset _cyclopsOuterHatchClose = AssetsHelper.CreateAsset("2105", "96e0fc8b-d997-4149-aa09-7ebc4bf7188c", "event:/sub/cyclops/outer_hatch_close");

        private static readonly FMODAsset _cyclopsOuterHatchOpen = AssetsHelper.CreateAsset("2106", "94631097-1268-47d8-bbce-b40a44221bc0", "event:/sub/cyclops/outer_hatch_open");

        private static readonly Vector3 UndockingSpeedSlow = new Vector3(0f, 0f, -0.2f);

        private static readonly Vector3 UndockingSpeedFast = new Vector3(0f, 0f, -0.7f);

        public static bool SimpleDocking = false;

        public static float AutoDockingTriggerSqrRange = 169f;

        public static float AutoDockingUndockSqrRange = Mathf.Pow(Mathf.Sqrt(SubControlFixer.AutoDockingTriggerSqrRange) + 5f, 2f);

        public static float AutoDockingDetectSqrRange = Mathf.Pow(Mathf.Sqrt(SubControlFixer.AutoDockingUndockSqrRange) + 1f, 2f);

        public static KeyCode ManualDockingKey = KeyCode.O;

        public static bool AutoDocking = true;

        private static bool RPoint = false;

        private static bool LRotation = false;

        private static bool CLRotation = false;

        public static float DockingStt = -1f;

        private static float PDocking = -1f;

        private static float PDockingBis = -1f;

        private static float PAnim = -1f;

        private static bool Undocking = false;

        private static bool ManualDocking = false;

        private static int PUndock = 0;

        private static int UndockingCnt = 0;

        private static float PUndocking = -1f;

        public static readonly Dictionary<string, BasePart> DockedSubs = new Dictionary<string, BasePart>();

        private static float AutoDockingGetAwaySqrRange
		{
			get => (!SubControlFixer.SimpleDocking ? 40f : 4f);
		}

		private static float PointRange
		{
			get => (!SubControlFixer.SimpleDocking ? 3f : 1f);
		}

		private static void Reset()
		{
			SubControlFixer.DockingStt = -1f;
			SubControlFixer.PAnim = -1f;
			SubControlFixer.UndockingCnt = 0;
			SubControlFixer.LRotation = false;
			SubControlFixer.RPoint = false;
			SubControlFixer.CLRotation = false;
			SubControlFixer.PDocking = -1f;
			SubControlFixer.PDockingBis = -1f;
			SubControlFixer.Undocking = false;
			SubControlFixer.ManualDocking = false;
			SubControlFixer.PUndocking = -1f;
			SubControlFixer.PUndock = 0;
		}

		private static float VerticalSpeed(float heightDiff)
		{
			if (heightDiff < 0.5f)
				return 0.01f;
			if (heightDiff < 1f)
				return 0.02f;
			if (heightDiff < 3f)
				return 0.05f;
			if (heightDiff < 6f)
				return 0.1f;
			if (heightDiff < 10f)
				return 0.3f;
			return 0.4f;
		}

		private static float GetAwaySpeed(float sqrDist)
		{
			if (sqrDist < 1f)
				return -0.01f;
			if (sqrDist < 4f)
				return -0.02f;
			if (sqrDist < 9f)
				return -0.04f;
			if (sqrDist < 16f)
				return -0.06f;
			if (sqrDist < 25f)
				return -0.08f;
			return -0.1f;
		}

		private static float AutoPilotForwardSpeed(float sqrMag)
		{
			if (sqrMag < 1f)
				return 0.05f;
			if (sqrMag < 2f)
				return 0.1f;
			if (sqrMag < 4f)
				return 0.2f;
			if (sqrMag < 9f)
				return 0.3f;
			if (sqrMag < 25f)
				return 0.5f;
			if (sqrMag < 100f)
				return 0.7f;
			return 0.9f;
		}

		private static float NegRot(float angleDiff)
		{
			if (angleDiff < -60f)
				return -0.7f;
			if (angleDiff < -20f)
				return -0.6f;
			if (angleDiff < -10f)
				return -0.5f;
			if (angleDiff < -5f)
				return -0.4f;
			if (angleDiff < -2f)
				return -0.2f;
			if (angleDiff < -1f)
				return -0.05f;
			return -0.015f;
		}

		private static float PosRot(float angleDiff)
		{
			if (angleDiff > 60f)
				return 0.7f;
			if (angleDiff > 20f)
				return 0.6f;
			if (angleDiff > 10f)
				return 0.5f;
			if (angleDiff > 5f)
				return 0.4f;
			if (angleDiff > 2f)
				return 0.2f;
			if (angleDiff > 1f)
				return 0.05f;
			return 0.015f;
		}

		public static bool Docked(string id) => (SubControlFixer.DockedSubs.ContainsKey(id) && SubControlFixer.DockedSubs[id] != null);

		private static KeyValuePair<BasePart, float> GetClosestDockingHatch(Vector3 pos, bool free = false)
		{
			BasePart basePart = null;
			float num = float.MaxValue;
			foreach (BasePart basePart2 in BaseFixer.BaseParts)
			{
				if (basePart2.type == 0 && (!free || basePart2.dock == null))
				{
					float sqrMagnitude = (pos - (basePart2.position + BasePart.P_CyclopsDockingHatch)).sqrMagnitude;
					if (sqrMagnitude < (SubControlFixer.AutoDocking ? SubControlFixer.AutoDockingDetectSqrRange : 10000f) && (basePart == null || sqrMagnitude < num))
					{
						num = sqrMagnitude;
						basePart = basePart2;
					}
				}
			}
			return new KeyValuePair<BasePart, float>(basePart, num);
		}

		internal static void CleanUp(BasePart bp, string pid, bool cleanBaseParts = false)
		{
			if (!string.IsNullOrEmpty(pid))
			{
				if (cleanBaseParts)
				{
					foreach (BasePart basePart in BaseFixer.BaseParts)
					{
						if (!(pid == basePart.dock))
						{
							SubRoot sub = basePart.sub;
							if (!(pid == ((sub != null) ? sub.GetComponent<PrefabIdentifier>().Id : null)))
								continue;
						}
						basePart.dock = null;
						basePart.sub = null;
						basePart.trapOpened = false;
					}
				}
				SubControlFixer.DockedSubs[pid] = null;
			}
			if (bp != null)
			{
				bp.dock = null;
				bp.sub = null;
				bp.trapOpened = false;
			}
		}

		private static void UndockCleanup(BasePart bp, string pid, bool cleanBaseParts = false)
		{
			if (bp.signGo != null)
			{
				Sign component = bp.signGo.GetComponent<Sign>();
				if (component != null)
				{
					component.text = ConfigOptions.LblNoCyclopsDocked;
					component.signInput.text = ConfigOptions.LblNoCyclopsDocked;
				}
			}
			bp.PlayDockingAnim(CyclopsHatchConnector.CyclopsDockingAnim.UNDOCKED);
			SubControlFixer.CleanUp(bp, pid, cleanBaseParts);
			SubControlFixer.Reset();
		}

		private static void OpenTrap(CyclopsEntryHatch entry, bool playSfx)
		{
			if (!(bool)SubControlFixer._hatchOpen.GetValue(entry))
			{
				if (playSfx && Player.main.IsUnderwater())
					entry.openSFX.Play();
				SubControlFixer._hatchOpen.SetValue(entry, true);
			}
		}

		private static void CloseTrap(CyclopsEntryHatch entry, bool playSfx)
		{
			if ((bool)SubControlFixer._hatchOpen.GetValue(entry))
			{
				if (playSfx && Player.main.IsUnderwater())
					entry.closeSFX.Play();
				SubControlFixer._hatchOpen.SetValue(entry, false);
			}
		}

		public static void ToggleTrap(SubRoot sub, bool open, bool playSfx)
		{
			GameObject gameObject = sub.gameObject.FindChild("EntryHatch");
			if (gameObject != null)
			{
				CyclopsEntryHatch component = gameObject.GetComponent<CyclopsEntryHatch>();
				if (component != null && SubControlFixer._hatchOpen != null)
				{
					if (open)
					{
						SubControlFixer.OpenTrap(component, playSfx);
						return;
					}
					SubControlFixer.CloseTrap(component, playSfx);
				}
			}
		}

		private static void Throttle(SubControl subCtrl, Vector3 throttle, bool canAccel, string pid, bool autoPilot = true, bool canEject = true, BasePart bp = null)
		{
			try
			{
				subCtrl.appliedThrottle = false;
				SubControlFixer._throttle.SetValue(subCtrl, throttle);
				float magnitude = throttle.magnitude;
				if (canAccel && (double)magnitude > 0.0001)
				{
					float num = magnitude * subCtrl.cyclopsMotorMode.GetPowerConsumption() * Time.deltaTime / ((SubRoot)SubControlFixer._sub.GetValue(subCtrl)).GetPowerRating();
					float num2;
					if (!GameModeUtils.RequiresPower() || subCtrl.powerRelay.ConsumeEnergy(num, out num2))
					{
						SubControlFixer._lastTimeThrottled.SetValue(subCtrl, Time.time);
						subCtrl.appliedThrottle = true;
					}
				}
				if (subCtrl.appliedThrottle && canAccel)
				{
					float num3 = 0.33f;
					if (subCtrl.useThrottleIndex == 1)
						num3 = 0.66f;
					if (subCtrl.useThrottleIndex == 2)
						num3 = 1f;
					subCtrl.engineRPMManager.AccelerateInput(num3);
					int num4 = ((ISubThrottleHandler[])SubControlFixer._throttleHandlers.GetValue(subCtrl)).Length;
					for (int i = 0; i < num4; i++)
					{
						((ISubThrottleHandler[])SubControlFixer._throttleHandlers.GetValue(subCtrl))[i].OnSubAppliedThrottle();
					}
					if ((float)SubControlFixer._lastTimeThrottled.GetValue(subCtrl) < Time.time - 5f)
					{
						Utils.PlayFMODAsset(subCtrl.engineStartSound, MainCamera.camera.transform, 20f);
					}
				}
				if (AvatarInputHandler.main.IsEnabled())
				{
					if (GameInput.GetButtonDown(GameInput.Button.RightHand))
						subCtrl.transform.parent.BroadcastMessage("ToggleFloodlights", null, SendMessageOptions.DontRequireReceiver);
					if (GameInput.GetButtonDown(GameInput.Button.Exit))
					{
						if (autoPilot)
							SubControlFixer.HandleExit();
						else
							SubControlFixer.HandleExit(canEject, pid, bp);
					}
				}
				if (!subCtrl.appliedThrottle)
					SubControlFixer._throttle.SetValue(subCtrl, Vector3.zero);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception caught in Throttle. Ex=[" + ex.ToString() + "]");
			}
		}

		private static void HandleExit(bool canEject, string pid, BasePart bp)
		{
			if (canEject && !SubControlFixer.Undocking)
			{
				SubControlFixer.StartUndocking();
				return;
			}
			if (SubControlFixer.DockingStt > 0f)
			{
				if (Time.time > SubControlFixer.DockingStt + 20f)
				{
					ErrorMessage.AddDebug(AutoPilot.Lbl_ForceEject);
					SubControlFixer.UndockCleanup(bp, pid, true);
					Player.main.TryEject();
					return;
				}
				if (SubControlFixer.Undocking)
				{
					ErrorMessage.AddDebug(AutoPilot.Lbl_WaitUndockCompletion);
					return;
				}
				if (SubControlFixer.UndockingCnt < 2)
				{
					SubControlFixer.UndockingCnt++;
					return;
				}
				ErrorMessage.AddDebug(AutoPilot.Lbl_ForceUndock);
				SubControlFixer.StartUndocking();
			}
		}

		private static void HandleExit()
		{
			Player.main.TryEject();
		}

		private static Vector3 UndockingAnimate(float dSqrMag, CyclopsEngineChangeState engine, BasePart bp)
		{
			if (SubControlFixer.PUndocking > 0f)
			{
				if (SubControlFixer.PUndock == 0)
				{
					engine.StartCoroutine((IEnumerator)SubControlFixer._EngineStartCameraShake.Invoke(engine, new object[] { 0.2f, 1f, 0f }));
					engine.StartCoroutine((IEnumerator)SubControlFixer._EngineStartCameraShake.Invoke(engine, new object[] { 0.2f, -1f, 1.1f }));
					FMODUWE.PlayOneShot(SubControlFixer._cyclopsDockDoorsClose, MainCamera.camera.transform.position, 1f);
					bp.PlayDockingAnim(CyclopsHatchConnector.CyclopsDockingAnim.UNDOCKING);
					SubControlFixer.PUndock++;
				}
				else if (SubControlFixer.PUndock == 1 && Time.time > SubControlFixer.PUndocking + 3f)
				{
					FMODUWE.PlayOneShot(SubControlFixer._cyclopsDockDoorsOpen, MainCamera.camera.transform.position, 1f);
					engine.StartCoroutine((IEnumerator)SubControlFixer._EngineStartCameraShake.Invoke(engine, new object[] { 0.05f, 3f, 0f }));
					engine.StartCoroutine((IEnumerator)SubControlFixer._EngineStartCameraShake.Invoke(engine, new object[] { 0.05f, -1f, 3.1f }));
					SubControlFixer.PUndock++;
				}
				else if (SubControlFixer.PUndock == 2 && Time.time > SubControlFixer.PUndocking + 8f)
				{
					FMODUWE.PlayOneShot(SubControlFixer._cyclopsOuterHatchClose, MainCamera.camera.transform.position, 1f);
					SubControlFixer.PUndock++;
				}
			}
			if (dSqrMag >= 25f)
				return SubControlFixer.UndockingSpeedFast;
			return SubControlFixer.UndockingSpeedSlow;
		}

		private static bool EndUndocking(SubControl subCtrl, BasePart bp, string pid)
		{
			SubRoot subRoot;
			if ((subRoot = bp.sub) == null && (subRoot = subCtrl.gameObject.GetComponent<SubRoot>()) == null)
			{
				subRoot = Player.main.GetCurrentSub() ?? BaseFixer.GetSubRoot(bp.dock);
			}
			SubRoot subRoot2 = subRoot;
			if (subRoot2 != null)
			{
				SubControlFixer.ToggleTrap(subRoot2, false, true);
				bp.trapOpened = false;
			}
			SubControlFixer.UndockCleanup(bp, pid, false);
			if (AutoPilot.UndockStartRecord)
			{
				AutoPilot.UndockStartRecord = false;
				AutoPilot.StartRecording(subCtrl, null, 2, bp);
			}
			if (AutoPilot.SubsPlayingRoutes.ContainsKey(pid) && AutoPilot.SubsPlayingRoutes[pid].UndockStartPlaying)
			{
				AutoPilot.SubsPlayingRoutes[pid].UndockStartPlaying = false;
				AutoPilot.StartPlayingRoute(pid);
			}
			return true;
		}

		private static void EndAnimate(SubControl subCtrl, BasePart bp, string id, CyclopsEngineChangeState engine)
		{
			SubRoot subRoot = subCtrl.gameObject.GetComponent<SubRoot>() ?? Player.main.GetCurrentSub();
			SubControlFixer.Reset();
			bp.dock = id;
			bp.sub = subRoot;
			SubControlFixer.DockedSubs[id] = bp;
			subRoot.voiceNotificationManager.ClearQueue();
			engine.OnClick();
			if (subRoot != null)
			{
				if (bp.signGo != null)
				{
					Sign component = bp.signGo.GetComponent<Sign>();
					if (component != null)
					{
						string text = string.Format(ConfigOptions.LblCyclopsDocked, BaseFixer.GetCyclopsEnergy(subRoot));
						component.text = text;
						component.signInput.text = text;
					}
				}
				SubControlFixer.ToggleTrap(subRoot, true, true);
				bp.trapOpened = true;
				if (ConfigOptions.EnableAutopilotFeature)
					AutoPilot.RefreshHud(subRoot, true);
			}
			if (AutoPilot.IsRecording)
			{
				AutoPilot.StopRecording(subCtrl, bp, true);
			}
		}

		private static Vector3 Compute(SubControl subCtrl, SubRoot sub, Vector3 pos, Vector3 dockingPos, Vector3 subDirAngles, Vector3 dirAngles, float dSqrMag, string id, BasePart bp, CyclopsEngineChangeState engine)
		{
			Vector3 zero = Vector3.zero;
			float num = Math.Abs(dockingPos.y - pos.y);
			if (pos.y > dockingPos.y + ((!SubControlFixer.RPoint) ? 0.05f : 0.1f))
			{
				zero.y = SubControlFixer.VerticalSpeed(num) * -1f;
			}
			else if (pos.y < dockingPos.y - ((!SubControlFixer.RPoint) ? 0.05f : 0.1f))
			{
				zero.y = SubControlFixer.VerticalSpeed(num);
			}
			float num2 = FastHelper.AngleDiff(subDirAngles.y, dirAngles.y);
			if (!SubControlFixer.RPoint)
			{
				bool flag = Mathf.Abs(num2) > 0.1f;
				if (dSqrMag < SubControlFixer.AutoDockingGetAwaySqrRange || flag)
				{
					if (!SubControlFixer.LRotation)
					{
						float num3 = ((flag || SubControlFixer.SimpleDocking) ? (-0.1f) : SubControlFixer.GetAwaySpeed(dSqrMag));
						zero.z = num3;
					}
				}
				else if (!SubControlFixer.CLRotation && !flag)
				{
					SubControlFixer.CLRotation = true;
					SubControlFixer.DockingStt = Time.time;
				}
			}
			Vector3 vector = new Vector3(pos.x, dockingPos.y, pos.z);
			if (!SubControlFixer.RPoint && num2 < -0.05f && (pos.x > dockingPos.x + 0.1f || pos.x < dockingPos.x - 0.1f || pos.z > dockingPos.z + 0.1f || pos.z < dockingPos.z - 0.1f))
				zero.x = SubControlFixer.NegRot(num2) * 0.5f;
			else if (!SubControlFixer.RPoint && num2 > 0.05f && (pos.x > dockingPos.x + 0.1f || pos.x < dockingPos.x - 0.1f || pos.z > dockingPos.z + 0.1f || pos.z < dockingPos.z - 0.1f))
				zero.x = SubControlFixer.PosRot(num2) * 0.5f;
			else if (SubControlFixer.CLRotation && (pos.x > dockingPos.x + ((!SubControlFixer.RPoint) ? SubControlFixer.PointRange : 0.2f) || pos.x < dockingPos.x - ((!SubControlFixer.RPoint) ? SubControlFixer.PointRange : 0.2f) || pos.z > dockingPos.z + ((!SubControlFixer.RPoint) ? SubControlFixer.PointRange : 0.2f) || pos.z < dockingPos.z - ((!SubControlFixer.RPoint) ? SubControlFixer.PointRange : 0.2f)))
			{
				if (!SubControlFixer.LRotation)
					SubControlFixer.LRotation = true;
				else if (SubControlFixer.PDocking > 0f && Time.time > SubControlFixer.PDocking + (SubControlFixer.SimpleDocking ? 2f : 7f))
				{
					SubControlFixer.PDocking = -1f;
					FMODUWE.PlayOneShot(SubControlFixer._cyclopsDockDoorsOpen, MainCamera.camera.transform.position, 1f);
					engine.StartCoroutine((IEnumerator)SubControlFixer._EngineStartCameraShake.Invoke(engine, new object[] { 0.05f, 3f, 0f }));
					engine.StartCoroutine((IEnumerator)SubControlFixer._EngineStartCameraShake.Invoke(engine, new object[] { 0.05f, -1f, 3.1f }));
					bp.PlayDockingAnim(CyclopsHatchConnector.CyclopsDockingAnim.DOCKING);
				}
				else if (SubControlFixer.PDockingBis > 0f && Time.time > SubControlFixer.PDockingBis + (SubControlFixer.SimpleDocking ? 6.5f : 11.5f))
				{
					SubControlFixer.PDockingBis = -1f;
					engine.StartCoroutine((IEnumerator)SubControlFixer._EngineStartCameraShake.Invoke(engine, new object[] { 0.2f, 1f, 0f }));
					engine.StartCoroutine((IEnumerator)SubControlFixer._EngineStartCameraShake.Invoke(engine, new object[] { 0.2f, -1f, 1.1f }));
					FMODUWE.PlayOneShot(SubControlFixer._cyclopsDockDoorsClose, MainCamera.camera.transform.position, 1f);
				}
				if (SubControlFixer.LRotation && SubControlFixer.PAnim > 0f && Time.time > SubControlFixer.PAnim + 1f)
				{
					SubControlFixer.EndAnimate(subCtrl, bp, id, engine);
					bp.PlayDockingAnim(CyclopsHatchConnector.CyclopsDockingAnim.DOCKED);
				}
				else
				{
					zero.z = ((dSqrMag < 0.5f) ? 0.005f : ((dSqrMag < 1f) ? 0.01f : ((dSqrMag < 25f) ? 0.05f : ((dSqrMag < 100f) ? 0.1f : 0.4f)))) * ((Vector3.Dot(sub.subAxis.forward, (dockingPos - vector).normalized) < 0f) ? (-1f) : 1f);
					if (SubControlFixer.SimpleDocking)
						zero.z *= 2f;
				}
			}
			else if (SubControlFixer.CLRotation && SubControlFixer.LRotation)
			{
				if (!SubControlFixer.RPoint)
				{
					SubControlFixer.RPoint = true;
					FMODUWE.PlayOneShot(SubControlFixer._cyclopsOuterHatchOpen, MainCamera.camera.transform.position, 1f);
					SubControlFixer.PDocking = Time.time;
					SubControlFixer.PDockingBis = SubControlFixer.PDocking;
				}
				else if (SubControlFixer.PAnim < 0f)
				{
					if (SubControlFixer.PDocking > 0f || SubControlFixer.PDockingBis > 0f)
					{
						engine.StartCoroutine((IEnumerator)SubControlFixer._EngineStartCameraShake.Invoke(engine, new object[] { 0.2f, 1f, 0f }));
						engine.StartCoroutine((IEnumerator)SubControlFixer._EngineStartCameraShake.Invoke(engine, new object[] { 0.2f, -1f, 1.1f }));
						if (SubControlFixer.PDocking > 0f)
						{
							SubControlFixer.PDocking = -1f;
							FMODUWE.PlayOneShot(SubControlFixer._cyclopsDockDoorsOpen, MainCamera.camera.transform.position, 1f);
						}
						if (SubControlFixer.PDockingBis > 0f)
						{
							SubControlFixer.PDockingBis = -1f;
							FMODUWE.PlayOneShot(SubControlFixer._cyclopsDockDoorsClose, MainCamera.camera.transform.position, 1f);
						}
					}
					SubControlFixer.PAnim = Time.time;
				}
				else if (Time.time > SubControlFixer.PAnim + 1f)
				{
					SubControlFixer.EndAnimate(subCtrl, bp, id, engine);
					bp.PlayDockingAnim(CyclopsHatchConnector.CyclopsDockingAnim.DOCKED);
				}
			}
			return zero;
		}

		public static void StartUndocking()
		{
			SubControlFixer.Undocking = true;
			SubControlFixer.PUndocking = Time.time;
			SubControlFixer.PUndock = 0;
		}

		private static void StabilizerThrottle(Rigidbody rb, SubRoot subRoot, SubControl subCtrl, Vector3 throttle)
		{
			subCtrl.appliedThrottle = false;
			SubControlFixer._throttle.SetValue(subCtrl, throttle);
			float magnitude = throttle.magnitude;
			if ((bool)SubControlFixer._canAccel.GetValue(subCtrl))
			{
				if ((double)magnitude > 0.0001)
				{
					float num = magnitude * subCtrl.cyclopsMotorMode.GetPowerConsumption() * Time.deltaTime / subRoot.GetPowerRating();
					float num2;
					if (!GameModeUtils.RequiresPower() || subCtrl.powerRelay.ConsumeEnergy(num, out num2))
					{
						SubControlFixer._lastTimeThrottled.SetValue(subCtrl, Time.time);
						subCtrl.appliedThrottle = true;
					}
				}
				if (subCtrl.appliedThrottle)
				{
					float num3 = 0.33f;
					if (subCtrl.useThrottleIndex == 1)
						num3 = 0.66f;
					if (subCtrl.useThrottleIndex == 2)
						num3 = 1f;
					subCtrl.engineRPMManager.AccelerateInput(num3);
					int num4 = ((ISubThrottleHandler[])SubControlFixer._throttleHandlers.GetValue(subCtrl)).Length;
					for (int i = 0; i < num4; i++)
						((ISubThrottleHandler[])SubControlFixer._throttleHandlers.GetValue(subCtrl))[i].OnSubAppliedThrottle();
					if ((float)SubControlFixer._lastTimeThrottled.GetValue(subCtrl) < Time.time - 5f)
						Utils.PlayFMODAsset(subCtrl.engineStartSound, rb.transform, 30f);
				}
			}
			if (!subCtrl.appliedThrottle)
				SubControlFixer._throttle.SetValue(subCtrl, Vector3.zero);
		}

		private static KeyValuePair<int, Vector3> AutoPilotCompute(Rigidbody rb, SubRoot sub, Vector3 pos, SubRoutePlaying route)
		{
			int num = 0;
			Vector3 zero = Vector3.zero;
			Vector3? vector = null;
			bool flag = route.Playing.WayPoints == null || route.CurrentRouteIndex >= route.Playing.WayPoints.Count;
			if (flag)
			{
				if (route.Playing.BasePartPosEnd != null)
					vector = route.Playing.BasePartPosEnd;
			}
			else
				vector = ((route.Playing.WayPoints != null) ? new Vector3?(route.Playing.WayPoints[route.CurrentRouteIndex]) : null);
			if (vector != null)
			{
				Vector3 eulerAngles = Quaternion.LookRotation(sub.subAxis.forward, sub.subAxis.up).eulerAngles;
				AutoPilot.ComputeUnstuck(route, pos, eulerAngles);
				if (rb != null && !rb.isKinematic)
				{
					Vector3 vector2 = rb.transform.position + rb.transform.up;
					Vector3 vector3 = rb.transform.position + Vector3.up;
					Vector3 vector4 = 10f * (vector3 - vector2);
					rb.AddForceAtPosition(vector4, vector2, ForceMode.Acceleration);
					vector2 = rb.transform.position - rb.transform.up;
					vector3 = rb.transform.position - Vector3.up;
					vector4 = 10f * (vector3 - vector2);
					rb.AddForceAtPosition(vector4, vector2, ForceMode.Acceleration);
				}
				float sqrMagnitude = (vector.Value - pos).sqrMagnitude;
				Vector3 vector5 = new Vector3(pos.x, vector.Value.y, pos.z);
				Vector3 vector6 = vector.Value - vector5;
				float sqrMagnitude2 = vector6.sqrMagnitude;
				Vector3 eulerAngles2 = Quaternion.LookRotation(vector6).eulerAngles;
				float num2 = FastHelper.AngleDiff(eulerAngles.y, eulerAngles2.y);
				float num3 = Vector3.Dot(sub.subAxis.forward, vector6.normalized);
				if (num3 >= 0f && num2 < -3f)
				{
					zero.x = SubControlFixer.NegRot(num2) * 0.8f;
					zero.z = ((sqrMagnitude2 < 1f) ? 0.05f : ((sqrMagnitude2 < 9f) ? 0.1f : ((sqrMagnitude2 < 25f) ? 0.2f : 0.4f))) * ((num3 < 0f) ? (-1f) : 1f);
				}
				else if (num3 >= 0f && num2 > 3f)
				{
					zero.x = SubControlFixer.PosRot(num2) * 0.8f;
					zero.z = ((sqrMagnitude2 < 1f) ? 0.05f : ((sqrMagnitude2 < 9f) ? 0.1f : ((sqrMagnitude2 < 25f) ? 0.2f : 0.4f))) * ((num3 < 0f) ? (-1f) : 1f);
				}
				else
					zero.z = SubControlFixer.AutoPilotForwardSpeed(sqrMagnitude2) * ((num3 < 0f) ? (-1f) : 1f);
				float y = vector.Value.y;
				float num4 = Math.Abs(y - pos.y);
				if (pos.y > y + 0.1f)
					zero.y = SubControlFixer.VerticalSpeed(num4) * -1f;
				else if (pos.y < y - 0.1f)
					zero.y = SubControlFixer.VerticalSpeed(num4);
				if (sqrMagnitude < 9f || zero == Vector3.zero || (flag && sqrMagnitude < SubControlFixer.AutoDockingTriggerSqrRange - 4f))
				{
					if (flag)
					{
						ErrorMessage.AddDebug(AutoPilot.Lbl_ReachedRouteEnd);
						num = 1;
					}
					else
						route.CurrentRouteIndex++;
				}
			}
			else
			{
				ErrorMessage.AddDebug(AutoPilot.Lbl_ReachedRouteEnd);
				num = 2;
			}
			return new KeyValuePair<int, Vector3>(num, (num == 0 && route.IsStuck) ? new Vector3(zero.x, route.AlternateUnstuck ? 0.1f : (-0.1f), route.AlternateUnstuckBis ? 0.1f : (-0.1f)) : zero);
		}

		private static bool RouteEndFreeForManualDocking(string pid, Vector3 pos)
		{
			if (!SubControlFixer.ManualDocking && !SubControlFixer.Docked(pid) && AutoPilot.SubsPlayingRoutes[pid].Playing.BasePartPosEnd != null)
			{
				KeyValuePair<BasePart, float> closestDockingHatch = SubControlFixer.GetClosestDockingHatch(pos, true);
				if (closestDockingHatch.Key != null && closestDockingHatch.Value < 10000f && FastHelper.IsNear(AutoPilot.SubsPlayingRoutes[pid].Playing.BasePartPosEnd.Value, closestDockingHatch.Key.position))
					return true;
			}
			return false;
		}

		private static bool Play(SubControl subCtrl, SubRoot subRoot, string pid, Vector3 pos, bool canAccel, SubRoutePlaying route)
		{
			KeyValuePair<int, Vector3> keyValuePair = SubControlFixer.AutoPilotCompute(subRoot.GetComponent<Rigidbody>(), subRoot, pos, route);
			if (keyValuePair.Key > 0)
			{
				bool flag = keyValuePair.Key == 1;
				if (flag && !SubControlFixer.AutoDocking)
				{
					if (SubControlFixer.RouteEndFreeForManualDocking(pid, pos))
						SubControlFixer.ManualDocking = true;
					else
						flag = false;
				}
				AutoPilot.StopPlayingRoute(pid, subCtrl.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI"), true, false);
				return !flag;
			}
			SubControlFixer.Throttle(subCtrl, keyValuePair.Value, canAccel, pid, true, true, null);
			return false;
		}

		internal static bool StabilizePlay(Rigidbody rb, SubRoot subRoot, string pid, Vector3 pos, SubRoutePlaying route)
		{
			KeyValuePair<int, Vector3> keyValuePair = SubControlFixer.AutoPilotCompute(rb, subRoot, pos, route);
			SubControl component = subRoot.GetComponent<SubControl>();
			if (keyValuePair.Key > 0 || keyValuePair.Value == Vector3.zero)
			{
				AutoPilot.StopPlayingRoute(pid, component.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI"), true, false);
				return true;
			}
			SubControlFixer.StabilizerThrottle(rb, subRoot, component, keyValuePair.Value);
			return false;
		}

		public static bool Update_Prefix(SubControl __instance)
		{
			bool flag = !__instance.LOD.IsFull();
			if (flag && !AutoPilot.KeepDrivingOnPartialLOD)
				return true;
			bool flag2 = (bool)SubControlFixer._canAccel.GetValue(__instance);
			if (!flag2)
				return true;
			SubRoot subRoot = (SubRoot)SubControlFixer._sub.GetValue(__instance);
			if (subRoot == null)
				return true;
			PrefabIdentifier component = subRoot.GetComponent<PrefabIdentifier>();
			if (component == null)
				return true;
			if (__instance.controlMode != SubControl.Mode.DirectInput)
				return !AutoPilot.KeepDrivingWhenEjected || !AutoPilot.SubsPlayingRoutes.ContainsKey(component.Id) || !AutoPilot.SubsPlayingRoutes[component.Id].IsPlayingRoute;
			if (flag)
				return true;
			if (__instance.transform == null)
				return true;
			Transform transform = __instance.transform.Find("CyclopsMeshAnimated/submarine_outer_hatch_01");
			if (transform == null)
				return true;
			Vector3 position = transform.position;
			if (AutoPilot.SubsPlayingRoutes.ContainsKey(component.Id))
			{
				SubRoutePlaying subRoutePlaying = AutoPilot.SubsPlayingRoutes[component.Id];
				if (subRoutePlaying.IsPlayingRoute)
					return SubControlFixer.Play(__instance, subRoot, component.Id, position, flag2, subRoutePlaying);
			}
			bool flag3 = SubControlFixer.Docked(component.Id);
			KeyValuePair<BasePart, float> keyValuePair;
			if (!SubControlFixer.Undocking && !flag3)
				keyValuePair = SubControlFixer.GetClosestDockingHatch(position, true);
			else
				keyValuePair = SubControlFixer.GetClosestDockingHatch(position, false);
			if (keyValuePair.Key != null)
			{
				bool flag4 = SubControlFixer.AutoDocking && keyValuePair.Value < SubControlFixer.AutoDockingTriggerSqrRange;
				bool flag5 = !SubControlFixer.AutoDocking && keyValuePair.Value < 10000f;
				if (!SubControlFixer.Undocking && !flag4 && !SubControlFixer.ManualDocking && flag5 && !flag3 && Input.GetKeyDown(SubControlFixer.ManualDockingKey))
					SubControlFixer.ManualDocking = true;
				if (SubControlFixer.Undocking || flag4 || (flag5 && (SubControlFixer.ManualDocking || flag3)))
				{
					bool flag6 = false;
					Vector3 vector = Vector3.zero;
					if (SubControlFixer.Undocking || (!flag3 && (flag4 || (flag5 && SubControlFixer.ManualDocking))))
					{
						Vector3 vector2 = keyValuePair.Key.position + BasePart.P_CyclopsDockingHatch;
						Vector3 vector3 = vector2 - position;
						float sqrMagnitude = vector3.sqrMagnitude;
						if (SubControlFixer.Undocking)
						{
							if (sqrMagnitude >= SubControlFixer.AutoDockingUndockSqrRange)
								return SubControlFixer.EndUndocking(__instance, keyValuePair.Key, component.Id);
							vector = SubControlFixer.UndockingAnimate(sqrMagnitude, __instance.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI/EngineOff_Button").GetComponent<CyclopsEngineChangeState>(), keyValuePair.Key);
						}
						else
						{
							bool flag7 = __instance.cyclopsMotorMode.cyclopsMotorMode > CyclopsMotorMode.CyclopsMotorModes.Slow;
							if (sqrMagnitude < 64f)
							{
								if (flag7)
									__instance.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI/Speed_01").GetComponent<CyclopsMotorModeButton>().OnClick();
							}
							else if (sqrMagnitude < 900f && flag7 && __instance.cyclopsMotorMode.cyclopsMotorMode != CyclopsMotorMode.CyclopsMotorModes.Standard)
								__instance.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI/Speed_02").GetComponent<CyclopsMotorModeButton>().OnClick();
							vector = SubControlFixer.Compute(__instance, subRoot, position, vector2, Quaternion.LookRotation(subRoot.subAxis.forward, subRoot.subAxis.up).eulerAngles, Quaternion.LookRotation(vector3).eulerAngles, sqrMagnitude, component.Id, keyValuePair.Key, __instance.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI/EngineOff_Button").GetComponent<CyclopsEngineChangeState>());
						}
					}
					else
						flag6 = true;
					SubControlFixer.Throttle(__instance, vector, flag2, component.Id, false, flag6, keyValuePair.Key);
					return false;
				}
			}
			return true;
		}
	}
}
