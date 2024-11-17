using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using ndsSharp.Core.Objects.Exports.Meshes;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Core.Objects.Exports.Textures;
using ndsSharp.FileExplorer.Framework;
using ndsSharp.FileExplorer.WindowModels;

namespace ndsSharp.FileExplorer.Windows;

public partial class STRMWindow : WindowBase<STRMWindowModel>
{
    private static STRMWindow? Instance;
    
    public STRMWindow()
    {
        InitializeComponent();
        DataContext = WindowModel;
        Instance = this;
    }

    public STRMWindow(STRM strm) : this()
    {
        WindowModel.LoadSTRM(strm);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        WindowModel.OutputDevice.Stop();
        Instance = null;
    }

    public static STRMWindow Create(STRM strm)
    {
        if (Instance is not null)
        {
            Instance.WindowModel.LoadSTRM(strm);
            Instance.BringToTop();
            return Instance;
        }
        
        var window = new STRMWindow(strm);
        
        window.Show();
        window.BringToTop();
        return window;
    }
    
    private void OnSliderValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (sender is not Slider slider) return;
        WindowModel.Scrub(TimeSpan.FromSeconds(slider.Value));
    }
}