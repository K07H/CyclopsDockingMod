using System.Collections;
using System.Reflection;
using UnityEngine;

namespace CyclopsDockingMod.Fixers
{
	public static class uGUI_BuilderMenuFixer
    {
        private static readonly FieldInfo _beginCoroutine = typeof(uGUI_BuilderMenu).GetField("beginCoroutine", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _TryGetCachedPrefab = typeof(uGUI_BuilderMenu).GetMethod("TryGetCachedPrefab", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _BeginAsync = typeof(Builder).GetMethod("BeginAsync", BindingFlags.Static | BindingFlags.Public);

        public static bool SelectedDocking = false;

        public static IEnumerator BeginAsync_Postfix(IEnumerator values, uGUI_BuilderMenu __instance, TechType techType)
		{
			if (techType == CyclopsDockingMod.CyclopsHatchConnector)
			{
				uGUI_BuilderMenuFixer.SelectedDocking = true;
				techType = TechType.BaseConnector;
			}
			GameObject gameObject = (GameObject)uGUI_BuilderMenuFixer._TryGetCachedPrefab.Invoke(__instance, new object[] { techType });
			if (gameObject != null)
			{
				Builder.Begin(techType, gameObject);
				uGUI_BuilderMenuFixer._beginCoroutine.SetValue(__instance, null);
				yield break;
			}
			yield return (IEnumerator)uGUI_BuilderMenuFixer._BeginAsync.Invoke(null, new object[] { techType });
			yield break;
		}
	}
}
