using Avalonia.Controls;
using ndsSharp.Viewer.Shared.Services;

namespace ndsSharp.Viewer.Shared.Framework;

public abstract class WindowBase<T> : WindowBase where T : ViewModelBase, new()
{
    protected new readonly T WindowModel;

    public WindowBase(bool initializeWindowModel = true)
    {
        WindowModel = ViewModelRegistry.New<T>();

        if (initializeWindowModel)
        {
            TaskService.Run(WindowModel.Initialize);
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        ViewModelRegistry.Unregister<T>();
    }
}

public abstract class WindowBase : Window
{
    protected readonly WindowModelBase WindowModel;
    
    public void BringToTop()
    {
       Topmost = true;
       Topmost = false;
    }
}