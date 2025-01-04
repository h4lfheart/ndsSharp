using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using NAudio.Wave;
using ndsSharp.Core.Conversion.Sounds.Formats;
using ndsSharp.Core.Conversion.Sounds.Stream;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Viewer.Shared.Extensions;
using ndsSharp.Viewer.Shared.Plugins;
using ndsSharp.Viewer.Shared.Services;
using ImageExtensions = ndsSharp.Core.Conversion.Textures.Images.ImageExtensions;

namespace ndsSharp.Viewer.Plugins.Sound.FileViewers;

public partial class STRMViewer : BaseFileViewer<STRMViewerModel>
{
    public override bool OnlyOneWindow => true;

    public STRMViewer()
    {
        InitializeComponent();
        DataContext = WindowModel;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        
        WindowModel.OutputDevice.Stop();
    }

    private void OnSliderValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (sender is not Slider slider) return;
        WindowModel.Scrub(TimeSpan.FromSeconds(slider.Value));
    }
}

public partial class STRMViewerModel : BaseFileViewerModel<STRM>
{
    public override string Title => "STRM Viewer";
    
    [ObservableProperty] private TimeSpan _currentTime;
    [ObservableProperty] private TimeSpan _totalTime;
    
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PauseIcon))] private bool _isPaused;
    public MaterialIconKind PauseIcon => IsPaused ? MaterialIconKind.Play : MaterialIconKind.Pause;

    public WaveFileReader AudioReader;

    public WaveOutEvent OutputDevice = new()
    {
        DesiredLatency = 250
    };

    public Wave StreamWave;
    
    private readonly DispatcherTimer UpdateTimer = new();

    public override async Task Initialize()
    {
        UpdateTimer.Tick += OnUpdateTimerTick;
        UpdateTimer.Interval = TimeSpan.FromMilliseconds(1);
        UpdateTimer.Start();
    }

    public override void Load(STRM obj)
    {
        base.Load(obj);
        
        StreamWave = obj.ToWave();
        
        TaskService.Run(Play);
    }
    
    private void OnUpdateTimerTick(object? sender, EventArgs e)
    {
        if (AudioReader is null) return;
        
        TotalTime = AudioReader.TotalTime;
        CurrentTime = AudioReader.CurrentTime;
    }

    public async Task Play()
    {
        var stream = new MemoryStream(StreamWave.GetBuffer());
        AudioReader = new WaveFileReader(stream);
        
        OutputDevice.Stop();
        OutputDevice.Init(AudioReader);
        OutputDevice.Play();
        while (OutputDevice.PlaybackState != PlaybackState.Stopped) { }
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        
        if (IsPaused)
        {
            OutputDevice.Pause();
        }
        else
        {
            OutputDevice.Play();
        }
    }

    public void Scrub(TimeSpan time)
    {
        AudioReader.CurrentTime = time;
    }
    
    public void UpdateOutputDevice()
    {
        OutputDevice.Stop();
        OutputDevice = new WaveOutEvent { DeviceNumber = 0 };
        OutputDevice.Init(AudioReader);
        
        if (!IsPaused && AudioReader is not null)
        {
            OutputDevice.Play();
        }
    }
}