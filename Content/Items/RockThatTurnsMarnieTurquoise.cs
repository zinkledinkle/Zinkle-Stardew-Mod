using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using ZinkleStardewMod.Content.ContentTypes;
using ZinkleStardewMod.Utils;

namespace ZinkleStardewMod.Content.Items;

[HarmonyPatch(typeof(BasicProjectile), nameof(BasicProjectile.behaviorOnCollisionWithMonster), [typeof(NPC), typeof(GameLocation)])]
public class RockThatTurnsMarnieTurquoise : ModObject
{
    public override string DisplayName => "Rock That Turns Marnie Turquoise";
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
    public override void SetProperties()
    {
        Type = Types.Basic;
        Category = Categories.Mineral;
        AddRecipe()
            .AddIngredient(SObject.aquamarineID)
            .AddIngredient(SObject.stoneID, 3);
    }
    public override bool CanAttachTo(Tool tool, int slot) => tool is Slingshot;
    [HarmonyPrefix]
    private static bool PrefixShootMarnie(BasicProjectile __instance, NPC n, GameLocation location)
    {
        if (!n.displayName.Contains("Marnie") || __instance.itemId.Value != $"(O){RockThatTurnsMarnieTurquoise.ItemID}")
            return true;
        MarnieData.SetMarnieTurquoise(n, true);
        for (int i = 0; i < 10; i++)
            Game1.createRadialDebris(location, 1, (int)(n.position.X + 32f + Game1.random.Next(-32, 32)) / 64, (int)(n.position.Y + 32f + Game1.random.Next(-48, 48)) / 64, 6, false, 1, false, Color.Turquoise);
        location.projectiles.Remove(__instance);
        Game1.playSound("boulderBreak");
        Game1.playSound("breakingGlass");
        Game1.playSound("fairy_heal");
        return false;
    }
}