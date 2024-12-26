using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace ndsSharp.Viewer.Rendering.Rendering.Meshes;

public sealed class Section : BaseMesh
{
    public Materials.Material Material;

    public Core.Conversion.Models.Mesh.Section SectionData;
    
    public Section(Core.Conversion.Models.Mesh.Section section, Core.Conversion.Models.Mesh.Material material) : base(RenderManager.Instance.ObjectShader)
    {
        RegisterAttribute("Position", 3, VertexAttribPointerType.Float);
        RegisterAttribute("TexCoord", 2, VertexAttribPointerType.Float);
        RegisterAttribute("Normal", 3, VertexAttribPointerType.Float);

        SectionData = section;
        Material = new Materials.Material(material);

        var indices = new List<uint>();
        foreach (var face in section.Faces)
        {
            foreach (var chunk in face.VertexIndices.Chunk(3))
            {
                var chunkIndices = chunk.Select(val => (uint) (val - section.FirstVertexIndex)).ToArray();
                indices.AddRange(chunkIndices);
            }
        }
        
        Indices.AddRange(indices);

        var vertices = section.Polygons.SelectMany(polygon => polygon.Vertices).ToArray();
        var normals = section.Polygons.SelectMany(polygon => polygon.Normals).ToArray();
        var texCoords = section.Polygons.SelectMany(polygon => polygon.TexCoords).ToArray();
        var colors = section.Polygons.SelectMany(polygon => polygon.Colors).ToArray();
        
        for (var i = 0; i < vertices.Length; i++)
        {
            var position = vertices[i];
            var normal = normals[i];
            var texCoord = texCoords[i];
            var color = colors[i];
            
            Vertices.AddRange([
                position.X, position.Y, position.Z,
                texCoord.X, -texCoord.Y,
                normal.X, normal.Y, normal.Z,
                
            ]);
        }
    }

    public override void Render(Camera camera)
    {
        base.Render(camera);
        GL.Disable(EnableCap.CullFace);
        
        Shader.Use();
        Shader.SetMatrix4("uTransform", Transform);
        Shader.SetMatrix4("uView", camera.GetViewMatrix());
        Shader.SetMatrix4("uProjection", camera.GetProjectionMatrix());

        VAO.Bind();
        Material.Render(Shader);
        //GL.DrawArrays(PrimitiveType.Triangles, 0, SectionData.Polygons.Sum(x => x.Vertices.Count));
        GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);

        GL.Enable(EnableCap.CullFace);
    }
}