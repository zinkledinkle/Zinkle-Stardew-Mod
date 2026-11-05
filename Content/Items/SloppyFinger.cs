using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Runtime.CompilerServices;
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
        noise = Instance.Helper.ModContent.Load<Texture2D>("Assets/vein.png");
        var bytes = File.ReadAllBytes(Path.Combine(Instance.Helper.DirectoryPath, "Assets", "sloppyshader.mgfx"));
        shader = new(Game1.graphics.GraphicsDevice, bytes); 
        SoundLoader.AddSound("scream", out Sound, SoundLoader.Categories.Sound, true, false);

        var letter = $"{Instance.ModManifest.UniqueID}_LinusLetter";
        Instance.Helper.Events.Content.AssetRequested += (_, e) =>
        {
            if (!e.NameWithoutLocale.IsEquivalentTo("Data/Mail")) return;
            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, string>().Data;
                data[letter] = "I'm so sorry. I had to get rid of it.^It's cursed. You cannot destroy it. ^You cannot throw it away.^It beckons.^^- Linus %item id (T)Zinkle.ZinkleStardewMod_SloppyFinger 1 %%[#]Cursed Object";
            });
        };
        Instance.Helper.Events.GameLoop.SaveLoaded += (_, _) =>
        {
            if (!Game1.player.hasOrWillReceiveMail(letter)) Game1.player.mailbox.Add(letter);
        };
    }
    private static Texture2D? texture;
    private static Texture2D? noise;
    private static Effect? shader;
    private const int maxTime = 60;
    private int time = maxTime;
    public override void SetProperties()
    {
        Price = -100;
    }
    public override void PostBeginUsing(Tool tool, GameLocation location, Farmer who, int x, int y, ref bool __result)
    {
        time = maxTime;
        Game1.playSound(Sound);
        who.health = who.maxHealth;
        who.animateOnce(FarmerSprite.showHoldingEdible);
        __result = true;
    }
    public override bool PreTickUpdate(Tool tool, GameTime gameTime, Farmer who)
    {
        if (Game1.player.hasMenuOpen.Value && Game1.player.Items.Any(i => i != null && i.ItemId == "Zinkle.ZinkleStardewMod_SloppyFinger"))
        {
            Game1.MusicDuckTimer = 60f;
        }
        if (!who.UsingTool || who.CurrentTool != tool) return true;
        if (time <= 0 && who.UsingTool)
        {
            who.EndUsingTool();
            return true;
        }
        Point standingPixel = who.StandingPixel;
        who.currentLocation.debris.Add(new Debris(1, new Vector2(standingPixel.X + 8, standingPixel.Y), Color.Red, 1f, who));
        who.playNearbySoundAll("ow");
        who.health--;
        Game1.hitShakeTimer = 100;
        time--;
        return true;
    }
    public override bool PreDraw(Tool tool, SpriteBatch spriteBatch)
    {
        return false;
    }
    public override bool PreDrawInMenu(Tool tool, SpriteBatch spriteBatch, Vector2 position, float scale, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        var bobX = MathF.Sin((float)Game1.currentGameTime.TotalGameTime.TotalSeconds * 0.5f) * 6f;
        var bobY = MathF.Sin((float)Game1.currentGameTime.TotalGameTime.TotalSeconds * 2) * 3f;
        position.X += bobX;
        position.Y += bobY;

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, shader);

        var evilAssColor = new Color(0.3f, 0.02f, 0.2f, 1f);

        shader?.Parameters["uTime"]?.SetValue((float)Game1.currentGameTime.TotalGameTime.TotalSeconds);
        shader?.Parameters["uThreshold"]?.SetValue(1.5f);
        shader?.CurrentTechnique.Passes[0].Apply();

        var origin = new Vector2(texture!.Width, texture!.Height) * 0.5f;
        var norigin = new Vector2(noise!.Width, noise!.Height) * 0.5f;
        position += Vector2.One * 8f * Game1.pixelZoom;
        spriteBatch.Draw(noise, position, null, evilAssColor, 0f, norigin, 0.5f, SpriteEffects.None, layerDepth);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null);

        const int afterimageCount = 10;
        const float offset = 12f;
        for (int i = 0; i < afterimageCount; i++)
        {
            var t = i / (float)afterimageCount;
            var aaPos = new Vector2(
                MathF.Cos(t * MathF.Tau + ((float)Game1.currentGameTime.TotalGameTime.TotalSeconds)),
                MathF.Sin(t * MathF.Tau + ((float)Game1.currentGameTime.TotalGameTime.TotalSeconds))) * offset;
            spriteBatch.Draw(texture, position + aaPos, null, evilAssColor * (1 / (float)afterimageCount) * 2f, 0f, origin, scale * 2f, SpriteEffects.None, layerDepth);
        }
        spriteBatch.Draw(texture, position, null, Color.White * transparency, 0f, origin, scale * 2f, SpriteEffects.None, layerDepth);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null);
        return false;
    }
}
