using StardewModdingAPI.Events;
using StardewValley.GameData;

namespace ZinkleStardewMod.Content;

public static class SoundLoader
{
    private record struct SoundProperties(string Path, string Category, bool Reverb, bool Loop);
    private static readonly List<SoundProperties> sounds = [];
    public static void AddSound(string soundPath, out string fullPath, string category = Categories.Default, bool reverb = false, bool loop = false)
    {
        sounds.Add(new SoundProperties(soundPath, category, reverb, loop));
        fullPath = $"{Instance.ModManifest.UniqueID}_{soundPath}";
    }
    public static void LoadAllSounds(AssetRequestedEventArgs e)
    {
        e.Edit(asset =>
        {
            var cues = asset.AsDictionary<string, AudioCueData>().Data;

            foreach(var sound in sounds)
            {
                var fullPath = $"{Instance.ModManifest.UniqueID}_{sound.Path}";
                cues[fullPath] = new AudioCueData
                {
                    Id = fullPath,
                    Category = sound.Category,
                    UseReverb = sound.Reverb,
                    FilePaths =
                    [
                        Path.Combine(Instance.Helper.DirectoryPath, "assets", "sounds", sound.Path + ".wav")
                    ],
                    StreamedVorbis = false,
                    Looped = sound.Loop
                };
            }
        });
    }
    public static class Categories
    {
        public const string Default = "Default";
        public const string Music = "Music";
        public const string Sound = "Sound";
        public const string Ambient = "Ambient";
        public const string Footsteps = "Footsteps ";
    }
}