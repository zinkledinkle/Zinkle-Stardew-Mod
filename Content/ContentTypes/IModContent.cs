namespace ZinkleStardewMod.Content.ContentTypes;

public interface IModContent
{
    public void Load();
    public string SpriteDirectory { get; }
    public int SpriteIndex { get; }
    public string TexturePath => $"Mods/{ModEntry.Instance.ModManifest.UniqueID}/{SpriteDirectory}";
}
