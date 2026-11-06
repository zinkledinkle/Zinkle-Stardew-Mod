namespace ZinkleStardewMod.Content.ContentTypes;

public interface IModContent
{
    public void Load();
    public void SetProperties();
    public string InternalName { get; }
    public string SpriteDirectory { get; }
    public int SpriteIndex { get; }
    public string TexturePath => $"Mods/{Instance.ModManifest.UniqueID}/{SpriteDirectory}";
}
