using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Tools;
using ZinkleStardewMod.content;

namespace ZinkleStardewMod.Content.ContentTypes;

public abstract class ModTool : Tool, IModContent
{
    public virtual void Load() { }
    public virtual string InternalName => $"{Instance.ModManifest.UniqueID}_{GetType().Name}";
    public virtual string SpriteDirectory { get; } = "Tools";
    public string TexturePath => $"Mods/{Instance.ModManifest.UniqueID}/{SpriteDirectory}";
    public abstract int SpriteIndex { get; }
    public virtual int MenuSpriteIndex => SpriteIndex;
    public new abstract string DisplayName { get; }
    public new virtual string Description { get; } = "";
    public sealed override string loadDisplayName() => DisplayName;
    public sealed override string getDescription() => Description;
    public sealed override string loadDescription() => Description;

    public virtual string Type => Types.Generic;
    public int Price { get; protected set; } = 1;
    public bool CanDropOnDeath { get; protected set; } = false;
    public int AttachmentSlots { get; protected set; } = 0;
    public string ConventionalUpgradeFrom { get; protected set; } = "";
    public Dictionary<string, string> ModData { get; protected set; } = [];
    public virtual void SetProperties() { }
    public virtual bool CanAttachTo(Tool tool, int slot) => false;
    public virtual IEnumerable<(string, int)> GetRecipe(ref int outputAmount) => [];
    public virtual bool PreDraw(SpriteBatch spriteBatch, Vector2 position, float scale, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow) => true;
    public sealed override int attachmentSlots() => AttachmentSlots;
    public sealed override bool canBeDropped() => CanDropOnDeath;
    public sealed override int salePrice(bool ignoreProfitMargins = false) => Price;
    #region Helpers
    public static class Types
    {
        public const string Generic = "GenericTool";
        public const string Axe = "Axe";
        public const string FishingRod = "FishingRod";
        public const string Hoe = "Hoe";
        public const string MeleeWeapon = "MeleeWeapon";
        public const string MilkPail = "MilkPail";
        public const string Pan = "Pan";
        public const string Pickaxe = "Pickaxe";
        public const string Shears = "Shears";
        public const string Wand = "Wand";
        public const string WateringCan = "WateringCan";
    }
    #endregion
}
public class ModToolLoader : ContentLoader<ModTool>
{
    private static readonly Dictionary<string, ModTool> _tools = [];
    public static IReadOnlyDictionary<string, ModTool> Tools => _tools;
    public override void AddContent(ModTool content, AssetRequestedEventArgs e)
    {
        content.Load();
        content.SetProperties();
        _tools[content.InternalName] = content;
        if (e.NameWithoutLocale.IsEquivalentTo(content.TexturePath))
            e.LoadFromModFile<Texture2D>($"Assets/{content.SpriteDirectory}.png", AssetLoadPriority.Exclusive);
        if (e.NameWithoutLocale.IsEquivalentTo("Data/Tools"))
        {
            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, ToolData>().Data;
                ToolData toolData = new()
                {
                    Name = content.InternalName,
                    DisplayName = content.DisplayName,
                    Description = content.Description,
                    Texture = content.TexturePath,
                    SpriteIndex = content.SpriteIndex,
                    ClassName = content.Type,
                    ModData = content.ModData,
                    ConventionalUpgradeFrom = content.ConventionalUpgradeFrom,
                    UpgradeLevel = content.UpgradeLevel,
                    SalePrice = content.Price,
                    MenuSpriteIndex = content.MenuSpriteIndex,
                    CanBeLostOnDeath = content.CanDropOnDeath,
                    AttachmentSlots = content.AttachmentSlots
                };
                data[content.InternalName] = toolData;
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
}
public static class ModToolHelper
{
    extension(Tool tool)
    {
        public ModTool? ModTool => ModToolLoader.Tools.Values.FirstOrDefault(i => i.InternalName == tool.ItemId);
    }
}
