using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace CyclopsDockingMod
{
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
			if (!StylesHelper.Initialized)
			{
				StylesHelper.Initialized = true;
				StylesHelper._sLeftArrowBtnStyle = new GUIStyle(GUI.skin.button);
				StylesHelper._sLeftArrowBtnStyle.normal.background = StylesHelper._leftArrowBtnNormal.texture;
				StylesHelper._sLeftArrowBtnStyle.hover.background = StylesHelper._leftArrowBtnHover.texture;
				StylesHelper._sLeftArrowBtnStyle.active.background = StylesHelper._leftArrowBtnActive.texture;
				StylesHelper._sLeftArrowBtnStyle.onNormal.background = StylesHelper._leftArrowBtnNormal.texture;
				StylesHelper._sLeftArrowBtnStyle.onHover.background = StylesHelper._leftArrowBtnHover.texture;
				StylesHelper._sLeftArrowBtnStyle.onActive.background = StylesHelper._leftArrowBtnActive.texture;
				StylesHelper._sRightArrowBtnStyle = new GUIStyle(GUI.skin.button);
				StylesHelper._sRightArrowBtnStyle.normal.background = StylesHelper._rightArrowBtnNormal.texture;
				StylesHelper._sRightArrowBtnStyle.hover.background = StylesHelper._rightArrowBtnHover.texture;
				StylesHelper._sRightArrowBtnStyle.active.background = StylesHelper._rightArrowBtnActive.texture;
				StylesHelper._sRightArrowBtnStyle.onNormal.background = StylesHelper._rightArrowBtnNormal.texture;
				StylesHelper._sRightArrowBtnStyle.onHover.background = StylesHelper._rightArrowBtnHover.texture;
				StylesHelper._sRightArrowBtnStyle.onActive.background = StylesHelper._rightArrowBtnActive.texture;
				StylesHelper._sArrowsBtnLblStyle = new GUIStyle(GUI.skin.label);
				StylesHelper._sArrowsBtnLblStyle.margin.top = 10;
				StylesHelper._sArrowsBtnPrefixLblStyle = new GUIStyle(GUI.skin.label);
				StylesHelper._sArrowsBtnPrefixLblStyle.margin.top = 8;
				StylesHelper._sBoldFontBtnStyle = new GUIStyle(GUI.skin.button);
				StylesHelper._sBoldFontBtnStyle.fontStyle = FontStyle.Bold;
				StylesHelper._sSliderTipLblStyle = new GUIStyle(GUI.skin.label);
				StylesHelper._sSliderTipLblStyle.fontSize = 16;
				StylesHelper._sSliderTipLblStyle.fontStyle = FontStyle.Italic;
				StylesHelper._sSliderTipLblStyle.margin.top = 8;
				StylesHelper._sTooltipLblStyle = new GUIStyle(GUI.skin.label);
				StylesHelper._sTooltipLblStyle.fontSize = 16;
				StylesHelper._sTooltipLblStyle.fontStyle = FontStyle.Italic;
				StylesHelper._sTooltipLblStyle.alignment = TextAnchor.LowerLeft;
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
			GUILayout.Label(prefix, StylesHelper._sArrowsBtnPrefixLblStyle, new GUILayoutOption[] { prefixWidth });
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button(string.Empty, StylesHelper._sLeftArrowBtnStyle, new GUILayoutOption[] { GUILayout.Width(40f), GUILayout.Height(40f) }))
				selected = StylesHelper.GetPrevElem(selected, items);
			GUILayout.FlexibleSpace();
			GUILayout.Label(selected, StylesHelper._sArrowsBtnLblStyle, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(string.Empty, StylesHelper._sRightArrowBtnStyle, new GUILayoutOption[] { GUILayout.Width(40f), GUILayout.Height(40f) }))
				selected = StylesHelper.GetNextElem(selected, items);
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			return selected;
		}

		public static float Slider(string prefix, GUILayoutOption prefixWidth, string tooltip, float selected, float min, float max)
		{
			GUILayout.BeginHorizontal(new GUIContent(string.Empty, tooltip), GUI.skin.label, Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(new GUILayoutOption[] { prefixWidth });
			GUILayout.Label(prefix, Array.Empty<GUILayoutOption>());
			GUILayout.Label(Convert.ToString(selected, CultureInfo.CurrentCulture.NumberFormat), StylesHelper._sSliderTipLblStyle, Array.Empty<GUILayoutOption>());
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
			GUILayout.Label(GUI.tooltip, StylesHelper._sTooltipLblStyle, new GUILayoutOption[] { GUILayout.ExpandHeight(true) });
		}
	}
}
