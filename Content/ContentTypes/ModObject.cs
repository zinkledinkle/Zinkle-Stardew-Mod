using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Tools;
using ZinkleStardewMod.content;

namespace ZinkleStardewMod.Content.ContentTypes;

public abstract class ModObject : ModItem
{
    public override string SpriteDirectory => "objects";
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
    public virtual bool PreDrawInMenu(SObject sobject, SpriteBatch spriteBatch, Vector2 position, float scale, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow) => true;
    public virtual bool PreDraw(SObject sobject, SpriteBatch spriteBatch, int x, int y, float alpha) => true;
    public virtual bool PrePlace(GameLocation location, int x, int y, ref bool __result) => true;
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
public class ModObjectLoader : ContentLoader<ModObject>
{
    public static IReadOnlyDictionary<string, ModObject> Objects => ((ContentLoader<ModObject>)ContentManager.Loaders["ModObject"]).Content.ToDictionary(t => t.InternalName);
    public override void Load()
    {
        base.Load();
        HarmonyInstance.Patch(
            original: AccessTools.DeclaredMethod(typeof(Slingshot), nameof(Slingshot.canThisBeAttached), [typeof(SObject), typeof(int)]),
            prefix: new HarmonyMethod(typeof(ModObjectLoader), nameof(PrefixAttach)));
        HarmonyInstance.Patch(
            original: AccessTools.DeclaredMethod(typeof(FishingRod), nameof(Slingshot.canThisBeAttached), [typeof(SObject), typeof(int)]),
            prefix: new HarmonyMethod(typeof(ModObjectLoader), nameof(PrefixAttach)));
        HarmonyInstance.Patch(
            original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.drawInMenu), [typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool)]),
            prefix: new HarmonyMethod(typeof(ModObjectLoader), nameof(PrefixDrawInMenu)));
        HarmonyInstance.Patch(
            original: AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.draw), [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]),
            prefix: new HarmonyMethod(typeof(ModObjectLoader), nameof(PrefixDraw)));
        HarmonyInstance.Patch(
            original: AccessTools.DeclaredMethod(typeof(Utility), nameof(Utility.tryToPlaceItem), [typeof(GameLocation), typeof(SObject), typeof(int), typeof(int)]),
            prefix: new HarmonyMethod(typeof(ModObjectLoader), nameof(PrefixPlace)));
    }
    public override void AddContent(ModObject content, AssetRequestedEventArgs e)
    {
        content.SetProperties();

        if (e.NameWithoutLocale.IsEquivalentTo(content.TexturePath) && ReferenceEquals(content, Content.First(item => item.TexturePath == content.TexturePath))) //only load once
            e.LoadFromModFile<Texture2D>($"Assets/{content.SpriteDirectory}.png", AssetLoadPriority.High);
        if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
        {
            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, ObjectData>().Data;
                ObjectData objectData = new()
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
                data[content.InternalName] = objectData;
            });
        }
        if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
        {
            e.Edit(asset =>
            {
                var recipe = content.Recipe.Value;
                if (string.IsNullOrEmpty(recipe)) return;
                var recipes = asset.AsDictionary<string, string>().Data;
                recipes[content.InternalName] = recipe;
            });
        }
    }
    private static bool PrefixAttach(ref bool __result, Tool __instance, SObject o, int slot)
    {
        if (o.ModObject != null)
        {
            Game1.timeOfDay = 900;
            __result = o.ModObject!.CanAttachTo(__instance, slot);
            return false;
        }
        return true;
    }
    private static bool PrefixDrawInMenu(SObject __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        if (Objects.TryGetValue(__instance.ItemId, out var obj))
            return obj.PreDrawInMenu(__instance, spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        return true;
    }
    private static bool PrefixDraw(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
    {
        if (Objects.TryGetValue(__instance.ItemId, out var obj))
            return obj.PreDraw(__instance, spriteBatch, x, y, alpha);
        return true;
    }
    private static bool PrefixPlace(GameLocation location, SObject item, int x, int y, ref bool __result)
    {
        if (Objects.TryGetValue(item.ItemId, out var obj))
            return obj.PrePlace(location, x, y, ref __result);
        return true;
    }
}
public static class ModObjectHelper
{
    extension(SObject obj)
    {
        public ModObject? ModObject => ModObjectLoader.Objects.Values.FirstOrDefault(i => i.InternalName == obj.ItemId);
    }
}