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
    }
    public override void SetProperties()
    {
        Type = Types.Basic;
        Category = Categories.Cooking;
        Placeable = true;
        Price = -10000;
        IsGiftable = true;
    }
    public override IEnumerable<(string, int)> GetRecipe(ref int outputAmount) => [(SObject.woodID, 5)];
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
    public override bool PreDraw(SObject sobject, SpriteBatch spriteBatch, Vector2 position, float scale, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
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
            for (int i = 0; i < 10; i++)
                Game1.createRadialDebris(location, 1, (int)(__instance.position.X + 32f + Game1.random.Next(-16, 16)) / 64, (int)(__instance.position.Y + 32f + Game1.random.Next(-16, 16)) / 64, 6, false, 1, false, Color.Red);
            for (int x = (int)__instance.Position.X - 50; x < __instance.Position.X + 50; x++)
            {
                if (Game1.random.NextBool(0.995f)) continue;
                for (int y = (int)__instance.Position.Y - 80; y < __instance.Position.Y + 50; y++)
                {
                    if (Game1.random.NextBool(0.995f)) continue;
                    var pos = new Vector2(x, y);
                    location.temporarySprites.Add(new TemporaryAnimatedSprite(13, pos, Color.Red, 2, false, 100f, 0, -1, 1f, -1, Game1.random.Next(60)));
                }
            }
            MarnieData.SetMarnieResurrectionTime(__instance, MarnieData.GetMarnieResurrectionTime(__instance) - 1);
            if (MarnieData.GetMarnieResurrectionTime(__instance) <= 0)
            {
                MarnieData.SetMarnieBeingResurrected(__instance, false);
                MarnieData.SetMarnieDeadAsFuck(__instance, false);
            }
            return false;
        }
      
        return true;
    }
}
