using System.Reflection;
using ndsSharp.Core.Extensions;

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
    
    public static void Register(string extension, Type fileType)
    {
        Types.Add(new KeyValuePair<string, Type>(extension, fileType));
    }
    
    public static bool TryGetExtension(string str, out string extension)
    {
        extension = Types.FirstOrDefault(kvp => kvp.Key.Equals(str, StringComparison.OrdinalIgnoreCase)
                                                || kvp.Key.Equals(str.Flip(), StringComparison.OrdinalIgnoreCase)).Key?.ToLower()!;
        return extension is not null;
    }
    
    public static Type? GetTypeOrDefault(string str, Type? defaultType = null)
    {
        return TryGetType(str, out var foundType) ? foundType : defaultType;
    }
    
    public static bool TryGetType(string str, out Type type)
    {
        type = Types.FirstOrDefault(kvp => kvp.Key.Equals(str, StringComparison.OrdinalIgnoreCase)
                                           || kvp.Key.Equals(str.Flip(), StringComparison.OrdinalIgnoreCase)).Value;
        return type is not null;
    }
    
    public static bool TryGetType(string str, Type ownerType, out Type type)
    {
        type = Types.FirstOrDefault(kvp => kvp.Value.DeclaringType == ownerType 
                                           && (kvp.Key.Equals(str, StringComparison.OrdinalIgnoreCase) 
                                               || kvp.Key.Equals(str.Flip(), StringComparison.OrdinalIgnoreCase))).Value;
        type ??= Types.FirstOrDefault(kvp => kvp.Key.Equals(str, StringComparison.OrdinalIgnoreCase)
                 || kvp.Key.Equals(str.Flip(), StringComparison.OrdinalIgnoreCase)).Value;
        return type is not null;
    }
}