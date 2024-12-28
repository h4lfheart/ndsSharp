namespace ndsSharp.Viewer.Shared.Rendering.Rendering;

public interface IRenderable : IDisposable
{
    public void Setup();
    public void Render(Camera camera);
}