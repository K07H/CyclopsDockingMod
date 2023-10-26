namespace CyclopsDockingMod.Routing;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyInputField : uGUI_InputField
{
    private void ChangeName(string str)
    {
        SubRoot subRoot = Utils.FindAncestorWithComponent<SubRoot>(gameObject);
        if (this.CurrentVal != str && subRoot != null)
        {
            string id = subRoot.GetComponent<PrefabIdentifier>().Id;
            foreach (Route route in AutoPilot.Routes)
                if (route.Id == AutoPilot.SubsPlayingRoutes[id].SelectedRoute)
                {
                    route.Name = str;
                    break;
                }
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        this.CurrentVal = text;
        AvatarInputHandler.main.gameObject.SetActive(false);
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        AvatarInputHandler.main.gameObject.SetActive(true);
        this.ChangeName(text);
        base.OnDeselect(eventData);
    }

    protected override void OnDestroy()
    {
        GameObject gameObject = AvatarInputHandler.main.gameObject;
        if (gameObject != null)
            gameObject.SetActive(true);
        base.OnDestroy();
    }

    public string CurrentVal;
}
