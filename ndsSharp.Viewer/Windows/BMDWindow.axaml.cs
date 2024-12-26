using System;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.WindowModels;

namespace ndsSharp.Viewer.Windows;

public partial class BMDWindow : WindowBase<BMDWindowModel>
{
    private static BMDWindow? Instance;
    
    public BMDWindow()
    {
        InitializeComponent();
        DataContext = WindowModel;
        Instance = this;
    }

    public BMDWindow(BMD btx) : this()
    {
        WindowModel.LoadBMD(btx);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        Instance = null;
    }

    public static BMDWindow Create(BMD bmd)
    {
        if (Instance is not null)
        {
            Instance.WindowModel.LoadBMD(bmd);
            Instance.BringToTop();
            return Instance;
        }
        
        var window = new BMDWindow(bmd);
        
        window.Show();
        window.BringToTop();
        return window;
    }
}