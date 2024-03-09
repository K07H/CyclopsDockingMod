using System.Globalization;
using CyclopsDockingMod.Routing;
using UnityEngine;

namespace CyclopsDockingMod.Fixers
{
	public static class SubRootFixer
    {
        public const float CyclopsRechargeRatioM = 2500f;

        public const float CyclopsRechargeRatioD = 0.0004f;

        public static float CyclopsRechargeSpeed = 0.01f;

        public static bool UpdateThermalReactorCharge_Prefix(SubRoot __instance)
		{
			if (__instance.powerRelay != null && GameModeUtils.RequiresPower())
			{
				PrefabIdentifier component = __instance.GetComponent<PrefabIdentifier>();
				if (component != null && SubControlFixer.Docked(component.Id) && __instance.powerRelay.GetPower() < __instance.powerRelay.GetMaxPower())
				{
					BaseRoot root = SubControlFixer.DockedSubs[component.Id].root;
					float num;
					if (((root != null) ? root.powerRelay : null) != null && SubControlFixer.DockedSubs[component.Id].root.powerRelay.IsPowered() && SubControlFixer.DockedSubs[component.Id].root.powerRelay.ConsumeEnergy(SubRootFixer.CyclopsRechargeSpeed, out num) && num > 0f && num < 1f)
					{
						float num2 = 100f / __instance.powerRelay.GetMaxPower();
						int num3 = Mathf.RoundToInt(num2 * __instance.powerRelay.GetPower());
						float num4;
						__instance.powerRelay.AddEnergy(num, out num4);
						int num5 = Mathf.RoundToInt(num2 * __instance.powerRelay.GetPower());
						if (num3 != num5 && SubControlFixer.DockedSubs[component.Id].signGo != null)
						{
							Sign component2 = SubControlFixer.DockedSubs[component.Id].signGo.GetComponent<Sign>();
							if (component2 != null)
							{
								string text = string.Format(ConfigOptions.LblCyclopsDocked, num5);
								component2.text = text;
								component2.signInput.text = text;
							}
						}
					}
				}
			}
			return true;
		}

		public static void Start_Postfix(SubRoot __instance)
		{
			PrefabIdentifier component = __instance.gameObject.GetComponent<PrefabIdentifier>();
			if (component == null)
				return;
			if (!AutoPilot.SubsPlayingRoutes.ContainsKey(component.Id))
				AutoPilot.SubsPlayingRoutes.Add(component.Id, new SubRoutePlaying());
			if (SubControlFixer.Docked(component.Id))
			{
				Transform transform = __instance.gameObject.transform.Find("CyclopsMeshAnimated/submarine_outer_hatch_01");
				if (transform == null)
					return;
				Vector3 vector = transform.position - (SubControlFixer.DockedSubs[component.Id].position + BasePart.P_CyclopsDockingHatch);
				float sqrMagnitude = vector.sqrMagnitude;
				if (sqrMagnitude > SubControlFixer.AutoDockingTriggerSqrRange)
				{
					string text = "Found Cyclops docked but too far away from dock ({0}m) at coordinates {1} {2} {3}. Undocking it.";
					object[] array = new object[4];
					array[0] = Mathf.Sqrt(sqrMagnitude).ToString("0.00", CultureInfo.InvariantCulture);
					int num = 1;
					vector = transform.position;
					array[num] = vector.x.ToString("0.00", CultureInfo.InvariantCulture);
					int num2 = 2;
					vector = transform.position;
					array[num2] = vector.y.ToString("0.00", CultureInfo.InvariantCulture);
					int num3 = 3;
					vector = transform.position;
					array[num3] = vector.z.ToString("0.00", CultureInfo.InvariantCulture);
					Logger.Info(text, array);
					SubControlFixer.CleanUp(SubControlFixer.DockedSubs[component.Id], component.Id, true);
					SubControlFixer.ToggleTrap(__instance, false, false);
					if (SubControlFixer.DockedSubs[component.Id].trapOpened)
					{
						SubControlFixer.DockedSubs[component.Id].trapOpened = false;
						return;
					}
				}
				else
				{
					SubControlFixer.ToggleTrap(__instance, true, false);
					if (!SubControlFixer.DockedSubs[component.Id].trapOpened)
						SubControlFixer.DockedSubs[component.Id].trapOpened = true;
				}
			}
		}
	}
}
