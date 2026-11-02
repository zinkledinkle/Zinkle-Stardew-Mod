namespace ZinkleStardewMod.Content.ContentTypes;

public interface IModContent
{
    public void Load();
    public string DisplayName { get; }
    public string Description { get; }
    public string SpriteDirectory { get; }
    public int SpriteIndex { get; }
    public string TexturePath => $"Mods/{ModEntry.Instance.ModManifest.UniqueID}/{SpriteDirectory}";
}
