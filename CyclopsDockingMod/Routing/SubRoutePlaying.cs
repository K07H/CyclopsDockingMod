using UnityEngine;

namespace CyclopsDockingMod.Routing
{
	internal class SubRoutePlaying
	{
		public bool IsPlayingRoute;

		public bool UndockStartPlaying;

		public int SelectedRoute = -1;

		public float LastTime = -1f;

		public Vector3? LastPos;

		public float? LastAngle;

		public Route Playing;

		public int CurrentRouteIndex;

		public bool IsStuck;

		public bool AlternateUnstuck = true;

		public bool AlternateUnstuckBis;
	}
}
