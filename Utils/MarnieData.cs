using Microsoft.Xna.Framework;
using StardewValley;
namespace ZinkleStardewMod.Utils;

public static class MarnieData
{
    public static bool GetMarnieBeingResurrected(NPC marnie) => marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarnieRessurecting", out var value) && bool.TryParse(value, out var result) && result;
    public static void SetMarnieBeingResurrected(NPC marnie, bool value) => marnie.modData[$"{Instance.ModManifest.UniqueID}_MarnieRessurecting"] = value.ToString();
    public static bool GetMarnieTurquoise(NPC marnie) => marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarnieTurquoise", out var value) && bool.TryParse(value, out var result) && result;
    public static void SetMarnieTurquoise(NPC marnie, bool value) => marnie.modData[$"{Instance.ModManifest.UniqueID}_MarnieTurquoise"] = value.ToString();
    public static bool GetMarnieDeadAsFuck(NPC marnie) => marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarnieDeadAsFuck", out var value) && bool.TryParse(value, out var result) && result;
    public static void SetMarnieDeadAsFuck(NPC marnie, bool value) => marnie.modData[$"{Instance.ModManifest.UniqueID}_MarnieDeadAsFuck"] = value.ToString();
    public static int GetMarnieResurrectionTime(NPC marnie) => marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarnieResurrectionTime", out var value) && int.TryParse(value, out var result) ? result : 0;
    public static void SetMarnieResurrectionTime(NPC marnie, int value) => marnie.modData[$"{Instance.ModManifest.UniqueID}_MarnieResurrectionTime"] = value.ToString();
    //public static Vector2 GetMarniePosition(NPC marnie)
    //{
    //    var x = marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarniePosX", out var valueX) && float.TryParse(valueX, out var resultX) ? resultX : 0;
    //    var y = marnie.modData.TryGetValue($"{Instance.ModManifest.UniqueID}_MarniePosY", out var valueY) && float.TryParse(valueY, out var resultY) ? resultY : 0;
    //    return new(x, y);
    //}
    //public static void SetMarniePosition(NPC marnie, Vector2 value)
    //{
    //    marnie.modData[$"{Instance.ModManifest.UniqueID}_MarniePosX"] = value.X.ToString();
    //    marnie.modData[$"{Instance.ModManifest.UniqueID}_MarniePosY"] = value.Y.ToString();
    //}
    public const int MarnieResurrectionTimeMax = 60;
}
