using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Tools;
using System.Text;
using ZinkleStardewMod.content;

namespace ZinkleStardewMod.Content.ContentTypes;

public abstract class ModTool : ModItem
{
    public override string SpriteDirectory => "Tools";
    public virtual int MenuSpriteIndex => SpriteIndex;
    public virtual string Type => Types.Generic;
    public int Price { get; protected set; } = 1;
    public bool CanDropOnDeath { get; protected set; } = false;
    public bool CanBeGifted { get; protected set; } = false;
    public int AttachmentSlots { get; protected set; } = 0;
    public int UpgradeLevel { get; protected set; } = -1;
    public string ConventionalUpgradeFrom { get; protected set; } = "";
    public Dictionary<string, string> ModData { get; protected set; } = [];
    public virtual bool CanAttachTo(Tool tool, int slot) => false;
    public virtual IEnumerable<(string, int)> GetRecipe(ref int outputAmount) => [];
    public virtual bool PreDrawInMenu(Tool tool, SpriteBatch spriteBatch, Vector2 position, float scale, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow) => true;
    public virtual bool PreDraw(Tool tool, SpriteBatch spriteBatch) => true;
    public virtual bool PreDrawTooltip(Tool tool, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, StringBuilder overrideText) => true;
    public virtual bool PreBeginUsing(Tool tool, GameLocation location, Farmer who, int x, int y, ref bool __result) => true;
    public virtual void PostBeginUsing(Tool tool, GameLocation location, Farmer who, int x, int y, ref bool __result) { }
    public virtual bool PreEndUsing(Tool tool, GameLocation location, Farmer who) => true;
    public virtual bool PreUpdate(Tool tool, int direction, int motionFrame, Farmer who) => true;
    public virtual bool PreTickUpdate(Tool tool, GameTime gameTime, Farmer who) => true;
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
    public static IReadOnlyDictionary<string, ModTool> Tools => ((ContentLoader<ModTool>)ContentManager.Loaders["ModTool"]).Content.ToDictionary(t => t.InternalName);
    public override void AddContent(ModTool content, AssetRequestedEventArgs e)
    {
        content.SetProperties();

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
                var recipe = content.Recipe.Value;
                if (string.IsNullOrEmpty(recipe)) return;
                var recipes = asset.AsDictionary<string, string>().Data;
                recipes[content.InternalName] = recipe;
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
[HarmonyPatch(typeof(Tool))]
public static class ModToolPatches
{
    [HarmonyPatch(nameof(Tool.drawInMenu))]
    [HarmonyPrefix]
    private static bool DrawInMenuPrefix(Tool __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow) =>
        __instance.ModTool is not ModTool modTool || modTool.PreDrawInMenu(__instance, spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
    [HarmonyPatch(nameof(Tool.draw))]
    [HarmonyPrefix]
    private static bool DrawPrefix(Tool __instance, SpriteBatch b) =>
        __instance.ModTool is not ModTool modTool || modTool.PreDraw(__instance, b);
    [HarmonyPatch(nameof(Tool.drawTooltip))]
    [HarmonyPrefix]
    private static bool DrawTooltipPrefix(Tool __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, StringBuilder overrideText) =>
        __instance.ModTool is not ModTool modTool || modTool.PreDrawTooltip(__instance, spriteBatch, ref x, ref y, font, alpha, overrideText);
    [HarmonyPatch(nameof(Tool.beginUsing))]
    [HarmonyPrefix]
    private static bool BeginUsingPrefix(Tool __instance, GameLocation location, int x, int y, Farmer who, ref bool __result)
    {
        if (__instance.ModTool is ModTool modTool)
        {
            return modTool.PreBeginUsing(__instance, location, who, x, y, ref __result);
        }
        return true;
    }
    [HarmonyPatch(nameof(Tool.beginUsing))]
    [HarmonyPostfix]
    private static void BeginUsingPostfix(Tool __instance, GameLocation location, int x, int y, Farmer who, ref bool __result)
    {
        if (__instance.ModTool is ModTool modTool)
            modTool.PostBeginUsing(__instance, location, who, x, y, ref __result);
    }
    [HarmonyPatch(nameof(Tool.endUsing))]
    [HarmonyPrefix]
    private static bool EndUsingPrefix(Tool __instance, GameLocation location, Farmer who) =>
        __instance.ModTool is not ModTool modTool || modTool.PreEndUsing(__instance, location, who);
    [HarmonyPatch(nameof(Tool.Update))]
    [HarmonyPrefix]
    private static bool UpdatePrefix(Tool __instance, int direction, int farmerMotionFrame, Farmer who) =>
        __instance.ModTool is not ModTool modTool || modTool.PreUpdate(__instance, direction, farmerMotionFrame, who);
    [HarmonyPatch(nameof(Tool.tickUpdate))]
    [HarmonyPrefix]
    private static bool TickUpdatePrefix(Tool __instance, GameTime time, Farmer who) =>
        __instance.ModTool is not ModTool modTool || modTool.PreTickUpdate(__instance, time, who);
    [HarmonyPatch(nameof(Tool.attachmentSlots))]
    [HarmonyPrefix]
    private static bool AttachmentSlotsPrefix(Tool __instance, ref int __result)
    {
        if (__instance.ModTool is ModTool modTool)
            __result = modTool.AttachmentSlots;
        return true;
    }
    [HarmonyPatch(nameof(Tool.salePrice))]
    [HarmonyPrefix]
    private static bool SalePricePrefix(Tool __instance, bool ignoreProfitMargins, ref int __result)
    {
        if (__instance.ModTool is ModTool modTool)
            __result = modTool.Price;
        return true;
    }
    [HarmonyPatch(nameof(Tool.CanBeLostOnDeath))]
    [HarmonyPrefix]
    private static bool CanBeLostOnDeathPrefix(Tool __instance, ref bool __result)
    {
        if (__instance.ModTool is ModTool modTool)
            __result = modTool.CanDropOnDeath;
        return true;
    }
    [HarmonyPatch(typeof(Item), nameof(Item.canBeGivenAsGift))]
    [HarmonyPrefix]
    private static bool CanBeGivenAsGiftPrefix(Item __instance, ref bool __result)
    {
        if (__instance is Tool tool && tool.ModTool is ModTool modTool)
            __result = modTool.CanBeGifted;
        return true;
    }
}
