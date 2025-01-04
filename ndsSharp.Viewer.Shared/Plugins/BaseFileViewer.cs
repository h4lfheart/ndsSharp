using System.Reflection;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using ndsSharp.Core.Objects;
using ndsSharp.Core.Objects.Exports;
using ndsSharp.Viewer.Shared.Framework;

namespace ndsSharp.Viewer.Shared.Plugins;

public abstract partial class BaseFileViewer<T> : WindowBase<T> where T : BaseFileViewerModel, new()
{
    public static BaseFileViewer<T>? Instance;
    
    public virtual bool OnlyOneWindow => false;
    
    public static void Load(Type windowType, BaseDeserializable obj)
    {
        var targetWindow = GetOrOpenWindow(windowType);
        targetWindow.Show();
        targetWindow.BringToTop();
        targetWindow.WindowModel.Load(obj);
    }
    
    private static BaseFileViewer<T> GetOrOpenWindow(Type windowType)
    {
        if (Instance is not null) return Instance;
        
        var newWindow = (BaseFileViewer<T>) Activator.CreateInstance(windowType)!;
        if (newWindow.OnlyOneWindow)
        {
            Instance = newWindow;
        }
            
        return newWindow;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        Instance = null;
    }
}