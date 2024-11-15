using ndsSharp.Core.Data;
using Serilog;

namespace ndsSharp.Core.Objects.Exports.Meshes.MDL;

public class MDL0Model : DeserializableWithName
{
    public List<MDL0Transform> ObjectTransforms = [];
    public List<MDL0RenderCommand> RenderCommands = [];
    public List<MDL0Material> Materials = [];
    public List<MDL0Polygon> Polygons = [];
    public List<MDL0Transform> InvBindTransforms = [];
    
    public uint Size;
    public uint RenderCommandsOffset;
    public uint MaterialsOffset;
    public uint PolygonsOffset;
    public uint InvBindsOffset;

    public byte NumObjects;
    public byte NumMaterials;
    public byte NumPolygons;
    public ushort NumVertices;
    public ushort NumFaces;
    public ushort NumTriangles;
    public ushort NumQuads;

    public float UpScale;
    public float DownScale;
    
    public override void Deserialize(BaseReader reader)
    {
        ReadHeader(reader);
        ReadModelInfo(reader);
        
        reader.ReadWithZeroedPosition(ReadObjects);
        reader.ReadWithZeroedPosition(ReadRenderCommands, (int) RenderCommandsOffset);
        reader.ReadWithZeroedPosition(ReadMaterials, (int) MaterialsOffset);
        reader.ReadWithZeroedPosition(ReadPolygons, (int) PolygonsOffset);
        if (InvBindsOffset != reader.Length)
            reader.ReadWithZeroedPosition(ReadInvBindMatrices, (int) InvBindsOffset);
    }
    
    private void ReadHeader(BaseReader reader)
    {
        Size = reader.Read<uint>();
        RenderCommandsOffset = reader.Read<uint>();
        MaterialsOffset = reader.Read<uint>();
        PolygonsOffset = reader.Read<uint>();
        InvBindsOffset = reader.Read<uint>();
    }
    
    private void ReadModelInfo(BaseReader reader)
    {
        reader.Position += 3; // unknown

        NumObjects = reader.Read<byte>();
        NumMaterials = reader.Read<byte>();
        NumPolygons = reader.Read<byte>();
        
        reader.Position += 2; // unknown

        UpScale = reader.ReadIntAsFloat();
        DownScale = reader.ReadIntAsFloat();
        
        NumVertices = reader.Read<ushort>();
        NumFaces = reader.Read<ushort>();
        NumTriangles = reader.Read<ushort>();
        NumQuads = reader.Read<ushort>();

        reader.Position += sizeof(ushort) * 6; // bounding box min/max
        reader.Position += sizeof(uint) * 2; // bounding box up/down scale
    }
    
    private void ReadObjects(BaseReader reader)
    {
        var transformList = reader.ReadNameListPrimitive<uint>();
        foreach (var (name, offset) in transformList.Items)
        {
            reader.Position = (int) offset;

            var objectTransform = reader.ReadObject<MDL0Transform>();
            objectTransform.Name = name;
            ObjectTransforms.Add(objectTransform);
        }
    }
    
    private void ReadRenderCommands(BaseReader reader)
    {
        MDL0RenderCommand command;
        do
        {
            command = reader.ReadObject<MDL0RenderCommand>();
            RenderCommands.Add(command);
        }
        while (command.OpCode != RenderCommandOpCode.END);
    }

    private void ReadMaterials(BaseReader reader)
    {
        var textureListOffset = reader.Read<ushort>();
        var paletteListOffset = reader.Read<ushort>();

        var materialList = reader.ReadNameListPrimitive<uint>();

        reader.Position = textureListOffset;
        var textureList = reader.ReadNameList<MDL0MaterialMapping>();
        var textureNames = new string[materialList.Count];
        for (var i = 0; i < textureList.Count; i++)
        {
            var (name, mappings) = textureList.ElementAt(i);
            reader.Position = mappings.Offset;
            for (var n = 0; n < mappings.NumMaterials; n++)
            {
                var matIndex = reader.Read<byte>();
                textureNames[matIndex] = name;
            }
        }
        
        reader.Position = paletteListOffset;
        var paletteList = reader.ReadNameList<MDL0MaterialMapping>();
        var paletteNames = new string[materialList.Count];
        for (var i = 0; i < paletteList.Count; i++)
        {
            var (name, mappings) = paletteList.ElementAt(i);
            reader.Position = mappings.Offset;
            for (var n = 0; n < mappings.NumMaterials; n++)
            {
                var matIndex = reader.Read<byte>();
                paletteNames[matIndex] = name;
            }
        }
        
        for (byte matIndex = 0; matIndex < materialList.Count; matIndex++)
        {
            var (name, offset) = materialList.ElementAt(matIndex);
            reader.Position = (int) offset;

            var material = reader.ReadObject<MDL0Material>();
            material.Name = name;
            
            if (matIndex < textureNames.Length)
                material.TextureName = textureNames[matIndex];
            
            if (matIndex < paletteNames.Length)
            material.PaletteName = paletteNames[matIndex];
            
            Materials.Add(material);
        }
    }

    private void ReadPolygons(BaseReader reader)
    {
        var polygonDataList = reader.ReadNameListPrimitive<uint>();
        var polygons = reader.ReadArray(polygonDataList.Count, () => reader.ReadObject<MDL0Polygon>());

        foreach (var (polygon, (name, _)) in polygons.Zip(polygonDataList))
        {
            polygon.Name = name;
            
            var commandEnd = reader.Position + polygon.CommandLength;
            while (reader.Position < commandEnd)
            {
                var opCodes = reader.ReadArray(4, reader.ReadEnum<PolygonCommandOpCode, byte>);
                foreach (var opCode in opCodes)
                {
                    var command = reader.ReadObject<MDL0PolygonCommand>(dataModifier: polygonCommand => polygonCommand.OpCode = opCode);
                    polygon.Commands.Add(command);
                }

            }
            
            reader.Position = (int) commandEnd;
            Polygons.Add(polygon);
        }
    }
    
    private void ReadInvBindMatrices(BaseReader reader)
    {
        // TODO
    }
}