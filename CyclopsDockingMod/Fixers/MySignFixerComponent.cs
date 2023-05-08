using UnityEngine;

namespace CyclopsDockingMod.Fixers
{
	public class MySignFixerComponent : MonoBehaviour
	{
		private void MyRestoreSignState()
		{
			if (base.enabled && base.transform != null && base.transform.parent != null)
			{
				Sign component = base.transform.parent.GetComponent<Sign>();
				if (component != null)
					component.OnProtoDeserialize(null);
			}
		}

		public void Awake()
		{
			if (base.enabled)
				base.Invoke("MyRestoreSignState", 1f);
		}
	}
}
