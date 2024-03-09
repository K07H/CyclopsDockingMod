using UnityEngine;
#if SUBNAUTICA_NAUTILUS
using Nautilus.Assets;
using System.Diagnostics.CodeAnalysis;
#endif

namespace CyclopsDockingMod
{
#if SUBNAUTICA_NAUTILUS
    public abstract class BaseItem : Nautilus.Assets.CustomPrefab, IBaseItem
    {
        [SetsRequiredMembers]
        public BaseItem(string classID, string name, string desc, string icon) : this(
            PrefabInfo.WithTechType(classID, name, desc, unlockAtStart: true)
            .WithFileName(DefaultResourcePath + classID)
            .WithIcon(AssetsHelper.Assets.LoadAsset<Sprite>(icon)))
        { }

        [SetsRequiredMembers]
        public BaseItem(string classID, string name, string desc, Atlas.Sprite icon) : this(
            PrefabInfo.WithTechType(classID, name, desc, unlockAtStart: true)
            .WithFileName(DefaultResourcePath + classID)
            .WithIcon(icon))
        { }

        [SetsRequiredMembers]
        protected BaseItem(PrefabInfo info) : base(info)
        {
            SetGameObject(GetGameObject);
        }
#else
    public abstract class BaseItem : SMLHelper.V2.Assets.ModPrefab, IBaseItem
    {
        protected BaseItem() : base("", "") { }
#endif

        public const string DefaultResourcePath = "WorldEntities/Environment/Wrecks/";

        public bool IsRegistered = false;

        public bool IsHabitatBuilder = false;

        public GameObject GameObject { get; set; }

#if SUBNAUTICA_NAUTILUS
        public Nautilus.Crafting.RecipeData Recipe { get; set; }
        public string ClassID => Info.ClassID;
        public string PrefabFileName => Info.PrefabFileName;
        public TechType TechType => Info.TechType;
        public abstract GameObject GetGameObject();
#else
        public SMLHelper.V2.Crafting.TechData Recipe { get; set; }
#endif

        public virtual void RegisterItem()
        {
            if (this.IsRegistered == false && this.GameObject != null)
            {
                if (this.Recipe != null)
#if SUBNAUTICA_NAUTILUS
                    Nautilus.Handlers.CraftDataHandler.SetRecipeData(this.Info.TechType, this.Recipe);

                this.Register();
#else
                    SMLHelper.V2.Handlers.CraftDataHandler.SetTechData(this.TechType, this.Recipe);

                SMLHelper.V2.Handlers.PrefabHandler.RegisterPrefab(this);
#endif

                this.IsRegistered = true;
            }
        }
    }
}
