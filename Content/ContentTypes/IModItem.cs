using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ZinkleStardewMod.Content.ContentTypes;

public interface IModItem : IModContent
{
    public string DisplayName { get; }
    public string Description { get; }
    public virtual bool PreDraw(SpriteBatch spriteBatch, Vector2 position, float scale, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow) => true;
}
public static class ModItemHelper
{
    extension<T>(T) where T : IModItem
    {
        public static string ItemID => $"{Instance.ModManifest.UniqueID}_{typeof(T).Name}";
    }
}