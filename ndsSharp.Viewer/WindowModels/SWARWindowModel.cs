using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using NAudio.Wave;
using ndsSharp.Core.Conversion.Sounds.Formats;
using ndsSharp.Core.Conversion.Sounds.WaveArchive;
using ndsSharp.Core.Objects.Exports.Sounds;
using ndsSharp.Viewer.Services;
using ndsSharp.Viewer.Shared.Framework;
using ndsSharp.Viewer.Shared.Services;

namespace ndsSharp.Viewer.WindowModels;

public partial class SWARWindowModel : WindowModelBase
{
    [ObservableProperty] private TimeSpan _currentTime;
    [ObservableProperty] private TimeSpan _totalTime;
    
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PauseIcon))] private bool _isPaused;
    public MaterialIconKind PauseIcon => IsPaused ? MaterialIconKind.Play : MaterialIconKind.Pause;
    
    [ObservableProperty] private int _maximumWaveIndex;
    [ObservableProperty] private int _currentWaveIndex = 0;

    public WaveFileReader AudioReader;

    public WaveOutEvent OutputDevice = new()
    {
        DesiredLatency = 250
    };

    public List<Wave> Waves = [];
    
    private readonly DispatcherTimer UpdateTimer = new();

    public override async Task Initialize()
    {
        UpdateTimer.Tick += OnUpdateTimerTick;
        UpdateTimer.Interval = TimeSpan.FromMilliseconds(1);
        UpdateTimer.Start();
    }
    
    public void LoadSWAR(SWAR swar)
    {
        TitleString = $"SWAR Viewer - {swar.Owner!.Path}";

        Waves = swar.ExtractWaves();
        MaximumWaveIndex = Waves.Count - 1;
        CurrentWaveIndex = 0;
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
    }

    public async Task Play()
    {
        if (AudioReader is not null && Math.Abs(AudioReader.TotalTime.TotalSeconds - AudioReader.CurrentTime.TotalSeconds) < 0.1f)
        {
            AudioReader.CurrentTime = TimeSpan.Zero;
        }
        
        var stream = new MemoryStream(Waves[CurrentWaveIndex].GetBuffer());
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
            if (AudioReader is not null && Math.Abs(AudioReader.TotalTime.TotalSeconds - AudioReader.CurrentTime.TotalSeconds) < 0.1f)
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