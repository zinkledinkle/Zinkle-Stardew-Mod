using HarmonyLib;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using ZinkleStardewMod.Content.ContentTypes;
using ZinkleStardewMod.Utils;

namespace ZinkleStardewMod.Content.Items;

[HarmonyPatch(typeof(BasicProjectile), nameof(BasicProjectile.behaviorOnCollisionWithMonster), [typeof(NPC), typeof(GameLocation)])]
public class RockThatTurnsMarnieBig : ModObject
{
    public override string DisplayName => "Rock That Makes Marnie Big";
    public override string Description => "Th";
    public override int SpriteIndex => 0;
    public override void SetProperties()
    {
        Type = Types.Basic;
        Category = Categories.Mineral;
        AddRecipe()
            .AddIngredient("(F)0");
    }
    public override bool CanAttachTo(Tool tool, int slot) => tool is Slingshot;
    [HarmonyPrefix]
    private static bool PrefixShootMarnie(BasicProjectile __instance, NPC n, GameLocation location)
    {
        if (!n.displayName.Contains("Marnie") || __instance.itemId.Value != $"(O){RockThatTurnsMarnieBig.ItemID}")
            return true;
        MarnieData.SetMarnieBig(n, true);
        location.projectiles.Remove(__instance);
        Game1.playSound("boulderBreak");
        return false;
    }
}