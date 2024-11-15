using System.Diagnostics;
using System.Numerics;
using ndsSharp.Core.Conversion.Models.Mesh;
using ndsSharp.Core.Objects.Exports.Meshes.MDL;

namespace ndsSharp.Core.Conversion.Models.Processing;

public class SectionProcessor(MDL0Model Model)
{
    private List<Section> Sections = [];

    private MeshProcessingUnit ProcessingUnit = new();
    
    public List<Section> Process()
    {
        Model.RenderCommands.ForEach(ProcessCommand);
        
        return Sections;
    } 
    
    public void ProcessCommand(MDL0RenderCommand command)
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
                var meshInfoIndex = command.Parameters[0];

                var loadIndex = -1;
                var storeIndex = -1;
                switch (command.Flags)
                {
                    case 0: // 3 params
                        break;
                    case 1: // 4 params
                        storeIndex = command.Parameters[3];
                        break;
                    case 2: // 4 params
                        loadIndex = command.Parameters[3];
                        break;
                    case 3: // 5 params
                        storeIndex = command.Parameters[3];
                        loadIndex = command.Parameters[4];
                        break;
                }

                if (loadIndex != -1)
                {
                    ProcessingUnit.Load(loadIndex);
                }

                var data = Model.ObjectTransforms[meshInfoIndex];
                ProcessingUnit.Multiply(data.Matrix);

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