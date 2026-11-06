using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Text;

namespace ZinkleStardewMod.Content.ContentTypes;

public abstract class ModItem : IModContent
{
    public abstract string DisplayName { get; }
    public virtual string Description => "";
    public abstract string SpriteDirectory { get; }
    public virtual int SpriteIndex => 0;
    public string InternalName => $"{Instance.ModManifest.UniqueID}_{GetType().Name}";
    public string TexturePath => $"Mods/{Instance.ModManifest.UniqueID}/{SpriteDirectory}";
    public virtual void SetProperties() { }
    public virtual void Load() { }
    public ItemRecipe Recipe { get; private set; } = "";
    public ItemRecipe AddRecipe(int Output = 1) => (Recipe = new(InternalName, Output));
}
public record ItemRecipe(string ContentID, int Output = 1)
{
    public static implicit operator ItemRecipe(string value) => new(value);
    public static implicit operator string(ItemRecipe r) => r.Value;
    private readonly StringBuilder _ingredients = new();
    private readonly StringBuilder _unlock = new("default");
    public ItemRecipe AddIngredient(string itemID, int count = 1)
    {
        _ingredients.Append(itemID).Append(' ').Append(count).Append(' ');
        return this;
    }
    public ItemRecipe AddRequirement(Requirement req)
    {
        if (_unlock.StringsEqual("default")) _unlock.Clear().Append(req);
        else _unlock.Append(req);
        return this;
    }
    public string Value => _ingredients.Length > 0 ? $"{_ingredients}/Home/{ContentID} {Output}/false/{_unlock}" : "";
}
public readonly record struct Requirement(string Value)
{
    public static implicit operator string(Requirement r) => r.Value;
    public static Requirement FishingLevel(int level)
        => new($"s Fishing {level} ");
    public static Requirement MiningLevel(int level)
        => new($"s Mining {level} ");
    public static Requirement CombatLevel(int level)
        => new($"s Combat {level} ");
    public static Requirement FarmingLevel(int level)
        => new($"s Farming {level} ");
    public static Requirement ForagingLevel(int level)
        => new($"s Foraging {level} ");
}
public static class ModItemHelper
{
    extension<T>(T) where T : ModItem
    {
        public static string ItemID => $"{Instance.ModManifest.UniqueID}_{typeof(T).Name}";
    }
}