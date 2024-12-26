using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.WindowModels;

namespace ndsSharp.Viewer.Windows;

public partial class SWARWindow : WindowBase<SWARWindowModel>
{
    private static SWARWindow? Instance;
    
    public SWARWindow()
    {
        InitializeComponent();
        DataContext = WindowModel;
        Instance = this;
    }

    public SWARWindow(SWAR strm) : this()
    {
        WindowModel.LoadSWAR(strm);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        WindowModel.OutputDevice.Stop();
        Instance = null;
    }

    public static SWARWindow Create(SWAR swar)
    {
        if (Instance is not null)
        {
            Instance.WindowModel.LoadSWAR(swar);
            Instance.BringToTop();
            return Instance;
        }
        
        var window = new SWARWindow(swar);
        
        window.Show();
        window.BringToTop();
        return window;
    }
    
    private void OnSliderValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (sender is not Slider slider) return;
        if (Math.Abs(e.NewValue - e.OldValue) < 0.1f || Math.Abs(slider.Value - WindowModel.AudioReader.CurrentTime.TotalSeconds) < 0.1f)
        {
            WindowModel.IsPaused = true;
            return;
        }
        
        WindowModel.Scrub(TimeSpan.FromSeconds(slider.Value));
    }
}