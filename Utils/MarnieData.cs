using StardewValley;
namespace ZinkleStardewMod.Utils;

public static class MarnieData
{
    public static bool GetMarnieBeingResurrected(NPC marnie) => marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarnieRessurecting", out var value) && bool.TryParse(value, out var result) && result;
    public static void SetMarnieBeingResurrected(NPC marnie, bool value) => marnie.modData[$"{Instance.ModManifest.UniqueID}_MarnieRessurecting"] = value.ToString();
    public static bool GetMarnieTurquoise(NPC marnie) => marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarnieTurquoise", out var value) && bool.TryParse(value, out var result) && result;
    public static void SetMarnieTurquoise(NPC marnie, bool value) => marnie.modData[$"{Instance.ModManifest.UniqueID}_MarnieTurquoise"] = value.ToString();
    public static bool GetMarnieBig(NPC marnie) => marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarnieBig", out var value) && bool.TryParse(value, out var result) && result;
    public static void SetMarnieBig(NPC marnie, bool value) => marnie.modData[$"{Instance.ModManifest.UniqueID}_MarnieBig"] = value.ToString();
    public static bool GetMarnieUpsideDown(NPC marnie) => marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarnieUpsideDown", out var value) && bool.TryParse(value, out var result) && result;
    public static void SetMarnieUpsideDown(NPC marnie, bool value) => marnie.modData[$"{Instance.ModManifest.UniqueID}_MarnieUpsideDown"] = value.ToString();
    public static bool GetMarnieDeadAsFuck(NPC marnie) => marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarnieDeadAsFuck", out var value) && bool.TryParse(value, out var result) && result;
    public static void SetMarnieDeadAsFuck(NPC marnie, bool value) => marnie.modData[$"{Instance.ModManifest.UniqueID}_MarnieDeadAsFuck"] = value.ToString();
    public static int GetMarnieResurrectionTime(NPC marnie) => marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarnieResurrectionTime", out var value) && int.TryParse(value, out var result) ? result : 0;
    public static void SetMarnieResurrectionTime(NPC marnie, int value) => marnie.modData[$"{Instance.ModManifest.UniqueID}_MarnieResurrectionTime"] = value.ToString();
    public const int MarnieResurrectionTimeMax = 60;
}
