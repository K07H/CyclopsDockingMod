using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CyclopsDockingMod.Controllers
{
	public class RecordRouteController : MonoBehaviour
    {
        public readonly List<Vector3> RecordedWayPoints = new List<Vector3>();

        public string RouteName;

        public int RouteSpeed = 2;

        public Vector3? BasePartPosStt;

        public Vector3? BasePartPosEnd;

        public bool IsRecording;

        private Transform _outerHatch;

        private float _lastRec = -1f;

        public void Reset()
		{
			this.RecordedWayPoints.Clear();
			this.RouteName = null;
			this.RouteSpeed = 2;
			this.BasePartPosStt = null;
			this.BasePartPosEnd = null;
			this._outerHatch = base.gameObject.transform.Find("CyclopsMeshAnimated/submarine_outer_hatch_01");
			this._lastRec = -1f;
		}

		public void LateUpdate()
		{
			if (!base.enabled || !this.IsRecording || this._outerHatch == null)
				return;
			if (this._lastRec < 0f || Time.time > this._lastRec + 3f)
			{
				this._lastRec = Time.time;
				if (this.RecordedWayPoints.Count == 0 || (this.RecordedWayPoints.Last<Vector3>() - this._outerHatch.position).sqrMagnitude > 196f)
					this.RecordedWayPoints.Add(this._outerHatch.position);
			}
		}
	}
}
