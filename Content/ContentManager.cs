using ZinkleStardewMod.content;

namespace ZinkleStardewMod.Content;

public static class ContentManager
{
    public static void LoadAllModContent()
    {
        var asm = typeof(ContentManager).Assembly;
        var loaders = asm.GetTypes().Where(t => typeof(ContentLoader).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters);
        foreach (var type in loaders)
        {
            var loader = (ContentLoader)Activator.CreateInstance(type);
            loader.Load();
        }
    }
}
