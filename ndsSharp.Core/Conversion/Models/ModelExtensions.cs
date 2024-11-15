using System.ComponentModel;
using ndsSharp.Core.Conversion.Models.Export;
using ndsSharp.Core.Conversion.Models.Mesh;
using ndsSharp.Core.Conversion.Textures.Images;
using SixLabors.ImageSharp;

namespace ndsSharp.Core.Conversion.Models;

public static class ModelExtensions
{
    public static void SaveToDirectory(this Model model, string path, MeshExportType exportType)
    {
        Directory.CreateDirectory(path);
        model.SaveModel(Path.Combine(path, $"{model.Name}.obj"), exportType);
        model.SaveTextures(path);
    }

    public static void SaveTextures(this Model model, string path)
    {
        foreach (var material in model.Materials)
        {
            material.Texture?.ToImage().SaveAsPng(Path.Combine(path, $"{material.Texture?.Name}.png"));
        }
    }

    public static void SaveModel(this Model model, string path, MeshExportType exportType)
    {
        switch (exportType)
        {
            case MeshExportType.OBJ:
                new OBJ(model).Save(path);
                break;
        }
    }
}

public enum MeshExportType
{
    [Description("Wavefront .obj")]
    OBJ,
    
}