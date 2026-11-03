using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ZinkleStardewMod.Content.ContentTypes;

public interface ICustomDrawnItem
{
    public bool PreDraw(SpriteBatch spriteBatch, Vector2 position, float scale, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow);
}
[HarmonyPatch(typeof(Item), nameof(Item.drawInMenu), [typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool)])]
public static class CustomItemDrawer
{
    private static bool Prefix(Item __instance, SpriteBatch spriteBatch, Vector2 position, float scaleSize, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        if (ModItemLoader.Items.TryGetValue(__instance.ItemId, out var item) && item is ICustomDrawnItem customDrawnItem)
            return customDrawnItem.PreDraw(spriteBatch, position, scaleSize, layerDepth, drawStackNumber, color, drawShadow);
        return true;
    }
}