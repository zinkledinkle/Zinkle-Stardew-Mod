using StardewModdingAPI.Events;
using ZinkleStardewMod.Content.ContentTypes;

namespace ZinkleStardewMod.content;

public abstract class ContentLoader
{
    public abstract void Load();
}

public abstract class ContentLoader<T> : ContentLoader where T : IModContent
{
    protected readonly List<T> _content = [];
    public IReadOnlyList<T> Content => _content;
    public override void Load()
    {
        var asm = GetType().Assembly;
        var contentTypes = asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters && typeof(T).IsAssignableFrom(t));
        foreach (var type in contentTypes)
        {
            var instance = (T)Activator.CreateInstance(type)!;
            instance.Load();
            _content.Add(instance);
        }
        Instance.Helper.Events.Content.AssetRequested += (sender, e) =>
        {
            foreach (var content in Content) AddContent(content, e);
        };
    }
    public abstract void AddContent(T content, AssetRequestedEventArgs e);
}
