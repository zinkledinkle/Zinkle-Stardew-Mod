using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using ZinkleStardewMod.Content.ContentTypes;

namespace ZinkleStardewMod.Content.Items;

public class SloppyFinger : ModTool
{
    public override string DisplayName => "Sloppy Finger";
    public override string Description => "\n\n\n\n\n\n\n\n\n\n\n\n";
    public override string SpriteDirectory => "sloppyfinger";
    public override int SpriteIndex => 0;
    private static string Sound = "";
    public override void Load()
    {
        texture = Instance.Helper.ModContent.Load<Texture2D>("Assets/sloppyfinger.png");
        shadow = Instance.Helper.ModContent.Load<Texture2D>("Assets/dark.png");
        SoundLoader.AddSound("scream", out Sound, SoundLoader.Categories.Sound, true, false);
    }
    private static Texture2D? texture;
    private static Texture2D? shadow;
    public override void SetProperties()
    {
        Price = -100;
    }
    public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
    {
        return true;
    }
    public override void Update(int direction, int farmerMotionFrame, Farmer who)
    {
        who.takeDamage(1, true, null);
        Game1.playSound(Sound);
    }
    public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        var origin = new Vector2(texture!.Width, texture!.Height) * 0.5f;
        var shorigin = new Vector2(shadow!.Width, shadow!.Height) * 0.5f;
        spriteBatch.Draw(shadow, location, null, Color.White * transparency, 0f, shorigin, scaleSize * 0.1f, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(texture, location, null, Color.White * transparency, 0f, origin, scaleSize * 0.5f, SpriteEffects.None, layerDepth);
        base.drawInMenu(spriteBatch, location, scaleSize, 1f, layerDepth, drawStackNumber, color, false); //draw the stack stuff but no item
    }
    public override Item GetOneNew() => new SloppyFinger();
}
