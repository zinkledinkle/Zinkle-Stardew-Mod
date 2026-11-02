using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Tools;
using ZinkleStardewMod.content;

namespace ZinkleStardewMod.Content.ContentTypes;

public abstract class ModItem : IModContent
{
    public virtual void Load() { }
    public virtual string InternalName => $"{ModEntry.Instance.ModManifest.UniqueID}_{GetType().Name}";
    public abstract string DisplayName { get; }
    public virtual string Description { get; } = "";
    public virtual string SpriteDirectory { get; } = "Objects";
    public string TexturePath => $"Mods/{ModEntry.Instance.ModManifest.UniqueID}/{SpriteDirectory}";
    public abstract int SpriteIndex { get; }
    public int Category { get; protected set; } = Categories.Junk;
    public string Type { get; protected set; } = Types.Basic;
    public int Price { get; protected set; } = 1;
    private readonly HashSet<string> _contextTags = [];
    public IReadOnlyList<string> ContextTags => [.._contextTags];
    public bool IsGiftable
    {
        get; protected set
        {
            if (value)
                _contextTags.Remove("not_giftable");
            else
                _contextTags.Add("not_giftable");
        }
    } = false;
    public bool CanDonateToMuseum
    {
        get; protected set
        {
            if (value)
            {
                _contextTags.Remove("not_museum_donatable");
                _contextTags.Add("museum_donatable");
            }
            else
            {
                _contextTags.Remove("museum_donatable");
                _contextTags.Add("not_museum_donatable");
            }
        }
    } = false;
    public bool Placeable
    {
        get; protected set
        {
            if (value)
            {
                _contextTags.Remove("not_placeable");
                _contextTags.Add("placeable");
            }
            else
            {
                _contextTags.Remove("placeable");
                _contextTags.Add("not_placeable");
            }
        }
    } = false;
    public bool CanDropOnDeath
    {
        get; protected set
        {
            if (value)
                _contextTags.Remove("prevent_loss_on_death");
            else
                _contextTags.Add("prevent_loss_on_death");
        }
    } = true;
    public bool Torch
    {
        get; protected set
        {
            if (value)
                _contextTags.Add("torch_item");
            else
                _contextTags.Remove("torch_item");
        }
    } = false;

    public virtual void SetProperties() { }
    public virtual bool CanAttachTo(Tool tool, int slot) => false;
    public virtual IEnumerable<(string, int)> GetRecipe(ref int outputAmount) => [];

    #region Helpers
    public static class Categories
    {
        public const int Artisan = SObject.artisanGoodsCategory;
        public const int Bait = SObject.baitCategory;
        public const int BigCraftable = SObject.BigCraftableCategory;
        public const int Books = SObject.booksCategory;
        public const int Boots = SObject.bootsCategory;
        public const int Clothing = SObject.clothingCategory;
        public const int ClothingSortValue = SObject.clothingCategorySortValue;
        public const int Cooking = SObject.CookingCategory;
        public const int Crafting = SObject.CraftingCategory;
        public const int Egg = SObject.EggCategory;
        public const int Equipment = SObject.equipmentCategory;
        public const int Fertilizer = SObject.fertilizerCategory;
        public const int Fish = SObject.FishCategory;
        public const int Flowers = SObject.flowersCategory;
        public const int Fruits = SObject.FruitsCategory;
        public const int Furniture = SObject.furnitureCategory;
        public const int Gem = SObject.GemCategory;
        public const int Greens = SObject.GreensCategory;
        public const int Hat = SObject.hatCategory;
        public const int Ingredient = SObject.ingredientsCategory;
        public const int Junk = SObject.junkCategory;
        public const int Litter = SObject.litterCategory;
        public const int Meat = SObject.meatCategory;
        public const int Milk = SObject.MilkCategory;
        public const int Mineral = SObject.mineralsCategory;
        public const int MonsterLoot = SObject.monsterLootCategory;
        public const int Ring = SObject.ringCategory;
        public const int Seeds = SObject.SeedsCategory;
        public const int SellAtFishShop = SObject.sellAtFishShopCategory;
        public const int SkillBooks = SObject.skillBooksCategory;
        public const int Syrup = SObject.syrupCategory;
        public const int Tackle = SObject.tackleCategory;
        public const int Tool = SObject.toolCategory;
        public const int Trinket = SObject.trinketCategory;
        public const int Vegetable = SObject.VegetableCategory;
        public const int Weapon = SObject.weaponCategory;
        public const int Delicate = SObject.fragility_Delicate;
    }
    public static class Types
    {
        public const string Basic = "Basic";
        public const string Artifact = "Arch";
        public const string Litter = "Litter";
        public const string Minerals = "Minerals";
        public const string Quest = "Quest";
        public const string Crafting = "Crafting";
        public const string Fish = "Fish";
        public const string Cooking = "Cooking";
        public const string Seeds = "Seeds";
        public const string Ring = "Ring";
        public const string Interactive = "interactive";
    }
    #endregion
}
public class ModItemLoader : ContentLoader<ModItem>
{
    private static readonly Dictionary<string, ModItem> _items = [];
    public static IReadOnlyDictionary<string, ModItem> Items => _items;
    public override void Load()
    {
        base.Load();
        HarmonyInstance.Patch(
            original: AccessTools.DeclaredMethod(typeof(Slingshot), nameof(Slingshot.canThisBeAttached), [typeof(SObject), typeof(int)]),
            prefix: new HarmonyMethod(typeof(ModItemLoader), nameof(PrefixSlingshotAttach)));
    }
    public override void AddContent(ModItem content, AssetRequestedEventArgs e)
    {
        content.SetProperties();
        _items[content.InternalName] = content;
        if (e.NameWithoutLocale.IsEquivalentTo(content.TexturePath))
            e.LoadFromModFile<Texture2D>("assets/Objects.png", AssetLoadPriority.Exclusive);
        if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
        {
            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, ObjectData>().Data;
                ObjectData rktmData = new()
                {
                    Name = content.InternalName,
                    DisplayName = content.DisplayName,
                    Description = content.Description,
                    Type = content.Type,
                    Category = content.Category,
                    Price = content.Price,
                    Texture = content.TexturePath,
                    SpriteIndex = content.SpriteIndex,
                    ContextTags = [..content.ContextTags],
                };
                data[content.InternalName] = rktmData;
            });
        }
        if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
        {
            e.Edit(asset =>
            {
                int outputCount = 1;
                var recipe = content.GetRecipe(ref outputCount);
                if (!recipe.Any()) return;

                var recipes = asset.AsDictionary<string, string>().Data;
                string ingredients = "";
                foreach(var (itemId, amount) in recipe)
                    ingredients += $"{itemId} {amount} ";

                string outputString = outputCount.ToString();
                string unlockConditions = "default";
                string recipeValue = $"{ingredients}/Home/{content.InternalName} {outputString}/false/{unlockConditions}";
                recipes[content.InternalName] = recipeValue;
            });
        }
    }
    private static bool PrefixSlingshotAttach(ref bool __result, SObject o, int slot)
    {
        if (o.ModItem != null)
        {
            __result = o.ModItem!.CanAttachTo(new Slingshot(), slot);
            return false;
        }
        return true;
    }
}
public static class ModItemHelper
{
    extension(SObject obj)
    {
        public ModItem? ModItem => ModItemLoader.Items.Values.FirstOrDefault(i => i.InternalName == obj.ItemId);
    }
}