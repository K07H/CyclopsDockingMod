using System;
using System.Collections.Generic;
using System.Globalization;
using CyclopsDockingMod.Fixers;
using UnityEngine;

namespace CyclopsDockingMod.UI
{
	public class CyclopsDockingModUI : MonoBehaviour
	{
		private static float CDMScreenSttPosX { get; set; } = (float)Screen.width / 7f;

		private static float CDMScreenSttPosY { get; set; } = (float)Screen.height / 7f;

        public static KeyCode OpenUI = KeyCode.F7;

        private static readonly float CDMScreenWidth = 700f;

        private static readonly float CDMScreenHeight = 550f;

        private static readonly float CDMScreenMinWidth = 400f;

        private static readonly float CDMScreenMinHeight = 26f;

        private static readonly GUILayoutOption FirstColumnWidth = GUILayout.Width(350f);

        private static float CDMScreenMaxPosX = Math.Max((float)Screen.width - CyclopsDockingModUI.CDMScreenWidth, 0f);

        private static float CDMScreenMaxPosY = Math.Max((float)Screen.height - CyclopsDockingModUI.CDMScreenHeight, 0f);

        public static Rect CDMScreen = new Rect(CyclopsDockingModUI.CDMScreenSttPosX, CyclopsDockingModUI.CDMScreenSttPosY, CyclopsDockingModUI.CDMScreenWidth, CyclopsDockingModUI.CDMScreenHeight);

        private Vector2 _scrollPosition = Vector2.zero;

        public static bool _toggleDiff = false;

        public static bool _previousState = false;

        public static bool _isToggled = false;

        private bool _isOpenUIGrabKey;

        private bool _isManualDockingGrabKey;

        private bool ShowUI;

        private bool IsMinimized;

        public static string CfgOpenUIKeyText = CyclopsDockingModUI.OpenUI.ToString();

        public static float CfgRechargeSpeed = 25f;

        public static float CfgRechargeSpeedOrig = 25f;

        public static int CfgAutoDockingRange = 13;

        public static int CfgAutoDockingRangeOrig = 13;

        public static bool CfgManualDockingMode = false;

        public static bool CfgManualDockingModeOrig = false;

        public static string CfgManualDockingKeyText = SubControlFixer.ManualDockingKey.ToString();

        public static string CfgLadderTintColor = "None";

        public static string CfgLadderTintColorOrig = "None";

        public static int CfgLadderTintColorVal = 0;

        public static List<string> CfgLadderTintColors = new List<string> { "None", "Cyan", "Black", "White", "Red", "Yellow", "Green", "Blue", "Purple", "Pink" };

        public static int CfgSignTextScale = 1;

        public static int CfgSignTextScaleOrig = 1;

        public static string CfgSignTextColor = "Yellow";

        public static string CfgSignTextColorOrig = "Yellow";

        public static int CfgSignTextColorVal = 4;

        public static List<string> CfgSignTextColors = new List<string> { "Cyan", "Black", "White", "Red", "Yellow", "Green", "Blue", "Purple" };

        public static bool CfgSignBackgroundVisible = true;

        public static bool CfgSignBackgroundVisibleOrig = true;

        public static bool CfgSimpleDockingManeuver = false;

        public static bool CfgSimpleDockingManeuverOrig = false;

        private void ToggleCursor(bool lockCursor)
		{
			if (CyclopsDockingModUI._isToggled)
				UWE.Utils.lockCursor = CyclopsDockingModUI._previousState;
			else
			{
				CyclopsDockingModUI._previousState = UWE.Utils.lockCursor;
				UWE.Utils.lockCursor = lockCursor;
			}
			CyclopsDockingModUI._isToggled = !CyclopsDockingModUI._isToggled;
		}

		private void CollapseWindow()
		{
			this.IsMinimized = !this.IsMinimized;
			if (this.IsMinimized)
			{
				CyclopsDockingModUI.CDMScreen = new Rect(CyclopsDockingModUI.CDMScreenSttPosX, CyclopsDockingModUI.CDMScreenSttPosY, CyclopsDockingModUI.CDMScreenMinWidth, CyclopsDockingModUI.CDMScreenMinHeight);
				GUI.skin.window.alignment = TextAnchor.UpperLeft;
				GUI.skin.window.padding.left = 15;
				return;
			}
			CyclopsDockingModUI.CDMScreen = new Rect(CyclopsDockingModUI.CDMScreenSttPosX, CyclopsDockingModUI.CDMScreenSttPosY, CyclopsDockingModUI.CDMScreenWidth, CyclopsDockingModUI.CDMScreenHeight);
			GUI.skin.window.padding.left = 0;
			GUI.skin.window.alignment = TextAnchor.UpperCenter;
		}

		private void CloseWindow()
		{
			this.ShowUI = false;
			this.ToggleCursor(!this.ShowUI);
		}

		private void ScreenMenuBox()
		{
			if (GUI.Button(new Rect(CyclopsDockingModUI.CDMScreen.width - 54f, 0f, 27f, 26f), this.IsMinimized ? "☐" : "-", this.IsMinimized ? StylesHelper._sBoldFontBtnStyle : GUI.skin.button))
				this.CollapseWindow();
			if (GUI.Button(new Rect(CyclopsDockingModUI.CDMScreen.width - 27f, 0f, 27f, 26f), "X", GUI.skin.button))
				this.CloseWindow();
		}

        private void ModOptionsBox()
        {
            GUILayout.Space(23f);
            using (var optionsScope = new GUILayout.ScrollViewScope(_scrollPosition, GUI.skin.box, GUILayout.Height(CDMScreenHeight - 32f)))
            {
                if (StylesHelper.Button("Open mod settings key:", FirstColumnWidth, "Defines the keyboard key that is used to open Cyclops Docking mod settings.", CfgOpenUIKeyText, GUILayout.Height(32f)))
                    if (!_isOpenUIGrabKey && !_isManualDockingGrabKey)
                    {
                        _isOpenUIGrabKey = true;
                        CfgOpenUIKeyText = string.Empty;
                    }
                CfgRechargeSpeed = StylesHelper.Slider("Powercells charge speed:", FirstColumnWidth, "The speed at which Cyclops powercells gets charged when Cyclops is docked.", CfgRechargeSpeed, 1.0f, 100.0f);
                CfgAutoDockingRange = (int)Math.Round(StylesHelper.Slider("Automatic docking range:", FirstColumnWidth, "The distance (in meters) at which the automatic docking procedure will trigger.", CfgAutoDockingRange, 10.0f, 50.0f), MidpointRounding.AwayFromZero);
                CfgManualDockingMode = StylesHelper.Toggle("Manual docking mode:", FirstColumnWidth, "Defines if docking is manual (with shortcut key) or automatic.", CfgManualDockingMode);
                if (StylesHelper.Button("Manual docking key:", FirstColumnWidth, "Defines the keyboard key that is used to start manual docking.", CfgManualDockingKeyText, GUILayout.Height(32f)))
                    if (!_isOpenUIGrabKey && !_isManualDockingGrabKey)
                    {
                        _isManualDockingGrabKey = true;
                        CfgManualDockingKeyText = string.Empty;
                    }
                CfgLadderTintColor = StylesHelper.ArrowsButton("Ladder tint color:", FirstColumnWidth, "The color of the ladder used to climb into Cyclops.", CfgLadderTintColor, CfgLadderTintColors);
                CfgSignTextScale = (int)Math.Round(StylesHelper.Slider("Sign's text size:", FirstColumnWidth, "The size of sign's text inside the docking corridor.", CfgSignTextScale, -3.0f, 3.0f), MidpointRounding.AwayFromZero);
                CfgSignTextColor = StylesHelper.ArrowsButton("Sign's text color:", FirstColumnWidth, "The color of sign's text inside the docking corridor.", CfgSignTextColor, CfgSignTextColors);
                CfgSignBackgroundVisible = StylesHelper.Toggle("Sign's background visible:", FirstColumnWidth, "Defines if sign's background in the docking corridor is visible or not.", CfgSignBackgroundVisible);
                CfgSimpleDockingManeuver = StylesHelper.Toggle("Simplified docking maneuver:", FirstColumnWidth, "Defines if simplified docking maneuver is used or not.", CfgSimpleDockingManeuver);
                StylesHelper.Tooltip();
            }

            if (CfgRechargeSpeed != CfgRechargeSpeedOrig)
            {
                CfgRechargeSpeedOrig = CfgRechargeSpeed;
                SubRootFixer.CyclopsRechargeSpeed = CfgRechargeSpeed * SubRootFixer.CyclopsRechargeRatioD;
                var powerCellChargeSpeed = Math.Round(CfgRechargeSpeed, 0, MidpointRounding.AwayFromZero);
                if (powerCellChargeSpeed < 1d)
                    powerCellChargeSpeed = 1d;
                if (powerCellChargeSpeed > 100d)
                    powerCellChargeSpeed = 100d;
                ConfigOptions.UpdateConfigFile(Environment.NewLine + "powercellsChargeSpeed=" + Environment.NewLine, Environment.NewLine + "powercellsChargeSpeed=" + Convert.ToInt32(powerCellChargeSpeed, CultureInfo.InvariantCulture.NumberFormat).ToString("D", CultureInfo.InvariantCulture.NumberFormat) + Environment.NewLine);
            }
            if (CfgAutoDockingRange != CfgAutoDockingRangeOrig)
            {
                CfgAutoDockingRangeOrig = CfgAutoDockingRange;
                SubControlFixer.AutoDockingTriggerSqrRange = Mathf.Pow(CfgAutoDockingRange, 2.0f);
                SubControlFixer.AutoDockingUndockSqrRange = Mathf.Pow(CfgAutoDockingRange + 5.0f, 2.0f);
                SubControlFixer.AutoDockingDetectSqrRange = Mathf.Pow(CfgAutoDockingRange + 6.0f, 2.0f);
                ConfigOptions.UpdateConfigFile(Environment.NewLine + "autoDockingRange=" + Environment.NewLine, Environment.NewLine + "autoDockingRange=" + CfgAutoDockingRange.ToString("D", CultureInfo.InvariantCulture.NumberFormat) + Environment.NewLine);
            }
            if (CfgManualDockingMode != CfgManualDockingModeOrig)
            {
                CfgManualDockingModeOrig = CfgManualDockingMode;
                SubControlFixer.AutoDocking = !CfgManualDockingMode;
                ConfigOptions.UpdateConfigFile(Environment.NewLine + "enableManualDocking=" + Environment.NewLine, Environment.NewLine + "enableManualDocking=" + (CfgManualDockingMode ? "true" : "false") + Environment.NewLine);
                ErrorMessage.AddDebug("Switched to " + (CfgManualDockingMode ? "manual" : "automatic") + " docking.");
            }
            if (CfgLadderTintColor != CfgLadderTintColorOrig)
            {
                CfgLadderTintColorOrig = CfgLadderTintColor;
                CfgLadderTintColorVal = 0;
                switch (CfgLadderTintColor)
                {
                    case "None": CfgLadderTintColorVal = 0; break;
                    case "Cyan": CfgLadderTintColorVal = 1; break;
                    case "Black": CfgLadderTintColorVal = 2; break;
                    case "White": CfgLadderTintColorVal = 3; break;
                    case "Red": CfgLadderTintColorVal = 4; break;
                    case "Yellow": CfgLadderTintColorVal = 5; break;
                    case "Green": CfgLadderTintColorVal = 6; break;
                    case "Blue": CfgLadderTintColorVal = 7; break;
                    case "Purple": CfgLadderTintColorVal = 8; break;
                    case "Pink": CfgLadderTintColorVal = 9; break;
                }
                ConfigOptions.UpdateConfigFile(Environment.NewLine + "ladderTintColor=" + Environment.NewLine, Environment.NewLine + "ladderTintColor=" + CfgLadderTintColorVal.ToString("D", CultureInfo.InvariantCulture.NumberFormat) + Environment.NewLine);
                ErrorMessage.AddDebug("Ladder tint color set to " + CfgLadderTintColor + ".");
            }
            if (CfgSignTextScale != CfgSignTextScaleOrig)
            {
                CfgSignTextScaleOrig = CfgSignTextScale;
                ConfigOptions.UpdateConfigFile(Environment.NewLine + "defaultTextSize=" + Environment.NewLine, Environment.NewLine + "defaultTextSize=" + CfgSignTextScale.ToString("D", CultureInfo.InvariantCulture.NumberFormat) + Environment.NewLine);
            }
            if (CfgSignTextColor != CfgSignTextColorOrig)
            {
                CfgSignTextColorOrig = CfgSignTextColor;
                CfgSignTextColorVal = 4;
                switch (CfgSignTextColor)
                {
                    case "Cyan": CfgSignTextColorVal = 0; break;
                    case "Black": CfgSignTextColorVal = 1; break;
                    case "White": CfgSignTextColorVal = 2; break;
                    case "Red": CfgSignTextColorVal = 3; break;
                    case "Yellow": CfgSignTextColorVal = 4; break;
                    case "Green": CfgSignTextColorVal = 5; break;
                    case "Blue": CfgSignTextColorVal = 6; break;
                    case "Purple": CfgSignTextColorVal = 7; break;
                }
                ConfigOptions.UpdateConfigFile(Environment.NewLine + "defaultTextColor=" + Environment.NewLine, Environment.NewLine + "defaultTextColor=" + CfgSignTextColorVal.ToString("D", CultureInfo.InvariantCulture.NumberFormat) + Environment.NewLine);
                ErrorMessage.AddDebug("Sign default text color set to " + CfgSignTextColor + ".");
            }
            if (CfgSignBackgroundVisible != CfgSignBackgroundVisibleOrig)
            {
                CfgSignBackgroundVisibleOrig = CfgSignBackgroundVisible;
                ConfigOptions.UpdateConfigFile(Environment.NewLine + "defaultBackground=" + Environment.NewLine, Environment.NewLine + "defaultBackground=" + (CfgSignBackgroundVisible ? "true" : "false") + Environment.NewLine);
                ErrorMessage.AddDebug("Sign background is now " + (CfgSignBackgroundVisible ? "visible" : "hidden") + " by default.");
            }
            if (CfgSimpleDockingManeuver != CfgSimpleDockingManeuverOrig)
            {
                CfgSimpleDockingManeuverOrig = CfgSimpleDockingManeuver;
                SubControlFixer.SimpleDocking = CfgSimpleDockingManeuver;
                ConfigOptions.UpdateConfigFile(Environment.NewLine + "simpleDockingManeuver=" + Environment.NewLine, Environment.NewLine + "simpleDockingManeuver=" + (CfgSimpleDockingManeuver ? "true" : "false") + Environment.NewLine);
                ErrorMessage.AddDebug("Simplified docking maneuver " + (CfgSimpleDockingManeuver ? "enabled" : "disabled") + ".");
            }
        }

        private void DrawCDMScreen(int windowID)
		{
			CyclopsDockingModUI.CDMScreenSttPosX = Math.Min(Math.Max(CyclopsDockingModUI.CDMScreen.x, 0f), CyclopsDockingModUI.CDMScreenMaxPosX);
			CyclopsDockingModUI.CDMScreenSttPosY = Math.Min(Math.Max(CyclopsDockingModUI.CDMScreen.y, 0f), CyclopsDockingModUI.CDMScreenMaxPosY);
			this.ScreenMenuBox();
			if (!this.IsMinimized)
				this.ModOptionsBox();
			GUI.DragWindow(new Rect(0f, 0f, 10000f, 10000f));
		}

		private void InitWindow()
		{
			CyclopsDockingModUI.CDMScreen = GUI.Window(this.GetHashCode(), CyclopsDockingModUI.CDMScreen, new GUI.WindowFunction(this.DrawCDMScreen), "Cyclops Docking mod v2.1.1, by OSubMarin", GUI.skin.window);
		}

		private void Update()
		{
			if (Input.GetKeyDown(CyclopsDockingModUI.OpenUI))
			{
				this.ShowUI = !this.ShowUI;
				this.ToggleCursor(!this.ShowUI);
			}
		}

		private void OnGUI()
		{
			GUI.skin = StylesHelper.CDMUISkin;
			if (!StylesHelper.Initialized)
			{
				StylesHelper.InitStyles();
			}
			if (this._isOpenUIGrabKey && Event.current.isKey && Event.current.type == EventType.KeyDown)
			{
				this._isOpenUIGrabKey = false;
				CyclopsDockingModUI.OpenUI = Event.current.keyCode;
				CyclopsDockingModUI.CfgOpenUIKeyText = CyclopsDockingModUI.OpenUI.ToString();
				ConfigOptions.UpdateConfigFile(Environment.NewLine + "openModSettingsKey=" + Environment.NewLine, Environment.NewLine + "openModSettingsKey=" + CyclopsDockingModUI.CfgOpenUIKeyText + Environment.NewLine);
				ErrorMessage.AddDebug("Open mod settings key is now \"" + CyclopsDockingModUI.CfgOpenUIKeyText + "\".");
			}
			if (this._isManualDockingGrabKey && Event.current.isKey && Event.current.type == EventType.KeyDown)
			{
				this._isManualDockingGrabKey = false;
				SubControlFixer.ManualDockingKey = Event.current.keyCode;
				CyclopsDockingModUI.CfgManualDockingKeyText = SubControlFixer.ManualDockingKey.ToString();
				ConfigOptions.UpdateConfigFile(Environment.NewLine + "manualDockingKey=" + Environment.NewLine, Environment.NewLine + "manualDockingKey=" + CyclopsDockingModUI.CfgManualDockingKeyText + Environment.NewLine);
				ErrorMessage.AddDebug("Manual docking key is now \"" + CyclopsDockingModUI.CfgManualDockingKeyText + "\".");
			}
			if (this.ShowUI)
				this.InitWindow();
		}
	}
}
