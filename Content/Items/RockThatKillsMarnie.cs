using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Projectiles;
using StardewValley.Tools;
using ZinkleStardewMod.Content.ContentTypes;
using ZinkleStardewMod.Utils;

namespace ZinkleStardewMod.Content.Items;

[HarmonyPatch(typeof(BasicProjectile), nameof(BasicProjectile.behaviorOnCollisionWithMonster), [typeof(NPC), typeof(GameLocation)])]
public class RockThatKillsMarnie : ModObject
{
    public override string DisplayName => "Rock That Kills Marnie";
    public override string Description => 
        "hmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm" +
        "\nmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm";
    public override int SpriteIndex => 0;
    private static string Sound = "";
    public override void Load()
    {
        SoundLoader.AddSound("KILL", out Sound, SoundLoader.Categories.Sound, true, false);
    }
    public override void SetProperties()
    {
        Type = Types.Basic;
        Category = Categories.Mineral;
    }
    public override IEnumerable<(string, int)> GetRecipe(ref int outputAmount) => [(SObject.stoneID, 5)];
    public override bool CanAttachTo(Tool tool, int slot) => tool is Slingshot;
    [HarmonyPrefix]
    private static bool PrefixShootMarnie(BasicProjectile __instance, NPC n, GameLocation location)
    {
        if (!n.displayName.Contains("Marnie") || __instance.itemId.Value != $"(O){RockThatKillsMarnie.ItemID}")
            return true;
        Game1.playSound(Sound);
        MarnieData.SetMarnieDeadAsFuck(n, true);
        for (int i = 0; i < 10; i++)
            Game1.createRadialDebris(location, 1, (int)(n.position.X + 32f + Game1.random.Next(-16, 16)) / 64, (int)(n.position.Y + 32f + Game1.random.Next(-16, 16)) / 64, 6, false, 1, false, Color.Red);
        for (int x = (int)n.Position.X - 50; x < n.Position.X + 50; x++)
        {
            if (Game1.random.NextBool(0.95f)) continue;
            for (int y = (int)n.Position.Y - 80; y < n.Position.Y + 50; y++)
            {
                if (Game1.random.NextBool(0.95f)) continue;
                var pos = new Vector2(x, y);
                location.temporarySprites.Add(new TemporaryAnimatedSprite(13, pos, Color.Red, 2, false, 100f, 0, -1, 1f, -1, Game1.random.Next(60)));
            }
        }
        n.position.Y -= 2000; //bye bye

        return false;
    }
}
