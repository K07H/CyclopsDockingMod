namespace CyclopsDockingMod.Fixers;

using System;
using System.Collections;
using System.Collections.Generic;
using global::CyclopsDockingMod.Routing;
using UnityEngine;
using Logger = Logger;

public static class SubControlFixer
{
    private static readonly FMODAsset _cyclopsDockDoorsClose = AssetsHelper.CreateAsset("2093", "6abdf213-0d28-412a-b0c7-a622c0a03912", "event:/sub/cyclops/docking_doors_close");

    private static readonly FMODAsset _cyclopsDockDoorsOpen = AssetsHelper.CreateAsset("2094", "c39c942e-793e-47c7-ab74-6175420abf25", "event:/sub/cyclops/docking_doors_open");

    private static readonly FMODAsset _cyclopsOuterHatchClose = AssetsHelper.CreateAsset("2105", "96e0fc8b-d997-4149-aa09-7ebc4bf7188c", "event:/sub/cyclops/outer_hatch_close");

    private static readonly FMODAsset _cyclopsOuterHatchOpen = AssetsHelper.CreateAsset("2106", "94631097-1268-47d8-bbce-b40a44221bc0", "event:/sub/cyclops/outer_hatch_open");

    private static readonly Vector3 UndockingSpeedSlow = new Vector3(0f, 0f, -0.2f);

    private static readonly Vector3 UndockingSpeedFast = new Vector3(0f, 0f, -0.7f);

    public static bool SimpleDocking = false;

    public static float AutoDockingTriggerSqrRange = 169f;

    public static float AutoDockingUndockSqrRange = Mathf.Pow(Mathf.Sqrt(AutoDockingTriggerSqrRange) + 5f, 2f);

    public static float AutoDockingDetectSqrRange = Mathf.Pow(Mathf.Sqrt(AutoDockingUndockSqrRange) + 1f, 2f);

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
        get => (!SimpleDocking ? 40f : 4f);
    }

    private static float PointRange
    {
        get => (!SimpleDocking ? 3f : 1f);
    }

    private static void Reset()
    {
        DockingStt = -1f;
        PAnim = -1f;
        UndockingCnt = 0;
        LRotation = false;
        RPoint = false;
        CLRotation = false;
        PDocking = -1f;
        PDockingBis = -1f;
        Undocking = false;
        ManualDocking = false;
        PUndocking = -1f;
        PUndock = 0;
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

    public static bool Docked(string id) => (DockedSubs.ContainsKey(id) && DockedSubs[id] != null);

    private static KeyValuePair<BasePart, float> GetClosestDockingHatch(Vector3 pos, bool free = false)
    {
        BasePart basePart = null;
        float num = float.MaxValue;
        foreach (BasePart basePart2 in BaseFixer.BaseParts)
        {
            if (basePart2.type == 0 && (!free || basePart2.dock == null))
            {
                float sqrMagnitude = (pos - (basePart2.position + BasePart.P_CyclopsDockingHatch)).sqrMagnitude;
                if (sqrMagnitude < (AutoDocking ? AutoDockingDetectSqrRange : 10000f) && (basePart == null || sqrMagnitude < num))
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
            DockedSubs[pid] = null;
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
        CleanUp(bp, pid, cleanBaseParts);
        Reset();
    }

    private static void OpenTrap(CyclopsEntryHatch entry, bool playSfx)
    {
        if (!entry.hatchOpen)
        {
            if (playSfx && Player.main.IsUnderwater())
                entry.openSFX.Play();
            entry.hatchOpen = true;
        }
    }

    private static void CloseTrap(CyclopsEntryHatch entry, bool playSfx)
    {
        if (entry.hatchOpen)
        {
            if (playSfx && Player.main.IsUnderwater())
                entry.closeSFX.Play();
            entry.hatchOpen = false;
        }
    }

    public static void ToggleTrap(SubRoot sub, bool open, bool playSfx)
    {
        GameObject gameObject = sub.gameObject.FindChild("EntryHatch");
        if (gameObject != null)
        {
            CyclopsEntryHatch component = gameObject.GetComponent<CyclopsEntryHatch>();
            if (component != null)
            {
                if (open)
                {
                    OpenTrap(component, playSfx);
                    return;
                }
                CloseTrap(component, playSfx);
            }
        }
    }

    private static void Throttle(SubControl subCtrl, Vector3 throttle, bool canAccel, string pid, bool autoPilot = true, bool canEject = true, BasePart bp = null)
    {
        try
        {
            subCtrl.appliedThrottle = false;
            subCtrl.throttle = throttle;
            float magnitude = throttle.magnitude;
            if (canAccel && (double)magnitude > 0.0001)
            {
                float num = magnitude * subCtrl.cyclopsMotorMode.GetPowerConsumption() * Time.deltaTime / subCtrl.sub.GetPowerRating();
                float num2;
                if (!GameModeUtils.RequiresPower() || subCtrl.powerRelay.ConsumeEnergy(num, out num2))
                {
                    subCtrl.lastTimeThrottled = Time.time;
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
                int num4 = subCtrl.throttleHandlers.Length;
                for (int i = 0; i < num4; i++)
                {
                    subCtrl.throttleHandlers[i].OnSubAppliedThrottle();
                }
                if (subCtrl.lastTimeThrottled < Time.time - 5f)
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
                        HandleExit();
                    else
                        HandleExit(canEject, pid, bp);
                }
            }
            if (!subCtrl.appliedThrottle)
                subCtrl.throttle = Vector3.zero;
        }
        catch (Exception ex)
        {
            Logger.Log("ERROR: Exception caught in Throttle. Ex=[" + ex.ToString() + "]");
        }
    }

    private static void HandleExit(bool canEject, string pid, BasePart bp)
    {
        if (canEject && !Undocking)
        {
            StartUndocking();
            return;
        }
        if (DockingStt > 0f)
        {
            if (Time.time > DockingStt + 20f)
            {
                ErrorMessage.AddDebug(AutoPilot.Lbl_ForceEject);
                UndockCleanup(bp, pid, true);
                Player.main.TryEject();
                return;
            }
            if (Undocking)
            {
                ErrorMessage.AddDebug(AutoPilot.Lbl_WaitUndockCompletion);
                return;
            }
            if (UndockingCnt < 2)
            {
                UndockingCnt++;
                return;
            }
            ErrorMessage.AddDebug(AutoPilot.Lbl_ForceUndock);
            StartUndocking();
        }
    }

    private static void HandleExit()
    {
        Player.main.TryEject();
    }

    private static Vector3 UndockingAnimate(float dSqrMag, CyclopsEngineChangeState engine, BasePart bp)
    {
        if (PUndocking > 0f)
        {
            if (PUndock == 0)
            {
                engine.StartCoroutine(engine.EngineStartCameraShake(0.2f, 1f, 0f));
                engine.StartCoroutine(engine.EngineStartCameraShake(0.2f, -1f, 1.1f));
                FMODUWE.PlayOneShot(_cyclopsDockDoorsClose, MainCamera.camera.transform.position, 1f);
                bp.PlayDockingAnim(CyclopsHatchConnector.CyclopsDockingAnim.UNDOCKING);
                PUndock++;
            }
            else if (PUndock == 1 && Time.time > PUndocking + 3f)
            {
                FMODUWE.PlayOneShot(_cyclopsDockDoorsOpen, MainCamera.camera.transform.position, 1f);
                engine.StartCoroutine(engine.EngineStartCameraShake(0.05f, 3f, 0f));
                engine.StartCoroutine(engine.EngineStartCameraShake(0.05f, -1f, 3.1f));
                PUndock++;
            }
            else if (PUndock == 2 && Time.time > PUndocking + 8f)
            {
                FMODUWE.PlayOneShot(_cyclopsOuterHatchClose, MainCamera.camera.transform.position, 1f);
                PUndock++;
            }
        }
        if (dSqrMag >= 25f)
            return UndockingSpeedFast;
        return UndockingSpeedSlow;
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
            ToggleTrap(subRoot2, false, true);
            bp.trapOpened = false;
        }
        UndockCleanup(bp, pid, false);
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
        Reset();
        bp.dock = id;
        bp.sub = subRoot;
        DockedSubs[id] = bp;
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
            ToggleTrap(subRoot, true, true);
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
        if (pos.y > dockingPos.y + ((!RPoint) ? 0.05f : 0.1f))
        {
            zero.y = VerticalSpeed(num) * -1f;
        }
        else if (pos.y < dockingPos.y - ((!RPoint) ? 0.05f : 0.1f))
        {
            zero.y = VerticalSpeed(num);
        }
        float num2 = FastHelper.AngleDiff(subDirAngles.y, dirAngles.y);
        if (!RPoint)
        {
            bool flag = Mathf.Abs(num2) > 0.1f;
            if (dSqrMag < AutoDockingGetAwaySqrRange || flag)
            {
                if (!LRotation)
                {
                    float num3 = ((flag || SimpleDocking) ? (-0.1f) : GetAwaySpeed(dSqrMag));
                    zero.z = num3;
                }
            }
            else if (!CLRotation && !flag)
            {
                CLRotation = true;
                DockingStt = Time.time;
            }
        }
        Vector3 vector = new Vector3(pos.x, dockingPos.y, pos.z);
        if (!RPoint && num2 < -0.05f && (pos.x > dockingPos.x + 0.1f || pos.x < dockingPos.x - 0.1f || pos.z > dockingPos.z + 0.1f || pos.z < dockingPos.z - 0.1f))
            zero.x = NegRot(num2) * 0.5f;
        else if (!RPoint && num2 > 0.05f && (pos.x > dockingPos.x + 0.1f || pos.x < dockingPos.x - 0.1f || pos.z > dockingPos.z + 0.1f || pos.z < dockingPos.z - 0.1f))
            zero.x = PosRot(num2) * 0.5f;
        else if (CLRotation && (pos.x > dockingPos.x + ((!RPoint) ? PointRange : 0.2f) || pos.x < dockingPos.x - ((!RPoint) ? PointRange : 0.2f) || pos.z > dockingPos.z + ((!RPoint) ? PointRange : 0.2f) || pos.z < dockingPos.z - ((!RPoint) ? PointRange : 0.2f)))
        {
            if (!LRotation)
                LRotation = true;
            else if (PDocking > 0f && Time.time > PDocking + (SimpleDocking ? 2f : 7f))
            {
                PDocking = -1f;
                FMODUWE.PlayOneShot(_cyclopsDockDoorsOpen, MainCamera.camera.transform.position, 1f);
                engine.StartCoroutine(engine.EngineStartCameraShake(0.05f, 3f, 0f));
                engine.StartCoroutine(engine.EngineStartCameraShake(0.05f, -1f, 3.1f));
                bp.PlayDockingAnim(CyclopsHatchConnector.CyclopsDockingAnim.DOCKING);
            }
            else if (PDockingBis > 0f && Time.time > PDockingBis + (SimpleDocking ? 6.5f : 11.5f))
            {
                PDockingBis = -1f;
                engine.StartCoroutine(engine.EngineStartCameraShake(0.2f, 1f, 0f));
                engine.StartCoroutine(engine.EngineStartCameraShake(0.2f, -1f, 1.1f));
                FMODUWE.PlayOneShot(_cyclopsDockDoorsClose, MainCamera.camera.transform.position, 1f);
            }
            if (LRotation && PAnim > 0f && Time.time > PAnim + 1f)
            {
                EndAnimate(subCtrl, bp, id, engine);
                bp.PlayDockingAnim(CyclopsHatchConnector.CyclopsDockingAnim.DOCKED);
            }
            else
            {
                zero.z = ((dSqrMag < 0.5f) ? 0.005f : ((dSqrMag < 1f) ? 0.01f : ((dSqrMag < 25f) ? 0.05f : ((dSqrMag < 100f) ? 0.1f : 0.4f)))) * ((Vector3.Dot(sub.subAxis.forward, (dockingPos - vector).normalized) < 0f) ? (-1f) : 1f);
                if (SimpleDocking)
                    zero.z *= 2f;
            }
        }
        else if (CLRotation && LRotation)
        {
            if (!RPoint)
            {
                RPoint = true;
                FMODUWE.PlayOneShot(_cyclopsOuterHatchOpen, MainCamera.camera.transform.position, 1f);
                PDocking = Time.time;
                PDockingBis = PDocking;
            }
            else if (PAnim < 0f)
            {
                if (PDocking > 0f || PDockingBis > 0f)
                {
                    engine.StartCoroutine(engine.EngineStartCameraShake(0.2f, 1f, 0f));
                    engine.StartCoroutine(engine.EngineStartCameraShake(0.2f, -1f, 1.1f));
                    if (PDocking > 0f)
                    {
                        PDocking = -1f;
                        FMODUWE.PlayOneShot(_cyclopsDockDoorsOpen, MainCamera.camera.transform.position, 1f);
                    }
                    if (PDockingBis > 0f)
                    {
                        PDockingBis = -1f;
                        FMODUWE.PlayOneShot(_cyclopsDockDoorsClose, MainCamera.camera.transform.position, 1f);
                    }
                }
                PAnim = Time.time;
            }
            else if (Time.time > PAnim + 1f)
            {
                EndAnimate(subCtrl, bp, id, engine);
                bp.PlayDockingAnim(CyclopsHatchConnector.CyclopsDockingAnim.DOCKED);
            }
        }
        return zero;
    }

    public static void StartUndocking()
    {
        Undocking = true;
        PUndocking = Time.time;
        PUndock = 0;
    }

    private static void StabilizerThrottle(Rigidbody rb, SubRoot subRoot, SubControl subCtrl, Vector3 throttle)
    {
        subCtrl.appliedThrottle = false;
        subCtrl.throttle = throttle;
        float magnitude = throttle.magnitude;
        if (subCtrl.canAccel)
        {
            if ((double)magnitude > 0.0001)
            {
                float num = magnitude * subCtrl.cyclopsMotorMode.GetPowerConsumption() * Time.deltaTime / subRoot.GetPowerRating();
                float num2;
                if (!GameModeUtils.RequiresPower() || subCtrl.powerRelay.ConsumeEnergy(num, out num2))
                {
                    subCtrl.lastTimeThrottled = Time.time;
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
                int num4 = subCtrl.throttleHandlers.Length;
                for (int i = 0; i < num4; i++)
                    subCtrl.throttleHandlers[i].OnSubAppliedThrottle();
                if (subCtrl.lastTimeThrottled < Time.time - 5f)
                    Utils.PlayFMODAsset(subCtrl.engineStartSound, rb.transform, 30f);
            }
        }
        if (!subCtrl.appliedThrottle)
            subCtrl.throttle = Vector3.zero;
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
                zero.x = NegRot(num2) * 0.8f;
                zero.z = ((sqrMagnitude2 < 1f) ? 0.05f : ((sqrMagnitude2 < 9f) ? 0.1f : ((sqrMagnitude2 < 25f) ? 0.2f : 0.4f))) * ((num3 < 0f) ? (-1f) : 1f);
            }
            else if (num3 >= 0f && num2 > 3f)
            {
                zero.x = PosRot(num2) * 0.8f;
                zero.z = ((sqrMagnitude2 < 1f) ? 0.05f : ((sqrMagnitude2 < 9f) ? 0.1f : ((sqrMagnitude2 < 25f) ? 0.2f : 0.4f))) * ((num3 < 0f) ? (-1f) : 1f);
            }
            else
                zero.z = AutoPilotForwardSpeed(sqrMagnitude2) * ((num3 < 0f) ? (-1f) : 1f);
            float y = vector.Value.y;
            float num4 = Math.Abs(y - pos.y);
            if (pos.y > y + 0.1f)
                zero.y = VerticalSpeed(num4) * -1f;
            else if (pos.y < y - 0.1f)
                zero.y = VerticalSpeed(num4);
            if (sqrMagnitude < 9f || zero == Vector3.zero || (flag && sqrMagnitude < AutoDockingTriggerSqrRange - 4f))
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
        if (!ManualDocking && !Docked(pid) && AutoPilot.SubsPlayingRoutes[pid].Playing.BasePartPosEnd != null)
        {
            KeyValuePair<BasePart, float> closestDockingHatch = GetClosestDockingHatch(pos, true);
            if (closestDockingHatch.Key != null && closestDockingHatch.Value < 10000f && FastHelper.IsNear(AutoPilot.SubsPlayingRoutes[pid].Playing.BasePartPosEnd.Value, closestDockingHatch.Key.position))
                return true;
        }
        return false;
    }

    private static bool Play(SubControl subCtrl, SubRoot subRoot, string pid, Vector3 pos, bool canAccel, SubRoutePlaying route)
    {
        KeyValuePair<int, Vector3> keyValuePair = AutoPilotCompute(subRoot.GetComponent<Rigidbody>(), subRoot, pos, route);
        if (keyValuePair.Key > 0)
        {
            bool flag = keyValuePair.Key == 1;
            if (flag && !AutoDocking)
            {
                if (RouteEndFreeForManualDocking(pid, pos))
                    ManualDocking = true;
                else
                    flag = false;
            }
            AutoPilot.StopPlayingRoute(pid, subCtrl.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI"), true, false);
            return !flag;
        }
        Throttle(subCtrl, keyValuePair.Value, canAccel, pid, true, true, null);
        return false;
    }

    internal static bool StabilizePlay(Rigidbody rb, SubRoot subRoot, string pid, Vector3 pos, SubRoutePlaying route)
    {
        KeyValuePair<int, Vector3> keyValuePair = AutoPilotCompute(rb, subRoot, pos, route);
        SubControl component = subRoot.GetComponent<SubControl>();
        if (keyValuePair.Key > 0 || keyValuePair.Value == Vector3.zero)
        {
            AutoPilot.StopPlayingRoute(pid, component.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI"), true, false);
            return true;
        }
        StabilizerThrottle(rb, subRoot, component, keyValuePair.Value);
        return false;
    }

    public static bool Update_Prefix(SubControl __instance)
    {
        bool flag = !__instance.LOD.IsFull();
        if (flag && !AutoPilot.KeepDrivingOnPartialLOD)
            return true;
        bool flag2 = __instance.canAccel;
        if (!flag2)
            return true;
        SubRoot subRoot = __instance.sub;
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
                return Play(__instance, subRoot, component.Id, position, flag2, subRoutePlaying);
        }
        bool flag3 = Docked(component.Id);
        KeyValuePair<BasePart, float> keyValuePair;
        if (!Undocking && !flag3)
            keyValuePair = GetClosestDockingHatch(position, true);
        else
            keyValuePair = GetClosestDockingHatch(position, false);
        if (keyValuePair.Key != null)
        {
            bool flag4 = AutoDocking && keyValuePair.Value < AutoDockingTriggerSqrRange;
            bool flag5 = !AutoDocking && keyValuePair.Value < 10000f;
            if (!Undocking && !flag4 && !ManualDocking && flag5 && !flag3 && Input.GetKeyDown(ManualDockingKey))
                ManualDocking = true;
            if (Undocking || flag4 || (flag5 && (ManualDocking || flag3)))
            {
                bool flag6 = false;
                Vector3 vector = Vector3.zero;
                if (Undocking || (!flag3 && (flag4 || (flag5 && ManualDocking))))
                {
                    Vector3 vector2 = keyValuePair.Key.position + BasePart.P_CyclopsDockingHatch;
                    Vector3 vector3 = vector2 - position;
                    float sqrMagnitude = vector3.sqrMagnitude;
                    if (Undocking)
                    {
                        if (sqrMagnitude >= AutoDockingUndockSqrRange)
                            return EndUndocking(__instance, keyValuePair.Key, component.Id);
                        vector = UndockingAnimate(sqrMagnitude, __instance.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI/EngineOff_Button").GetComponent<CyclopsEngineChangeState>(), keyValuePair.Key);
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
                        vector = Compute(__instance, subRoot, position, vector2, Quaternion.LookRotation(subRoot.subAxis.forward, subRoot.subAxis.up).eulerAngles, Quaternion.LookRotation(vector3).eulerAngles, sqrMagnitude, component.Id, keyValuePair.Key, __instance.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI/EngineOff_Button").GetComponent<CyclopsEngineChangeState>());
                    }
                }
                else
                    flag6 = true;
                Throttle(__instance, vector, flag2, component.Id, false, flag6, keyValuePair.Key);
                return false;
            }
        }
        return true;
    }
}