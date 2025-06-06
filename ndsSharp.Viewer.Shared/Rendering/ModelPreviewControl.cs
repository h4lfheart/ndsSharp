using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ndsSharp.Viewer.Shared.Rendering;

public class ModelPreviewControl : NativeControlHost
{
    public ModelViewerContext Context;
    public PlatformHandle Handle;
    
    public ModelPreviewControl(bool useGrid = true)
    {
        Context = new ModelViewerContext(useGrid);
    }
    
    protected override unsafe IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        Handle = new PlatformHandle(GLFW.GetWin32Window(Context.WindowPtr), "OpenTKWindow");
        return Handle;
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        Context.Run();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        
        Context.Close();
    }
}