using UnityEngine;

namespace CyclopsDockingMod
{
    public abstract class BaseItem : SMLHelper.V2.Assets.ModPrefab, IBaseItem
    {
        protected BaseItem() : base("", "") { }

        public const string DefaultResourcePath = "WorldEntities/Environment/Wrecks/";

        public bool IsRegistered = false;

        public bool IsHabitatBuilder = false;

        public GameObject GameObject { get; set; }

        public SMLHelper.V2.Crafting.TechData Recipe { get; set; }

        public virtual void RegisterItem()
        {
            if (this.IsRegistered == false && this.GameObject != null)
            {
                if (this.Recipe != null)
                    SMLHelper.V2.Handlers.CraftDataHandler.SetTechData(this.TechType, this.Recipe);

                SMLHelper.V2.Handlers.PrefabHandler.RegisterPrefab(this);

                this.IsRegistered = true;
            }
        }
    }
}
