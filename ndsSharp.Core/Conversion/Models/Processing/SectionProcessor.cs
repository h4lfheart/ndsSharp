using System.Diagnostics;
using System.Numerics;
using ndsSharp.Core.Conversion.Models.Mesh;
using ndsSharp.Core.Objects.Exports.Meshes.Model;

namespace ndsSharp.Core.Conversion.Models.Processing;

public class SectionProcessor(MDLModel Model)
{
    private List<Section> Sections = [];

    private MeshProcessingUnit ProcessingUnit = new();
    
    public List<Section> Process()
    {
        Model.RenderCommands.ForEach(ProcessCommand);
        
        return Sections;
    } 
    
    public void ProcessCommand(MDLRenderCommand command)
    {
         switch (command.OpCode)
        {
            case RenderCommandOpCode.LOAD_MATRIX:
                ProcessingUnit.Load(command.Parameters[0]);
                break;

            case RenderCommandOpCode.BIND_MATERIAL:
                ProcessingUnit.CurrentMaterial = Model.Materials[Math.Min(command.Parameters[0], Model.Materials.Count - 1)];
                break;

            case RenderCommandOpCode.DRAW_MESH:
                var polygon = Model.Polygons[command.Parameters[0]];
                var processor = new PolygonProcessor(polygon, ProcessingUnit);
                
                var section = processor.Process();
                section.Name = polygon.Name;
                section.MaterialName = ProcessingUnit.CurrentMaterial?.Name ?? "Material";
                Sections.Add(section);
                break;

            case RenderCommandOpCode.MULTIPLY_MATRIX:
                var objectIndex = command.Parameters[0];
                var parentIndex = command.Parameters[1];
                var unknown = command.Parameters[2];

                var (storeIndex, loadIndex) = command.Flags switch
                {
                    0 => (-1, -1),
                    1 => (command.Parameters[3], -1),
                    2 => (-1, command.Parameters[3]),
                    3 => (command.Parameters[3], command.Parameters[4])
                };

                if (loadIndex != -1)
                {
                    ProcessingUnit.Load(loadIndex);
                }

                ProcessingUnit.Multiply(Model.ObjectTransforms[objectIndex].Matrix);

                if (storeIndex != -1)
                {
                    ProcessingUnit.Store(storeIndex);
                }

                break;

            case RenderCommandOpCode.SCALE:
                ProcessingUnit.Multiply(command.Flags == 1 ? Matrix4x4.CreateScale(Model.DownScale) : Matrix4x4.CreateScale(Model.UpScale));
                break;
            
        }
    }
}