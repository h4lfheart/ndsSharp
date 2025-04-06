using ndsSharp.Core.Conversion.Models.Export;
using ndsSharp.Core.Conversion.Models.Mesh;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.Core.Objects.Exports.Textures.Blocks;
using SixLabors.ImageSharp;

namespace ndsSharp.Core.Conversion.Models.Processing;

public static class ModelProcessingExtensions
{
    public static List<Model> ExtractModels(this BMD bmd, TEX? overrideTextureData = null)
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
                section.FirstVertexIndex = vertexIndex;
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
                        {
                            for (var vtxCounter = 0; vtxCounter < polygon.Vertices.Count - 2; vtxCounter++)
                            {
                                var face = new Face(section.MaterialName);
                                if (vtxCounter % 2 == 0)
                                {
                                    face.AddIndex(vertexIndex + vtxCounter);
                                    face.AddIndex(vertexIndex + vtxCounter + 1);
                                    face.AddIndex(vertexIndex + vtxCounter + 2);
                                }
                                else
                                {
                                    face.AddIndex(vertexIndex + vtxCounter);
                                    face.AddIndex(vertexIndex + vtxCounter + 2);
                                    face.AddIndex(vertexIndex + vtxCounter + 1);
                                }

                                section.Faces.Add(face);
                            }
                            
                            vertexIndex += polygon.Vertices.Count;
                            break;
                        }
                        case PolygonType.QUAD_STRIP:
                        {
                            for (var vtxCounter = 0; vtxCounter < polygon.Vertices.Count - 3; vtxCounter += 2)
                            {
                                var firstFace = new Face(section.MaterialName);
                                firstFace.AddIndex(vertexIndex + vtxCounter + 0);
                                firstFace.AddIndex(vertexIndex + vtxCounter + 1);
                                firstFace.AddIndex(vertexIndex + vtxCounter + 3);
                                section.Faces.Add(firstFace);

                                var secondFace = new Face(section.MaterialName);
                                secondFace.AddIndex(vertexIndex + vtxCounter + 0);
                                secondFace.AddIndex(vertexIndex + vtxCounter + 3);
                                secondFace.AddIndex(vertexIndex + vtxCounter + 2);
                                section.Faces.Add(secondFace);
                            }
                            
                            vertexIndex += polygon.Vertices.Count;
                            break;
                        }
                    }
                }
            }

            foreach (var materialData in modelData.Materials)
            {
                var material = Material.FromMDL(materialData);
                material.Texture = textureData?.Textures.FirstOrDefault(texture =>
                    texture.Name.Equals(materialData.TextureName, StringComparison.OrdinalIgnoreCase));
                model.Materials.Add(material);
            }
            
            models.Add(model);
        }

        return models;
    }
}