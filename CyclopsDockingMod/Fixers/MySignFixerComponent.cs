namespace CyclopsDockingMod.Fixers;
using UnityEngine;

public class MySignFixerComponent : MonoBehaviour
{
    private void MyRestoreSignState()
    {
        if (enabled && transform != null && transform.parent != null)
        {
            Sign component = transform.parent.GetComponent<Sign>();
            if (component != null)
                component.OnProtoDeserialize(null);
        }
    }

    public void Awake()
    {
        if (enabled)
            Invoke("MyRestoreSignState", 1f);
    }
}
