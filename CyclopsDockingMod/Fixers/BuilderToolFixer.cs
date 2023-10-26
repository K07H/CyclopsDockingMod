namespace CyclopsDockingMod.Fixers;

using System.Collections.Generic;
using System.Text;
using global::CyclopsDockingMod.Controllers;
using UnityEngine;

public static class BuilderToolFixer
{
    private static string theConstructText = null;

    private static string theDeconstructText = null;

    private static string GetConstructDeconstructText(bool isConstruct)
    {
        if (theConstructText == null || theDeconstructText == null)
        {
            string buttonFormat = LanguageCache.GetButtonFormat("ConstructFormat", GameInput.Button.LeftHand);
            string buttonFormat2 = LanguageCache.GetButtonFormat("DeconstructFormat", GameInput.Button.Deconstruct);
            theConstructText = Language.main.GetFormat<string, string>("ConstructDeconstructFormat", buttonFormat, buttonFormat2);
            theDeconstructText = buttonFormat2;
            if (string.IsNullOrEmpty(theConstructText))
                theConstructText = "Construct " + CyclopsHatchConnector.CyclopsHatchConnectorName;
            if (string.IsNullOrEmpty(theDeconstructText))
                theDeconstructText = "Deconstruct " + CyclopsHatchConnector.CyclopsHatchConnectorName;
        }
        if (!isConstruct)
            return theDeconstructText;
        return theConstructText;
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
                                    main.SetText(HandReticle.TextType.Hand, GetConstructDeconstructText(false), false, GameInput.Button.Deconstruct);
                                    return false;
                                }
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(GetConstructDeconstructText(true));
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
                                main.SetText(HandReticle.TextType.Hand, CyclopsHatchConnector.CyclopsHatchConnectorName + " : " + sb.ToString(), false, GameInput.Button.None);
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
                HandReticle.main.SetText(HandReticle.TextType.Hand, CyclopsHatchConnector.CyclopsHatchConnectorName, false, GameInput.Button.LeftHand);
                return false;
            }
        }
        return true;
    }

    public static bool DeconstructOnHover_Prefix(BuilderTool __instance, BaseDeconstructable deconstructable)
    {
        if (deconstructable.recipe == TechType.BaseConnector)
        {
            Transform transform = deconstructable.gameObject.transform;
            if (transform != null)
            {
                using (List<BasePart>.Enumerator enumerator = BaseFixer.BaseParts.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                        if (FastHelper.IsNear(enumerator.Current.position, transform.position))
                        {
                            HandReticle.main.SetText(HandReticle.TextType.Hand, GetConstructDeconstructText(false), false, GameInput.Button.Deconstruct);
                            return false;
                        }
                }
                return true;
            }
        }
        return true;
    }
}
