using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using ZinkleStardewMod.Content.ContentTypes;
using ZinkleStardewMod.Utils;

namespace ZinkleStardewMod.Content.Items;

[HarmonyPatch(typeof(NPC), nameof(NPC.update))]
public class Marnie : ModObject
{
    public override string DisplayName => "Marnie";
    public override string Description => "Ok";
    public override int SpriteIndex => 0;
    public override string SpriteDirectory => "marnie";
    private static string Sound = "";
    public override void Load()
    {
        SoundLoader.AddSound("UNKILL", out Sound, SoundLoader.Categories.Sound, true, false);

        Instance.Helper.Events.Content.AssetRequested += (_, e) =>
        {
            if (!e.NameWithoutLocale.IsEquivalentTo("Data/NPCGiftTastes")) return;
            e.Edit(a =>
            {
                var data = a.AsDictionary<string, string>().Data;
                data["Universal_Hate"] += Marnie.ItemID;
            });
        };
    }
    public override void SetProperties()
    {
        Type = Types.Basic;
        Category = Categories.Cooking;
        Placeable = true;
        Price = -10000;
        IsGiftable = true;
        AddRecipe()
            .AddIngredient(SObject.woodID, 10)
            .AddIngredient("(O)129");
    }
    public override bool PrePlace(GameLocation location, int x, int y, ref bool __result)
    {
        NPC? marnie = Game1.getCharacterFromName("Marnie");
        if (marnie is null || !MarnieData.GetMarnieDeadAsFuck(marnie) || MarnieData.GetMarnieBeingResurrected(marnie))
        {
            __result = false;
            return false;
        }
        Game1.warpCharacter(marnie, location, new Vector2(x / 64f, y / 64f));
        MarnieData.SetMarnieBeingResurrected(marnie, true);
        MarnieData.SetMarnieResurrectionTime(marnie, MarnieData.MarnieResurrectionTimeMax);
        Game1.player.removeFirstOfThisItemFromInventory(Marnie.ItemID);
        return false;
    }
    public override bool PreDrawInMenu(SObject sobject, SpriteBatch spriteBatch, Vector2 position, float scale, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        sobject.AdjustMenuDrawForRecipes(ref transparency, ref scale);
        if (drawShadow)
            sobject.DrawShadow(spriteBatch, position, color, layerDepth);

        ParsedItemData itemData = ItemRegistry.GetDataOrErrorItem(sobject.QualifiedItemId);
        float drawnScale = scale;
        Rectangle sourceRect = itemData.GetSourceRect(0, sobject.ParentSheetIndex);
        sourceRect.Height *= 2;
        spriteBatch.Draw(itemData.GetTexture(), position + new Vector2(32f, 24f), sourceRect, color * transparency, 0f, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 4f * drawnScale, SpriteEffects.None, layerDepth);
        sobject.DrawMenuIcons(spriteBatch, position, scale, transparency, layerDepth, drawStackNumber, color);
        return false;
    }
    public override bool PreDraw(SObject sobject, SpriteBatch spriteBatch, int x, int y, float alpha)
    {
        if (!Game1.eventUp || (Game1.CurrentEvent != null && !Game1.CurrentEvent.isTileWalkedOn(x, y)))
        {
            Rectangle bounds = sobject.GetBoundingBoxAt(x, y);
            ParsedItemData itemData2 = ItemRegistry.GetDataOrErrorItem(sobject.QualifiedItemId);
            spriteBatch.Draw(itemData2.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32 + ((sobject.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), y * 64 - 32 + ((sobject.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), itemData2.GetSourceRect() with { Height = 32 }, Color.White * alpha, 0f, new Vector2(8f, 8f), (sobject.scale.Y > 1f) ? sobject.getScale().Y : 4f, sobject.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)(sobject.isPassable() ? bounds.Top : bounds.Center.Y) / 10000f);
        }
        return false;
    }
    [HarmonyPrefix]
    private static bool PrefixUpdateMarnie(NPC __instance, GameTime time, GameLocation location)
    {
        if (!__instance.displayName.Contains("Marnie"))
            return true;
        if (!MarnieData.GetMarnieBeingResurrected(__instance) && MarnieData.GetMarnieDeadAsFuck(__instance))
        {
            __instance.Position = new Vector2(80085, 80085);
            return false;
        }
        else if (MarnieData.GetMarnieBeingResurrected(__instance))
        {
            if (MarnieData.GetMarnieResurrectionTime(__instance) == MarnieData.MarnieResurrectionTimeMax)
                Game1.playSound(Sound);
            float progress = 1 - MarnieData.GetMarnieResurrectionTime(__instance) / (float)MarnieData.MarnieResurrectionTimeMax;
            int yPos = (int)(progress * 26 * Game1.pixelZoom); 
            Game1.currentLocation.debris.Add(new Debris(2, Game1.random.Next(4), new Vector2(__instance.position.X + 64f + Game1.random.Next(-32, 32), __instance.GetBoundingBox().Bottom - yPos + 16f), Game1.player.position.Value, 1, Color.Red));
            for (int x = (int)__instance.Position.X - 30; x < __instance.Position.X + 30; x++)
            {
                if (Game1.random.NextBool(0.95f)) continue;
                var pos = new Vector2(x, __instance.GetBoundingBox().Bottom - yPos - 32f + Game1.random.Next(-5, 5));
                location.temporarySprites.Add(new TemporaryAnimatedSprite(13, pos, Color.Red, 2, false, 100f, 0, -1, 1f, -1, Game1.random.Next(60)));
            }
            MarnieData.SetMarnieResurrectionTime(__instance, MarnieData.GetMarnieResurrectionTime(__instance) - 1);
            if (MarnieData.GetMarnieResurrectionTime(__instance) <= 0)
            {
                MarnieData.SetMarnieBeingResurrected(__instance, false);
                MarnieData.SetMarnieResurrectionTime(__instance, MarnieData.MarnieResurrectionTimeMax);
                MarnieData.SetMarnieDeadAsFuck(__instance, false);
            }
            return false;
        }
      
        return true;
    }
}
