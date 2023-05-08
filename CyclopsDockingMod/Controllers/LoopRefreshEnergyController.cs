using CyclopsDockingMod.Fixers;
using UnityEngine;

namespace CyclopsDockingMod.Controllers
{
	public class LoopRefreshEnergyController : MonoBehaviour
    {
        public float _lastCheck = -1f;

        public BasePart _bp;

        public void Update()
		{
			if (base.enabled && GameModeUtils.RequiresPower() && this._bp != null && (this._lastCheck < 0f || Time.time > this._lastCheck + 40f))
			{
				this._lastCheck = Time.time;
				SubRoot subRoot = this._bp.sub ?? ((this._bp.dock != null) ? BaseFixer.GetSubRoot(this._bp.dock) : null);
				if (subRoot != null)
				{
					Sign component = base.gameObject.GetComponent<Sign>();
					if (component != null)
					{
						string text = string.Format(ConfigOptions.LblCyclopsDocked, BaseFixer.GetCyclopsEnergy(subRoot));
						component.text = text;
						component.signInput.text = text;
					}
				}
			}
		}
	}
}
