#if SUBNAUTICA_NAUTI
using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Diagnostics.CodeAnalysis;
#else
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
#endif
using UnityEngine;

namespace CyclopsDockingMod
{
#if SUBNAUTICA_NAUTI
    public abstract class BaseItem : Nautilus.Assets.CustomPrefab, IBaseItem
    {
        [SetsRequiredMembers]
        public BaseItem(string classID, string name, string desc, string icon) : this(PrefabInfo.WithTechType(classID, name, desc, unlockAtStart: true).WithFileName(DefaultResourcePath + classID).WithIcon(AssetsHelper.Assets.LoadAsset<Sprite>(icon)))
        {
        }

        [SetsRequiredMembers]
        public BaseItem(string classID, string name, string desc, Atlas.Sprite icon) : this(PrefabInfo.WithTechType(classID, name, desc, unlockAtStart: true).WithFileName(DefaultResourcePath + classID).WithIcon(icon))
        {
        }

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

#if SUBNAUTICA_NAUTI
        public RecipeData Recipe { get; set; }
        public string ClassID => Info.ClassID;
        public string PrefabFileName => Info.PrefabFileName;
        public TechType TechType => Info.TechType;
        public abstract GameObject GetGameObject();
#else
        public TechData Recipe { get; set; }
#endif

        public virtual void RegisterItem()
        {
            if (this.IsRegistered == false && this.GameObject != null)
            {
                if (this.Recipe != null)
#if SUBNAUTICA_NAUTI
                    CraftDataHandler.SetRecipeData(this.Info.TechType, this.Recipe);
#else
                    CraftDataHandler.SetTechData(this.TechType, this.Recipe);
#endif

#if SUBNAUTICA_NAUTI
                this.Register();
#else
                PrefabHandler.RegisterPrefab(this);
#endif

                this.IsRegistered = true;
            }
        }
    }
}
