namespace CyclopsDockingMod.Controllers;
using System.Globalization;
using UnityEngine;

public class SignController : MonoBehaviour, IProtoEventListener
{
    public void OnProtoSerialize(ProtobufSerializer serializer)
    {
    }

    public void OnProtoDeserialize(ProtobufSerializer serializer)
    {
        GameObject gameObject = base.gameObject;
        if (((gameObject != null) ? gameObject.transform : null) != null)
        {
            try
            {
                foreach (object obj in base.gameObject.transform)
                {
                    Transform transform = (Transform)obj;
                    try
                    {
                        if (!string.IsNullOrEmpty(transform.name) && transform.name.StartsWith("Sign(Clone)", true, CultureInfo.InvariantCulture) && transform.gameObject != null)
                            Destroy(transform.gameObject);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
