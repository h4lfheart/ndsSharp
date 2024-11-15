using ndsSharp.Core.Conversion.Models.Export;
using ndsSharp.Core.Conversion.Models.Mesh;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Textures;
using SixLabors.ImageSharp;

namespace ndsSharp.Core.Conversion.Models.Processing;

public static class ModelProcessingExtensions
{
    public static List<Model> ExtractModels(this BMD0 bmd, TEX0? overrideTextureData = null)
    {
        var textureData = overrideTextureData ?? bmd.TextureData;

        var models = new List<Model>();
        foreach (var modelData in bmd.ModelData.Models)
        {
            var sectionProcessor = new SectionProcessor(modelData); 
            
            var model = new Model();
            model.Name = modelData.Name;
            model.Sections = sectionProcessor.Process();

            var vertexIndex = 0;
            foreach (var section in model.Sections)
            {
                foreach (var polygon in section.Polygons)
                {
                    switch (polygon.PolygonType)
                    {
                        case PolygonType.TRI:
                        {
                            for (var vtxCounter = 0; vtxCounter < polygon.Vertices.Count; vtxCounter += 3)
                            {
                                var face = new Face(section.MaterialName);
                                for (var vtxIdx = 0; vtxIdx < 3; vtxIdx++)
                                {
                                    face.AddIndex(vertexIndex);
                                    vertexIndex++;
                                }
                                section.Faces.Add(face);
                            }
                            break;
                        }
                        case PolygonType.QUAD:
                        {
                            for (var vtxCounter = 0; vtxCounter < polygon.Vertices.Count; vtxCounter += 4)
                            {
                                var face = new Face(section.MaterialName);
                                var indices = new int[4];
                                for (var vtxIdx = 0; vtxIdx < 4; vtxIdx++)
                                {
                                    indices[vtxIdx] = vertexIndex;
                                    vertexIndex++;
                                }
                                
                                face.AddIndex(indices[0]);
                                face.AddIndex(indices[1]);
                                face.AddIndex(indices[2]);
                                
                                face.AddIndex(indices[2]);
                                face.AddIndex(indices[3]);
                                face.AddIndex(indices[0]);
                                
                                section.Faces.Add(face);
                            }
                            break;
                        }
                        case PolygonType.TRI_STRIP:
                        case PolygonType.QUAD_STRIP:
                        {
                            for (var vtxCounter = 0; vtxCounter + 2 < polygon.Vertices.Count; vtxCounter += 2)
                            {
                                var firstFace = new Face(section.MaterialName);
                                firstFace.AddIndex(vertexIndex + vtxCounter);
                                firstFace.AddIndex(vertexIndex + vtxCounter + 1);
                                firstFace.AddIndex(vertexIndex + vtxCounter + 2);
                                section.Faces.Add(firstFace);

                                if (vtxCounter + 3 < polygon.Vertices.Count)
                                {
                                    var extraFace = new Face(section.MaterialName);
                                    extraFace.AddIndex(vertexIndex + vtxCounter + 1);
                                    extraFace.AddIndex(vertexIndex + vtxCounter + 3);
                                    extraFace.AddIndex(vertexIndex + vtxCounter + 2);
                                    section.Faces.Add(extraFace);
                                }
                                
                            }
                            
                            vertexIndex += polygon.Vertices.Count;
                            break;
                        }
                    }
                }
            }

            foreach (var materialData in modelData.Materials)
            {
                var material = new Material
                {
                    Name = materialData.Name,
                    Texture = textureData?.Textures.FirstOrDefault(texture => texture.Name.Equals(materialData.TextureName, StringComparison.OrdinalIgnoreCase)),
                    FlipU = materialData.FlipU,
                    FlipV = materialData.FlipV,
                    RepeatU = materialData.RepeatU,
                    RepeatV = materialData.RepeatV
                };
                model.Materials.Add(material);
            }
            
            models.Add(model);
        }

        return models;
    }
}