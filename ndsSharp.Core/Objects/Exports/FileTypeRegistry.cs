using System.Reflection;

namespace ndsSharp.Core.Objects.Exports;

public class FileTypeRegistry
{
    private static readonly Dictionary<string, Type> Types = [];
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
                        Types[obj.Magic] = type;
                }
                
                if (DefaultBlockType.IsAssignableFrom(type))
                {
                    var block = (NdsBlock) Activator.CreateInstance(type)!;
                    if (!string.IsNullOrEmpty(block.Magic))
                        Types[block.Magic] = type;
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
        return Types.Keys.Any(type => type.Equals(str, StringComparison.OrdinalIgnoreCase));
    }
    
    public static bool TryGetType(string str, out Type type)
    {
        return Types.TryGetValue(str, out type);
    }
}