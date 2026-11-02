using StardewModdingAPI.Events;
using ZinkleStardewMod.Content.ContentTypes;

namespace ZinkleStardewMod.content;

public abstract class ContentLoader
{
    public abstract void Load();
}

public abstract class ContentLoader<T> : ContentLoader where T : IModContent
{
    public override void Load()
    {
        ModEntry.Instance.Helper.Events.Content.AssetRequested += (sender, e) =>
        {
            var asm = GetType().Assembly;
            var contentTypes = asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters && typeof(T).IsAssignableFrom(t));
            foreach (var type in contentTypes)
                AddContent((T)Activator.CreateInstance(type), e);
        };
    }
    public abstract void AddContent(T content, AssetRequestedEventArgs e);
}
