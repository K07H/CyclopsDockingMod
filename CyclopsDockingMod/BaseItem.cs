namespace CyclopsDockingMod;

using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using UnityEngine;

public abstract class BaseItem: IBaseItem
{
    protected BaseItem() 
    {

    }

    public const string DefaultResourcePath = "WorldEntities/Environment/Wrecks/";

    public bool IsRegistered = false;

    public bool IsHabitatBuilder = false;

    public GameObject GameObject { get; set; }

    public RecipeData Recipe { get; set; }

    public virtual string ClassID { get; set; }

    public virtual string PrefabFileName { get; set; }

    public virtual TechType TechType { get; set; }

    public virtual void RegisterItem()
    {
        if (!this.IsRegistered && this.GameObject != null)
        {
            if (this.Recipe != null)
                CraftDataHandler.SetRecipeData(this.TechType, this.Recipe);

            var info = new PrefabInfo(ClassID, PrefabFileName, TechType);

            var customPrefab = new CustomPrefab(info);
            customPrefab.SetGameObject(GetGameObject);
            customPrefab.Register();
            this.IsRegistered = true;
        }
    }

    public abstract GameObject GetGameObject();
}
