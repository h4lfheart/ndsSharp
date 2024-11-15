namespace ndsSharp.Core.Conversion.Models.Mesh;

public class Section
{
    public string Name;
    public string MaterialName;
    public List<Polygon> Polygons = [];
    public List<Face> Faces = [];
}