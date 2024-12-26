using System.Collections.Generic;
using System.Linq;
using ndsSharp.Viewer.Rendering.Rendering.Materials;
using ndsSharp.Viewer.Rendering.Rendering.Meshes;
using ndsSharp.Viewer.Rendering.Rendering.Viewport;
using OpenTK.Mathematics;

namespace ndsSharp.Viewer.Rendering.Rendering;

public class RenderManager : IRenderable
{
    public static RenderManager Instance;
    
    public Grid Grid;
    public Shader ObjectShader;
    
    public readonly List<IRenderable> Objects = [];

    public RenderManager()
    {
        Instance = this;
    }
    
    public void Setup()
    {
        ObjectShader = new Shader("shader");
        
        Grid = new Grid();
        Grid.Setup();
    }

    public void Render(Camera camera)
    {
        Objects.ToList().ForEach(obj => obj.Render(camera));
        
        Grid.Render(camera);
    }

    public void Add(Actor actor)
    {
        var renderable = new Mesh(actor.Model, Matrix4.CreateFromQuaternion(actor.Rotation) * Matrix4.CreateTranslation(actor.Position));
        renderable.Setup();
        Objects.Add(renderable);
    }

    public void Clear()
    {
        Objects.Clear();
    }
    
    public void Dispose()
    {
        Clear();
        
        Objects.ForEach(obj => obj.Dispose());
        ObjectShader.Dispose();
        Grid.Dispose();
    }
}