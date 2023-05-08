using System.Reflection;
using UnityEngine;

namespace CyclopsDockingMod.Fixers
{
	public static class CyclopsEntryHatchFixer
    {
        private static readonly FieldInfo _hatchOpen = typeof(CyclopsEntryHatch).GetField("hatchOpen", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool OnTriggerExit_Prefix(CyclopsEntryHatch __instance, Collider col)
		{
			if (col.gameObject.Equals(Player.main.gameObject) && (bool)CyclopsEntryHatchFixer._hatchOpen.GetValue(__instance))
			{
				Transform transform = __instance.gameObject.transform;
				Transform transform2 = ((transform != null) ? transform.parent : null);
				if (transform2 != null)
				{
					PrefabIdentifier component = transform2.GetComponent<PrefabIdentifier>();
					if (component != null && SubControlFixer.Docked(component.Id))
						return false;
				}
			}
			return true;
		}
	}
}
