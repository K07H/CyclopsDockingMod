using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CyclopsDockingMod.Routing;
using UnityEngine;

namespace CyclopsDockingMod.Fixers
{
	public static class StabilizerFixer
    {
        private static readonly FieldInfo _body = typeof(Stabilizer).GetField("body", BindingFlags.Instance | BindingFlags.Public) ?? typeof(Stabilizer).GetField("body", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _subRoot = typeof(Stabilizer).GetField("subRoot", BindingFlags.Instance | BindingFlags.Public) ?? typeof(Stabilizer).GetField("subRoot", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly KeyValuePair<int, float> ResetVal = new KeyValuePair<int, float>(0, -1f);

        public static readonly Dictionary<string, KeyValuePair<int, float>> SubsNotStabilizing = new Dictionary<string, KeyValuePair<int, float>>();

        public static void RefreshStabilizers()
		{
			List<string> list = StabilizerFixer.SubsNotStabilizing.Keys.ToList<string>();
			if (list != null)
				foreach (string text in list)
					if (!string.IsNullOrEmpty(text))
						StabilizerFixer.SubsNotStabilizing[text] = StabilizerFixer.ResetVal;
		}

		private static Vector3? GetHatchPos(Transform tr)
		{
			foreach (object obj in tr)
			{
				Transform transform = (Transform)obj;
				if (transform.name == "CyclopsMeshAnimated")
				{
					foreach (object obj2 in transform)
					{
						Transform transform2 = (Transform)obj2;
						if (transform2.name == "submarine_outer_hatch_01")
							return new Vector3?(transform2.position);
					}
					return null;
				}
			}
			return null;
		}

		private static float SpeedRatio(float sqrMag)
		{
			if (sqrMag > 200f)
				return 10f;
			if (sqrMag > 100f)
				return 8f;
			if (sqrMag > 64f)
				return 6f;
			if (sqrMag > 36f)
				return 5f;
			if (sqrMag > 20f)
				return 4f;
			if (sqrMag > 9f)
				return 3f;
			if (sqrMag > 4f)
				return 2f;
			return 1f;
		}

		private static float AdjustVSpeed(float hDiff, float yDiff)
		{
			if (hDiff > 8f)
				return Mathf.Clamp01(yDiff + 0.7f);
			if (hDiff > 5f)
				return Mathf.Clamp01(yDiff + 0.5f);
			if (hDiff > 2f)
				return Mathf.Clamp01(yDiff + 0.4f);
			if (hDiff > 1f)
				return Mathf.Clamp01(yDiff + 0.2f);
			if (hDiff > 0.1f)
				return Mathf.Clamp01(yDiff + 0.1f);
			return yDiff;
		}

		private static bool AdjustPosAndRot(SubRoot sub, Rigidbody rb, Vector3 pos, BasePart bp)
		{
			bool flag = false;
			Vector3 vector = bp.position + BasePart.P_CyclopsDockingHatch;
			if (!FastHelper.IsNear(pos, vector))
			{
				Vector3 vector2 = vector - pos;
				float num = vector.y - pos.y;
				float num2 = StabilizerFixer.SpeedRatio(vector2.sqrMagnitude);
				vector2 = Vector3.ClampMagnitude(vector2, Mathf.Clamp01(0.1f * num2));
				vector2.y = StabilizerFixer.AdjustVSpeed(num, vector2.y);
				rb.AddForceAtPosition(vector2, rb.transform.position, ForceMode.Acceleration);
				flag = true;
			}
			Transform dockingHatch = bp.GetDockingHatch();
			if (dockingHatch != null)
			{
				int num3 = Mathf.RoundToInt(Quaternion.LookRotation(sub.subAxis.forward, sub.subAxis.up).eulerAngles.y) - 90;
				if (num3 != Mathf.RoundToInt(Quaternion.LookRotation(dockingHatch.forward, dockingHatch.up).eulerAngles.y))
				{
					dockingHatch.eulerAngles = new Vector3(dockingHatch.eulerAngles.x, (float)num3, dockingHatch.eulerAngles.z);
					flag = true;
				}
			}
			return flag;
		}

		public static void FixedUpdate_Postfix(Stabilizer __instance)
		{
			if (__instance.stabilizerEnabled)
			{
				Rigidbody rigidbody = (Rigidbody)StabilizerFixer._body.GetValue(__instance);
				if (rigidbody == null || rigidbody.isKinematic)
					return;
				PrefabIdentifier component = __instance.gameObject.GetComponent<PrefabIdentifier>();
				if (component == null)
					return;
				if (SubControlFixer.Docked(component.Id))
				{
					if (!StabilizerFixer.SubsNotStabilizing.ContainsKey(component.Id))
					{
						StabilizerFixer.SubsNotStabilizing.Add(component.Id, StabilizerFixer.ResetVal);
					}
					int key = StabilizerFixer.SubsNotStabilizing[component.Id].Key;
					float value = StabilizerFixer.SubsNotStabilizing[component.Id].Value;
					if (key < 1 || Time.time > value + 10f)
					{
						Vector3? hatchPos = StabilizerFixer.GetHatchPos(__instance.gameObject.transform);
						if (hatchPos != null)
						{
							if (StabilizerFixer.AdjustPosAndRot((SubRoot)StabilizerFixer._subRoot.GetValue(__instance), rigidbody, hatchPos.Value, SubControlFixer.DockedSubs[component.Id]))
							{
								StabilizerFixer.SubsNotStabilizing[component.Id] = StabilizerFixer.ResetVal;
								return;
							}
							if (key < 100)
							{
								StabilizerFixer.SubsNotStabilizing[component.Id] = new KeyValuePair<int, float>(key + 1, value);
								return;
							}
							StabilizerFixer.SubsNotStabilizing[component.Id] = new KeyValuePair<int, float>(key, Time.time);
							return;
						}
					}
				}
				else if (!Player.main.IsPiloting() && AutoPilot.SubsPlayingRoutes.ContainsKey(component.Id) && AutoPilot.SubsPlayingRoutes[component.Id].IsPlayingRoute)
				{
					Vector3? hatchPos2 = StabilizerFixer.GetHatchPos(__instance.gameObject.transform);
					if (hatchPos2 != null)
						SubControlFixer.StabilizePlay(rigidbody, (SubRoot)StabilizerFixer._subRoot.GetValue(__instance), component.Id, hatchPos2.Value, AutoPilot.SubsPlayingRoutes[component.Id]);
				}
			}
		}
	}
}
