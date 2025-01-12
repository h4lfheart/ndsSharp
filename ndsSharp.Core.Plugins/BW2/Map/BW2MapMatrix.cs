using ndsSharp.Core.Data;
using ndsSharp.Core.Mathematics;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Plugins.BW2.Map;

public class BW2MapMatrix : BaseDeserializable
{
    public bool HasHeaders;
    public Matrix<int> Maps = new();
    public Matrix<int> Headers = new();
    
    public override void Deserialize(DataReader reader)
    {
        HasHeaders = reader.Read<int>() == 1;
        var width = reader.Read<ushort>();
        var height = reader.Read<ushort>();

        Maps = new Matrix<int>(width, height);
        for (var matrixIndex = 0; matrixIndex < width * height; matrixIndex++)
        {
            Maps[matrixIndex] = reader.Read<int>();
        }

        if (HasHeaders)
        {
            Headers = new Matrix<int>(width, height);
            for (var matrixIndex = 0; matrixIndex < width * height; matrixIndex++)
            {
                Headers[matrixIndex] = reader.Read<int>();
            }
        }
    }
}