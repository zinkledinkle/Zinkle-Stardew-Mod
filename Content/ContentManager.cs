using ZinkleStardewMod.content;

namespace ZinkleStardewMod.Content;

public static class ContentManager
{
    private static readonly Dictionary<string, ContentLoader> _loaders = [];
    public static IReadOnlyDictionary<string, ContentLoader> Loaders => _loaders;
    public static void LoadAllModContent()
    {
        var asm = typeof(ContentManager).Assembly;
        var loaderTypes = asm.GetTypes().Where(t => typeof(ContentLoader).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters);
        foreach (var type in loaderTypes)
        {
            Type? genericType = type.BaseType?.GenericTypeArguments.FirstOrDefault();
            if (genericType is null) continue;
            var loader = (ContentLoader)Activator.CreateInstance(type)!;
            _loaders.Add(genericType.Name, loader);
            loader.Load();
        }
    }
}
