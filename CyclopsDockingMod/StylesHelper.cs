namespace CyclopsDockingMod;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class StylesHelper
{
    public static GUISkin CDMUISkin = AssetsHelper.Assets.LoadAsset<GUISkin>("OSubMarinGUISkin");

    private static readonly Sprite _leftArrowBtnNormal = AssetsHelper.Assets.LoadAsset<Sprite>("leftarrow_normal");

    private static readonly Sprite _leftArrowBtnHover = AssetsHelper.Assets.LoadAsset<Sprite>("leftarrow_hover");

    private static readonly Sprite _leftArrowBtnActive = AssetsHelper.Assets.LoadAsset<Sprite>("leftarrow_active");

    private static readonly Sprite _rightArrowBtnNormal = AssetsHelper.Assets.LoadAsset<Sprite>("rightarrow_normal");

    private static readonly Sprite _rightArrowBtnHover = AssetsHelper.Assets.LoadAsset<Sprite>("rightarrow_hover");

    private static readonly Sprite _rightArrowBtnActive = AssetsHelper.Assets.LoadAsset<Sprite>("rightarrow_active");

    public static bool Initialized = false;

    private static GUIStyle _sLeftArrowBtnStyle = null;

    private static GUIStyle _sRightArrowBtnStyle = null;

    private static GUIStyle _sArrowsBtnLblStyle = null;

    private static GUIStyle _sArrowsBtnPrefixLblStyle = null;

    private static GUIStyle _sSliderTipLblStyle = null;

    private static GUIStyle _sTooltipLblStyle = null;

    public static GUIStyle _sBoldFontBtnStyle = null;

    public static void InitStyles()
    {
        if (!Initialized)
        {
            Initialized = true;
            _sLeftArrowBtnStyle = new GUIStyle(GUI.skin.button);
            _sLeftArrowBtnStyle.normal.background = _leftArrowBtnNormal.texture;
            _sLeftArrowBtnStyle.hover.background = _leftArrowBtnHover.texture;
            _sLeftArrowBtnStyle.active.background = _leftArrowBtnActive.texture;
            _sLeftArrowBtnStyle.onNormal.background = _leftArrowBtnNormal.texture;
            _sLeftArrowBtnStyle.onHover.background = _leftArrowBtnHover.texture;
            _sLeftArrowBtnStyle.onActive.background = _leftArrowBtnActive.texture;
            _sRightArrowBtnStyle = new GUIStyle(GUI.skin.button);
            _sRightArrowBtnStyle.normal.background = _rightArrowBtnNormal.texture;
            _sRightArrowBtnStyle.hover.background = _rightArrowBtnHover.texture;
            _sRightArrowBtnStyle.active.background = _rightArrowBtnActive.texture;
            _sRightArrowBtnStyle.onNormal.background = _rightArrowBtnNormal.texture;
            _sRightArrowBtnStyle.onHover.background = _rightArrowBtnHover.texture;
            _sRightArrowBtnStyle.onActive.background = _rightArrowBtnActive.texture;
            _sArrowsBtnLblStyle = new GUIStyle(GUI.skin.label);
            _sArrowsBtnLblStyle.margin.top = 10;
            _sArrowsBtnPrefixLblStyle = new GUIStyle(GUI.skin.label);
            _sArrowsBtnPrefixLblStyle.margin.top = 8;
            _sBoldFontBtnStyle = new GUIStyle(GUI.skin.button);
            _sBoldFontBtnStyle.fontStyle = FontStyle.Bold;
            _sSliderTipLblStyle = new GUIStyle(GUI.skin.label);
            _sSliderTipLblStyle.fontSize = 16;
            _sSliderTipLblStyle.fontStyle = FontStyle.Italic;
            _sSliderTipLblStyle.margin.top = 8;
            _sTooltipLblStyle = new GUIStyle(GUI.skin.label);
            _sTooltipLblStyle.fontSize = 16;
            _sTooltipLblStyle.fontStyle = FontStyle.Italic;
            _sTooltipLblStyle.alignment = TextAnchor.LowerLeft;
        }
    }

    private static string GetNextElem(string _currentlySelected, List<string> _elements)
    {
        if (!string.IsNullOrEmpty(_currentlySelected) && _elements.Count > 1)
        {
            string selected = _currentlySelected;
            int num = _elements.FindIndex((string x) => x.CompareTo(selected) == 0);
            if (num >= 0 && num + 1 < _elements.Count)
                return _elements[num + 1];
        }
        if (_elements.Count <= 0)
            return string.Empty;
        return _elements[0];
    }

    private static string GetPrevElem(string _currentlySelected, List<string> _elements)
    {
        if (!string.IsNullOrEmpty(_currentlySelected) && _elements.Count > 1)
        {
            string selected = _currentlySelected;
            int num = _elements.FindIndex((string x) => x.CompareTo(selected) == 0);
            if (num == 0)
                return _elements[_elements.Count - 1];
            if (num > 0)
                return _elements[num - 1];
        }
        if (_elements.Count <= 0)
            return string.Empty;
        return _elements[0];
    }

    public static string ArrowsButton(string prefix, GUILayoutOption prefixWidth, string tooltip, string selected, List<string> items, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(new GUIContent(string.Empty, tooltip), GUI.skin.label, options);
        GUILayout.Label(prefix, _sArrowsBtnPrefixLblStyle, new GUILayoutOption[] { prefixWidth });
        GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
        if (GUILayout.Button(string.Empty, _sLeftArrowBtnStyle, new GUILayoutOption[] { GUILayout.Width(40f), GUILayout.Height(40f) }))
            selected = GetPrevElem(selected, items);
        GUILayout.FlexibleSpace();
        GUILayout.Label(selected, _sArrowsBtnLblStyle, Array.Empty<GUILayoutOption>());
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(string.Empty, _sRightArrowBtnStyle, new GUILayoutOption[] { GUILayout.Width(40f), GUILayout.Height(40f) }))
            selected = GetNextElem(selected, items);
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
        return selected;
    }

    public static float Slider(string prefix, GUILayoutOption prefixWidth, string tooltip, float selected, float min, float max)
    {
        GUILayout.BeginHorizontal(new GUIContent(string.Empty, tooltip), GUI.skin.label, Array.Empty<GUILayoutOption>());
        GUILayout.BeginHorizontal(new GUILayoutOption[] { prefixWidth });
        GUILayout.Label(prefix, Array.Empty<GUILayoutOption>());
        GUILayout.Label(Convert.ToString(selected, CultureInfo.CurrentCulture), _sSliderTipLblStyle, Array.Empty<GUILayoutOption>());
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        selected = (float)Math.Round((double)GUILayout.HorizontalSlider(selected, min, max, Array.Empty<GUILayoutOption>()), MidpointRounding.AwayFromZero);
        GUILayout.EndHorizontal();
        return selected;
    }

    public static bool Toggle(string prefix, GUILayoutOption prefixWidth, string tooltip, bool selected)
    {
        GUILayout.BeginHorizontal(new GUIContent(string.Empty, tooltip), GUI.skin.label, Array.Empty<GUILayoutOption>());
        if (GUILayout.Button(prefix, GUI.skin.label, new GUILayoutOption[] { prefixWidth }))
            selected = !selected;
        selected = GUILayout.Toggle(selected, string.Empty, GUI.skin.toggle, Array.Empty<GUILayoutOption>());
        GUILayout.EndHorizontal();
        return selected;
    }

    public static bool Button(string prefix, GUILayoutOption prefixWidth, string tooltip, string btnText, params GUILayoutOption[] btnOptions)
    {
        GUILayout.BeginHorizontal(new GUIContent(string.Empty, tooltip), GUI.skin.label, Array.Empty<GUILayoutOption>());
        GUILayout.Label(prefix, new GUILayoutOption[] { prefixWidth });
        if (GUILayout.Button(btnText, btnOptions))
            return true;
        GUILayout.EndHorizontal();
        return false;
    }

    public static void Tooltip()
    {
        GUILayout.Label(GUI.tooltip, _sTooltipLblStyle, new GUILayoutOption[] { GUILayout.ExpandHeight(true) });
    }
}
