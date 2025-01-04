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
using ndsSharp.Core.Conversion.Sounds.WaveArchive;
using ndsSharp.Core.Conversion.Textures.Images;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Viewer.Shared.Extensions;
using ndsSharp.Viewer.Shared.Plugins;
using ndsSharp.Viewer.Shared.Services;
using ImageExtensions = ndsSharp.Core.Conversion.Textures.Images.ImageExtensions;

namespace ndsSharp.Viewer.Plugins.Sound.FileViewers;

public partial class SWARViewer : BaseFileViewer<SWARViewerModel>
{
    public override bool OnlyOneWindow => true;

    public SWARViewer()
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
        
        if (Math.Abs(slider.Value - WindowModel.AudioReader.CurrentTime.TotalSeconds) < 0.1f)
        {
            return;
        }
        
        WindowModel.Scrub(TimeSpan.FromSeconds(slider.Value));
    }
}

public partial class SWARViewerModel : BaseFileViewerModel<SWAR>
{
    public override string Title => "SWAR Viewer";
    
    [ObservableProperty] private TimeSpan _currentTime;
    [ObservableProperty] private TimeSpan _totalTime;
    
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PauseIcon))] private bool _isPaused;
    public MaterialIconKind PauseIcon => IsPaused ? MaterialIconKind.Play : MaterialIconKind.Pause;
    
    [ObservableProperty] private int _currentWaveIndex = 0;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(MaximumWaveIndex))] private ObservableCollection<Wave> _waves = [];
    public int MaximumWaveIndex => Waves.Count - 1;

    public WaveFileReader AudioReader;

    public WaveOutEvent OutputDevice = new()
    {
        DesiredLatency = 250
    };

    
    private readonly DispatcherTimer UpdateTimer = new();

    public override async Task Initialize()
    {
        UpdateTimer.Tick += OnUpdateTimerTick;
        UpdateTimer.Interval = TimeSpan.FromMilliseconds(1);
        UpdateTimer.Start();
    }

    public override void Load(SWAR obj)
    {
        base.Load(obj);
        
        Waves = new ObservableCollection<Wave>(obj.ExtractWaves());
        TaskService.Run(Play);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(CurrentWaveIndex):
            {
                TaskService.Run(Play);
                break;
            }
        }
    }

    private void OnUpdateTimerTick(object? sender, EventArgs e)
    {
        if (AudioReader is null) return;
        
        TotalTime = AudioReader.TotalTime;
        CurrentTime = AudioReader.CurrentTime;

        if (Math.Abs(TotalTime.TotalSeconds - CurrentTime.TotalSeconds) < 0.01f)
        {
            TogglePause(true);
        }
    }

    public async Task Play()
    {
        if (AudioReader is not null)
        {
            AudioReader.CurrentTime = TimeSpan.Zero;
        }
        
        var stream = new MemoryStream(Waves[CurrentWaveIndex].GetBuffer());
        AudioReader = new WaveFileReader(stream);
        OutputDevice.Stop();
        OutputDevice.Init(AudioReader);
        AudioReader.CurrentTime = TimeSpan.Zero;
        TogglePause(false);
        while (OutputDevice.PlaybackState != PlaybackState.Stopped) { }
    }

    public void TogglePause()
    {
        TogglePause(!IsPaused);
    }
    
    public void TogglePause(bool isPaused)
    {
        IsPaused = isPaused;
        
        if (IsPaused)
        {
            OutputDevice.Pause();
        }
        else
        {
            if (AudioReader is not null && Math.Abs(AudioReader.TotalTime.TotalSeconds - AudioReader.CurrentTime.TotalSeconds) < 0.01f)
            {
                AudioReader.CurrentTime = TimeSpan.Zero;
            }
            
            OutputDevice.Play();
        }
    }

    public void Scrub(TimeSpan time)
    {
        if (AudioReader.CurrentTime == TimeSpan.Zero) return;
        AudioReader.CurrentTime = time;
    }
}