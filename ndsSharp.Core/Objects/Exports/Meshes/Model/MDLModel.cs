using ndsSharp.Core.Data;
using ndsSharp.Core.Mathematics;

namespace ndsSharp.Core.Objects.Exports.Meshes.Model;

public class MDLModel : DeserializableWithName
{
    public List<MDLTransform> ObjectTransforms = [];
    public List<MDLRenderCommand> RenderCommands = [];
    public List<MDLMaterial> Materials = [];
    public List<MDLPolygon> Polygons = [];
    public List<Matrix3x4> InvBindMatrices = [];
    
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
    
    public override void Deserialize(DataReader reader)
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
    
    private void ReadHeader(DataReader reader)
    {
        Size = reader.Read<uint>();
        RenderCommandsOffset = reader.Read<uint>();
        MaterialsOffset = reader.Read<uint>();
        PolygonsOffset = reader.Read<uint>();
        InvBindsOffset = reader.Read<uint>();
    }
    
    private void ReadModelInfo(DataReader reader)
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
    
    private void ReadObjects(DataReader reader)
    {
        var transformList = reader.ReadNameListPrimitive<uint>();
        foreach (var (name, offset) in transformList.Items)
        {
            reader.Position = (int) offset;

            var objectTransform = reader.ReadObject<MDLTransform>();
            objectTransform.Name = name;
            ObjectTransforms.Add(objectTransform);
        }
    }
    
    private void ReadRenderCommands(DataReader reader)
    {
        MDLRenderCommand command;
        do
        {
            command = reader.ReadObject<MDLRenderCommand>();
            RenderCommands.Add(command);
        }
        while (command.OpCode != RenderCommandOpCode.END);
    }

    private void ReadMaterials(DataReader reader)
    {
        var textureListOffset = reader.Read<ushort>();
        var paletteListOffset = reader.Read<ushort>();

        var materialList = reader.ReadNameListPrimitive<uint>();

        reader.Position = textureListOffset;
        var textureList = reader.ReadNameList<MDLMaterialMapping>();
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
        var paletteList = reader.ReadNameList<MDLMaterialMapping>();
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

            var material = reader.ReadObject<MDLMaterial>();
            material.Name = name;
            
            if (matIndex < textureNames.Length)
                material.TextureName = textureNames[matIndex];
            
            if (matIndex < paletteNames.Length)
                material.PaletteName = paletteNames[matIndex];
            
            Materials.Add(material);
        }
    }

    private void ReadPolygons(DataReader reader)
    {
        var polygonDataList = reader.ReadNameListPrimitive<uint>();
        var polygons = reader.ReadArray(polygonDataList.Count, () => reader.ReadObject<MDLPolygon>());

        foreach (var (polygon, (name, _)) in polygons.Zip(polygonDataList, (polygon, pair) => (polygon, pair)))
        {
            polygon.Name = name;
            
            var commandEnd = reader.Position + polygon.CommandLength;
            while (reader.Position < commandEnd)
            {
                var opCodes = reader.ReadArray(4, reader.ReadEnum<PolygonCommandOpCode>);
                foreach (var opCode in opCodes)
                {
                    var command = reader.ReadObject<MDLPolygonCommand>(dataModifier: polygonCommand => polygonCommand.OpCode = opCode);
                    polygon.Commands.Add(command);
                }

            }
            
            reader.Position = (int) commandEnd;
            Polygons.Add(polygon);
        }
    }
    
    private void ReadInvBindMatrices(DataReader reader)
    {
        for (var matrixIndex = 0; matrixIndex < NumObjects; matrixIndex++)
        {
           var matrixValues = reader.ReadArray(12, reader.ReadShortAsFloat);
           var unknownValues = reader.ReadArray(9, reader.ReadShortAsFloat);
           
           InvBindMatrices.Add(new Matrix3x4
           {
               M11 = matrixValues[0], M21 = matrixValues[4], M31 = matrixValues[8],
               M12 = matrixValues[1], M22 = matrixValues[5], M32 = matrixValues[9],
               M13 = matrixValues[2], M23 = matrixValues[6], M33 = matrixValues[10],
               M14 = matrixValues[3], M24 = matrixValues[7], M34 = matrixValues[1]
           }); 
        }
    }
    
}