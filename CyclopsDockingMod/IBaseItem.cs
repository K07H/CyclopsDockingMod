namespace CyclopsDockingMod;
using UnityEngine;

public interface IBaseItem
{
    string ClassID { get; }
    string PrefabFileName { get; }
    TechType TechType { get; }
    GameObject GameObject { get; set; }

    GameObject GetGameObject();
    void RegisterItem();
}
