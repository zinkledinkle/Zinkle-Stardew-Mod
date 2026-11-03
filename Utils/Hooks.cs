namespace ZinkleStardewMod.Utils;

public static class Hooks
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LoadHook : Attribute;

    public static void Load()
    {
        var methods = Instance.GetType().Assembly.GetTypes().SelectMany(t => t.GetMethods()).Where(m => m.GetCustomAttributes(typeof(LoadHook), false).Length > 0);
        foreach (var method in methods)
            method.Invoke(null, null);
    }
}
