using System.Reflection;
using System.Text;
using ndsSharp.Core.Data;
using ndsSharp.Core.Objects;

namespace ndsSharp.Core.Conversion.Sounds.Sequence.Events;

public class BaseSequenceEvent : BaseDeserializable
{
    public int Offset;
    public byte ReadCommand;
    
    public override void Deserialize(DataReader reader)
    {
        Offset = reader.Position;
        
        var type = GetType();
        var fields = type.GetFields()
            .Where(field => field.GetCustomAttributes<SequenceParameter>().ToArray() is { Length: > 0 });

        foreach (var field in fields)
        {
            var parameter = field.GetCustomAttribute<SequenceParameter>()!;

            string readName;
            if (field.FieldType.IsEnum)
            {
                readName = field.FieldType.GetEnumUnderlyingType().Name;
            }
            else
            {
                readName = field.FieldType.Name;
            }

            object readValue = readName switch
            {
                "Boolean" => reader.Read<byte>() == 1,
                "Byte" => reader.Read<byte>(),
                "UInt16" => reader.Read<ushort>(),
                "UInt32" => reader.Read<uint>(),
                "Int16" => reader.Read<short>(),
                "Int32" => parameter.VariableLength
                    ? reader.ReadVariableLength()
                    : (parameter.ThreeByteInteger
                        ? reader.Read<byte>() | (reader.Read<byte>() << 8) | (reader.Read<byte>() << 16)
                        : reader.Read<int>())
            };
            
            if (field.FieldType.IsEnum)
            {
                field.SetValue(this, Enum.ToObject(field.FieldType, readValue));
            }
            else
            {
                field.SetValue(this, readValue);
            }
        }
    }
}

public class SequenceParameter : Attribute
{
    public bool VariableLength;
    public bool ThreeByteInteger;
}