using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CyclopsDockingMod.Controllers;
using UnityEngine;

namespace CyclopsDockingMod.Fixers
{
	public static class BuilderToolFixer
    {
#if !SUBNAUTICA_EXP
        private static readonly FieldInfo _deconstructText = typeof(BuilderTool).GetField("deconstructText", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo _constructText = typeof(BuilderTool).GetField("constructText", BindingFlags.Instance | BindingFlags.NonPublic);
#endif

        private static readonly FieldInfo _recipe = typeof(BaseDeconstructable).GetField("recipe", BindingFlags.Instance | BindingFlags.NonPublic);

        private static string theConstructText = null;

        private static string theDeconstructText = null;

        private static string GetConstructDeconstructText(bool isConstruct)
		{
			if (BuilderToolFixer.theConstructText == null || BuilderToolFixer.theDeconstructText == null)
			{
				string buttonFormat = LanguageCache.GetButtonFormat("ConstructFormat", GameInput.Button.LeftHand);
				string buttonFormat2 = LanguageCache.GetButtonFormat("DeconstructFormat", GameInput.Button.Deconstruct);
				BuilderToolFixer.theConstructText = Language.main.GetFormat<string, string>("ConstructDeconstructFormat", buttonFormat, buttonFormat2);
				BuilderToolFixer.theDeconstructText = buttonFormat2;
				if (string.IsNullOrEmpty(BuilderToolFixer.theConstructText))
					BuilderToolFixer.theConstructText = "Construct " + CyclopsHatchConnector.CyclopsHatchConnectorName;
				if (string.IsNullOrEmpty(BuilderToolFixer.theDeconstructText))
					BuilderToolFixer.theDeconstructText = "Deconstruct " + CyclopsHatchConnector.CyclopsHatchConnectorName;
			}
			if (!isConstruct)
				return BuilderToolFixer.theDeconstructText;
			return BuilderToolFixer.theConstructText;
		}

		public static bool ConstructOnHover_Prefix(BuilderTool __instance, Constructable constructable)
        {
            if (constructable.techType == TechType.BaseCorridor)
            {
                Transform t = constructable.gameObject.transform;
                if (t != null)
                    foreach (Transform tr in t)
                        if (tr.name.StartsWith("BaseGhost"))
                        {
                            foreach (Transform subTr in tr)
                                if (subTr.name.StartsWith(BuilderFixer.BaseConnectorL) || subTr.name.StartsWith(CyclopsHatchConnector.ModelName))
                                {
                                    HandReticle main = HandReticle.main;
                                    if (constructable.constructed)
                                    {
#if SUBNAUTICA_EXP
                                        main.SetText(HandReticle.TextType.Hand, GetConstructDeconstructText(false), false, GameInput.Button.Deconstruct);
#else
#pragma warning disable CS0618
                                        main.SetInteractText(CyclopsHatchConnector.CyclopsHatchConnectorName, (string)_deconstructText.GetValue(__instance), false, false, false);
#pragma warning restore CS0618
#endif
                                        return false;
                                    }
                                    StringBuilder sb = new StringBuilder();
#if SUBNAUTICA_EXP
                                    sb.AppendLine(GetConstructDeconstructText(true));
#else
                                    sb.AppendLine((string)_constructText.GetValue(__instance));
#endif
                                    foreach (KeyValuePair<TechType, int> res in constructable.GetRemainingResources())
                                    {
                                        TechType key = res.Key;
                                        string text = Language.main.Get(key);
                                        int value = res.Value;
                                        if (value > 1)
                                            sb.AppendLine(Language.main.GetFormat<string, int>("RequireMultipleFormat", text, value));
                                        else
                                            sb.AppendLine(text);
                                    }
#if SUBNAUTICA_EXP
                                    main.SetText(HandReticle.TextType.Hand, CyclopsHatchConnector.CyclopsHatchConnectorName + " : " + sb.ToString(), false, GameInput.Button.None);
#else
#pragma warning disable CS0618
                                    main.SetInteractText(CyclopsHatchConnector.CyclopsHatchConnectorName, sb.ToString(), false, false, false);
#pragma warning restore CS0618
#endif
                                    main.SetProgress(constructable.amount);
                                    main.SetIcon(HandReticle.IconType.Progress, 1.5f);
                                    return false;
                                }
                            break;
                        }
            }
            else if (constructable.techType == TechType.Sign)
            {
                LoopRefreshEnergyController lrec = constructable.gameObject.transform.GetComponent<LoopRefreshEnergyController>();
                if (lrec != null)
                {
#if SUBNAUTICA_EXP
                    HandReticle.main.SetText(HandReticle.TextType.Hand, CyclopsHatchConnector.CyclopsHatchConnectorName, false, GameInput.Button.LeftHand);
#else
                    HandReticle.main.SetInteractText(CyclopsHatchConnector.CyclopsHatchConnectorName, false);
#endif
                    return false;
                }
            }
            return true;
        }

		public static bool DeconstructOnHover_Prefix(BuilderTool __instance, BaseDeconstructable deconstructable)
		{
			if ((TechType)BuilderToolFixer._recipe.GetValue(deconstructable) == TechType.BaseConnector)
			{
				Transform transform = deconstructable.gameObject.transform;
				if (transform != null)
				{
					using (List<BasePart>.Enumerator enumerator = BaseFixer.BaseParts.GetEnumerator())
					{
						while (enumerator.MoveNext())
							if (FastHelper.IsNear(enumerator.Current.position, transform.position))
							{
								HandReticle.main.SetText(HandReticle.TextType.Hand, BuilderToolFixer.GetConstructDeconstructText(false), false, GameInput.Button.Deconstruct);
								return false;
							}
					}
					return true;
				}
			}
			return true;
		}
	}
}
