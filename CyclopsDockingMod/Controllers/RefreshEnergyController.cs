﻿using CyclopsDockingMod.Fixers;
using CyclopsDockingMod.Routing;
using UnityEngine;

namespace CyclopsDockingMod.Controllers
{
	public class RefreshEnergyController : MonoBehaviour
    {
        public float _lastCheck = -1f;

        public BasePart _bp;

        public void Update()
		{
			if (!base.enabled)
				return;
			if (this._bp == null || this._bp.dock == null || !SubControlFixer.Docked(this._bp.dock))
			{
				SubControlFixer.CleanUp(this._bp, this._bp.dock, false);
				UnityEngine.Object.Destroy(this, 2f);
				base.enabled = false;
				return;
			}
			if (Time.time > this._lastCheck + 4f || this._lastCheck < 0f)
			{
				this._lastCheck = Time.time;
				SubRoot subRoot = this._bp.sub ?? BaseFixer.GetSubRoot(this._bp.dock);
				if (subRoot != null)
				{
					if (this._bp.sub == null)
						this._bp.sub = subRoot;
					if (SubControlFixer.DockedSubs[this._bp.dock].sub == null)
						SubControlFixer.DockedSubs[this._bp.dock].sub = subRoot;
					Sign component = base.gameObject.GetComponent<Sign>();
					if (component != null)
					{
						string text = string.Format(ConfigOptions.LblCyclopsDocked, BaseFixer.GetCyclopsEnergy(subRoot));
						component.text = text;
						component.signInput.text = text;
					}
					if (!this._bp.trapOpened)
					{
						SubControlFixer.ToggleTrap(subRoot, true, false);
						this._bp.trapOpened = true;
					}
					if (!AutoPilot.SubsPlayingRoutes.ContainsKey(this._bp.dock))
						AutoPilot.SubsPlayingRoutes.Add(this._bp.dock, new SubRoutePlaying());
					if (ConfigOptions.EnableAutopilotFeature)
						AutoPilot.RefreshHud(subRoot, true);
					UnityEngine.Object.Destroy(this, 2f);
					base.enabled = false;
				}
			}
		}
	}
}
