using ndsSharp.Core.Conversion.Models.Mesh;

namespace ndsSharp.Core.Conversion.Models.Export;

public class OBJ(Model Model)
{
    public void Save(string path)
    {
        using var sw = File.CreateText(path);
        
        sw.WriteLine("# OBJ - Exported by ndsSharp");
        foreach (var section in Model.Sections)
        {
            sw.WriteLine();
            sw.WriteLine($"o {section.Name}");

            foreach (var polygon in section.Polygons)
            {
                foreach (var vertex in polygon.Vertices)
                {
                    sw.WriteLine($"v {vertex.X:0.000000} {vertex.Y:0.000000} {vertex.Z:0.000000}");
                }

                foreach (var texCoord in polygon.TexCoords)
                {
                    sw.WriteLine($"vt {texCoord.X:0.000000} {texCoord.Y:0.000000}");
                }

                foreach (var normal in polygon.Normals)
                {
                    sw.WriteLine($"vn {normal.X:0.000000} {normal.Y:0.000000} {normal.Z:0.000000}");
                }

                foreach (var vertexColor in polygon.Colors)
                {
                    sw.WriteLine($"vc {(float) vertexColor.R / byte.MaxValue:0.000000} {(float) vertexColor.G / byte.MaxValue:0.000000} {(float) vertexColor.B / byte.MaxValue:0.000000}");
                }
            }
            
            var currentMaterial = string.Empty;
            foreach (var face in section.Faces)
            {
                if (currentMaterial != face.MaterialName)
                {
                    currentMaterial = face.MaterialName;
                    sw.WriteLine("usemtl {0}", face.MaterialName);
                }

                if (face.VertexIndices.Count == 0)
                {
                    continue;
                }

                var hasNormalIndices = face.NormalIndices.Count != 0;
                var hasTextCoordIndices = face.TexCoordIndices.Count != 0;
                var hasVertexColorIndices = face.ColorIndices.Count != 0;

                sw.Write("f");
                if (hasNormalIndices && hasTextCoordIndices && hasVertexColorIndices)
                {
                    for (int i = 0; i < face.VertexIndices.Count; i++)
                    {
                        sw.Write(" {0}/{1}/{2}/{3}", face.VertexIndices[i] + 1, face.TexCoordIndices[i] + 1, face.NormalIndices[i] + 1, face.ColorIndices[i] + 1);
                    }
                }
                else if (hasNormalIndices && hasTextCoordIndices)
                {
                    for (int i = 0; i < face.VertexIndices.Count; i++)
                    {
                        sw.Write(" {0}/{1}/{2}", face.VertexIndices[i] + 1, face.TexCoordIndices[i] + 1, face.NormalIndices[i] + 1);
                    }
                }
                else if (hasTextCoordIndices)
                {
                    for (int i = 0; i < face.VertexIndices.Count; i++)
                    {
                        sw.Write(" {0}/{1}", face.VertexIndices[i] + 1, face.TexCoordIndices[i] + 1);
                    }
                }
                else if (hasNormalIndices)
                {
                    for (int i = 0; i < face.VertexIndices.Count; i++)
                    {
                        sw.Write(" {0}//{1}", face.VertexIndices[i] + 1, face.NormalIndices[i] + 1);
                    }
                }
                else // only has vertex indices
                {
                    for (int i = 0; i < face.VertexIndices.Count; i++)
                    {
                        sw.Write(" {0}", face.VertexIndices[i] + 1);
                    }
                }
                sw.WriteLine();
            }
        }
    }
}