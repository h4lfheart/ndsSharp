using System.Reflection;

namespace ndsSharp.Core.Objects.Exports;

public class FileTypeRegistry
{
    private static readonly List<KeyValuePair<string, Type>> Types = [];
    private static readonly Type DefaultObjectType = typeof(NdsObject);
    private static readonly Type DefaultBlockType = typeof(NdsBlock);
    
    static FileTypeRegistry()
    {
        Register(Assembly.GetExecutingAssembly());
    }

    public static void Register(Assembly assembly)
    {
        foreach (var type in assembly.DefinedTypes)
        {
            if (type.IsAbstract || type.IsInterface) continue;
            if (DefaultObjectType == type) continue;
            
            try
            {
                if (DefaultObjectType.IsAssignableFrom(type))
                {
                    var obj = (NdsObject) Activator.CreateInstance(type)!;
                    if (!string.IsNullOrEmpty(obj.Magic))
                        Types.Add(new KeyValuePair<string, Type>(obj.Magic, type));
                }
                
                if (DefaultBlockType.IsAssignableFrom(type))
                {
                    var block = (NdsBlock) Activator.CreateInstance(type)!;
                    if (!string.IsNullOrEmpty(block.Magic))
                        Types.Add(new KeyValuePair<string, Type>(block.Magic, type));
                }
            }
            catch (Exception)
            {
                // ignored
            }

        }
    }

    public static bool Contains(string str)
    {
        return Types.Any(kvp => kvp.Key.Equals(str, StringComparison.OrdinalIgnoreCase));
    }
    
    public static bool TryGetType(string str, out Type type)
    {
        type = Types.FirstOrDefault(kvp => kvp.Key.Equals(str)).Value;
        return type is not null;
    }
    
    public static bool TryGetType(string str, Type ownerType, out Type type)
    {
        type = Types.FirstOrDefault(kvp => kvp.Key.Equals(str) && kvp.Value.DeclaringType == ownerType).Value;
        type ??= Types.FirstOrDefault(kvp => kvp.Key.Equals(str)).Value;
        return type is not null;
    }
}