using ndsSharp.Viewer.Shared.Extensions;
using ndsSharp.Viewer.Shared.Rendering.Rendering.Materials;
using ndsSharp.Viewer.Shared.Rendering.Rendering.Meshes;
using ndsSharp.Viewer.Shared.Rendering.Rendering.Viewport;
using OpenTK.Mathematics;

namespace ndsSharp.Viewer.Shared.Rendering.Rendering;

public class RenderManager : IRenderable
{
    public static RenderManager Instance;
    
    public Grid? Grid;
    public Shader ObjectShader;
    
    public readonly List<IRenderable> Objects = [];
    public readonly List<Section> OpaqueSections = [];
    public readonly List<Section> TransparentSections = [];

    public bool UseGrid;

    private bool _isRendering;

    public RenderManager(bool useGrid)
    {
        Instance = this;
        UseGrid = useGrid;
    }
    
    public void Setup()
    {
        ObjectShader = new Shader("shader");

        if (UseGrid)
        {
            Grid = new Grid();
            Grid.Setup();
        }
    }

    public void Render(Camera camera)
    {
        _isRendering = true;
        Objects.ForEach(obj => obj.Render(camera));
        OpaqueSections.ForEach(section => section.Render(camera));
        TransparentSections.ForEach(section => section.Render(camera));
        
        Grid?.Render(camera);
        _isRendering = false;
    }

    public void Add(Actor actor)
    {
        var renderable = new Mesh(actor.Model, Matrix4.CreateFromQuaternion(actor.Rotation) * Matrix4.CreateTranslation(actor.Location));
        renderable.Setup();

        var sections = renderable.Sections;
        OpaqueSections.AddRange(sections.Where(section => !section.Material.IsTransparent));
        TransparentSections.AddRange(sections.Where(section => section.Material.IsTransparent));
    }

    public void Clear()
    {
        while (_isRendering) { }
        
        Objects.Clear();
        OpaqueSections.Clear();
        TransparentSections.Clear();
    }
    
    public void Dispose()
    {
        Clear();
        
        Objects.ForEach(obj => obj.Dispose());
        ObjectShader.Dispose();
        Grid?.Dispose();
    }
}