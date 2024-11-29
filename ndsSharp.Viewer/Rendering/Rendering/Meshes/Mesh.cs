using System.Collections.Generic;
using System.Linq;
using ndsSharp.Core.Conversion.Models.Mesh;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ndsSharp.FileExplorer.Rendering.Rendering.Meshes;

public class Mesh : IRenderable
{
    public List<Section> Sections = [];
    
    public Mesh(Model model, Matrix4? transform = null)
    {
        foreach (var sectionData in model.Sections)
        {
            var material = model.Materials.FirstOrDefault(x => x.Name.Equals(sectionData.MaterialName));
            var section = new Section(sectionData, material);
            section.Transform = transform ?? Matrix4.Identity;
            Sections.Add(section);
        }
        
    }

    public void Dispose()
    {
        foreach (var section in Sections)
        {
            section.Dispose();
        }
    }

    public void Setup()
    {
        foreach (var section in Sections)
        {
            section.Setup();
        }
    }

    public void Render(Camera camera)
    {
        foreach (var section in Sections)
        {
            section.Render(camera);
        }
    }
}