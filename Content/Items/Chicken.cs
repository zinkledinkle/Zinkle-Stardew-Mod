using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using ZinkleStardewMod.Content.ContentTypes;

namespace ZinkleStardewMod.Content.Items;

[HarmonyPatch(typeof(Game1), nameof(Game1.UpdateControlInput))]
public class Chicken : ModObject
{
    public override string DisplayName => "Chicken";
    public override string Description => "";
    public override int SpriteIndex => 0;
    public override string SpriteDirectory => "chickenItem";
    public static string Sound { get; private set; } = "";
    public static Texture2D Texture { get; private set; } = null!;
    internal static float progress = 1f;
    internal const int maxtime = 42;
    public override void Load()
    {
        SoundLoader.AddSound("chicken", out var p, SoundLoader.Categories.Sound, true, false);
        Sound = p;
        Texture = Instance.Helper.ModContent.Load<Texture2D>("Assets/chicken.png");
        Instance.Helper.Events.Multiplayer.ModMessageReceived += (_, e) =>
        {
            if (e.FromModID != Instance.ModManifest.UniqueID || e.Type != "chicken") return;
            Game1.playSound(Sound);
            progress = 0f;
        };
    }
    public override void SetProperties()
    {
        Type = Types.Basic;
        Category = Categories.Junk;
        Price = 50;
        AddRecipe()
            .AddIngredient(SObject.woodID, 5)
            .AddRequirement(Requirement.FishingLevel(10));
    }
    public override bool PreDrawInMenu(SObject sobject, SpriteBatch spriteBatch, Vector2 position, float scale, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        var origin = new Vector2(Texture!.Width, Texture!.Height) * 0.5f;
        position += Vector2.One * 8f * Game1.pixelZoom;
        spriteBatch.Draw(Texture, position, null, Color.White * transparency, 0f, origin, scale * 0.3f, SpriteEffects.None, layerDepth);
        return false;
    }
    private static bool prevMouse;
    [HarmonyPrefix]
    private static void CheckInput(GameTime time)
    {
        var mouseDown = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed;
        if (mouseDown && !prevMouse && Game1.player.CurrentItem.ItemId == Chicken.ItemID)
        {
            progress = 0f;
            Instance.Helper.Multiplayer.SendMessage(true, "chicken", [Instance.ModManifest.UniqueID]);
        }
        prevMouse = mouseDown;
    }
}
[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.drawAboveAlwaysFrontLayer))]
public static class ChickenDraw
{
    [HarmonyPostfix]
    private static void PostDraw(SpriteBatch b)
    {
        var deltaTime = ((float)Game1.currentGameTime.ElapsedGameTime.TotalSeconds);
        if (Chicken.progress == 1f) return;

        var texture = Chicken.Texture;
        var orig = new Vector2(texture.Bounds.Width, texture.Bounds.Height) * 0.5f;
        var padding = orig.X * 3f;
        var start = new Vector2(-padding, Game1.graphics.PreferredBackBufferHeight / 2f);
        var end = new Vector2(Game1.graphics.PreferredBackBufferWidth + padding, Game1.graphics.PreferredBackBufferHeight / 2f);
        var position = Vector2.Lerp(end, start, Chicken.progress);
        const int aaCount = 10;
        const float aaDistance = 40f;
        for (int i = 0; i < aaCount; i++)
        {
            float a = 1 - i / (float)aaCount;
            b.Draw(texture, position + Vector2.UnitX * aaDistance * i, null, Color.White * a, 0f, orig, 1080f / Game1.options.preferredResolutionY * 1.3f, SpriteEffects.None, 0f);
        }

        Chicken.progress = MathF.Min(Chicken.progress + deltaTime * (60f / Chicken.maxtime), 1f);
    }
}