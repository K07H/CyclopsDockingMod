using CyclopsDockingMod.UI;
using UnityEngine;

namespace CyclopsDockingMod.Fixers
{
	public static class UtilsFixer
	{
		public static bool UpdateCusorLockState_Prefix()
		{
			if (CyclopsDockingModUI._isToggled)
				CyclopsDockingModUI._toggleDiff = UWE.Utils.alwaysLockCursor || !Cursor.visible;
			return true;
		}

		public static void UpdateCusorLockState_Postfix()
		{
			if (CyclopsDockingModUI._isToggled && CyclopsDockingModUI._toggleDiff != (UWE.Utils.alwaysLockCursor || !Cursor.visible))
				CyclopsDockingModUI._previousState = !CyclopsDockingModUI._previousState;
		}
	}
}
