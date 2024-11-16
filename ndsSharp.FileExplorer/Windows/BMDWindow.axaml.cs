using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.FileExplorer.Framework;
using ndsSharp.FileExplorer.WindowModels;

namespace ndsSharp.FileExplorer.Windows;

public partial class BMDWindow : WindowBase<BMDWindowModel>
{
    private static BMDWindow? Instance;
    
    public BMDWindow()
    {
        InitializeComponent();
        DataContext = WindowModel;
        Instance = this;
    }

    public BMDWindow(BMD0 btx) : this()
    {
        WindowModel.LoadBMD(btx);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        Instance = null;
    }

    public static BMDWindow Create(BMD0 bmd)
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