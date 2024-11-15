namespace ndsSharp.Core.Conversion.Models.Mesh;

public class Face(string materialName)
{
    public string MaterialName = materialName;
    public List<int> VertexIndices = [];
    public List<int> NormalIndices = [];
    public List<int> TexCoordIndices = [];
    public List<int> ColorIndices = [];

    public void AddIndex(int index)
    {
        VertexIndices.Add(index);
        NormalIndices.Add(index);
        TexCoordIndices.Add(index);
        ColorIndices.Add(index);
    }
}