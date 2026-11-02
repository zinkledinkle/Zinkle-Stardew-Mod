global using StardewModdingAPI;
global using SObject = StardewValley.Object;
global using static ZinkleStardewMod.ModEntry;
using HarmonyLib;
using ZinkleStardewMod.Content;

namespace ZinkleStardewMod;

internal sealed class ModEntry : Mod
{
    public static ModEntry Instance { get; private set; } = null!;
    public static Harmony HarmonyInstance { get; private set; } = null!;
    public override void Entry(IModHelper helper)
    {
        Instance = this;
        HarmonyInstance = new Harmony(ModManifest.UniqueID);
        ContentManager.LoadAllModContent();
    }
}