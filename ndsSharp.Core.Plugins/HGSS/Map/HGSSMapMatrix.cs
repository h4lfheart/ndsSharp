using System.Text;
using ndsSharp.Core.Data;
using ndsSharp.Core.Mathematics;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.HGSS.Map;

public class HGSSMapMatrix : BaseDeserializable
{
    public string Name;
    
    public Matrix<short> Maps = new();
    public Matrix<short> Headers = new();
    public Matrix<byte> Heights = new();

    public bool HasHeaders;
    public bool HasHeights;
    
    public override void Deserialize(DataReader reader)
    {
        var width = reader.Read<byte>();
        var height = reader.Read<byte>();

        HasHeaders = reader.Read<byte>() == 1;
        HasHeights = reader.Read<byte>() == 1;

        var nameLength = reader.Read<byte>();
        Name = reader.ReadString(nameLength, Encoding.UTF8);

        if (HasHeaders)
        {
            Headers = new Matrix<short>(width, height);
            for (var matrixIndex = 0; matrixIndex < width * height; matrixIndex++)
            {
                Headers[matrixIndex] = reader.Read<short>();
            }
        }
        
        
        if (HasHeights)
        {
            Heights = new Matrix<byte>(width, height);
            for (var matrixIndex = 0; matrixIndex < width * height; matrixIndex++)
            {
                Heights[matrixIndex] = reader.Read<byte>();
            }
        }
        
        Maps = new Matrix<short>(width, height);
        for (var matrixIndex = 0; matrixIndex < width * height; matrixIndex++)
        {
            Maps[matrixIndex] = reader.Read<short>();
        }
    }
}