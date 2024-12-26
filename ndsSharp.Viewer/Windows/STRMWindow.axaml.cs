using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.WindowModels;

namespace ndsSharp.Viewer.Windows;

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