using System.Numerics;
using ndsSharp.Core.Objects.Exports.Meshes.Blocks.MDL;

namespace ndsSharp.Core.Conversion.Models.Processing;

public class MeshProcessingUnit
{
    public Matrix4x4[] MatrixStack;
    public Matrix4x4 CurrentMatrix = Matrix4x4.Identity;
    public MDL0Material? CurrentMaterial;
    
    public MeshProcessingUnit()
    {
        MatrixStack = new Matrix4x4[32];
        for (var i = 0; i < MatrixStack.Length; i++)
        {
            MatrixStack[i] = Matrix4x4.Identity;
        }
    }
    
    public void Load(int stackIndex)
    {
        CurrentMatrix = MatrixStack[stackIndex];
    }

    public void Store(int stackIndex)
    {
        MatrixStack[stackIndex] = CurrentMatrix;
    }

    public void Multiply(Matrix4x4 matrix)
    {
        CurrentMatrix = matrix * CurrentMatrix;
    }
}